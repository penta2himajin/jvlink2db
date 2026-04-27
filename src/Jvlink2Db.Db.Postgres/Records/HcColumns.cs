namespace Jvlink2Db.Db.Postgres.Records;

internal static class HcColumns
{
    public static readonly string[] All =
    [
        "record_spec", "data_kubun", "make_date",
        "tresen_kubun", "chokyo_date", "chokyo_time", "ketto_num",
        "haron_time_4", "lap_time_4", "haron_time_3", "lap_time_3",
        "haron_time_2", "lap_time_2", "lap_time_1",
    ];

    public static readonly string[] PrimaryKey =
    [
        "tresen_kubun", "chokyo_date", "chokyo_time", "ketto_num",
    ];
}
