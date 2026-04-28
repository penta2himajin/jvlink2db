namespace Jvlink2Db.Core.Records;

/// <summary>YS (開催スケジュール — year schedule). JV-Data §JV_YS_SCHEDULE, 382 bytes.</summary>
public sealed record Ys
{
    public required string RecordSpec { get; init; }
    public required string DataKubun { get; init; }
    public required string MakeDate { get; init; }

    /// <summary>RACE_ID2 — Year/MonthDay/JyoCD/Kaiji/Nichiji (no RaceNum).</summary>
    public required string Year { get; init; }
    public required string MonthDay { get; init; }
    public required string JyoCD { get; init; }
    public required string Kaiji { get; init; }
    public required string Nichiji { get; init; }

    public required string YoubiCD { get; init; }

    /// <summary>JyusyoInfo[3] — TokuNum, Hondai, Ryakusyo10/6/3, Nkai, GradeCD, SyubetuCD, KigoCD, JyuryoCD, Kyori, TrackCD.</summary>
    public required string[] JyusyoTokuNum { get; init; }
    public required string[] JyusyoHondai { get; init; }
    public required string[] JyusyoRyakusyo10 { get; init; }
    public required string[] JyusyoRyakusyo6 { get; init; }
    public required string[] JyusyoRyakusyo3 { get; init; }
    public required string[] JyusyoNkai { get; init; }
    public required string[] JyusyoGradeCD { get; init; }
    public required string[] JyusyoSyubetuCD { get; init; }
    public required string[] JyusyoKigoCD { get; init; }
    public required string[] JyusyoJyuryoCD { get; init; }
    public required string[] JyusyoKyori { get; init; }
    public required string[] JyusyoTrackCD { get; init; }
}
