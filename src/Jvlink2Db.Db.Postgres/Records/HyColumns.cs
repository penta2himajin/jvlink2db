namespace Jvlink2Db.Db.Postgres.Records;

internal static class HyColumns
{
    public static readonly string[] All =
    [
        "record_spec", "data_kubun", "make_date", "ketto_num", "bamei", "origin",
    ];

    public static readonly string[] PrimaryKey = ["ketto_num"];
}
