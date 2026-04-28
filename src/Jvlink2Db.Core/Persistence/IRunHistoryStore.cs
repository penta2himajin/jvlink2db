using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Jvlink2Db.Core.Persistence;

/// <summary>
/// Append-only run-history log. Each CLI invocation produces one row,
/// inserted when the run starts, and updated when the run finishes
/// (success or failure).
/// </summary>
public interface IRunHistoryStore
{
    Task<long> StartAsync(RunHistoryStart start, CancellationToken cancellationToken);

    Task FinishAsync(long id, RunHistoryFinish finish, CancellationToken cancellationToken);
}

public sealed record RunHistoryStart(
    string Mode,
    string Dataspec,
    int Option,
    string Fromtime,
    DateTimeOffset StartedAt);

public sealed record RunHistoryFinish(
    DateTimeOffset FinishedAt,
    string Outcome,
    int? OpenReturnCode,
    int? ReadCount,
    int? DownloadCount,
    string? LastFileTimestamp,
    IReadOnlyDictionary<string, int>? RecordCounts,
    IReadOnlyDictionary<string, long>? RecordsInserted,
    string? ErrorMessage);
