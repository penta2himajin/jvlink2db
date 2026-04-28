using Jvlink2Db.Core.Records;

namespace Jvlink2Db.Db.Postgres.Tests.Records;

internal static class CkBuilder
{
    public static Ck Sample(string ketto = "2020100123") => new()
    {
        RecordSpec = "CK",
        DataKubun = "1",
        MakeDate = "20260415",
        Year = "2026",
        MonthDay = "0503",
        JyoCD = "05",
        Kaiji = "02",
        Nichiji = "06",
        RaceNum = "11",
        KettoNum = ketto,
        Bamei = "テスト馬",
        RuikeiHonsyoHeiti = "000123450",
        RuikeiHonsyoSyogai = "000000000",
        RuikeiFukaHeichi = "000012345",
        RuikeiFukaSyogai = "000000000",
        RuikeiSyutokuHeichi = "000045678",
        RuikeiSyutokuSyogai = "000000000",
        Kyakusitu = ["050", "030", "015", "005"],
        RaceCount = "042",
        KisyuCode = "00001",
        KisyuName = "テスト騎手",
        ChokyosiCode = "00099",
        ChokyosiName = "テスト調教師",
        BanusiCode = "000123",
        BanusiNameCo = "テスト馬主株式会社",
        BanusiName = "テスト馬主",
        BreederCode = "00012345",
        BreederNameCo = "テスト牧場有限会社",
        BreederName = "テスト牧場",
    };
}
