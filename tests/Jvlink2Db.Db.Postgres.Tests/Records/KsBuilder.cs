using Jvlink2Db.Core.Records;

namespace Jvlink2Db.Db.Postgres.Tests.Records;

internal static class KsBuilder
{
    public static Ks Empty() => new()
    {
        RecordSpec = "KS",
        DataKubun = string.Empty,
        MakeDate = string.Empty,
        KisyuCode = string.Empty,
        DelKubun = string.Empty,
        IssueDate = string.Empty,
        DelDate = string.Empty,
        BirthDate = string.Empty,
        KisyuName = string.Empty,
        Reserved = string.Empty,
        KisyuNameKana = string.Empty,
        KisyuRyakusyo = string.Empty,
        KisyuNameEng = string.Empty,
        SexCD = string.Empty,
        SikakuCD = string.Empty,
        MinaraiCD = string.Empty,
        TozaiCD = string.Empty,
        Syotai = string.Empty,
        ChokyosiCode = string.Empty,
        ChokyosiRyakusyo = string.Empty,
        HatuKiJyoYear = ["", ""],
        HatuKiJyoMonthDay = ["", ""],
        HatuKiJyoJyoCD = ["", ""],
        HatuKiJyoKaiji = ["", ""],
        HatuKiJyoNichiji = ["", ""],
        HatuKiJyoRaceNum = ["", ""],
        HatuKiJyoSyussoTosu = ["", ""],
        HatuKiJyoKettoNum = ["", ""],
        HatuKiJyoBamei = ["", ""],
        HatuKiJyoKakuteiJyuni = ["", ""],
        HatuKiJyoIJyoCD = ["", ""],
        HatuSyoriYear = ["", ""],
        HatuSyoriMonthDay = ["", ""],
        HatuSyoriJyoCD = ["", ""],
        HatuSyoriKaiji = ["", ""],
        HatuSyoriNichiji = ["", ""],
        HatuSyoriRaceNum = ["", ""],
        HatuSyoriSyussoTosu = ["", ""],
        HatuSyoriKettoNum = ["", ""],
        HatuSyoriBamei = ["", ""],
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

    public static Ks Sample(string code = "00123", string name = "テスト騎手") =>
        Empty() with
        {
            DataKubun = "1",
            MakeDate = "20260331",
            KisyuCode = code,
            KisyuName = name,
            BirthDate = "20000401",
            SexCD = "1",
        };
}
