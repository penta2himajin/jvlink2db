namespace Jvlink2Db.Core.Records;

/// <summary>HY (馬名の意味由来 — bamei origin). JV-Data §JV_HY_BAMEIORIGIN, 123 bytes.</summary>
public sealed record Hy
{
    public required string RecordSpec { get; init; }
    public required string DataKubun { get; init; }
    public required string MakeDate { get; init; }

    public required string KettoNum { get; init; }
    public required string Bamei { get; init; }
    public required string Origin { get; init; }
}
