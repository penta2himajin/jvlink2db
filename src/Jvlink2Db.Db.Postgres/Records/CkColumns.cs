namespace Jvlink2Db.Db.Postgres.Records;

internal static class CkColumns
{
    public static readonly string[] All =
    [
        "record_spec", "data_kubun", "make_date",
        "year", "month_day", "jyo_cd", "kaiji", "nichiji", "race_num",
        "ketto_num", "bamei",
        "ruikei_honsyo_heiti", "ruikei_honsyo_syogai",
        "ruikei_fuka_heichi", "ruikei_fuka_syogai",
        "ruikei_syutoku_heichi", "ruikei_syutoku_syogai",
        "kyakusitu", "race_count",
        "kisyu_code", "kisyu_name",
        "chokyosi_code", "chokyosi_name",
        "banusi_code", "banusi_name_co", "banusi_name",
        "breeder_code", "breeder_name_co", "breeder_name",
    ];

    public static readonly string[] PrimaryKey =
        ["year", "month_day", "jyo_cd", "kaiji", "nichiji", "race_num", "ketto_num"];
}
