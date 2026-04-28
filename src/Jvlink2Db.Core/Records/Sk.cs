namespace Jvlink2Db.Core.Records;

/// <summary>SK (産駒マスタ — progeny master). JV-Data §JV_SK_SANKU, 208 bytes.</summary>
public sealed record Sk
{
    public required string RecordSpec { get; init; }
    public required string DataKubun { get; init; }
    public required string MakeDate { get; init; }

    public required string KettoNum { get; init; }
    public required string BirthDate { get; init; }
    public required string SexCD { get; init; }
    public required string HinsyuCD { get; init; }
    public required string KeiroCD { get; init; }
    public required string SankuMochiKubun { get; init; }
    public required string ImportYear { get; init; }
    public required string BreederCode { get; init; }
    public required string SanchiName { get; init; }

    /// <summary>3代血統繁殖登録番号[14].</summary>
    public required string[] HansyokuNum { get; init; }
}
