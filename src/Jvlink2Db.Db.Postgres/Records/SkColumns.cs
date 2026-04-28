namespace Jvlink2Db.Db.Postgres.Records;

internal static class SkColumns
{
    public static readonly string[] All =
    [
        "record_spec", "data_kubun", "make_date",
        "ketto_num", "birth_date", "sex_cd", "hinsyu_cd", "keiro_cd",
        "sanku_mochi_kubun", "import_year", "breeder_code", "sanchi_name",
        "hansyoku_num",
    ];

    public static readonly string[] PrimaryKey = ["ketto_num"];
}
