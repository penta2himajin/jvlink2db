namespace Jvlink2Db.Core.Records;

/// <summary>
/// Decoded CH (調教師マスタ — trainer master) record.
/// JV-Data 4.9.0.1 §JV_CH_CHOKYOSI, 3862 bytes. Primary key is
/// ChokyosiCode. HonZenRuikei[3] (deeply-nested annual stat blocks)
/// is captured in the spec but deferred to a future PR.
/// </summary>
public sealed record Ch
{
    public required string RecordSpec { get; init; }
    public required string DataKubun { get; init; }
    public required string MakeDate { get; init; }

    public required string ChokyosiCode { get; init; }
    public required string DelKubun { get; init; }
    public required string IssueDate { get; init; }
    public required string DelDate { get; init; }
    public required string BirthDate { get; init; }
    public required string ChokyosiName { get; init; }
    public required string ChokyosiNameKana { get; init; }
    public required string ChokyosiRyakusyo { get; init; }
    public required string ChokyosiNameEng { get; init; }
    public required string SexCD { get; init; }
    public required string TozaiCD { get; init; }
    public required string Syotai { get; init; }

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
