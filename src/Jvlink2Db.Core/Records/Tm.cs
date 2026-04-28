namespace Jvlink2Db.Core.Records;

/// <summary>TM (対戦型データマイニング予想 — head-to-head DM). JV-Data §JV_TM_INFO, 141 bytes.</summary>
public sealed record Tm
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

    public required string MakeHM { get; init; }

    /// <summary>TMInfo[18] — Umaban (2) / TMScore (4).</summary>
    public required string[] Umaban { get; init; }
    public required string[] TMScore { get; init; }
}
