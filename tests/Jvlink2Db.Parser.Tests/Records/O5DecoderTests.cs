using System;
using Jvlink2Db.Parser.Records;
using Jvlink2Db.Parser.Tests.Fixtures;
using Xunit;

namespace Jvlink2Db.Parser.Tests.Records;

public class O5DecoderTests
{
    [Fact]
    public void Decode_throws_when_buffer_is_too_short()
    {
        var ex = Assert.Throws<ArgumentException>(() => O5Decoder.Decode(new byte[12292]));
        Assert.Contains("12293", ex.Message);
    }

    [Fact]
    public void Decode_throws_when_record_spec_is_not_O5()
    {
        var bytes = new O5FixtureBuilder().RecordSpec("RA").Build();
        Assert.Throws<InvalidOperationException>(() => O5Decoder.Decode(bytes));
    }

    [Fact]
    public void Decode_extracts_816_entries()
    {
        var bytes = new O5FixtureBuilder()
            .HappyoTime("03311534")
            .Entry(0, "010203", "012345", "001")
            .Entry(407, "070809", "098765", "408")
            .Entry(815, "141516", "999999", "816")
            .TotalHyosuSanrenpuku("00400000000")
            .Build();

        var o5 = O5Decoder.Decode(bytes);

        Assert.Equal(816, o5.Kumi.Length);
        Assert.Equal("010203", o5.Kumi[0]);
        Assert.Equal("070809", o5.Kumi[407]);
        Assert.Equal("141516", o5.Kumi[815]);
        Assert.Equal("00400000000", o5.TotalHyosuSanrenpuku);
    }
}
