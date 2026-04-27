using System;
using Jvlink2Db.Parser.Records;
using Jvlink2Db.Parser.Tests.Fixtures;
using Xunit;

namespace Jvlink2Db.Parser.Tests.Records;

public class HrDecoderTests
{
    [Fact]
    public void Decode_throws_when_buffer_is_too_short()
    {
        var ex = Assert.Throws<ArgumentException>(() => HrDecoder.Decode(new byte[718]));
        Assert.Contains("719", ex.Message);
    }

    [Fact]
    public void Decode_throws_when_record_spec_is_not_HR()
    {
        var bytes = new HrFixtureBuilder().RecordSpec("RA").Build();
        var ex = Assert.Throws<InvalidOperationException>(() => HrDecoder.Decode(bytes));
        Assert.Contains("HR", ex.Message);
    }

    [Fact]
    public void Decode_extracts_pk_and_counts()
    {
        var bytes = new HrFixtureBuilder()
            .DataKubun("1")
            .MakeDate("20260331")
            .RaceId("2026", "0331", "06", "01", "08", "11")
            .TorokuTosu("16")
            .SyussoTosu("16")
            .Build();

        var hr = HrDecoder.Decode(bytes);

        Assert.Equal("HR", hr.RecordSpec);
        Assert.Equal("2026", hr.Year);
        Assert.Equal("11", hr.RaceNum);
        Assert.Equal("16", hr.TorokuTosu);
        Assert.Equal("16", hr.SyussoTosu);
    }

    [Fact]
    public void Decode_extracts_pay_arrays_with_correct_sizes()
    {
        var bytes = new HrFixtureBuilder()
            .PayTansyo(0, "05", "000000345", "01")
            .PayTansyo(1, "08", "000001200", "02")
            .PayFukusyo(0, "05", "000000150", "01")
            .PayUmaren(0, "0508", "000004520", "002")
            .PaySanrentan(0, "050803", "000123450", "0010")
            .Build();

        var hr = HrDecoder.Decode(bytes);

        Assert.Equal(3, hr.PayTansyoUmaban.Length);
        Assert.Equal("05", hr.PayTansyoUmaban[0]);
        Assert.Equal("000000345", hr.PayTansyoPay[0]);
        Assert.Equal("01", hr.PayTansyoNinki[0]);
        Assert.Equal("08", hr.PayTansyoUmaban[1]);
        Assert.Equal(string.Empty, hr.PayTansyoUmaban[2]);

        Assert.Equal(5, hr.PayFukusyoUmaban.Length);
        Assert.Equal("05", hr.PayFukusyoUmaban[0]);

        Assert.Equal(3, hr.PayUmarenKumi.Length);
        Assert.Equal("0508", hr.PayUmarenKumi[0]);
        Assert.Equal("000004520", hr.PayUmarenPay[0]);
        Assert.Equal("002", hr.PayUmarenNinki[0]);

        Assert.Equal(6, hr.PaySanrentanKumi.Length);
        Assert.Equal("050803", hr.PaySanrentanKumi[0]);
        Assert.Equal("000123450", hr.PaySanrentanPay[0]);
        Assert.Equal("0010", hr.PaySanrentanNinki[0]);
    }

    [Fact]
    public void Decode_returns_empty_strings_for_unset_fields()
    {
        var hr = HrDecoder.Decode(new HrFixtureBuilder().Build());

        Assert.Equal(9, hr.FuseirituFlag.Length);
        Assert.Equal(28, hr.HenkanUma.Length);
        Assert.Equal(7, hr.PayWideKumi.Length);
        Assert.Equal(6, hr.PaySanrentanKumi.Length);
        Assert.All(hr.PaySanrentanKumi, s => Assert.Equal(string.Empty, s));
    }
}
