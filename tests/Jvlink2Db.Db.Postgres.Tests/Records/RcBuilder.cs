using Jvlink2Db.Core.Records;

namespace Jvlink2Db.Db.Postgres.Tests.Records;

internal static class RcBuilder
{
    public static Rc Empty() => new()
    {
        RecordSpec = "RC",
        DataKubun = string.Empty,
        MakeDate = string.Empty,
        RecInfoKubun = string.Empty,
        Year = string.Empty,
        MonthDay = string.Empty,
        JyoCD = string.Empty,
        Kaiji = string.Empty,
        Nichiji = string.Empty,
        RaceNum = string.Empty,
        TokuNum = string.Empty,
        Hondai = string.Empty,
        GradeCD = string.Empty,
        SyubetuCD = string.Empty,
        Kyori = string.Empty,
        TrackCD = string.Empty,
        RecKubun = string.Empty,
        RecTime = string.Empty,
        TenkoCD = string.Empty,
        SibaBabaCD = string.Empty,
        DirtBabaCD = string.Empty,
        RecUmaKettoNum = ["", "", ""],
        RecUmaBamei = ["", "", ""],
        RecUmaUmaKigoCD = ["", "", ""],
        RecUmaSexCD = ["", "", ""],
        RecUmaChokyosiCode = ["", "", ""],
        RecUmaChokyosiName = ["", "", ""],
        RecUmaFutan = ["", "", ""],
        RecUmaKisyuCode = ["", "", ""],
        RecUmaKisyuName = ["", "", ""],
    };

    public static Rc Sample(string recInfoKubun = "1", string raceNum = "11", string hondai = "有馬記念") =>
        Empty() with
        {
            DataKubun = "1",
            MakeDate = "20260331",
            RecInfoKubun = recInfoKubun,
            Year = "2026",
            MonthDay = "0331",
            JyoCD = "06",
            Kaiji = "01",
            Nichiji = "08",
            RaceNum = raceNum,
            Hondai = hondai,
            GradeCD = "E",
            Kyori = "2500",
            TrackCD = "23",
            RecTime = "1485",
            RecUmaKettoNum = ["2018104567", "2019100000", "2020100000"],
            RecUmaBamei = ["ホースＡ", "ホースＢ", "ホースＣ"],
            RecUmaFutan = ["570", "560", "550"],
        };
}
