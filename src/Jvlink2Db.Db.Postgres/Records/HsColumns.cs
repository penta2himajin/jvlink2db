namespace Jvlink2Db.Db.Postgres.Records;

internal static class HsColumns
{
    public static readonly string[] All =
    [
        "record_spec", "data_kubun", "make_date",
        "ketto_num", "hansyoku_f_num", "hansyoku_m_num", "birth_year",
        "sale_code", "sale_host_name", "sale_name",
        "from_date", "to_date", "barei", "price",
    ];

    public static readonly string[] PrimaryKey = ["ketto_num", "sale_code"];
}
