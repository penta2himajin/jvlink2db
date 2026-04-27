namespace Jvlink2Db.Core.Records;

/// <summary>
/// Decoded BN (馬主マスタ — owner master) record.
/// JV-Data 4.9.0.1 §JV_BN_BANUSI, 477 bytes.
/// </summary>
public sealed record Bn
{
    public required string RecordSpec { get; init; }
    public required string DataKubun { get; init; }
    public required string MakeDate { get; init; }

    public required string BanusiCode { get; init; }
    public required string BanusiNameCo { get; init; }
    public required string BanusiName { get; init; }
    public required string BanusiNameKana { get; init; }
    public required string BanusiNameEng { get; init; }
    public required string Fukusyoku { get; init; }

    public required string[] HonRuikeiSetYear { get; init; }
    public required string[] HonRuikeiHonsyokinTotal { get; init; }
    public required string[] HonRuikeiFukaSyokin { get; init; }
    public required string[] HonRuikeiChakuKaisu { get; init; }
}
