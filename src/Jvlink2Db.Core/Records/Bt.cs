namespace Jvlink2Db.Core.Records;

/// <summary>BT (系統情報 — bloodline). JV-Data §JV_BT_KEITO, 6889 bytes.</summary>
public sealed record Bt
{
    public required string RecordSpec { get; init; }
    public required string DataKubun { get; init; }
    public required string MakeDate { get; init; }

    public required string HansyokuNum { get; init; }
    public required string KeitoId { get; init; }
    public required string KeitoName { get; init; }
    public required string KeitoEx { get; init; }
}
