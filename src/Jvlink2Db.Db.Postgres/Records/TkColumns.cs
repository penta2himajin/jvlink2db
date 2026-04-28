namespace Jvlink2Db.Db.Postgres.Records;

internal static class TkColumns
{
    public static readonly string[] All =
    [
        "record_spec", "data_kubun", "make_date",
        "year", "month_day", "jyo_cd", "kaiji", "nichiji", "race_num",
        "youbi_cd", "toku_num", "hondai", "fukudai", "kakko",
        "hondai_eng", "fukudai_eng", "kakko_eng",
        "ryakusyo10", "ryakusyo6", "ryakusyo3",
        "kubun", "nkai",
        "grade_cd",
        "syubetu_cd", "kigo_cd", "jyuryo_cd", "jyoken_cd",
        "kyori", "track_cd", "course_kubun_cd", "handi_date", "toroku_tosu",
        "toku_num_seq", "ketto_num", "bamei", "uma_kigo_cd", "sex_cd",
        "tozai_cd", "chokyosi_code", "chokyosi_ryakusyo", "futan", "koryu",
    ];

    public static readonly string[] PrimaryKey = ["year", "month_day", "jyo_cd", "kaiji", "nichiji", "race_num"];
}
