namespace Jvlink2Db.Db.Postgres.Records;

internal static class DmColumns
{
    public static readonly string[] All =
    [
        "record_spec", "data_kubun", "make_date",
        "year", "month_day", "jyo_cd", "kaiji", "nichiji", "race_num", "make_hm",
        "umaban", "dm_time", "dm_gosa_p", "dm_gosa_m",
    ];

    public static readonly string[] PrimaryKey =
    [
        "year", "month_day", "jyo_cd", "kaiji", "nichiji", "race_num",
    ];
}
