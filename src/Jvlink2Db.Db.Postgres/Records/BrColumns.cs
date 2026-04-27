namespace Jvlink2Db.Db.Postgres.Records;

internal static class BrColumns
{
    public static readonly string[] All =
    [
        "record_spec", "data_kubun", "make_date",
        "breeder_code", "breeder_name_co", "breeder_name", "breeder_name_kana", "breeder_name_eng", "address",
        "hon_ruikei_set_year", "hon_ruikei_honsyokin_total", "hon_ruikei_fuka_syokin", "hon_ruikei_chaku_kaisu",
    ];

    public static readonly string[] PrimaryKey = ["breeder_code"];
}
