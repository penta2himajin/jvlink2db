namespace Jvlink2Db.Core.Records;

/// <summary>
/// Decoded UM (競走馬マスタ — horse master) record.
/// JV-Data 4.9.0.1 §JV_UM_UMA, 1609 bytes. Primary key is KettoNum.
/// </summary>
public sealed record Um
{
    public required string RecordSpec { get; init; }
    public required string DataKubun { get; init; }
    public required string MakeDate { get; init; }

    public required string KettoNum { get; init; }
    public required string DelKubun { get; init; }
    public required string RegDate { get; init; }
    public required string DelDate { get; init; }
    public required string BirthDate { get; init; }
    public required string Bamei { get; init; }
    public required string BameiKana { get; init; }
    public required string BameiEng { get; init; }
    public required string ZaikyuFlag { get; init; }
    public required string Reserved { get; init; }
    public required string UmaKigoCD { get; init; }
    public required string SexCD { get; init; }
    public required string HinsyuCD { get; init; }
    public required string KeiroCD { get; init; }

    /// <summary>Ketto3Info[14] — 3代血統情報 (HansyokuNum / Bamei).</summary>
    public required string[] KettoHansyokuNum { get; init; }
    public required string[] KettoBamei { get; init; }

    public required string TozaiCD { get; init; }
    public required string ChokyosiCode { get; init; }
    public required string ChokyosiRyakusyo { get; init; }
    public required string Syotai { get; init; }
    public required string BreederCode { get; init; }
    public required string BreederName { get; init; }
    public required string SanchiName { get; init; }
    public required string BanusiCode { get; init; }
    public required string BanusiName { get; init; }

    public required string RuikeiHonsyoHeiti { get; init; }
    public required string RuikeiHonsyoSyogai { get; init; }
    public required string RuikeiFukaHeichi { get; init; }
    public required string RuikeiFukaSyogai { get; init; }
    public required string RuikeiSyutokuHeichi { get; init; }
    public required string RuikeiSyutokuSyogai { get; init; }

    /// <summary>総合着回数 — 6 placements.</summary>
    public required string[] ChakuSogo { get; init; }
    public required string[] ChakuChuo { get; init; }

    /// <summary>馬場別着回数 — 7 ba × 6 placements (flat, ba*6 + placement).</summary>
    public required string[] ChakuKaisuBa { get; init; }
    /// <summary>馬場状態別着回数 — 12 jyotai × 6 placements (flat, jyotai*6 + placement).</summary>
    public required string[] ChakuKaisuJyotai { get; init; }
    /// <summary>距離別着回数 — 6 kyori × 6 placements (flat, kyori*6 + placement).</summary>
    public required string[] ChakuKaisuKyori { get; init; }

    public required string[] Kyakusitu { get; init; }     // 4

    public required string RaceCount { get; init; }
}
