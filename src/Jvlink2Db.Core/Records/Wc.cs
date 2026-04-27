namespace Jvlink2Db.Core.Records;

/// <summary>WC (ウッドチップ調教 — wood-chip training). JV-Data §JV_WC_WOOD, 105 bytes.</summary>
public sealed record Wc
{
    public required string RecordSpec { get; init; }
    public required string DataKubun { get; init; }
    public required string MakeDate { get; init; }

    public required string TresenKubun { get; init; }
    public required string ChokyoDate { get; init; }
    public required string ChokyoTime { get; init; }
    public required string KettoNum { get; init; }
    public required string Course { get; init; }
    public required string BabaAround { get; init; }
    public required string Reserved { get; init; }

    public required string HaronTime10 { get; init; }
    public required string LapTime10 { get; init; }
    public required string HaronTime9 { get; init; }
    public required string LapTime9 { get; init; }
    public required string HaronTime8 { get; init; }
    public required string LapTime8 { get; init; }
    public required string HaronTime7 { get; init; }
    public required string LapTime7 { get; init; }
    public required string HaronTime6 { get; init; }
    public required string LapTime6 { get; init; }
    public required string HaronTime5 { get; init; }
    public required string LapTime5 { get; init; }
    public required string HaronTime4 { get; init; }
    public required string LapTime4 { get; init; }
    public required string HaronTime3 { get; init; }
    public required string LapTime3 { get; init; }
    public required string HaronTime2 { get; init; }
    public required string LapTime2 { get; init; }
    public required string LapTime1 { get; init; }
}
