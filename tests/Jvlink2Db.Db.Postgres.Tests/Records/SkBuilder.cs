using Jvlink2Db.Core.Records;

namespace Jvlink2Db.Db.Postgres.Tests.Records;

internal static class SkBuilder
{
    public static Sk Sample(string ketto = "2020100123") => new()
    {
        RecordSpec = "SK",
        DataKubun = "1",
        MakeDate = "20260415",
        KettoNum = ketto,
        BirthDate = "20200315",
        SexCD = "1",
        HinsyuCD = "1",
        KeiroCD = "01",
        SankuMochiKubun = "0",
        ImportYear = "0000",
        BreederCode = "00012345",
        SanchiName = "北海道",
        HansyokuNum = ["0001234567", "0007654321", "", "", "", "", "", "", "", "", "", "", "", "0099999999"],
    };
}
