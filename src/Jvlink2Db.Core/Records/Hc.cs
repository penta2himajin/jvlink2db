namespace Jvlink2Db.Core.Records;

/// <summary>HC (坂路調教 — slope training). JV-Data §JV_HC_HANRO, 60 bytes.</summary>
public sealed record Hc
{
    public required string RecordSpec { get; init; }
    public required string DataKubun { get; init; }
    public required string MakeDate { get; init; }

    public required string TresenKubun { get; init; }
    public required string ChokyoDate { get; init; }
    public required string ChokyoTime { get; init; }
    public required string KettoNum { get; init; }
    public required string HaronTime4 { get; init; }
    public required string LapTime4 { get; init; }
    public required string HaronTime3 { get; init; }
    public required string LapTime3 { get; init; }
    public required string HaronTime2 { get; init; }
    public required string LapTime2 { get; init; }
    public required string LapTime1 { get; init; }
}
