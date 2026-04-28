namespace Jvlink2Db.Db.Postgres.Records;

internal static class TmColumns
{
    public static readonly string[] All =
    [
        "record_spec", "data_kubun", "make_date",
        "year", "month_day", "jyo_cd", "kaiji", "nichiji", "race_num",
        "make_hm", "umaban", "tm_score",
    ];

    public static readonly string[] PrimaryKey =
    [
        "year", "month_day", "jyo_cd", "kaiji", "nichiji", "race_num",
    ];
}
