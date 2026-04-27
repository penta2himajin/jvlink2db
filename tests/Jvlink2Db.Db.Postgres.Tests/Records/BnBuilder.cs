using Jvlink2Db.Core.Records;

namespace Jvlink2Db.Db.Postgres.Tests.Records;

internal static class BnBuilder
{
    public static Bn Empty() => new()
    {
        RecordSpec = "BN",
        DataKubun = string.Empty,
        MakeDate = string.Empty,
        BanusiCode = string.Empty,
        BanusiNameCo = string.Empty,
        BanusiName = string.Empty,
        BanusiNameKana = string.Empty,
        BanusiNameEng = string.Empty,
        Fukusyoku = string.Empty,
        HonRuikeiSetYear = ["", ""],
        HonRuikeiHonsyokinTotal = ["", ""],
        HonRuikeiFukaSyokin = ["", ""],
        HonRuikeiChakuKaisu = new string[12],
    };

    public static Bn Sample(string code = "123456", string name = "テストオーナー") =>
        Empty() with
        {
            DataKubun = "1",
            MakeDate = "20260331",
            BanusiCode = code,
            BanusiName = name,
            Fukusyoku = "赤・白縦縞",
            HonRuikeiSetYear = ["2025", "9999"],
            HonRuikeiHonsyokinTotal = ["0001000000", "0009999999"],
            HonRuikeiFukaSyokin = ["0000200000", "0001234567"],
            HonRuikeiChakuKaisu = ["10", "5", "3", "2", "1", "20", "50", "30", "20", "10", "5", "100"],
        };
}
