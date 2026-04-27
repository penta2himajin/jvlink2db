using Jvlink2Db.Core.Records;

namespace Jvlink2Db.Db.Postgres.Tests.Records;

internal static class HcBuilder
{
    public static Hc Sample(string ketto = "2020100123", string time = "0800") => new()
    {
        RecordSpec = "HC",
        DataKubun = "1",
        MakeDate = "20260415",
        TresenKubun = "0",
        ChokyoDate = "20260415",
        ChokyoTime = time,
        KettoNum = ketto,
        HaronTime4 = "0567",
        LapTime4 = "150",
        HaronTime3 = "0420",
        LapTime3 = "140",
        HaronTime2 = "0285",
        LapTime2 = "138",
        LapTime1 = "130",
    };
}
