using Jvlink2Db.Core.Records;

namespace Jvlink2Db.Db.Postgres.Tests.Records;

internal static class BrBuilder
{
    public static Br Empty() => new()
    {
        RecordSpec = "BR",
        DataKubun = string.Empty,
        MakeDate = string.Empty,
        BreederCode = string.Empty,
        BreederNameCo = string.Empty,
        BreederName = string.Empty,
        BreederNameKana = string.Empty,
        BreederNameEng = string.Empty,
        Address = string.Empty,
        HonRuikeiSetYear = ["", ""],
        HonRuikeiHonsyokinTotal = ["", ""],
        HonRuikeiFukaSyokin = ["", ""],
        HonRuikeiChakuKaisu = new string[12],
    };

    public static Br Sample(string code = "00012345", string name = "テスト牧場") =>
        Empty() with
        {
            DataKubun = "1",
            MakeDate = "20260331",
            BreederCode = code,
            BreederName = name,
            Address = "北海道",
            HonRuikeiSetYear = ["2025", "9999"],
            HonRuikeiHonsyokinTotal = ["0001000000", "0009999999"],
            HonRuikeiFukaSyokin = ["0000200000", "0001234567"],
            HonRuikeiChakuKaisu = ["10", "5", "3", "2", "1", "20", "50", "30", "20", "10", "5", "100"],
        };
}
