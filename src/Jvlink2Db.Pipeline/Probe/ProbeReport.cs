using System.Collections.Generic;

namespace Jvlink2Db.Pipeline.Probe;

public sealed record ProbeReport(
    int OpenReturnCode,
    int ReadCount,
    int DownloadCount,
    string LastFileTimestamp,
    IReadOnlyDictionary<string, int> RecordCountsById,
    IReadOnlyList<string> Filenames,
    IReadOnlyList<string> SampleRecordIds);
