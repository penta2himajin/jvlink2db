namespace Jvlink2Db.Db.Postgres.Records;

internal static class WfColumns
{
    public static readonly string[] All =
    [
        "record_spec", "data_kubun", "make_date",
        "kaisai_date", "reserved1",
        "race_jyo_cd", "race_kaiji", "race_nichiji", "race_num",
        "reserved2", "hatsubai_hyo", "yuko_hyo",
        "henkan_flag", "fuseiritsu_flag", "tekichunashi_flag",
        "co_shoki", "co_zan_daka",
        "pay_kumiban", "pay_amount", "pay_tekichu_hyo",
    ];

    public static readonly string[] PrimaryKey = ["kaisai_date"];
}
