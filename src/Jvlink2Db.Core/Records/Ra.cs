namespace Jvlink2Db.Core.Records;

/// <summary>
/// Decoded RA (race detail) record. One instance per (Year, MonthDay,
/// JyoCD, Kaiji, Nichiji, RaceNum) tuple. All fields are right-trimmed
/// strings; numeric coercion is the database layer's responsibility.
/// </summary>
public sealed record Ra
{
    // <レコードヘッダー> — JV-Data RECORD_ID, bytes 1..11
    public required string RecordSpec { get; init; }
    public required string DataKubun { get; init; }
    public required string MakeDate { get; init; }

    // <競走識別情報> — JV-Data RACE_ID, bytes 12..27
    public required string Year { get; init; }
    public required string MonthDay { get; init; }
    public required string JyoCD { get; init; }
    public required string Kaiji { get; init; }
    public required string Nichiji { get; init; }
    public required string RaceNum { get; init; }

    // <レース情報> — JV-Data RACE_INFO, bytes 28..614
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

    // グレード — bytes 615..616
    public required string GradeCD { get; init; }
    public required string GradeCDBefore { get; init; }

    // <競走条件コード> — JV-Data RACE_JYOKEN, bytes 617..637
    public required string SyubetuCD { get; init; }
    public required string KigoCD { get; init; }
    public required string JyuryoCD { get; init; }
    public required string[] JyokenCD { get; init; } // 5 entries

    public required string JyokenName { get; init; }
    public required string Kyori { get; init; }
    public required string KyoriBefore { get; init; }
    public required string TrackCD { get; init; }
    public required string TrackCDBefore { get; init; }
    public required string CourseKubunCD { get; init; }
    public required string CourseKubunCDBefore { get; init; }

    public required string[] Honsyokin { get; init; }       // 7 entries
    public required string[] HonsyokinBefore { get; init; } // 5 entries
    public required string[] Fukasyokin { get; init; }      // 5 entries
    public required string[] FukasyokinBefore { get; init; } // 3 entries

    public required string HassoTime { get; init; }
    public required string HassoTimeBefore { get; init; }
    public required string TorokuTosu { get; init; }
    public required string SyussoTosu { get; init; }
    public required string NyusenTosu { get; init; }

    // <天候・馬場状態> — TENKO_BABA_INFO, bytes 888..890
    public required string TenkoCD { get; init; }
    public required string SibaBabaCD { get; init; }
    public required string DirtBabaCD { get; init; }

    public required string[] LapTime { get; init; } // 25 entries

    public required string SyogaiMileTime { get; init; }
    public required string HaronTimeS3 { get; init; }
    public required string HaronTimeS4 { get; init; }
    public required string HaronTimeL3 { get; init; }
    public required string HaronTimeL4 { get; init; }

    // <コーナー通過順位> — CORNER_INFO[4], bytes 982..1269
    public required RaCorner[] Corners { get; init; }

    public required string RecordUpKubun { get; init; }
}

public sealed record RaCorner(string Corner, string Syukaisu, string Jyuni);
