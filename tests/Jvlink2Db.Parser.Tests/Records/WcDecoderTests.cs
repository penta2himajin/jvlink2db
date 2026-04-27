using System;
using Jvlink2Db.Parser.Records;
using Jvlink2Db.Parser.Tests.Fixtures;
using Xunit;

namespace Jvlink2Db.Parser.Tests.Records;

public class WcDecoderTests
{
    [Fact]
    public void Decode_throws_when_buffer_is_too_short()
    {
        var ex = Assert.Throws<ArgumentException>(() => WcDecoder.Decode(new byte[104]));
        Assert.Contains("105", ex.Message);
    }

    [Fact]
    public void Decode_throws_when_record_spec_is_not_WC()
    {
        var bytes = new WcFixtureBuilder().RecordSpec("RA").Build();
        Assert.Throws<InvalidOperationException>(() => WcDecoder.Decode(bytes));
    }

    [Fact]
    public void Decode_extracts_pk_and_haron_times()
    {
        var bytes = new WcFixtureBuilder()
            .TresenKubun("0")
            .ChokyoDate("20260415")
            .ChokyoTime("0800")
            .KettoNum("2020100123")
            .HaronTime10("1450")
            .LapTime1("130")
            .Build();

        var wc = WcDecoder.Decode(bytes);

        Assert.Equal("WC", wc.RecordSpec);
        Assert.Equal("0", wc.TresenKubun);
        Assert.Equal("20260415", wc.ChokyoDate);
        Assert.Equal("2020100123", wc.KettoNum);
        Assert.Equal("1450", wc.HaronTime10);
        Assert.Equal("130", wc.LapTime1);
    }
}
