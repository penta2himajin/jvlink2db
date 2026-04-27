using System.Threading;
using System.Threading.Tasks;

namespace Jvlink2Db.Core.Persistence;

/// <summary>
/// Provisions the database schema. Idempotent: calling this on an
/// already-provisioned schema must succeed without changing the
/// observable state.
/// </summary>
public interface ISchemaProvisioner
{
    Task EnsureCreatedAsync(CancellationToken cancellationToken);
}
