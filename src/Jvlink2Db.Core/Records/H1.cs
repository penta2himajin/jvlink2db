namespace Jvlink2Db.Core.Records;

/// <summary>
/// Decoded H1 (票数 全掛式) record.
/// JV-Data 4.9.0.1 §JV_H1_HYOSU_ZENKAKE, 28955 bytes.
/// </summary>
public sealed record H1
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
    public required string[] HatubaiFlag { get; init; }       // 7
    public required string FukuChakuBaraiKey { get; init; }
    public required string[] HenkanUma { get; init; }         // 28
    public required string[] HenkanWaku { get; init; }        // 8
    public required string[] HenkanDoWaku { get; init; }      // 8

    /// <summary>単勝票数 — Umaban (2) / Hyo (11) / Ninki (2), 28 entries.</summary>
    public required string[] TansyoUmaban { get; init; }
    public required string[] TansyoHyo { get; init; }
    public required string[] TansyoNinki { get; init; }

    public required string[] FukusyoUmaban { get; init; }     // 28
    public required string[] FukusyoHyo { get; init; }
    public required string[] FukusyoNinki { get; init; }

    public required string[] WakurenKumi { get; init; }       // 36
    public required string[] WakurenHyo { get; init; }
    public required string[] WakurenNinki { get; init; }

    /// <summary>馬連 — Kumi (4) / Hyo (11) / Ninki (3), 153 entries.</summary>
    public required string[] UmarenKumi { get; init; }
    public required string[] UmarenHyo { get; init; }
    public required string[] UmarenNinki { get; init; }

    public required string[] WideKumi { get; init; }          // 153
    public required string[] WideHyo { get; init; }
    public required string[] WideNinki { get; init; }

    public required string[] UmatanKumi { get; init; }        // 306
    public required string[] UmatanHyo { get; init; }
    public required string[] UmatanNinki { get; init; }

    /// <summary>3連複 — Kumi (6) / Hyo (11) / Ninki (3), 816 entries.</summary>
    public required string[] SanrenpukuKumi { get; init; }
    public required string[] SanrenpukuHyo { get; init; }
    public required string[] SanrenpukuNinki { get; init; }

    /// <summary>票数合計 — 14 entries (one per pay type).</summary>
    public required string[] HyoTotal { get; init; }
}
