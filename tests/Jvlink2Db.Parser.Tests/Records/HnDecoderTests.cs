using System;
using Jvlink2Db.Parser.Records;
using Jvlink2Db.Parser.Tests.Fixtures;
using Xunit;

namespace Jvlink2Db.Parser.Tests.Records;

public class HnDecoderTests
{
    [Fact]
    public void Decode_throws_when_buffer_is_too_short()
    {
        var ex = Assert.Throws<ArgumentException>(() => HnDecoder.Decode(new byte[250]));
        Assert.Contains("251", ex.Message);
    }

    [Fact]
    public void Decode_extracts_pk_and_parents()
    {
        var bytes = new HnFixtureBuilder()
            .HansyokuNum("0099999999")
            .KettoNum("2018104567")
            .Bamei("テスト繁殖馬")
            .HansyokuFNum("0001234567")
            .HansyokuMNum("0007654321")
            .Build();

        var hn = HnDecoder.Decode(bytes);

        Assert.Equal("HN", hn.RecordSpec);
        Assert.Equal("0099999999", hn.HansyokuNum);
        Assert.Equal("2018104567", hn.KettoNum);
        Assert.Equal("テスト繁殖馬", hn.Bamei);
        Assert.Equal("0001234567", hn.HansyokuFNum);
        Assert.Equal("0007654321", hn.HansyokuMNum);
    }
}
