using System;
using Jvlink2Db.Parser.Records;
using Jvlink2Db.Parser.Tests.Fixtures;
using Xunit;

namespace Jvlink2Db.Parser.Tests.Records;

public class WhDecoderTests
{
    [Fact]
    public void Decode_throws_when_buffer_is_too_short()
    {
        var ex = Assert.Throws<ArgumentException>(() => WhDecoder.Decode(new byte[846]));
        Assert.Contains("847", ex.Message);
    }

    [Fact]
    public void Decode_extracts_race_id_and_bataijyu_entries()
    {
        var bytes = new WhFixtureBuilder()
            .Year("2026")
            .MonthDay("0503")
            .RaceNum("11")
            .Umaban(0, "01")
            .BaTaijyu(0, "478")
            .Umaban(17, "18")
            .BaTaijyu(17, "502")
            .Build();

        var wh = WhDecoder.Decode(bytes);

        Assert.Equal("WH", wh.RecordSpec);
        Assert.Equal("2026", wh.Year);
        Assert.Equal("11", wh.RaceNum);
        Assert.Equal(18, wh.Umaban.Length);
        Assert.Equal("01", wh.Umaban[0]);
        Assert.Equal("478", wh.BaTaijyu[0]);
        Assert.Equal("18", wh.Umaban[17]);
        Assert.Equal("502", wh.BaTaijyu[17]);
    }
}
