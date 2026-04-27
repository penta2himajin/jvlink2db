namespace Jvlink2Db.Core.Records;

/// <summary>
/// Decoded O1 (単勝・複勝・枠連オッズ) record.
/// JV-Data 4.9.0.1 §JV_O1_ODDS_TANFUKUWAKU, 962 bytes.
/// </summary>
public sealed record O1
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

    /// <summary>発表月日時分 (MMDDHHMM, 8 chars). Part of the PK.</summary>
    public required string HappyoTime { get; init; }
    public required string TorokuTosu { get; init; }
    public required string SyussoTosu { get; init; }

    public required string TansyoFlag { get; init; }
    public required string FukusyoFlag { get; init; }
    public required string WakurenFlag { get; init; }
    public required string FukuChakuBaraiKey { get; init; }

    /// <summary>単勝 — Umaban (2) / Odds (4) / Ninki (2), 28 entries.</summary>
    public required string[] TansyoUmaban { get; init; }
    public required string[] TansyoOdds { get; init; }
    public required string[] TansyoNinki { get; init; }

    /// <summary>複勝 — Umaban (2) / OddsLow (4) / OddsHigh (4) / Ninki (2), 28 entries.</summary>
    public required string[] FukusyoUmaban { get; init; }
    public required string[] FukusyoOddsLow { get; init; }
    public required string[] FukusyoOddsHigh { get; init; }
    public required string[] FukusyoNinki { get; init; }

    /// <summary>枠連 — Kumi (2) / Odds (5) / Ninki (2), 36 entries.</summary>
    public required string[] WakurenKumi { get; init; }
    public required string[] WakurenOdds { get; init; }
    public required string[] WakurenNinki { get; init; }

    public required string TotalHyosuTansyo { get; init; }
    public required string TotalHyosuFukusyo { get; init; }
    public required string TotalHyosuWakuren { get; init; }
}
