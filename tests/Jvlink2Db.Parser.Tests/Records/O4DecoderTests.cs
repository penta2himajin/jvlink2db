using System;
using Jvlink2Db.Parser.Records;
using Jvlink2Db.Parser.Tests.Fixtures;
using Xunit;

namespace Jvlink2Db.Parser.Tests.Records;

public class O4DecoderTests
{
    [Fact]
    public void Decode_throws_when_buffer_is_too_short()
    {
        var ex = Assert.Throws<ArgumentException>(() => O4Decoder.Decode(new byte[4030]));
        Assert.Contains("4031", ex.Message);
    }

    [Fact]
    public void Decode_throws_when_record_spec_is_not_O4()
    {
        var bytes = new O4FixtureBuilder().RecordSpec("RA").Build();
        Assert.Throws<InvalidOperationException>(() => O4Decoder.Decode(bytes));
    }

    [Fact]
    public void Decode_extracts_306_entries()
    {
        var bytes = new O4FixtureBuilder()
            .HappyoTime("03311534")
            .Entry(0, "0102", "012345", "001")
            .Entry(152, "0708", "098765", "152")
            .Entry(305, "1615", "999999", "306")
            .TotalHyosuUmatan("00800000000")
            .Build();

        var o4 = O4Decoder.Decode(bytes);

        Assert.Equal(306, o4.Kumi.Length);
        Assert.Equal("0102", o4.Kumi[0]);
        Assert.Equal("0708", o4.Kumi[152]);
        Assert.Equal("1615", o4.Kumi[305]);
        Assert.Equal("00800000000", o4.TotalHyosuUmatan);
    }
}
