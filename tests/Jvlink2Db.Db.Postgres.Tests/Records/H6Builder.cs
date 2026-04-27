using Jvlink2Db.Core.Records;

namespace Jvlink2Db.Db.Postgres.Tests.Records;

internal static class H6Builder
{
    public static H6 Empty() => new()
    {
        RecordSpec = "H6",
        DataKubun = string.Empty,
        MakeDate = string.Empty,
        Year = string.Empty,
        MonthDay = string.Empty,
        JyoCD = string.Empty,
        Kaiji = string.Empty,
        Nichiji = string.Empty,
        RaceNum = string.Empty,
        TorokuTosu = string.Empty,
        SyussoTosu = string.Empty,
        HatubaiFlag = string.Empty,
        HenkanUma = Repeat("", 18),
        SanrentanKumi = Repeat("", 4896),
        SanrentanHyo = Repeat("", 4896),
        SanrentanNinki = Repeat("", 4896),
        HyoTotal = Repeat("", 2),
    };

    public static H6 Sample() =>
        Empty() with
        {
            DataKubun = "2",
            MakeDate = "20260331",
            Year = "2026",
            MonthDay = "0331",
            JyoCD = "06",
            Kaiji = "01",
            Nichiji = "08",
            RaceNum = "11",
            TorokuTosu = "16",
            SyussoTosu = "16",
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
