using Jvlink2Db.Core.Records;

namespace Jvlink2Db.Db.Postgres.Tests.Records;

internal static class HnBuilder
{
    public static Hn Sample(string hansyokuNum = "0099999999") => new()
    {
        RecordSpec = "HN",
        DataKubun = "1",
        MakeDate = "20260415",
        HansyokuNum = hansyokuNum,
        Reserved = string.Empty,
        KettoNum = "2018104567",
        DelKubun = "0",
        Bamei = "テスト繁殖馬",
        BameiKana = string.Empty,
        BameiEng = string.Empty,
        BirthYear = "2018",
        SexCD = "2",
        HinsyuCD = "1",
        KeiroCD = "01",
        HansyokuMochiKubun = "0",
        ImportYear = "0000",
        SanchiName = "北海道",
        HansyokuFNum = "0001234567",
        HansyokuMNum = "0007654321",
    };
}
