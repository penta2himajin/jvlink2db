using System;
using Jvlink2Db.Parser.Records;
using Jvlink2Db.Parser.Tests.Fixtures;
using Xunit;

namespace Jvlink2Db.Parser.Tests.Records;

public class TmDecoderTests
{
    [Fact]
    public void Decode_throws_when_buffer_is_too_short()
    {
        var ex = Assert.Throws<ArgumentException>(() => TmDecoder.Decode(new byte[140]));
        Assert.Contains("141", ex.Message);
    }

    [Fact]
    public void Decode_extracts_18_score_entries()
    {
        var bytes = new TmFixtureBuilder()
            .RaceId("2026", "0331", "06", "01", "08", "11")
            .Entry(0, "01", "0850")
            .Entry(17, "18", "0210")
            .Build();

        var tm = TmDecoder.Decode(bytes);

        Assert.Equal("TM", tm.RecordSpec);
        Assert.Equal(18, tm.Umaban.Length);
        Assert.Equal("01", tm.Umaban[0]);
        Assert.Equal("0850", tm.TMScore[0]);
        Assert.Equal("18", tm.Umaban[17]);
        Assert.Equal("0210", tm.TMScore[17]);
    }
}
