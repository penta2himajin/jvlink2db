namespace Jvlink2Db.Core.Records;

/// <summary>HS (競走馬市場取引価格 — yearling-sale price). JV-Data §JV_HS_SALE, 200 bytes.</summary>
public sealed record Hs
{
    public required string RecordSpec { get; init; }
    public required string DataKubun { get; init; }
    public required string MakeDate { get; init; }

    public required string KettoNum { get; init; }
    public required string HansyokuFNum { get; init; }
    public required string HansyokuMNum { get; init; }
    public required string BirthYear { get; init; }
    public required string SaleCode { get; init; }
    public required string SaleHostName { get; init; }
    public required string SaleName { get; init; }
    public required string FromDate { get; init; }
    public required string ToDate { get; init; }
    public required string Barei { get; init; }
    public required string Price { get; init; }
}
