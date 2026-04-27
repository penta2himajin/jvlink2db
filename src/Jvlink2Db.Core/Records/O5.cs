namespace Jvlink2Db.Core.Records;

/// <summary>
/// Decoded O5 (3連複オッズ) record.
/// JV-Data 4.9.0.1 §JV_O5_ODDS_SANREN, 12293 bytes.
/// </summary>
public sealed record O5
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
    public required string SanrenpukuFlag { get; init; }

    /// <summary>3連複 — Kumi (6) / Odds (6) / Ninki (3), 816 entries.</summary>
    public required string[] Kumi { get; init; }
    public required string[] Odds { get; init; }
    public required string[] Ninki { get; init; }

    public required string TotalHyosuSanrenpuku { get; init; }
}
