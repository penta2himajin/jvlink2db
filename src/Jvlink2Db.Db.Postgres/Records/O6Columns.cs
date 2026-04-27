namespace Jvlink2Db.Db.Postgres.Records;

internal static class O6Columns
{
    public static readonly string[] All =
    [
        "record_spec", "data_kubun", "make_date",
        "year", "month_day", "jyo_cd", "kaiji", "nichiji", "race_num", "happyo_tm",
        "toroku_tosu", "syusso_tosu", "sanrentan_flag",
        "kumi", "odds", "ninki",
        "total_hyosu_sanrentan",
    ];

    public static readonly string[] PrimaryKey =
    [
        "year", "month_day", "jyo_cd", "kaiji", "nichiji", "race_num", "happyo_tm",
    ];
}
