using Jvlink2Db.Core.Records;

namespace Jvlink2Db.Db.Postgres.Tests.Records;

internal static class O6Builder
{
    public static O6 Empty() => new()
    {
        RecordSpec = "O6",
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
        SanrentanFlag = string.Empty,
        Kumi = Repeat("", 4896),
        Odds = Repeat("", 4896),
        Ninki = Repeat("", 4896),
        TotalHyosuSanrentan = string.Empty,
    };

    public static O6 Sample() =>
        Empty() with
        {
            DataKubun = "3",
            MakeDate = "20260331",
            Year = "2026",
            MonthDay = "0331",
            JyoCD = "06",
            Kaiji = "01",
            Nichiji = "08",
            RaceNum = "11",
            HappyoTime = "03311534",
            TorokuTosu = "16",
            SyussoTosu = "16",
            TotalHyosuSanrentan = "00200000000",
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
