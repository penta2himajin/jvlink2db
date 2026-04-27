namespace Jvlink2Db.Core.Records;

/// <summary>
/// Decoded HR (払戻 — payouts) record. One per race.
/// JV-Data 4.9.0.1 §JV_HR_PAY, 719 bytes.
/// </summary>
public sealed record Hr
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

    public required string[] FuseirituFlag { get; init; }   // 9
    public required string[] TokubaraiFlag { get; init; }   // 9
    public required string[] HenkanFlag { get; init; }      // 9
    public required string[] HenkanUma { get; init; }       // 28
    public required string[] HenkanWaku { get; init; }      // 8
    public required string[] HenkanDoWaku { get; init; }    // 8

    // Pay info — parallel arrays (combo / pay / popularity).
    // PAY_INFO1: Umaban (2 chars), Pay (9 chars), Ninki (2 chars).
    public required string[] PayTansyoUmaban { get; init; }   // 3
    public required string[] PayTansyoPay { get; init; }
    public required string[] PayTansyoNinki { get; init; }

    public required string[] PayFukusyoUmaban { get; init; }  // 5
    public required string[] PayFukusyoPay { get; init; }
    public required string[] PayFukusyoNinki { get; init; }

    public required string[] PayWakurenUmaban { get; init; }  // 3
    public required string[] PayWakurenPay { get; init; }
    public required string[] PayWakurenNinki { get; init; }

    // PAY_INFO2: Kumi (4), Pay (9), Ninki (3).
    public required string[] PayUmarenKumi { get; init; }     // 3
    public required string[] PayUmarenPay { get; init; }
    public required string[] PayUmarenNinki { get; init; }

    public required string[] PayWideKumi { get; init; }       // 7
    public required string[] PayWidePay { get; init; }
    public required string[] PayWideNinki { get; init; }

    public required string[] PayReserved1Kumi { get; init; }  // 3
    public required string[] PayReserved1Pay { get; init; }
    public required string[] PayReserved1Ninki { get; init; }

    public required string[] PayUmatanKumi { get; init; }     // 6
    public required string[] PayUmatanPay { get; init; }
    public required string[] PayUmatanNinki { get; init; }

    // PAY_INFO3: Kumi (6), Pay (9), Ninki (3).
    public required string[] PaySanrenpukuKumi { get; init; } // 3
    public required string[] PaySanrenpukuPay { get; init; }
    public required string[] PaySanrenpukuNinki { get; init; }

    // PAY_INFO4: Kumi (6), Pay (9), Ninki (4).
    public required string[] PaySanrentanKumi { get; init; }  // 6
    public required string[] PaySanrentanPay { get; init; }
    public required string[] PaySanrentanNinki { get; init; }
}
