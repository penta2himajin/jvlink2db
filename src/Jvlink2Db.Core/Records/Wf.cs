namespace Jvlink2Db.Core.Records;

/// <summary>
/// Decoded WF (WIN5) record. One row per kaisai-date.
/// JV-Data 4.9.0.1 §JV_WF_INFO, 7215 bytes.
/// </summary>
public sealed record Wf
{
    public required string RecordSpec { get; init; }
    public required string DataKubun { get; init; }
    public required string MakeDate { get; init; }

    /// <summary>開催年月日 — primary key.</summary>
    public required string KaisaiDate { get; init; }
    public required string Reserved1 { get; init; }

    /// <summary>5 races that make up the WIN5 — JyoCD / Kaiji / Nichiji / RaceNum parallel arrays.</summary>
    public required string[] RaceJyoCD { get; init; }   // 5
    public required string[] RaceKaiji { get; init; }
    public required string[] RaceNichiji { get; init; }
    public required string[] RaceNum { get; init; }

    public required string Reserved2 { get; init; }
    public required string HatsubaiHyo { get; init; }
    public required string[] YukoHyo { get; init; }     // 5

    public required string HenkanFlag { get; init; }
    public required string FuseiritsuFlag { get; init; }
    public required string TekichunashiFlag { get; init; }
    public required string COShoki { get; init; }
    public required string COZanDaka { get; init; }

    /// <summary>重勝式払戻 — 243 entries × (Kumiban / Pay / Tekichu_Hyo).</summary>
    public required string[] PayKumiban { get; init; }    // 243
    public required string[] PayAmount { get; init; }
    public required string[] PayTekichuHyo { get; init; }
}
