using System;
using Jvlink2Db.Parser.Records;
using Jvlink2Db.Parser.Tests.Fixtures;
using Xunit;

namespace Jvlink2Db.Parser.Tests.Records;

public class HyDecoderTests
{
    [Fact]
    public void Decode_throws_when_buffer_is_too_short()
    {
        var ex = Assert.Throws<ArgumentException>(() => HyDecoder.Decode(new byte[122]));
        Assert.Contains("123", ex.Message);
    }

    [Fact]
    public void Decode_extracts_all_fields()
    {
        var bytes = new HyFixtureBuilder()
            .KettoNum("2020100123")
            .Bamei("テストホース")
            .Origin("テスト由来")
            .Build();

        var hy = HyDecoder.Decode(bytes);

        Assert.Equal("HY", hy.RecordSpec);
        Assert.Equal("2020100123", hy.KettoNum);
        Assert.Equal("テストホース", hy.Bamei);
        Assert.Equal("テスト由来", hy.Origin);
    }
}
