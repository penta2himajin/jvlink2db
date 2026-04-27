using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Jvlink2Db.Core.Jvlink;

namespace Jvlink2Db.Pipeline.Probe;

public sealed class ProbeRunner
{
    private static readonly TimeSpan DefaultPollDelay = TimeSpan.FromMilliseconds(500);

    private readonly IJvLink _jvLink;
    private readonly TimeSpan _pollDelay;

    public ProbeRunner(IJvLink jvLink, TimeSpan? pollDelay = null)
    {
        _jvLink = jvLink;
        _pollDelay = pollDelay ?? DefaultPollDelay;
    }

    public async Task<ProbeReport> RunAsync(ProbeOptions options, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(options);

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

            return await ReadAllAsync(open, options.MaxSamples, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            _ = _jvLink.Close();
        }
    }

    private static ProbeReport Empty(JvLinkOpenResult open) => new(
        OpenReturnCode: open.ReturnCode,
        ReadCount: open.ReadCount,
        DownloadCount: open.DownloadCount,
        LastFileTimestamp: open.LastFileTimestamp,
        RecordCountsById: new Dictionary<string, int>(),
        Filenames: Array.Empty<string>(),
        SampleRecordIds: Array.Empty<string>());

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

    private async Task<ProbeReport> ReadAllAsync(JvLinkOpenResult open, int maxSamples, CancellationToken cancellationToken)
    {
        var counts = new Dictionary<string, int>(StringComparer.Ordinal);
        var samples = new List<string>(maxSamples);
        var filenames = new List<string>();

        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var read = _jvLink.Read();

            if (read.ReturnCode > 0)
            {
                var id = ExtractRecordId(read.Buffer);
                counts[id] = counts.GetValueOrDefault(id) + 1;
                if (samples.Count < maxSamples)
                {
                    samples.Add(id);
                }

                continue;
            }

            switch (read.ReturnCode)
            {
                case 0:
                    return new ProbeReport(
                        OpenReturnCode: open.ReturnCode,
                        ReadCount: open.ReadCount,
                        DownloadCount: open.DownloadCount,
                        LastFileTimestamp: open.LastFileTimestamp,
                        RecordCountsById: counts,
                        Filenames: filenames,
                        SampleRecordIds: samples);

                case -1:
                    if (read.Filename is { Length: > 0 } name)
                    {
                        filenames.Add(name);
                    }

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
