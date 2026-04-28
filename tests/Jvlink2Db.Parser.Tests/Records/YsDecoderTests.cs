using System;
using Jvlink2Db.Parser.Records;
using Jvlink2Db.Parser.Tests.Fixtures;
using Xunit;

namespace Jvlink2Db.Parser.Tests.Records;

public class YsDecoderTests
{
    [Fact]
    public void Decode_throws_when_buffer_is_too_short()
    {
        var ex = Assert.Throws<ArgumentException>(() => YsDecoder.Decode(new byte[381]));
        Assert.Contains("382", ex.Message);
    }

    [Fact]
    public void Decode_extracts_race_id2_and_jyusyo_entries()
    {
        var bytes = new YsFixtureBuilder()
            .Year("2026")
            .MonthDay("0503")
            .JyoCD("05")
            .Kaiji("02")
            .Nichiji("06")
            .JyusyoTokuNum(0, "0011")
            .JyusyoHondai(0, "天皇賞(春)")
            .JyusyoTokuNum(1, "0012")
            .Build();

        var ys = YsDecoder.Decode(bytes);

        Assert.Equal("YS", ys.RecordSpec);
        Assert.Equal("2026", ys.Year);
        Assert.Equal("0503", ys.MonthDay);
        Assert.Equal("05", ys.JyoCD);
        Assert.Equal("02", ys.Kaiji);
        Assert.Equal("06", ys.Nichiji);
        Assert.Equal(3, ys.JyusyoTokuNum.Length);
        Assert.Equal("0011", ys.JyusyoTokuNum[0]);
        Assert.Equal("0012", ys.JyusyoTokuNum[1]);
        Assert.Equal("天皇賞(春)", ys.JyusyoHondai[0]);
    }
}
