using Jvlink2Db.Core.Records;

namespace Jvlink2Db.Db.Postgres.Tests.Records;

internal static class ChBuilder
{
    public static Ch Empty() => new()
    {
        RecordSpec = "CH",
        DataKubun = string.Empty,
        MakeDate = string.Empty,
        ChokyosiCode = string.Empty,
        DelKubun = string.Empty,
        IssueDate = string.Empty,
        DelDate = string.Empty,
        BirthDate = string.Empty,
        ChokyosiName = string.Empty,
        ChokyosiNameKana = string.Empty,
        ChokyosiRyakusyo = string.Empty,
        ChokyosiNameEng = string.Empty,
        SexCD = string.Empty,
        TozaiCD = string.Empty,
        Syotai = string.Empty,
        SaikinJyusyoYear = ["", "", ""],
        SaikinJyusyoMonthDay = ["", "", ""],
        SaikinJyusyoJyoCD = ["", "", ""],
        SaikinJyusyoKaiji = ["", "", ""],
        SaikinJyusyoNichiji = ["", "", ""],
        SaikinJyusyoRaceNum = ["", "", ""],
        SaikinJyusyoHondai = ["", "", ""],
        SaikinJyusyoRyakusyo10 = ["", "", ""],
        SaikinJyusyoRyakusyo6 = ["", "", ""],
        SaikinJyusyoRyakusyo3 = ["", "", ""],
        SaikinJyusyoGradeCD = ["", "", ""],
        SaikinJyusyoSyussoTosu = ["", "", ""],
        SaikinJyusyoKettoNum = ["", "", ""],
        SaikinJyusyoBamei = ["", "", ""],
    };

    public static Ch Sample(string code = "00567", string name = "テスト調教師") =>
        Empty() with
        {
            DataKubun = "1",
            MakeDate = "20260331",
            ChokyosiCode = code,
            ChokyosiName = name,
            BirthDate = "19700101",
            SexCD = "1",
        };
}
