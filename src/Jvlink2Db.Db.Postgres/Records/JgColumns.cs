namespace Jvlink2Db.Db.Postgres.Records;

internal static class JgColumns
{
    public static readonly string[] All =
    [
        "record_spec", "data_kubun", "make_date",
        "year", "month_day", "jyo_cd", "kaiji", "nichiji", "race_num",
        "ketto_num", "bamei", "shutsuba_tohyo_jun", "shusso_kubun", "jogai_jotai_kubun",
    ];

    public static readonly string[] PrimaryKey =
    [
        "year", "month_day", "jyo_cd", "kaiji", "nichiji", "race_num", "ketto_num",
    ];
}
