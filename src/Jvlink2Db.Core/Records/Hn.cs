namespace Jvlink2Db.Core.Records;

/// <summary>HN (繁殖馬マスタ — brood-mare master). JV-Data §JV_HN_HANSYOKU, 251 bytes.</summary>
public sealed record Hn
{
    public required string RecordSpec { get; init; }
    public required string DataKubun { get; init; }
    public required string MakeDate { get; init; }

    public required string HansyokuNum { get; init; }
    public required string Reserved { get; init; }
    public required string KettoNum { get; init; }
    public required string DelKubun { get; init; }
    public required string Bamei { get; init; }
    public required string BameiKana { get; init; }
    public required string BameiEng { get; init; }
    public required string BirthYear { get; init; }
    public required string SexCD { get; init; }
    public required string HinsyuCD { get; init; }
    public required string KeiroCD { get; init; }
    public required string HansyokuMochiKubun { get; init; }
    public required string ImportYear { get; init; }
    public required string SanchiName { get; init; }
    public required string HansyokuFNum { get; init; }
    public required string HansyokuMNum { get; init; }
}
