namespace Jvlink2Db.Core.Records;

/// <summary>DM (タイム型データマイニング予想 — time DM). JV-Data §JV_DM_INFO, 303 bytes.</summary>
public sealed record Dm
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

    /// <summary>DMInfo[18] — Umaban (2) / DMTime (5) / DMGosaP (4) / DMGosaM (4).</summary>
    public required string[] Umaban { get; init; }
    public required string[] DMTime { get; init; }
    public required string[] DMGosaP { get; init; }
    public required string[] DMGosaM { get; init; }
}
