namespace Jvlink2Db.Db.Postgres.Records;

internal static class YsColumns
{
    public static readonly string[] All =
    [
        "record_spec", "data_kubun", "make_date",
        "year", "month_day", "jyo_cd", "kaiji", "nichiji",
        "youbi_cd",
        "jyusyo_toku_num", "jyusyo_hondai", "jyusyo_ryakusyo10",
        "jyusyo_ryakusyo6", "jyusyo_ryakusyo3", "jyusyo_nkai",
        "jyusyo_grade_cd", "jyusyo_syubetu_cd", "jyusyo_kigo_cd",
        "jyusyo_jyuryo_cd", "jyusyo_kyori", "jyusyo_track_cd",
    ];

    public static readonly string[] PrimaryKey = ["year", "month_day", "jyo_cd", "kaiji", "nichiji"];
}
