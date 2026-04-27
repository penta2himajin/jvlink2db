using Jvlink2Db.Core.Records;

namespace Jvlink2Db.Db.Postgres.Tests.Records;

internal static class UmBuilder
{
    public static Um Empty() => new()
    {
        RecordSpec = "UM",
        DataKubun = string.Empty,
        MakeDate = string.Empty,
        KettoNum = string.Empty,
        DelKubun = string.Empty,
        RegDate = string.Empty,
        DelDate = string.Empty,
        BirthDate = string.Empty,
        Bamei = string.Empty,
        BameiKana = string.Empty,
        BameiEng = string.Empty,
        ZaikyuFlag = string.Empty,
        Reserved = string.Empty,
        UmaKigoCD = string.Empty,
        SexCD = string.Empty,
        HinsyuCD = string.Empty,
        KeiroCD = string.Empty,
        KettoHansyokuNum = Repeat("", 14),
        KettoBamei = Repeat("", 14),
        TozaiCD = string.Empty,
        ChokyosiCode = string.Empty,
        ChokyosiRyakusyo = string.Empty,
        Syotai = string.Empty,
        BreederCode = string.Empty,
        BreederName = string.Empty,
        SanchiName = string.Empty,
        BanusiCode = string.Empty,
        BanusiName = string.Empty,
        RuikeiHonsyoHeiti = string.Empty,
        RuikeiHonsyoSyogai = string.Empty,
        RuikeiFukaHeichi = string.Empty,
        RuikeiFukaSyogai = string.Empty,
        RuikeiSyutokuHeichi = string.Empty,
        RuikeiSyutokuSyogai = string.Empty,
        ChakuSogo = Repeat("", 6),
        ChakuChuo = Repeat("", 6),
        ChakuKaisuBa = Repeat("", 42),
        ChakuKaisuJyotai = Repeat("", 72),
        ChakuKaisuKyori = Repeat("", 36),
        Kyakusitu = Repeat("", 4),
        RaceCount = string.Empty,
    };

    public static Um Sample(string kettoNum = "2020100123", string bamei = "テストホース") =>
        Empty() with
        {
            DataKubun = "1",
            MakeDate = "20260331",
            KettoNum = kettoNum,
            BirthDate = "20200315",
            Bamei = bamei,
            SexCD = "1",
            ChokyosiCode = "01234",
            BreederCode = "00056789",
            BanusiCode = "123456",
            RuikeiHonsyoHeiti = "100000000",
            RaceCount = "025",
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
