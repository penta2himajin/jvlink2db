using Jvlink2Db.Core.Records;

namespace Jvlink2Db.Db.Postgres.Tests.Records;

internal static class HsBuilder
{
    public static Hs Sample(string ketto = "2020100123", string saleCode = "000001") => new()
    {
        RecordSpec = "HS",
        DataKubun = "1",
        MakeDate = "20260415",
        KettoNum = ketto,
        HansyokuFNum = "0001234567",
        HansyokuMNum = "0007654321",
        BirthYear = "2020",
        SaleCode = saleCode,
        SaleHostName = "テスト主催",
        SaleName = "テスト市場",
        FromDate = "20210701",
        ToDate = "20210703",
        Barei = "1",
        Price = "0050000000",
    };
}
