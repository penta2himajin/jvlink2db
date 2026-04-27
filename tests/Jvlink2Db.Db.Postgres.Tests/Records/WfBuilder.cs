using Jvlink2Db.Core.Records;

namespace Jvlink2Db.Db.Postgres.Tests.Records;

internal static class WfBuilder
{
    public static Wf Empty() => new()
    {
        RecordSpec = "WF",
        DataKubun = string.Empty,
        MakeDate = string.Empty,
        KaisaiDate = string.Empty,
        Reserved1 = string.Empty,
        RaceJyoCD = Repeat("", 5),
        RaceKaiji = Repeat("", 5),
        RaceNichiji = Repeat("", 5),
        RaceNum = Repeat("", 5),
        Reserved2 = string.Empty,
        HatsubaiHyo = string.Empty,
        YukoHyo = Repeat("", 5),
        HenkanFlag = string.Empty,
        FuseiritsuFlag = string.Empty,
        TekichunashiFlag = string.Empty,
        COShoki = string.Empty,
        COZanDaka = string.Empty,
        PayKumiban = Repeat("", 243),
        PayAmount = Repeat("", 243),
        PayTekichuHyo = Repeat("", 243),
    };

    public static Wf Sample(string kaisaiDate = "20260328", string hatsubaiHyo = "01234567890") =>
        Empty() with
        {
            DataKubun = "1",
            MakeDate = "20260331",
            KaisaiDate = kaisaiDate,
            RaceJyoCD = ["06", "06", "06", "06", "08"],
            RaceKaiji = ["01", "01", "01", "01", "01"],
            RaceNichiji = ["08", "08", "08", "08", "08"],
            RaceNum = ["09", "10", "11", "12", "11"],
            HatsubaiHyo = hatsubaiHyo,
            YukoHyo = ["01000000000", string.Empty, string.Empty, string.Empty, "00500000000"],
            HenkanFlag = "0",
            FuseiritsuFlag = "0",
            TekichunashiFlag = "0",
            COShoki = "000000000000000",
            COZanDaka = "000000000000000",
            PayKumiban = SetSparse(243, (0, "0102030405"), (242, "0708090101")),
            PayAmount = SetSparse(243, (0, "000999000"), (242, "000050000")),
            PayTekichuHyo = SetSparse(243, (0, "0000000010"), (242, "0000000020")),
        };

    private static string[] Repeat(string value, int count)
    {
        var arr = new string[count];
        for (var i = 0; i < count; i++)
        {
            arr[i] = value;
        }

        return arr;
    }

    private static string[] SetSparse(int count, params (int Index, string Value)[] entries)
    {
        var arr = Repeat(string.Empty, count);
        foreach (var (idx, val) in entries)
        {
            arr[idx] = val;
        }

        return arr;
    }
}
