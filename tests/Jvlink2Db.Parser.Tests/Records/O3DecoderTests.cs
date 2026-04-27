using System;
using Jvlink2Db.Parser.Records;
using Jvlink2Db.Parser.Tests.Fixtures;
using Xunit;

namespace Jvlink2Db.Parser.Tests.Records;

public class O3DecoderTests
{
    [Fact]
    public void Decode_throws_when_buffer_is_too_short()
    {
        var ex = Assert.Throws<ArgumentException>(() => O3Decoder.Decode(new byte[2653]));
        Assert.Contains("2654", ex.Message);
    }

    [Fact]
    public void Decode_throws_when_record_spec_is_not_O3()
    {
        var bytes = new O3FixtureBuilder().RecordSpec("RA").Build();
        Assert.Throws<InvalidOperationException>(() => O3Decoder.Decode(bytes));
    }

    [Fact]
    public void Decode_extracts_low_high_odds_pair()
    {
        var bytes = new O3FixtureBuilder()
            .HappyoTime("03311534")
            .Entry(0, "0102", "01250", "02500", "001")
            .Entry(76, "0708", "00150", "00200", "030")
            .Entry(152, "1516", "99999", "99999", "153")
            .TotalHyosuWide("00500000000")
            .Build();

        var o3 = O3Decoder.Decode(bytes);

        Assert.Equal(153, o3.Kumi.Length);
        Assert.Equal("0102", o3.Kumi[0]);
        Assert.Equal("01250", o3.OddsLow[0]);
        Assert.Equal("02500", o3.OddsHigh[0]);
        Assert.Equal("001", o3.Ninki[0]);
        Assert.Equal("0708", o3.Kumi[76]);
        Assert.Equal("1516", o3.Kumi[152]);
        Assert.Equal("00500000000", o3.TotalHyosuWide);
    }
}
