namespace Jvlink2Db.Core.Records;

/// <summary>
/// Decoded KS (騎手マスタ — jockey master) record.
/// JV-Data 4.9.0.1 §JV_KS_KISYU, 4173 bytes. Primary key is KisyuCode.
/// HonZenRuikei[3] (deeply-nested 1052-byte annual stat blocks)
/// is captured at the byte level only and decoded in a future PR.
/// </summary>
public sealed record Ks
{
    public required string RecordSpec { get; init; }
    public required string DataKubun { get; init; }
    public required string MakeDate { get; init; }

    public required string KisyuCode { get; init; }
    public required string DelKubun { get; init; }
    public required string IssueDate { get; init; }
    public required string DelDate { get; init; }
    public required string BirthDate { get; init; }
    public required string KisyuName { get; init; }
    public required string Reserved { get; init; }
    public required string KisyuNameKana { get; init; }
    public required string KisyuRyakusyo { get; init; }
    public required string KisyuNameEng { get; init; }
    public required string SexCD { get; init; }
    public required string SikakuCD { get; init; }
    public required string MinaraiCD { get; init; }
    public required string TozaiCD { get; init; }
    public required string Syotai { get; init; }
    public required string ChokyosiCode { get; init; }
    public required string ChokyosiRyakusyo { get; init; }

    /// <summary>初騎乗情報[2] — first ride per (heichi / syogai).</summary>
    public required string[] HatuKiJyoYear { get; init; }
    public required string[] HatuKiJyoMonthDay { get; init; }
    public required string[] HatuKiJyoJyoCD { get; init; }
    public required string[] HatuKiJyoKaiji { get; init; }
    public required string[] HatuKiJyoNichiji { get; init; }
    public required string[] HatuKiJyoRaceNum { get; init; }
    public required string[] HatuKiJyoSyussoTosu { get; init; }
    public required string[] HatuKiJyoKettoNum { get; init; }
    public required string[] HatuKiJyoBamei { get; init; }
    public required string[] HatuKiJyoKakuteiJyuni { get; init; }
    public required string[] HatuKiJyoIJyoCD { get; init; }

    /// <summary>初勝利情報[2].</summary>
    public required string[] HatuSyoriYear { get; init; }
    public required string[] HatuSyoriMonthDay { get; init; }
    public required string[] HatuSyoriJyoCD { get; init; }
    public required string[] HatuSyoriKaiji { get; init; }
    public required string[] HatuSyoriNichiji { get; init; }
    public required string[] HatuSyoriRaceNum { get; init; }
    public required string[] HatuSyoriSyussoTosu { get; init; }
    public required string[] HatuSyoriKettoNum { get; init; }
    public required string[] HatuSyoriBamei { get; init; }

    /// <summary>最近重賞勝利情報[3].</summary>
    public required string[] SaikinJyusyoYear { get; init; }
    public required string[] SaikinJyusyoMonthDay { get; init; }
    public required string[] SaikinJyusyoJyoCD { get; init; }
    public required string[] SaikinJyusyoKaiji { get; init; }
    public required string[] SaikinJyusyoNichiji { get; init; }
    public required string[] SaikinJyusyoRaceNum { get; init; }
    public required string[] SaikinJyusyoHondai { get; init; }
    public required string[] SaikinJyusyoRyakusyo10 { get; init; }
    public required string[] SaikinJyusyoRyakusyo6 { get; init; }
    public required string[] SaikinJyusyoRyakusyo3 { get; init; }
    public required string[] SaikinJyusyoGradeCD { get; init; }
    public required string[] SaikinJyusyoSyussoTosu { get; init; }
    public required string[] SaikinJyusyoKettoNum { get; init; }
    public required string[] SaikinJyusyoBamei { get; init; }
}
