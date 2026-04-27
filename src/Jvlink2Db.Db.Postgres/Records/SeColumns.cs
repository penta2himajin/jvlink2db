namespace Jvlink2Db.Db.Postgres.Records;

internal static class SeColumns
{
    public static readonly string[] All =
    [
        "record_spec", "data_kubun", "make_date",
        "year", "month_day", "jyo_cd", "kaiji", "nichiji", "race_num", "umaban",
        "wakuban", "ketto_num", "bamei",
        "uma_kigo_cd", "sex_cd", "hinsyu_cd", "keiro_cd", "barei",
        "tozai_cd", "chokyosi_code", "chokyosi_ryakusyo",
        "banusi_code", "banusi_name", "fukusyoku", "reserved1",
        "futan", "futan_before", "blinker", "reserved2",
        "kisyu_code", "kisyu_code_before", "kisyu_ryakusyo", "kisyu_ryakusyo_before",
        "minarai_cd", "minarai_cd_before",
        "ba_taijyu", "zogen_fugo", "zogen_sa", "ijyo_cd",
        "nyusen_jyuni", "kakutei_jyuni", "dochaku_kubun", "dochaku_tosu",
        "\"time\"", "chakusa_cd", "chakusa_cd_p", "chakusa_cd_pp",
        "jyuni_1c", "jyuni_2c", "jyuni_3c", "jyuni_4c",
        "odds", "ninki", "honsyokin", "fukasyokin",
        "reserved3", "reserved4", "haron_time_l4", "haron_time_l3",
        "chaku_uma_1_ketto_num", "chaku_uma_1_bamei",
        "chaku_uma_2_ketto_num", "chaku_uma_2_bamei",
        "chaku_uma_3_ketto_num", "chaku_uma_3_bamei",
        "time_diff", "record_up_kubun",
        "dm_kubun", "dm_time", "dm_gosa_p", "dm_gosa_m", "dm_jyuni", "kyakusitu_kubun",
    ];

    public static readonly string[] PrimaryKey =
    [
        "year", "month_day", "jyo_cd", "kaiji", "nichiji", "race_num", "umaban",
    ];
}
