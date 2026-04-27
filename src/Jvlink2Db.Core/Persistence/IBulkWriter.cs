using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Jvlink2Db.Core.Persistence;

/// <summary>
/// Idempotent bulk writer for one record type. The implementation
/// must guarantee that re-writing the same records leaves the target
/// table observably unchanged (last-write-wins per primary key).
/// </summary>
public interface IBulkWriter<TRecord>
{
    /// <summary>
    /// Writes <paramref name="records"/> to the target table and
    /// returns the number of rows merged.
    /// </summary>
    Task<long> WriteAsync(IAsyncEnumerable<TRecord> records, CancellationToken cancellationToken);
}
