namespace Jvlink2Db.Db.Postgres.Records;

internal static class WcColumns
{
    public static readonly string[] All =
    [
        "record_spec", "data_kubun", "make_date",
        "tresen_kubun", "chokyo_date", "chokyo_time", "ketto_num",
        "course", "baba_around", "reserved",
        "haron_time_10", "lap_time_10",
        "haron_time_9", "lap_time_9",
        "haron_time_8", "lap_time_8",
        "haron_time_7", "lap_time_7",
        "haron_time_6", "lap_time_6",
        "haron_time_5", "lap_time_5",
        "haron_time_4", "lap_time_4",
        "haron_time_3", "lap_time_3",
        "haron_time_2", "lap_time_2",
        "lap_time_1",
    ];

    public static readonly string[] PrimaryKey =
    [
        "tresen_kubun", "chokyo_date", "chokyo_time", "ketto_num",
    ];
}
