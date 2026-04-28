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

    void Add(byte[] buffer);

    Task<long> FlushAsync(CancellationToken cancellationToken);
}
