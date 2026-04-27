namespace Jvlink2Db.Core.Records;

/// <summary>
/// Decoded O4 (馬単オッズ) record.
/// JV-Data 4.9.0.1 §JV_O4_ODDS_UMATAN, 4031 bytes.
/// </summary>
public sealed record O4
{
    public required string RecordSpec { get; init; }
    public required string DataKubun { get; init; }
    public required string MakeDate { get; init; }

    public required string Year { get; init; }
    public required string MonthDay { get; init; }
    public required string JyoCD { get; init; }
    public required string Kaiji { get; init; }
    public required string Nichiji { get; init; }
    public required string RaceNum { get; init; }

    public required string HappyoTime { get; init; }
    public required string TorokuTosu { get; init; }
    public required string SyussoTosu { get; init; }
    public required string UmatanFlag { get; init; }

    /// <summary>馬単 — Kumi (4) / Odds (6) / Ninki (3), 306 entries.</summary>
    public required string[] Kumi { get; init; }
    public required string[] Odds { get; init; }
    public required string[] Ninki { get; init; }

    public required string TotalHyosuUmatan { get; init; }
}
