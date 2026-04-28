using System.Collections.Generic;

namespace Jvlink2Db.Pipeline.Setup;

public sealed record SetupReport(
    int OpenReturnCode,
    int ReadCount,
    int DownloadCount,
    string LastFileTimestamp,
    IReadOnlyDictionary<string, int> RecordCountsById,
    IReadOnlyDictionary<string, long> RecordsInsertedById,
    string? LastConsumedFilename = null)
{
    public long RaInserted => RecordsInsertedById.TryGetValue("RA", out var v) ? v : 0;
}
