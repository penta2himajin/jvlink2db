using Jvlink2Db.Core.Records;

namespace Jvlink2Db.Db.Postgres.Tests.Records;

internal static class HrBuilder
{
    public static Hr Empty() => new()
    {
        RecordSpec = "HR",
        DataKubun = string.Empty,
        MakeDate = string.Empty,
        Year = string.Empty,
        MonthDay = string.Empty,
        JyoCD = string.Empty,
        Kaiji = string.Empty,
        Nichiji = string.Empty,
        RaceNum = string.Empty,
        TorokuTosu = string.Empty,
        SyussoTosu = string.Empty,
        FuseirituFlag = Repeat("", 9),
        TokubaraiFlag = Repeat("", 9),
        HenkanFlag = Repeat("", 9),
        HenkanUma = Repeat("", 28),
        HenkanWaku = Repeat("", 8),
        HenkanDoWaku = Repeat("", 8),
        PayTansyoUmaban = Repeat("", 3),
        PayTansyoPay = Repeat("", 3),
        PayTansyoNinki = Repeat("", 3),
        PayFukusyoUmaban = Repeat("", 5),
        PayFukusyoPay = Repeat("", 5),
        PayFukusyoNinki = Repeat("", 5),
        PayWakurenUmaban = Repeat("", 3),
        PayWakurenPay = Repeat("", 3),
        PayWakurenNinki = Repeat("", 3),
        PayUmarenKumi = Repeat("", 3),
        PayUmarenPay = Repeat("", 3),
        PayUmarenNinki = Repeat("", 3),
        PayWideKumi = Repeat("", 7),
        PayWidePay = Repeat("", 7),
        PayWideNinki = Repeat("", 7),
        PayReserved1Kumi = Repeat("", 3),
        PayReserved1Pay = Repeat("", 3),
        PayReserved1Ninki = Repeat("", 3),
        PayUmatanKumi = Repeat("", 6),
        PayUmatanPay = Repeat("", 6),
        PayUmatanNinki = Repeat("", 6),
        PaySanrenpukuKumi = Repeat("", 3),
        PaySanrenpukuPay = Repeat("", 3),
        PaySanrenpukuNinki = Repeat("", 3),
        PaySanrentanKumi = Repeat("", 6),
        PaySanrentanPay = Repeat("", 6),
        PaySanrentanNinki = Repeat("", 6),
    };

    public static Hr Sample(string raceNum = "11", string tansyoPay = "000000345") =>
        Empty() with
        {
            DataKubun = "2",
            MakeDate = "20260331",
            Year = "2026",
            MonthDay = "0331",
            JyoCD = "06",
            Kaiji = "01",
            Nichiji = "08",
            RaceNum = raceNum,
            TorokuTosu = "16",
            SyussoTosu = "16",
            PayTansyoUmaban = ["05", string.Empty, string.Empty],
            PayTansyoPay = [tansyoPay, string.Empty, string.Empty],
            PayTansyoNinki = ["01", string.Empty, string.Empty],
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
}
