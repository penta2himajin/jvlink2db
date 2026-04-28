namespace Jvlink2Db.Core.Records;

/// <summary>WH (馬体重 — horse weight). JV-Data §JV_WH_BATAIJYU, 847 bytes.</summary>
public sealed record Wh
{
    public required string RecordSpec { get; init; }
    public required string DataKubun { get; init; }
    public required string MakeDate { get; init; }

    /// <summary>RACE_ID — Year/MonthDay/JyoCD/Kaiji/Nichiji/RaceNum.</summary>
    public required string Year { get; init; }
    public required string MonthDay { get; init; }
    public required string JyoCD { get; init; }
    public required string Kaiji { get; init; }
    public required string Nichiji { get; init; }
    public required string RaceNum { get; init; }

    public required string HappyoTime { get; init; }

    /// <summary>BataijyuInfo[18] — Umaban, Bamei, BaTaijyu, ZogenFugo, ZogenSa.</summary>
    public required string[] Umaban { get; init; }
    public required string[] Bamei { get; init; }
    public required string[] BaTaijyu { get; init; }
    public required string[] ZogenFugo { get; init; }
    public required string[] ZogenSa { get; init; }
}
