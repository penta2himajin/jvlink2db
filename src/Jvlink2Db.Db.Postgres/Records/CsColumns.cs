namespace Jvlink2Db.Db.Postgres.Records;

internal static class CsColumns
{
    public static readonly string[] All =
    [
        "record_spec", "data_kubun", "make_date",
        "jyo_cd", "kyori", "track_cd", "kaishu_date", "course_ex",
    ];

    public static readonly string[] PrimaryKey = ["jyo_cd", "kyori", "track_cd", "kaishu_date"];
}
