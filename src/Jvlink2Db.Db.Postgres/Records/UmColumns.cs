namespace Jvlink2Db.Db.Postgres.Records;

internal static class UmColumns
{
    public static readonly string[] All =
    [
        "record_spec", "data_kubun", "make_date",
        "ketto_num", "del_kubun", "reg_date", "del_date", "birth_date",
        "bamei", "bamei_kana", "bamei_eng",
        "zaikyu_flag", "reserved", "uma_kigo_cd", "sex_cd", "hinsyu_cd", "keiro_cd",
        "ketto_hansyoku_num", "ketto_bamei",
        "tozai_cd", "chokyosi_code", "chokyosi_ryakusyo", "syotai",
        "breeder_code", "breeder_name", "sanchi_name", "banusi_code", "banusi_name",
        "ruikei_honsyo_heiti", "ruikei_honsyo_syogai",
        "ruikei_fuka_heichi", "ruikei_fuka_syogai",
        "ruikei_syutoku_heichi", "ruikei_syutoku_syogai",
        "chaku_sogo", "chaku_chuo",
        "chaku_kaisu_ba", "chaku_kaisu_jyotai", "chaku_kaisu_kyori",
        "kyakusitu", "race_count",
    ];

    public static readonly string[] PrimaryKey = ["ketto_num"];
}
