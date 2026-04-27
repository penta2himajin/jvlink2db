using System;
using Jvlink2Db.Parser.Records;
using Jvlink2Db.Parser.Tests.Fixtures;
using Xunit;

namespace Jvlink2Db.Parser.Tests.Records;

public class H6DecoderTests
{
    [Fact]
    public void Decode_throws_when_buffer_is_too_short()
    {
        var ex = Assert.Throws<ArgumentException>(() => H6Decoder.Decode(new byte[102889]));
        Assert.Contains("102890", ex.Message);
    }

    [Fact]
    public void Decode_throws_when_record_spec_is_not_H6()
    {
        var bytes = new H6FixtureBuilder().RecordSpec("RA").Build();
        Assert.Throws<InvalidOperationException>(() => H6Decoder.Decode(bytes));
    }

    [Fact]
    public void Decode_extracts_4896_sanrentan_entries()
    {
        var bytes = new H6FixtureBuilder()
            .RaceId("2026", "0331", "06", "01", "08", "11")
            .SanrentanEntry(0, "010203", "00000067890", "0001")
            .SanrentanEntry(2447, "070809", "00000012345", "2448")
            .SanrentanEntry(4895, "141516", "00000000005", "4896")
            .HyoTotal(0, "01000000000")
            .HyoTotal(1, "00500000000")
            .Build();

        var h6 = H6Decoder.Decode(bytes);

        Assert.Equal(4896, h6.SanrentanKumi.Length);
        Assert.Equal("010203", h6.SanrentanKumi[0]);
        Assert.Equal("00000067890", h6.SanrentanHyo[0]);
        Assert.Equal("0001", h6.SanrentanNinki[0]);
        Assert.Equal("070809", h6.SanrentanKumi[2447]);
        Assert.Equal("141516", h6.SanrentanKumi[4895]);

        Assert.Equal(2, h6.HyoTotal.Length);
        Assert.Equal("01000000000", h6.HyoTotal[0]);
        Assert.Equal("00500000000", h6.HyoTotal[1]);
    }
}
