namespace Jvlink2Db.Core.Records;

/// <summary>TK (特別登録馬 — special-race entries). JV-Data §JV_TK_TOKUUMA, 21657 bytes.</summary>
public sealed record Tk
{
    public required string RecordSpec { get; init; }
    public required string DataKubun { get; init; }
    public required string MakeDate { get; init; }

    /// <summary>RACE_ID — Year/MonthDay/JyoCD/Kaiji/Nichiji/RaceNum.</summary>
    public required string Year { get; init; }
    public required string MonthDay { get; init; }
    public required string JyoCD { get; init; }
    public required string Kaiji { get; init; }
    public required string Nichiji { get; init; }
    public required string RaceNum { get; init; }

    // RACE_INFO
    public required string YoubiCD { get; init; }
    public required string TokuNum { get; init; }
    public required string Hondai { get; init; }
    public required string Fukudai { get; init; }
    public required string Kakko { get; init; }
    public required string HondaiEng { get; init; }
    public required string FukudaiEng { get; init; }
    public required string KakkoEng { get; init; }
    public required string Ryakusyo10 { get; init; }
    public required string Ryakusyo6 { get; init; }
    public required string Ryakusyo3 { get; init; }
    public required string Kubun { get; init; }
    public required string Nkai { get; init; }

    public required string GradeCD { get; init; }

    // RACE_JYOKEN
    public required string SyubetuCD { get; init; }
    public required string KigoCD { get; init; }
    public required string JyuryoCD { get; init; }
    public required string[] JyokenCD { get; init; }

    public required string Kyori { get; init; }
    public required string TrackCD { get; init; }
    public required string CourseKubunCD { get; init; }
    public required string HandiDate { get; init; }
    public required string TorokuTosu { get; init; }

    /// <summary>TokuUmaInfo[300] — Num/KettoNum/Bamei/UmaKigoCD/SexCD/TozaiCD/ChokyosiCode/ChokyosiRyakusyo/Futan/Koryu.</summary>
    public required string[] TokuNumSeq { get; init; }
    public required string[] KettoNum { get; init; }
    public required string[] Bamei { get; init; }
    public required string[] UmaKigoCD { get; init; }
    public required string[] SexCD { get; init; }
    public required string[] TozaiCD { get; init; }
    public required string[] ChokyosiCode { get; init; }
    public required string[] ChokyosiRyakusyo { get; init; }
    public required string[] Futan { get; init; }
    public required string[] Koryu { get; init; }
}
