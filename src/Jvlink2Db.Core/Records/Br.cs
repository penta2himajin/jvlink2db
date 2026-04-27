namespace Jvlink2Db.Core.Records;

/// <summary>
/// Decoded BR (生産者マスタ — breeder master) record.
/// JV-Data 4.9.0.1 §JV_BR_BREEDER, 545 bytes.
/// </summary>
public sealed record Br
{
    public required string RecordSpec { get; init; }
    public required string DataKubun { get; init; }
    public required string MakeDate { get; init; }

    public required string BreederCode { get; init; }
    public required string BreederNameCo { get; init; }
    public required string BreederName { get; init; }
    public required string BreederNameKana { get; init; }
    public required string BreederNameEng { get; init; }
    public required string Address { get; init; }

    /// <summary>本年・累計成績情報 — 2 entries × (SetYear / HonSyokinTotal / FukaSyokin / ChakuKaisu[6]).</summary>
    public required string[] HonRuikeiSetYear { get; init; }       // 2
    public required string[] HonRuikeiHonsyokinTotal { get; init; }
    public required string[] HonRuikeiFukaSyokin { get; init; }
    public required string[] HonRuikeiChakuKaisu { get; init; }    // 12 (2 years × 6 placements)
}
