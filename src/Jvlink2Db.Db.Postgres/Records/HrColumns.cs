namespace Jvlink2Db.Db.Postgres.Records;

internal static class HrColumns
{
    public static readonly string[] All =
    [
        "record_spec", "data_kubun", "make_date",
        "year", "month_day", "jyo_cd", "kaiji", "nichiji", "race_num",
        "toroku_tosu", "syusso_tosu",
        "fuseiritu_flag", "tokubarai_flag", "henkan_flag",
        "henkan_uma", "henkan_waku", "henkan_do_waku",
        "pay_tansyo_umaban", "pay_tansyo_pay", "pay_tansyo_ninki",
        "pay_fukusyo_umaban", "pay_fukusyo_pay", "pay_fukusyo_ninki",
        "pay_wakuren_umaban", "pay_wakuren_pay", "pay_wakuren_ninki",
        "pay_umaren_kumi", "pay_umaren_pay", "pay_umaren_ninki",
        "pay_wide_kumi", "pay_wide_pay", "pay_wide_ninki",
        "pay_reserved1_kumi", "pay_reserved1_pay", "pay_reserved1_ninki",
        "pay_umatan_kumi", "pay_umatan_pay", "pay_umatan_ninki",
        "pay_sanrenpuku_kumi", "pay_sanrenpuku_pay", "pay_sanrenpuku_ninki",
        "pay_sanrentan_kumi", "pay_sanrentan_pay", "pay_sanrentan_ninki",
    ];

    public static readonly string[] PrimaryKey =
    [
        "year", "month_day", "jyo_cd", "kaiji", "nichiji", "race_num",
    ];
}
