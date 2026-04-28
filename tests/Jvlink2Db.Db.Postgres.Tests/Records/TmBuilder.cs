using Jvlink2Db.Core.Records;

namespace Jvlink2Db.Db.Postgres.Tests.Records;

internal static class TmBuilder
{
    public static Tm Sample() => new()
    {
        RecordSpec = "TM",
        DataKubun = "1",
        MakeDate = "20260331",
        Year = "2026",
        MonthDay = "0331",
        JyoCD = "06",
        Kaiji = "01",
        Nichiji = "08",
        RaceNum = "11",
        MakeHM = "1500",
        Umaban = ["01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15", "16", "", ""],
        TMScore = ["0850", "0820", "0790", "0750", "0720", "0700", "0680", "0650", "0620", "0600", "0580", "0550", "0520", "0500", "0480", "0210", "", ""],
    };
}
