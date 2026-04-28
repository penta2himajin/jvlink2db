using Jvlink2Db.Core.Records;

namespace Jvlink2Db.Db.Postgres.Tests.Records;

internal static class CsBuilder
{
    public static Cs Sample(string jyoCd = "05", string kyori = "3200", string trackCd = "10") => new()
    {
        RecordSpec = "CS",
        DataKubun = "1",
        MakeDate = "20260415",
        JyoCD = jyoCd,
        Kyori = kyori,
        TrackCD = trackCd,
        KaishuDate = "20240101",
        CourseEx = "テストコース説明文。",
    };
}
