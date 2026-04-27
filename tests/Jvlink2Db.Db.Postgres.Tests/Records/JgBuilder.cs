using Jvlink2Db.Core.Records;

namespace Jvlink2Db.Db.Postgres.Tests.Records;

internal static class JgBuilder
{
    public static Jg Sample(string raceNum = "11", string kettoNum = "2020100123", string bamei = "除外馬") => new()
    {
        RecordSpec = "JG",
        DataKubun = "1",
        MakeDate = "20260331",
        Year = "2026",
        MonthDay = "0331",
        JyoCD = "06",
        Kaiji = "01",
        Nichiji = "08",
        RaceNum = raceNum,
        KettoNum = kettoNum,
        Bamei = bamei,
        ShutsubaTohyoJun = "002",
        ShussoKubun = "1",
        JogaiJotaiKubun = "3",
    };
}
