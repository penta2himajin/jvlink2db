namespace Jvlink2Db.Core.Records;

/// <summary>
/// Decoded JG (競走馬除外情報 — withdrawal info) record. One per
/// (race, withdrawn horse). JV-Data 4.9.0.1 §JV_JG_JOGAIBA, 80 bytes.
/// </summary>
public sealed record Jg
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

    public required string KettoNum { get; init; }
    public required string Bamei { get; init; }
    public required string ShutsubaTohyoJun { get; init; }
    public required string ShussoKubun { get; init; }
    public required string JogaiJotaiKubun { get; init; }
}
