namespace Jvlink2Db.Core.Records;

/// <summary>
/// Decoded H6 (票数 3連単) record.
/// JV-Data 4.9.0.1 §JV_H6_HYOSU_SANRENTAN, 102890 bytes.
/// </summary>
public sealed record H6
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

    public required string TorokuTosu { get; init; }
    public required string SyussoTosu { get; init; }
    public required string HatubaiFlag { get; init; }
    public required string[] HenkanUma { get; init; }       // 18

    /// <summary>3連単票数 — Kumi (6) / Hyo (11) / Ninki (4), 4896 entries.</summary>
    public required string[] SanrentanKumi { get; init; }
    public required string[] SanrentanHyo { get; init; }
    public required string[] SanrentanNinki { get; init; }

    /// <summary>票数合計 — 2 entries.</summary>
    public required string[] HyoTotal { get; init; }
}
