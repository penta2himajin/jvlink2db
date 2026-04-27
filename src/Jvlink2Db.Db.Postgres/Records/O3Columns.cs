namespace Jvlink2Db.Db.Postgres.Records;

internal static class O3Columns
{
    public static readonly string[] All =
    [
        "record_spec", "data_kubun", "make_date",
        "year", "month_day", "jyo_cd", "kaiji", "nichiji", "race_num", "happyo_tm",
        "toroku_tosu", "syusso_tosu", "wide_flag",
        "kumi", "odds_low", "odds_high", "ninki",
        "total_hyosu_wide",
    ];

    public static readonly string[] PrimaryKey =
    [
        "year", "month_day", "jyo_cd", "kaiji", "nichiji", "race_num", "happyo_tm",
    ];
}
