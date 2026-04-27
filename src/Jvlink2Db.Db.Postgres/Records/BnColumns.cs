namespace Jvlink2Db.Db.Postgres.Records;

internal static class BnColumns
{
    public static readonly string[] All =
    [
        "record_spec", "data_kubun", "make_date",
        "banusi_code", "banusi_name_co", "banusi_name", "banusi_name_kana", "banusi_name_eng", "fukusyoku",
        "hon_ruikei_set_year", "hon_ruikei_honsyokin_total", "hon_ruikei_fuka_syokin", "hon_ruikei_chaku_kaisu",
    ];

    public static readonly string[] PrimaryKey = ["banusi_code"];
}
