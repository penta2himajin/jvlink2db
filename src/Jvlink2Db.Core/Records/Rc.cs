namespace Jvlink2Db.Core.Records;

/// <summary>
/// Decoded RC (コースレコードマスタ — course-record master) record.
/// JV-Data 4.9.0.1 §JV_RC_RECORD, 501 bytes.
/// </summary>
public sealed record Rc
{
    public required string RecordSpec { get; init; }
    public required string DataKubun { get; init; }
    public required string MakeDate { get; init; }

    public required string RecInfoKubun { get; init; }

    public required string Year { get; init; }
    public required string MonthDay { get; init; }
    public required string JyoCD { get; init; }
    public required string Kaiji { get; init; }
    public required string Nichiji { get; init; }
    public required string RaceNum { get; init; }

    public required string TokuNum { get; init; }
    public required string Hondai { get; init; }
    public required string GradeCD { get; init; }
    public required string SyubetuCD { get; init; }
    public required string Kyori { get; init; }
    public required string TrackCD { get; init; }
    public required string RecKubun { get; init; }
    public required string RecTime { get; init; }

    public required string TenkoCD { get; init; }
    public required string SibaBabaCD { get; init; }
    public required string DirtBabaCD { get; init; }

    /// <summary>RecUmaInfo[3] — flattened parallel arrays.</summary>
    public required string[] RecUmaKettoNum { get; init; }
    public required string[] RecUmaBamei { get; init; }
    public required string[] RecUmaUmaKigoCD { get; init; }
    public required string[] RecUmaSexCD { get; init; }
    public required string[] RecUmaChokyosiCode { get; init; }
    public required string[] RecUmaChokyosiName { get; init; }
    public required string[] RecUmaFutan { get; init; }
    public required string[] RecUmaKisyuCode { get; init; }
    public required string[] RecUmaKisyuName { get; init; }
}
