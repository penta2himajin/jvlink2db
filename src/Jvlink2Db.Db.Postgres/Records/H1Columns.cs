namespace Jvlink2Db.Db.Postgres.Records;

internal static class H1Columns
{
    public static readonly string[] All =
    [
        "record_spec", "data_kubun", "make_date",
        "year", "month_day", "jyo_cd", "kaiji", "nichiji", "race_num",
        "toroku_tosu", "syusso_tosu",
        "hatubai_flag", "fuku_chaku_barai_key",
        "henkan_uma", "henkan_waku", "henkan_do_waku",
        "tansyo_umaban", "tansyo_hyo", "tansyo_ninki",
        "fukusyo_umaban", "fukusyo_hyo", "fukusyo_ninki",
        "wakuren_kumi", "wakuren_hyo", "wakuren_ninki",
        "umaren_kumi", "umaren_hyo", "umaren_ninki",
        "wide_kumi", "wide_hyo", "wide_ninki",
        "umatan_kumi", "umatan_hyo", "umatan_ninki",
        "sanrenpuku_kumi", "sanrenpuku_hyo", "sanrenpuku_ninki",
        "hyo_total",
    ];

    public static readonly string[] PrimaryKey =
    [
        "year", "month_day", "jyo_cd", "kaiji", "nichiji", "race_num",
    ];
}
