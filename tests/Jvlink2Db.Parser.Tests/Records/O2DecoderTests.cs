using System;
using Jvlink2Db.Parser.Records;
using Jvlink2Db.Parser.Tests.Fixtures;
using Xunit;

namespace Jvlink2Db.Parser.Tests.Records;

public class O2DecoderTests
{
    [Fact]
    public void Decode_throws_when_buffer_is_too_short()
    {
        var ex = Assert.Throws<ArgumentException>(() => O2Decoder.Decode(new byte[2041]));
        Assert.Contains("2042", ex.Message);
    }

    [Fact]
    public void Decode_throws_when_record_spec_is_not_O2()
    {
        var bytes = new O2FixtureBuilder().RecordSpec("RA").Build();
        Assert.Throws<InvalidOperationException>(() => O2Decoder.Decode(bytes));
    }

    [Fact]
    public void Decode_extracts_153_entries_with_first_middle_last()
    {
        var bytes = new O2FixtureBuilder()
            .RaceId("2026", "0331", "06", "01", "08", "11")
            .HappyoTime("03311534")
            .Entry(0, "0102", "000345", "002")
            .Entry(76, "0708", "012345", "077")
            .Entry(152, "1516", "999999", "153")
            .TotalHyosuUmaren("01000000000")
            .Build();

        var o2 = O2Decoder.Decode(bytes);

        Assert.Equal(153, o2.Kumi.Length);
        Assert.Equal("0102", o2.Kumi[0]);
        Assert.Equal("000345", o2.Odds[0]);
        Assert.Equal("002", o2.Ninki[0]);
        Assert.Equal("0708", o2.Kumi[76]);
        Assert.Equal("1516", o2.Kumi[152]);
        Assert.Equal("999999", o2.Odds[152]);
        Assert.Equal("01000000000", o2.TotalHyosuUmaren);
    }
}
