namespace Jvlink2Db.Db.Postgres.Records;

internal static class RcColumns
{
    public static readonly string[] All =
    [
        "record_spec", "data_kubun", "make_date",
        "rec_info_kubun",
        "year", "month_day", "jyo_cd", "kaiji", "nichiji", "race_num",
        "toku_num", "hondai", "grade_cd", "syubetu_cd", "kyori", "track_cd", "rec_kubun", "rec_time",
        "tenko_cd", "siba_baba_cd", "dirt_baba_cd",
        "rec_uma_ketto_num", "rec_uma_bamei", "rec_uma_uma_kigo_cd", "rec_uma_sex_cd",
        "rec_uma_chokyosi_code", "rec_uma_chokyosi_name", "rec_uma_futan",
        "rec_uma_kisyu_code", "rec_uma_kisyu_name",
    ];

    public static readonly string[] PrimaryKey =
    [
        "rec_info_kubun", "year", "month_day", "jyo_cd", "kaiji", "nichiji", "race_num",
    ];
}
