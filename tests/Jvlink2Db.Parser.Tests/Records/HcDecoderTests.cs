using System;
using Jvlink2Db.Parser.Records;
using Jvlink2Db.Parser.Tests.Fixtures;
using Xunit;

namespace Jvlink2Db.Parser.Tests.Records;

public class HcDecoderTests
{
    [Fact]
    public void Decode_throws_when_buffer_is_too_short()
    {
        var ex = Assert.Throws<ArgumentException>(() => HcDecoder.Decode(new byte[59]));
        Assert.Contains("60", ex.Message);
    }

    [Fact]
    public void Decode_throws_when_record_spec_is_not_HC()
    {
        var bytes = new HcFixtureBuilder().RecordSpec("RA").Build();
        Assert.Throws<InvalidOperationException>(() => HcDecoder.Decode(bytes));
    }

    [Fact]
    public void Decode_extracts_pk_and_haron_times()
    {
        var bytes = new HcFixtureBuilder()
            .TresenKubun("0")
            .ChokyoDate("20260415")
            .ChokyoTime("0800")
            .KettoNum("2020100123")
            .HaronTime4("0567")
            .LapTime1("130")
            .Build();

        var hc = HcDecoder.Decode(bytes);

        Assert.Equal("HC", hc.RecordSpec);
        Assert.Equal("0", hc.TresenKubun);
        Assert.Equal("20260415", hc.ChokyoDate);
        Assert.Equal("0800", hc.ChokyoTime);
        Assert.Equal("2020100123", hc.KettoNum);
        Assert.Equal("0567", hc.HaronTime4);
        Assert.Equal("130", hc.LapTime1);
    }
}
