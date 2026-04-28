using Jvlink2Db.Core.Records;

namespace Jvlink2Db.Db.Postgres.Tests.Records;

internal static class HyBuilder
{
    public static Hy Sample(string ketto = "2020100123", string bamei = "テストホース") => new()
    {
        RecordSpec = "HY",
        DataKubun = "1",
        MakeDate = "20260415",
        KettoNum = ketto,
        Bamei = bamei,
        Origin = "テスト由来",
    };
}
