using System;
using Jvlink2Db.Parser.Records;
using Jvlink2Db.Parser.Tests.Fixtures;
using Xunit;

namespace Jvlink2Db.Parser.Tests.Records;

public class DmDecoderTests
{
    [Fact]
    public void Decode_throws_when_buffer_is_too_short()
    {
        var ex = Assert.Throws<ArgumentException>(() => DmDecoder.Decode(new byte[302]));
        Assert.Contains("303", ex.Message);
    }

    [Fact]
    public void Decode_extracts_18_dm_entries()
    {
        var bytes = new DmFixtureBuilder()
            .RaceId("2026", "0331", "06", "01", "08", "11")
            .Entry(0, "01", "13250", "0050", "0040")
            .Entry(17, "18", "13800", "0080", "0060")
            .Build();

        var dm = DmDecoder.Decode(bytes);

        Assert.Equal(18, dm.Umaban.Length);
        Assert.Equal("01", dm.Umaban[0]);
        Assert.Equal("13250", dm.DMTime[0]);
        Assert.Equal("0050", dm.DMGosaP[0]);
        Assert.Equal("13800", dm.DMTime[17]);
    }
}
