using Jvlink2Db.Core.Records;

namespace Jvlink2Db.Db.Postgres.Tests.Records;

internal static class O1Builder
{
    public static O1 Empty() => new()
    {
        RecordSpec = "O1",
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
        TansyoFlag = string.Empty,
        FukusyoFlag = string.Empty,
        WakurenFlag = string.Empty,
        FukuChakuBaraiKey = string.Empty,
        TansyoUmaban = Repeat("", 28),
        TansyoOdds = Repeat("", 28),
        TansyoNinki = Repeat("", 28),
        FukusyoUmaban = Repeat("", 28),
        FukusyoOddsLow = Repeat("", 28),
        FukusyoOddsHigh = Repeat("", 28),
        FukusyoNinki = Repeat("", 28),
        WakurenKumi = Repeat("", 36),
        WakurenOdds = Repeat("", 36),
        WakurenNinki = Repeat("", 36),
        TotalHyosuTansyo = string.Empty,
        TotalHyosuFukusyo = string.Empty,
        TotalHyosuWakuren = string.Empty,
    };

    public static O1 Sample(string raceNum = "11", string happyoTm = "03311534") =>
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
            TotalHyosuTansyo = "01000000000",
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
