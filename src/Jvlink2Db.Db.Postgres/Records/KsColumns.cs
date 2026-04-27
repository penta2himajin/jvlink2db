namespace Jvlink2Db.Db.Postgres.Records;

internal static class KsColumns
{
    public static readonly string[] All =
    [
        "record_spec", "data_kubun", "make_date",
        "kisyu_code", "del_kubun", "issue_date", "del_date", "birth_date",
        "kisyu_name", "reserved", "kisyu_name_kana", "kisyu_ryakusyo", "kisyu_name_eng",
        "sex_cd", "sikaku_cd", "minarai_cd", "tozai_cd", "syotai",
        "chokyosi_code", "chokyosi_ryakusyo",
        "hatu_kijyo_year", "hatu_kijyo_month_day", "hatu_kijyo_jyo_cd",
        "hatu_kijyo_kaiji", "hatu_kijyo_nichiji", "hatu_kijyo_race_num",
        "hatu_kijyo_syusso_tosu", "hatu_kijyo_ketto_num", "hatu_kijyo_bamei",
        "hatu_kijyo_kakutei_jyuni", "hatu_kijyo_ijyo_cd",
        "hatu_syori_year", "hatu_syori_month_day", "hatu_syori_jyo_cd",
        "hatu_syori_kaiji", "hatu_syori_nichiji", "hatu_syori_race_num",
        "hatu_syori_syusso_tosu", "hatu_syori_ketto_num", "hatu_syori_bamei",
        "saikin_jyusyo_year", "saikin_jyusyo_month_day", "saikin_jyusyo_jyo_cd",
        "saikin_jyusyo_kaiji", "saikin_jyusyo_nichiji", "saikin_jyusyo_race_num",
        "saikin_jyusyo_hondai", "saikin_jyusyo_ryakusyo10",
        "saikin_jyusyo_ryakusyo6", "saikin_jyusyo_ryakusyo3",
        "saikin_jyusyo_grade_cd", "saikin_jyusyo_syusso_tosu",
        "saikin_jyusyo_ketto_num", "saikin_jyusyo_bamei",
    ];

    public static readonly string[] PrimaryKey = ["kisyu_code"];
}
