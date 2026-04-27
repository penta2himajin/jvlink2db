namespace Jvlink2Db.Db.Postgres.Records;

internal static class H6Columns
{
    public static readonly string[] All =
    [
        "record_spec", "data_kubun", "make_date",
        "year", "month_day", "jyo_cd", "kaiji", "nichiji", "race_num",
        "toroku_tosu", "syusso_tosu",
        "hatubai_flag", "henkan_uma",
        "sanrentan_kumi", "sanrentan_hyo", "sanrentan_ninki",
        "hyo_total",
    ];

    public static readonly string[] PrimaryKey =
    [
        "year", "month_day", "jyo_cd", "kaiji", "nichiji", "race_num",
    ];
}
