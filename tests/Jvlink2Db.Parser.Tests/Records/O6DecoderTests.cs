using System;
using Jvlink2Db.Parser.Records;
using Jvlink2Db.Parser.Tests.Fixtures;
using Xunit;

namespace Jvlink2Db.Parser.Tests.Records;

public class O6DecoderTests
{
    [Fact]
    public void Decode_throws_when_buffer_is_too_short()
    {
        var ex = Assert.Throws<ArgumentException>(() => O6Decoder.Decode(new byte[83284]));
        Assert.Contains("83285", ex.Message);
    }

    [Fact]
    public void Decode_throws_when_record_spec_is_not_O6()
    {
        var bytes = new O6FixtureBuilder().RecordSpec("RA").Build();
        Assert.Throws<InvalidOperationException>(() => O6Decoder.Decode(bytes));
    }

    [Fact]
    public void Decode_extracts_4896_entries()
    {
        var bytes = new O6FixtureBuilder()
            .HappyoTime("03311534")
            .Entry(0, "010203", "0123456", "0001")
            .Entry(2447, "070809", "0987654", "2448")
            .Entry(4895, "141516", "9999999", "4896")
            .TotalHyosuSanrentan("00200000000")
            .Build();

        var o6 = O6Decoder.Decode(bytes);

        Assert.Equal(4896, o6.Kumi.Length);
        Assert.Equal("010203", o6.Kumi[0]);
        Assert.Equal("0123456", o6.Odds[0]);
        Assert.Equal("0001", o6.Ninki[0]);
        Assert.Equal("070809", o6.Kumi[2447]);
        Assert.Equal("141516", o6.Kumi[4895]);
        Assert.Equal("00200000000", o6.TotalHyosuSanrentan);
    }
}
