namespace Jvlink2Db.Db.Postgres.Records;

internal static class HnColumns
{
    public static readonly string[] All =
    [
        "record_spec", "data_kubun", "make_date",
        "hansyoku_num", "reserved", "ketto_num", "del_kubun",
        "bamei", "bamei_kana", "bamei_eng", "birth_year",
        "sex_cd", "hinsyu_cd", "keiro_cd",
        "hansyoku_mochi_kubun", "import_year", "sanchi_name",
        "hansyoku_f_num", "hansyoku_m_num",
    ];

    public static readonly string[] PrimaryKey = ["hansyoku_num"];
}
