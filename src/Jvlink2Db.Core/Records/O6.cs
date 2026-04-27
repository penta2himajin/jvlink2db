namespace Jvlink2Db.Core.Records;

/// <summary>
/// Decoded O6 (3連単オッズ) record.
/// JV-Data 4.9.0.1 §JV_O6_ODDS_SANRENTAN, 83285 bytes — the largest
/// JV record after JV_WF_INFO.
/// </summary>
public sealed record O6
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
    public required string SanrentanFlag { get; init; }

    /// <summary>3連単 — Kumi (6) / Odds (7) / Ninki (4), 4896 entries.</summary>
    public required string[] Kumi { get; init; }
    public required string[] Odds { get; init; }
    public required string[] Ninki { get; init; }

    public required string TotalHyosuSanrentan { get; init; }
}
