using Jvlink2Db.Core.Records;

namespace Jvlink2Db.Db.Postgres.Tests.Records;

internal static class DmBuilder
{
    private static string[] Filled(int count, string value)
    {
        var arr = new string[count];
        for (var i = 0; i < count; i++)
        {
            arr[i] = value;
        }

        return arr;
    }

    public static Dm Sample() => new()
    {
        RecordSpec = "DM",
        DataKubun = "1",
        MakeDate = "20260331",
        Year = "2026",
        MonthDay = "0331",
        JyoCD = "06",
        Kaiji = "01",
        Nichiji = "08",
        RaceNum = "11",
        MakeHM = "1500",
        Umaban = ["01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15", "16", "", ""],
        DMTime = ["13250", "13280", "13310", "13340", "13360", "13390", "13420", "13450", "13480", "13510", "13540", "13570", "13600", "13630", "13660", "13800", "", ""],
        DMGosaP = Filled(18, "0050"),
        DMGosaM = Filled(18, "0040"),
    };
}
