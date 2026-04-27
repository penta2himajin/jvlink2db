using Jvlink2Db.Core.Records;

namespace Jvlink2Db.Db.Postgres.Tests.Records;

internal static class H1Builder
{
    public static H1 Empty() => new()
    {
        RecordSpec = "H1",
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
        HatubaiFlag = Repeat("", 7),
        FukuChakuBaraiKey = string.Empty,
        HenkanUma = Repeat("", 28),
        HenkanWaku = Repeat("", 8),
        HenkanDoWaku = Repeat("", 8),
        TansyoUmaban = Repeat("", 28),
        TansyoHyo = Repeat("", 28),
        TansyoNinki = Repeat("", 28),
        FukusyoUmaban = Repeat("", 28),
        FukusyoHyo = Repeat("", 28),
        FukusyoNinki = Repeat("", 28),
        WakurenKumi = Repeat("", 36),
        WakurenHyo = Repeat("", 36),
        WakurenNinki = Repeat("", 36),
        UmarenKumi = Repeat("", 153),
        UmarenHyo = Repeat("", 153),
        UmarenNinki = Repeat("", 153),
        WideKumi = Repeat("", 153),
        WideHyo = Repeat("", 153),
        WideNinki = Repeat("", 153),
        UmatanKumi = Repeat("", 306),
        UmatanHyo = Repeat("", 306),
        UmatanNinki = Repeat("", 306),
        SanrenpukuKumi = Repeat("", 816),
        SanrenpukuHyo = Repeat("", 816),
        SanrenpukuNinki = Repeat("", 816),
        HyoTotal = Repeat("", 14),
    };

    public static H1 Sample() =>
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
