using System.Threading;
using System.Threading.Tasks;

namespace Jvlink2Db.Pipeline.Setup;

/// <summary>
/// Buffers raw JV-Data record bytes for a single record spec, then on
/// <see cref="FlushAsync"/> decodes them and forwards the typed records
/// to the configured bulk writer.
/// </summary>
public interface IRecordSink
{
    string RecordSpec { get; }

    int BufferedCount { get; }

    /// <summary>
    /// Number of records the sink has dropped because the decoder
    /// couldn't decode the buffer (e.g. legacy spec format). Reported
    /// at end of run; never causes the run itself to fail.
    /// </summary>
    int SkippedCount { get; }

    void Add(byte[] buffer);

    /// <summary>
    /// Discards all records currently buffered for the next flush.
    /// Used by <see cref="SetupRunner"/> to drop partial reads from
    /// a corrupt JV-Link file before retrying the read after a
    /// <c>-402</c>/<c>-403</c> recovery cycle.
    /// </summary>
    void ClearBuffer();

    Task<long> FlushAsync(CancellationToken cancellationToken);
}
