namespace Jvlink2Db.Core.Records;

/// <summary>CK (出走別着度数 — race-entry placement counts). JV-Data §JV_CK_CHAKU, 6870 bytes.</summary>
/// <remarks>
/// Top-level scalar fields and identifying codes for the per-race subjects (horse,
/// jockey, trainer, owner, breeder). Deeply-nested CHAKUKAISU3/4/5 placement arrays
/// and the 1220-byte HonRuikei[2] annual stat blocks are deferred to a follow-up,
/// matching the KS/CH precedent.
/// </remarks>
public sealed record Ck
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

    // UmaChaku (subset)
    public required string KettoNum { get; init; }
    public required string Bamei { get; init; }
    public required string RuikeiHonsyoHeiti { get; init; }
    public required string RuikeiHonsyoSyogai { get; init; }
    public required string RuikeiFukaHeichi { get; init; }
    public required string RuikeiFukaSyogai { get; init; }
    public required string RuikeiSyutokuHeichi { get; init; }
    public required string RuikeiSyutokuSyogai { get; init; }
    public required string[] Kyakusitu { get; init; }
    public required string RaceCount { get; init; }

    // KisyuChaku (subset)
    public required string KisyuCode { get; init; }
    public required string KisyuName { get; init; }

    // ChokyoChaku (subset)
    public required string ChokyosiCode { get; init; }
    public required string ChokyosiName { get; init; }

    // BanusiChaku (subset)
    public required string BanusiCode { get; init; }
    public required string BanusiNameCo { get; init; }
    public required string BanusiName { get; init; }

    // BreederChaku (subset)
    public required string BreederCode { get; init; }
    public required string BreederNameCo { get; init; }
    public required string BreederName { get; init; }
}
