namespace Jvlink2Db.Db.Postgres.Records;

internal static class ChColumns
{
    public static readonly string[] All =
    [
        "record_spec", "data_kubun", "make_date",
        "chokyosi_code", "del_kubun", "issue_date", "del_date", "birth_date",
        "chokyosi_name", "chokyosi_name_kana", "chokyosi_ryakusyo", "chokyosi_name_eng",
        "sex_cd", "tozai_cd", "syotai",
        "saikin_jyusyo_year", "saikin_jyusyo_month_day", "saikin_jyusyo_jyo_cd",
        "saikin_jyusyo_kaiji", "saikin_jyusyo_nichiji", "saikin_jyusyo_race_num",
        "saikin_jyusyo_hondai", "saikin_jyusyo_ryakusyo10",
        "saikin_jyusyo_ryakusyo6", "saikin_jyusyo_ryakusyo3",
        "saikin_jyusyo_grade_cd", "saikin_jyusyo_syusso_tosu",
        "saikin_jyusyo_ketto_num", "saikin_jyusyo_bamei",
    ];

    public static readonly string[] PrimaryKey = ["chokyosi_code"];
}
