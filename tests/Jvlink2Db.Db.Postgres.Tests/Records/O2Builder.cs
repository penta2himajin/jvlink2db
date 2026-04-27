using Jvlink2Db.Core.Records;

namespace Jvlink2Db.Db.Postgres.Tests.Records;

internal static class O2Builder
{
    public static O2 Empty() => new()
    {
        RecordSpec = "O2",
        DataKubun = string.Empty,
        MakeDate = string.Empty,
        Year = string.Empty,
        MonthDay = string.Empty,
        JyoCD = string.Empty,
        Kaiji = string.Empty,
        Nichiji = string.Empty,
        RaceNum = string.Empty,
        HappyoTime = string.Empty,
        TorokuTosu = string.Empty,
        SyussoTosu = string.Empty,
        UmarenFlag = string.Empty,
        Kumi = Repeat("", 153),
        Odds = Repeat("", 153),
        Ninki = Repeat("", 153),
        TotalHyosuUmaren = string.Empty,
    };

    public static O2 Sample(string raceNum = "11", string happyoTm = "03311534") =>
        Empty() with
        {
            DataKubun = "3",
            MakeDate = "20260331",
            Year = "2026",
            MonthDay = "0331",
            JyoCD = "06",
            Kaiji = "01",
            Nichiji = "08",
            RaceNum = raceNum,
            HappyoTime = happyoTm,
            TorokuTosu = "16",
            SyussoTosu = "16",
            TotalHyosuUmaren = "01000000000",
        };

    private static string[] Repeat(string value, int count)
    {
        var arr = new string[count];
        for (var i = 0; i < count; i++)
        {
            arr[i] = value;
        }

        return arr;
    }
}
