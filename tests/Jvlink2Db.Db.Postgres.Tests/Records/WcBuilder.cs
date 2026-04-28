using Jvlink2Db.Core.Records;

namespace Jvlink2Db.Db.Postgres.Tests.Records;

internal static class WcBuilder
{
    public static Wc Sample(string ketto = "2020100123", string time = "0800") => new()
    {
        RecordSpec = "WC",
        DataKubun = "1",
        MakeDate = "20260415",
        TresenKubun = "0",
        ChokyoDate = "20260415",
        ChokyoTime = time,
        KettoNum = ketto,
        Course = "A",
        BabaAround = "1",
        Reserved = " ",
        HaronTime10 = "1450", LapTime10 = "150",
        HaronTime9 = "1300", LapTime9 = "148",
        HaronTime8 = "1152", LapTime8 = "146",
        HaronTime7 = "1006", LapTime7 = "144",
        HaronTime6 = "0862", LapTime6 = "142",
        HaronTime5 = "0720", LapTime5 = "140",
        HaronTime4 = "0580", LapTime4 = "138",
        HaronTime3 = "0442", LapTime3 = "136",
        HaronTime2 = "0306", LapTime2 = "133",
        LapTime1 = "130",
    };
}
