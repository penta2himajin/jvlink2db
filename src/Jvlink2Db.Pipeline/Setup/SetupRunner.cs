using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Jvlink2Db.Core.Jvlink;
using Jvlink2Db.Core.Persistence;
using Jvlink2Db.Core.Records;
using Jvlink2Db.Parser.Records;

namespace Jvlink2Db.Pipeline.Setup;

/// <summary>
/// Runs the canonical JV-Link acquisition for a single dataspec and
/// persists every <c>RA</c> record into the database. Non-<c>RA</c>
/// records are counted and discarded; downstream phases will add the
/// remaining record types.
///
/// The runner reads the entire batch into memory before invoking the
/// writer so the database transaction is not held open across slow
/// JV-Link I/O. For Phase 2 volumes (a few thousand <c>RA</c> records
/// per 3-month window) this is bounded and simple.
/// </summary>
public sealed class SetupRunner
{
    private static readonly TimeSpan DefaultPollDelay = TimeSpan.FromMilliseconds(500);

    private readonly IJvLink _jvLink;
    private readonly ISchemaProvisioner _provisioner;
    private readonly IBulkWriter<Ra> _raWriter;
    private readonly TimeSpan _pollDelay;

    public SetupRunner(
        IJvLink jvLink,
        ISchemaProvisioner provisioner,
        IBulkWriter<Ra> raWriter,
        TimeSpan? pollDelay = null)
    {
        ArgumentNullException.ThrowIfNull(jvLink);
        ArgumentNullException.ThrowIfNull(provisioner);
        ArgumentNullException.ThrowIfNull(raWriter);

        _jvLink = jvLink;
        _provisioner = provisioner;
        _raWriter = raWriter;
        _pollDelay = pollDelay ?? DefaultPollDelay;
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

        var open = _jvLink.Open(options.Dataspec, options.Fromtime, options.Option);

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

            var (raRecords, counts) = await ReadAllAsync(cancellationToken).ConfigureAwait(false);

            var inserted = await _raWriter.WriteAsync(ToAsyncEnumerable(raRecords, cancellationToken), cancellationToken)
                .ConfigureAwait(false);

            return new SetupReport(
                OpenReturnCode: open.ReturnCode,
                ReadCount: open.ReadCount,
                DownloadCount: open.DownloadCount,
                LastFileTimestamp: open.LastFileTimestamp,
                RecordCountsById: counts,
                RaInserted: inserted);
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
        RaInserted: 0);

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

    private async Task<(List<Ra> Records, Dictionary<string, int> Counts)> ReadAllAsync(CancellationToken cancellationToken)
    {
        var ra = new List<Ra>();
        var counts = new Dictionary<string, int>(StringComparer.Ordinal);

        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var read = _jvLink.Read();
            if (read.ReturnCode > 0)
            {
                var id = ExtractRecordId(read.Buffer);
                counts[id] = counts.GetValueOrDefault(id) + 1;
                if (id == RaDecoder.RecordSpec)
                {
                    ra.Add(RaDecoder.Decode(read.Buffer!));
                }

                continue;
            }

            switch (read.ReturnCode)
            {
                case 0:
                    return (ra, counts);

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

    private static async IAsyncEnumerable<Ra> ToAsyncEnumerable(List<Ra> records, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        foreach (var r in records)
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return r;
            await Task.Yield();
        }
    }
}
