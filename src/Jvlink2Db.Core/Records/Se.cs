namespace Jvlink2Db.Core.Records;

/// <summary>
/// Decoded SE (per-horse race info) record. One instance per
/// (race, umaban). All fields are right-trimmed strings; numeric
/// coercion is the database layer's responsibility.
/// </summary>
public sealed record Se
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

    public required string Wakuban { get; init; }
    public required string Umaban { get; init; }
    public required string KettoNum { get; init; }
    public required string Bamei { get; init; }
    public required string UmaKigoCD { get; init; }
    public required string SexCD { get; init; }
    public required string HinsyuCD { get; init; }
    public required string KeiroCD { get; init; }
    public required string Barei { get; init; }
    public required string TozaiCD { get; init; }
    public required string ChokyosiCode { get; init; }
    public required string ChokyosiRyakusyo { get; init; }
    public required string BanusiCode { get; init; }
    public required string BanusiName { get; init; }
    public required string Fukusyoku { get; init; }
    public required string Reserved1 { get; init; }
    public required string Futan { get; init; }
    public required string FutanBefore { get; init; }
    public required string Blinker { get; init; }
    public required string Reserved2 { get; init; }
    public required string KisyuCode { get; init; }
    public required string KisyuCodeBefore { get; init; }
    public required string KisyuRyakusyo { get; init; }
    public required string KisyuRyakusyoBefore { get; init; }
    public required string MinaraiCD { get; init; }
    public required string MinaraiCDBefore { get; init; }
    public required string BaTaijyu { get; init; }
    public required string ZogenFugo { get; init; }
    public required string ZogenSa { get; init; }
    public required string IJyoCD { get; init; }
    public required string NyusenJyuni { get; init; }
    public required string KakuteiJyuni { get; init; }
    public required string DochakuKubun { get; init; }
    public required string DochakuTosu { get; init; }
    public required string Time { get; init; }
    public required string ChakusaCD { get; init; }
    public required string ChakusaCDP { get; init; }
    public required string ChakusaCDPP { get; init; }
    public required string Jyuni1c { get; init; }
    public required string Jyuni2c { get; init; }
    public required string Jyuni3c { get; init; }
    public required string Jyuni4c { get; init; }
    public required string Odds { get; init; }
    public required string Ninki { get; init; }
    public required string Honsyokin { get; init; }
    public required string Fukasyokin { get; init; }
    public required string Reserved3 { get; init; }
    public required string Reserved4 { get; init; }
    public required string HaronTimeL4 { get; init; }
    public required string HaronTimeL3 { get; init; }

    /// <summary>1着馬・相手馬3頭の血統番号と馬名。</summary>
    public required SeChakuUma[] ChakuUma { get; init; }

    public required string TimeDiff { get; init; }
    public required string RecordUpKubun { get; init; }
    public required string DMKubun { get; init; }
    public required string DMTime { get; init; }
    public required string DMGosaP { get; init; }
    public required string DMGosaM { get; init; }
    public required string DMJyuni { get; init; }
    public required string KyakusituKubun { get; init; }
}

public sealed record SeChakuUma(string KettoNum, string Bamei);
