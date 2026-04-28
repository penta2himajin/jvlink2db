using System;
using Jvlink2Db.Parser.Records;
using Jvlink2Db.Parser.Tests.Fixtures;
using Xunit;

namespace Jvlink2Db.Parser.Tests.Records;

public class CsDecoderTests
{
    [Fact]
    public void Decode_throws_when_buffer_is_too_short()
    {
        var ex = Assert.Throws<ArgumentException>(() => CsDecoder.Decode(new byte[6828]));
        Assert.Contains("6829", ex.Message);
    }

    [Fact]
    public void Decode_extracts_course_pk()
    {
        var bytes = new CsFixtureBuilder()
            .JyoCD("05")
            .Kyori("3200")
            .TrackCD("10")
            .KaishuDate("20240101")
            .Build();

        var cs = CsDecoder.Decode(bytes);

        Assert.Equal("CS", cs.RecordSpec);
        Assert.Equal("05", cs.JyoCD);
        Assert.Equal("3200", cs.Kyori);
        Assert.Equal("10", cs.TrackCD);
        Assert.Equal("20240101", cs.KaishuDate);
    }
}
