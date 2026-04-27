using System.Collections.Generic;
using System.Linq;

namespace Jvlink2Db.Db.Postgres.Records;

internal static class RaColumns
{
    /// <summary>
    /// All <c>jv.ra</c> columns in the order they are written by
    /// <see cref="PostgresRaWriter"/>'s binary COPY. The order must
    /// match both the COPY column list and the row writer.
    /// </summary>
    public static readonly string[] All =
    [
        "record_spec", "data_kubun", "make_date",
        "year", "month_day", "jyo_cd", "kaiji", "nichiji", "race_num",
        "youbi_cd", "toku_num", "hondai", "fukudai", "kakko",
        "hondai_eng", "fukudai_eng", "kakko_eng",
        "ryakusyo10", "ryakusyo6", "ryakusyo3", "kubun", "nkai",
        "grade_cd", "grade_cd_before",
        "syubetu_cd", "kigo_cd", "jyuryo_cd", "jyoken_cd",
        "jyoken_name", "kyori", "kyori_before",
        "track_cd", "track_cd_before", "course_kubun_cd", "course_kubun_cd_before",
        "honsyokin", "honsyokin_before", "fukasyokin", "fukasyokin_before",
        "hasso_time", "hasso_time_before",
        "toroku_tosu", "syusso_tosu", "nyusen_tosu",
        "tenko_cd", "siba_baba_cd", "dirt_baba_cd",
        "lap_time",
        "syogai_mile_time",
        "haron_time_s3", "haron_time_s4", "haron_time_l3", "haron_time_l4",
        "corner1_corner", "corner1_syukaisu", "corner1_jyuni",
        "corner2_corner", "corner2_syukaisu", "corner2_jyuni",
        "corner3_corner", "corner3_syukaisu", "corner3_jyuni",
        "corner4_corner", "corner4_syukaisu", "corner4_jyuni",
        "record_up_kubun",
    ];

    public static readonly string[] PrimaryKey = ["year", "month_day", "jyo_cd", "kaiji", "nichiji", "race_num"];

    public static IEnumerable<string> NonPrimaryKey() => All.Except(PrimaryKey);
}
