using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Jvlink2Db.Core.Jvlink;
using Jvlink2Db.Core.Persistence;
using Jvlink2Db.Pipeline.Retry;

namespace Jvlink2Db.Pipeline.Setup;

/// <summary>
/// Runs the canonical JV-Link acquisition for a single dataspec and
/// dispatches each record to its registered <see cref="IRecordSink"/>.
/// Records whose spec is not in the registry are counted and discarded.
///
/// The runner buffers each sink in memory before flushing so the
/// database transactions are not held open across slow JV-Link I/O.
/// For Phase 3 volumes this is bounded and simple.
/// </summary>
public sealed class SetupRunner
{
    private static readonly TimeSpan DefaultPollDelay = TimeSpan.FromMilliseconds(500);

    private readonly IJvLink _jvLink;
    private readonly ISchemaProvisioner _provisioner;
    private readonly Dictionary<string, IRecordSink> _sinks;
    private readonly TimeSpan _pollDelay;
    private readonly JvLinkRetryPolicy _retryPolicy;
    private readonly Action<ProgressEvent>? _progress;

    public SetupRunner(
        IJvLink jvLink,
        ISchemaProvisioner provisioner,
        IEnumerable<IRecordSink> sinks,
        TimeSpan? pollDelay = null,
        JvLinkRetryPolicy? retryPolicy = null,
        Action<ProgressEvent>? progress = null)
    {
        ArgumentNullException.ThrowIfNull(jvLink);
        ArgumentNullException.ThrowIfNull(provisioner);
        ArgumentNullException.ThrowIfNull(sinks);

        _jvLink = jvLink;
        _provisioner = provisioner;
        _sinks = sinks.ToDictionary(s => s.RecordSpec, StringComparer.Ordinal);
        _pollDelay = pollDelay ?? DefaultPollDelay;
        _retryPolicy = retryPolicy ?? new JvLinkRetryPolicy();
        _progress = progress;
    }

    public async Task<SetupReport> RunAsync(SetupOptions options, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(options);

        await _provisioner.EnsureCreatedAsync(cancellationToken).ConfigureAwait(false);

        var initRc = _jvLink.Init(options.Sid);
        if (initRc != 0)
        {
            throw new JvLinkException(initRc, "JVInit");
        }

        var open = await _retryPolicy.ExecuteOpenAsync(
            () => _jvLink.Open(options.Dataspec, options.Fromtime, options.Option),
            cancellationToken).ConfigureAwait(false);

        try
        {
            if (open.ReturnCode == -1)
            {
                return Empty(open);
            }

            if (open.ReturnCode < 0)
            {
                throw new JvLinkException(open.ReturnCode, "JVOpen");
            }

            await WaitForDownloadAsync(open.DownloadCount, cancellationToken).ConfigureAwait(false);

            SkipPastResume(options.ResumeFromFilename);

            var (counts, lastFilename) = await ReadAllAsync(open.ReadCount, cancellationToken).ConfigureAwait(false);

            var inserted = new Dictionary<string, long>(StringComparer.Ordinal);
            foreach (var sink in _sinks.Values)
            {
                var bufferedCount = counts.GetValueOrDefault(sink.RecordSpec);
                _progress?.Invoke(new FlushStartedEvent(sink.RecordSpec, bufferedCount));
                var flushStartedAt = DateTimeOffset.UtcNow;
                var rowsInserted = await sink.FlushAsync(cancellationToken).ConfigureAwait(false);
                _progress?.Invoke(new FlushCompletedEvent(sink.RecordSpec, rowsInserted, DateTimeOffset.UtcNow - flushStartedAt));
                inserted[sink.RecordSpec] = rowsInserted;
            }

            return new SetupReport(
                OpenReturnCode: open.ReturnCode,
                ReadCount: open.ReadCount,
                DownloadCount: open.DownloadCount,
                LastFileTimestamp: open.LastFileTimestamp,
                RecordCountsById: counts,
                RecordsInsertedById: inserted,
                LastConsumedFilename: lastFilename);
        }
        finally
        {
            _ = _jvLink.Close();
        }
    }

    private static SetupReport Empty(JvLinkOpenResult open) => new(
        OpenReturnCode: open.ReturnCode,
        ReadCount: open.ReadCount,
        DownloadCount: open.DownloadCount,
        LastFileTimestamp: open.LastFileTimestamp,
        RecordCountsById: new Dictionary<string, int>(),
        RecordsInsertedById: new Dictionary<string, long>());

    private async Task WaitForDownloadAsync(int downloadCount, CancellationToken cancellationToken)
    {
        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var status = _jvLink.Status();
            if (status >= downloadCount)
            {
                return;
            }

            if (status < 0)
            {
                throw new JvLinkException(status, "JVStatus");
            }

            await DelayAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    private void SkipPastResume(string? resumeFromFilename)
    {
        if (string.IsNullOrEmpty(resumeFromFilename))
        {
            return;
        }

        while (true)
        {
            var skip = _jvLink.Skip();
            if (skip.ReturnCode != 0)
            {
                return;
            }

            if (string.Equals(skip.Filename, resumeFromFilename, StringComparison.Ordinal))
            {
                return;
            }
        }
    }

    private async Task<(Dictionary<string, int> Counts, string? LastFilename)> ReadAllAsync(int totalFiles, CancellationToken cancellationToken)
    {
        var counts = new Dictionary<string, int>(StringComparer.Ordinal);
        string? lastFilename = null;
        var fileIndex = 0;
        var recordsInFile = 0;
        var recordsTotal = 0;
        var readStartedAt = DateTimeOffset.UtcNow;

        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var read = _jvLink.Read();
            if (read.ReturnCode > 0)
            {
                var id = ExtractRecordId(read.Buffer);
                counts[id] = counts.GetValueOrDefault(id) + 1;
                recordsInFile++;
                recordsTotal++;
                if (!string.IsNullOrEmpty(read.Filename))
                {
                    lastFilename = read.Filename;
                }

                if (_sinks.TryGetValue(id, out var sink))
                {
                    sink.Add(read.Buffer!);
                }

                continue;
            }

            if (read.ReturnCode == -1 && !string.IsNullOrEmpty(read.Filename))
            {
                lastFilename = read.Filename;
                fileIndex++;
                _progress?.Invoke(new FileCompletedEvent(
                    Filename: read.Filename!,
                    FileIndex: fileIndex,
                    TotalFiles: totalFiles,
                    RecordsInFile: recordsInFile,
                    RecordsTotal: recordsTotal,
                    Elapsed: DateTimeOffset.UtcNow - readStartedAt));
                recordsInFile = 0;
            }

            switch (read.ReturnCode)
            {
                case 0:
                    return (counts, lastFilename);

                case -1:
                    break;

                case -3:
                    await DelayAsync(cancellationToken).ConfigureAwait(false);
                    break;

                default:
                    throw new JvLinkException(read.ReturnCode, "JVGets");
            }
        }
    }

    private static string ExtractRecordId(byte[]? buffer)
    {
        if (buffer is null || buffer.Length < 2)
        {
            return string.Empty;
        }

        return Encoding.ASCII.GetString(buffer, 0, 2);
    }

    private async Task DelayAsync(CancellationToken cancellationToken)
    {
        if (_pollDelay <= TimeSpan.Zero)
        {
            await Task.Yield();
            return;
        }

        await Task.Delay(_pollDelay, cancellationToken).ConfigureAwait(false);
    }
}
