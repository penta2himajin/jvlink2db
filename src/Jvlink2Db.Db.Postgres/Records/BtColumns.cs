namespace Jvlink2Db.Db.Postgres.Records;

internal static class BtColumns
{
    public static readonly string[] All =
    [
        "record_spec", "data_kubun", "make_date",
        "hansyoku_num", "keito_id", "keito_name", "keito_ex",
    ];

    public static readonly string[] PrimaryKey = ["hansyoku_num"];
}
