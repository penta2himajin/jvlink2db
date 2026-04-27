using System.Collections.Generic;

namespace Jvlink2Db.Pipeline.Setup;

public sealed record SetupReport(
    int OpenReturnCode,
    int ReadCount,
    int DownloadCount,
    string LastFileTimestamp,
    IReadOnlyDictionary<string, int> RecordCountsById,
    long RaInserted);
