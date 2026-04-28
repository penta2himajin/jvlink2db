namespace Jvlink2Db.Core.Records;

/// <summary>CS (コース情報 — course information). JV-Data §JV_CS_COURSE, 6829 bytes.</summary>
public sealed record Cs
{
    public required string RecordSpec { get; init; }
    public required string DataKubun { get; init; }
    public required string MakeDate { get; init; }

    public required string JyoCD { get; init; }
    public required string Kyori { get; init; }
    public required string TrackCD { get; init; }
    public required string KaishuDate { get; init; }
    public required string CourseEx { get; init; }
}
