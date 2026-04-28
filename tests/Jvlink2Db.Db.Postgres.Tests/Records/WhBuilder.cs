using System.Linq;
using Jvlink2Db.Core.Records;

namespace Jvlink2Db.Db.Postgres.Tests.Records;

internal static class WhBuilder
{
    public static Wh Sample(string raceNum = "11") => new()
    {
        RecordSpec = "WH",
        DataKubun = "1",
        MakeDate = "20260415",
        Year = "2026",
        MonthDay = "0503",
        JyoCD = "05",
        Kaiji = "02",
        Nichiji = "06",
        RaceNum = raceNum,
        HappyoTime = "05031040",
        Umaban = Enumerable.Range(1, 18).Select(i => i.ToString("D2")).ToArray(),
        Bamei = Enumerable.Range(1, 18).Select(i => $"テスト馬{i}").ToArray(),
        BaTaijyu = Enumerable.Repeat("478", 18).ToArray(),
        ZogenFugo = Enumerable.Repeat("+", 18).ToArray(),
        ZogenSa = Enumerable.Repeat("004", 18).ToArray(),
    };
}
