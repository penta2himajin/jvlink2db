namespace Jvlink2Db.Db.Postgres.Records;

internal static class O1Columns
{
    public static readonly string[] All =
    [
        "record_spec", "data_kubun", "make_date",
        "year", "month_day", "jyo_cd", "kaiji", "nichiji", "race_num", "happyo_tm",
        "toroku_tosu", "syusso_tosu",
        "tansyo_flag", "fukusyo_flag", "wakuren_flag", "fuku_chaku_barai_key",
        "tansyo_umaban", "tansyo_odds", "tansyo_ninki",
        "fukusyo_umaban", "fukusyo_odds_low", "fukusyo_odds_high", "fukusyo_ninki",
        "wakuren_kumi", "wakuren_odds", "wakuren_ninki",
        "total_hyosu_tansyo", "total_hyosu_fukusyo", "total_hyosu_wakuren",
    ];

    public static readonly string[] PrimaryKey =
    [
        "year", "month_day", "jyo_cd", "kaiji", "nichiji", "race_num", "happyo_tm",
    ];
}
