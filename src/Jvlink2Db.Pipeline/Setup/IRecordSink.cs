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
