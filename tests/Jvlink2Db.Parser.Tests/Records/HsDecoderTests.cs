using System;
using Jvlink2Db.Parser.Records;
using Jvlink2Db.Parser.Tests.Fixtures;
using Xunit;

namespace Jvlink2Db.Parser.Tests.Records;

public class HsDecoderTests
{
    [Fact]
    public void Decode_throws_when_buffer_is_too_short()
    {
        var ex = Assert.Throws<ArgumentException>(() => HsDecoder.Decode(new byte[199]));
        Assert.Contains("200", ex.Message);
    }

    [Fact]
    public void Decode_extracts_pk_and_price()
    {
        var bytes = new HsFixtureBuilder()
            .KettoNum("2020100123")
            .SaleCode("000001")
            .Price("0050000000")
            .Build();

        var hs = HsDecoder.Decode(bytes);

        Assert.Equal("HS", hs.RecordSpec);
        Assert.Equal("2020100123", hs.KettoNum);
        Assert.Equal("000001", hs.SaleCode);
        Assert.Equal("0050000000", hs.Price);
    }
}
