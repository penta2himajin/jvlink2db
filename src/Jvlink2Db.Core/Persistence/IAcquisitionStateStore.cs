using System.Threading;
using System.Threading.Tasks;

namespace Jvlink2Db.Core.Persistence;

/// <summary>
/// Per-(dataspec, option) resume marker. Read at the start of a run
/// to recover the last consumed fromtime / filename, and updated at
/// the end of a successful run.
/// </summary>
public interface IAcquisitionStateStore
{
    Task<AcquisitionState?> GetAsync(string dataspec, int option, CancellationToken cancellationToken);

    Task UpsertAsync(AcquisitionState state, CancellationToken cancellationToken);
}

public sealed record AcquisitionState(
    string Dataspec,
    int Option,
    string? LastFromtime,
    string? LastFilename);
