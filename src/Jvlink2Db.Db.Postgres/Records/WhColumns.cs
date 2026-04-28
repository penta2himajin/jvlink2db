namespace Jvlink2Db.Db.Postgres.Records;

internal static class WhColumns
{
    public static readonly string[] All =
    [
        "record_spec", "data_kubun", "make_date",
        "year", "month_day", "jyo_cd", "kaiji", "nichiji", "race_num",
        "happyo_time",
        "umaban", "bamei", "ba_taijyu", "zogen_fugo", "zogen_sa",
    ];

    public static readonly string[] PrimaryKey = ["year", "month_day", "jyo_cd", "kaiji", "nichiji", "race_num"];
}
