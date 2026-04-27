namespace Jvlink2Db.Db.Postgres.Records;

internal static class O4Columns
{
    public static readonly string[] All =
    [
        "record_spec", "data_kubun", "make_date",
        "year", "month_day", "jyo_cd", "kaiji", "nichiji", "race_num", "happyo_tm",
        "toroku_tosu", "syusso_tosu", "umatan_flag",
        "kumi", "odds", "ninki",
        "total_hyosu_umatan",
    ];

    public static readonly string[] PrimaryKey =
    [
        "year", "month_day", "jyo_cd", "kaiji", "nichiji", "race_num", "happyo_tm",
    ];
}
