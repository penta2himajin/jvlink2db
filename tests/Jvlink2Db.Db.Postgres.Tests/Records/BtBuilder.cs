using Jvlink2Db.Core.Records;

namespace Jvlink2Db.Db.Postgres.Tests.Records;

internal static class BtBuilder
{
    public static Bt Sample(string hansyokuNum = "0001234567") => new()
    {
        RecordSpec = "BT",
        DataKubun = "1",
        MakeDate = "20260415",
        HansyokuNum = hansyokuNum,
        KeitoId = "TESTKEITOID",
        KeitoName = "テスト系統",
        KeitoEx = "テスト系統の説明文。",
    };
}
