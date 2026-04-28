using System.Linq;
using Jvlink2Db.Core.Records;

namespace Jvlink2Db.Db.Postgres.Tests.Records;

internal static class TkBuilder
{
    public static Tk Sample(string raceNum = "11") => new()
    {
        RecordSpec = "TK",
        DataKubun = "1",
        MakeDate = "20260415",
        Year = "2026",
        MonthDay = "0503",
        JyoCD = "05",
        Kaiji = "02",
        Nichiji = "06",
        RaceNum = raceNum,
        YoubiCD = "1",
        TokuNum = "0011",
        Hondai = "天皇賞(春)",
        Fukudai = "",
        Kakko = "",
        HondaiEng = "Tenno Sho Spring",
        FukudaiEng = "",
        KakkoEng = "",
        Ryakusyo10 = "天皇賞(春)",
        Ryakusyo6 = "天皇賞",
        Ryakusyo3 = "天皇",
        Kubun = "1",
        Nkai = "173",
        GradeCD = "A",
        SyubetuCD = "11",
        KigoCD = "A00",
        JyuryoCD = "3",
        JyokenCD = ["000", "000", "000", "000", "000"],
        Kyori = "3200",
        TrackCD = "10",
        CourseKubunCD = "",
        HandiDate = "20260415",
        TorokuTosu = "018",
        TokuNumSeq = Enumerable.Range(1, 300).Select(i => i.ToString("D3")).ToArray(),
        KettoNum = Enumerable.Range(1, 300).Select(i => $"20201{i:D5}").ToArray(),
        Bamei = Enumerable.Range(1, 300).Select(i => $"テスト馬{i}").ToArray(),
        UmaKigoCD = Enumerable.Repeat("00", 300).ToArray(),
        SexCD = Enumerable.Repeat("1", 300).ToArray(),
        TozaiCD = Enumerable.Repeat("1", 300).ToArray(),
        ChokyosiCode = Enumerable.Repeat("00001", 300).ToArray(),
        ChokyosiRyakusyo = Enumerable.Repeat("テスト師", 300).ToArray(),
        Futan = Enumerable.Repeat("580", 300).ToArray(),
        Koryu = Enumerable.Repeat("0", 300).ToArray(),
    };
}
