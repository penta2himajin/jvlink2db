using Jvlink2Db.Core.Records;

namespace Jvlink2Db.Db.Postgres.Tests.Records;

internal static class YsBuilder
{
    public static Ys Sample(string year = "2026", string monthDay = "0503", string nichiji = "06") => new()
    {
        RecordSpec = "YS",
        DataKubun = "1",
        MakeDate = "20260415",
        Year = year,
        MonthDay = monthDay,
        JyoCD = "05",
        Kaiji = "02",
        Nichiji = nichiji,
        YoubiCD = "1",
        JyusyoTokuNum = ["0011", "0000", "0000"],
        JyusyoHondai = ["天皇賞(春)", "", ""],
        JyusyoRyakusyo10 = ["天皇賞(春)", "", ""],
        JyusyoRyakusyo6 = ["天皇賞", "", ""],
        JyusyoRyakusyo3 = ["天皇", "", ""],
        JyusyoNkai = ["173", "000", "000"],
        JyusyoGradeCD = ["A", "0", "0"],
        JyusyoSyubetuCD = ["11", "00", "00"],
        JyusyoKigoCD = ["A00", "000", "000"],
        JyusyoJyuryoCD = ["3", "0", "0"],
        JyusyoKyori = ["3200", "0000", "0000"],
        JyusyoTrackCD = ["10", "00", "00"],
    };
}
