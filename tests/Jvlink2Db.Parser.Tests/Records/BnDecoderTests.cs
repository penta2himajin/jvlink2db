using System;
using Jvlink2Db.Parser.Records;
using Jvlink2Db.Parser.Tests.Fixtures;
using Xunit;

namespace Jvlink2Db.Parser.Tests.Records;

public class BnDecoderTests
{
    [Fact]
    public void Decode_throws_when_buffer_is_too_short()
    {
        var ex = Assert.Throws<ArgumentException>(() => BnDecoder.Decode(new byte[476]));
        Assert.Contains("477", ex.Message);
    }

    [Fact]
    public void Decode_throws_when_record_spec_is_not_BN()
    {
        var bytes = new BnFixtureBuilder().RecordSpec("RA").Build();
        Assert.Throws<InvalidOperationException>(() => BnDecoder.Decode(bytes));
    }

    [Fact]
    public void Decode_extracts_pk_and_fields()
    {
        var bytes = new BnFixtureBuilder()
            .DataKubun("1")
            .MakeDate("20260331")
            .BanusiCode("123456")
            .BanusiName("テストオーナー")
            .Fukusyoku("赤・白縦縞")
            .HonRuikei(0, "2025", "0001000000", "0000200000",
                ["000010", "000005", "000003", "000002", "000001", "000020"])
            .Build();

        var bn = BnDecoder.Decode(bytes);

        Assert.Equal("BN", bn.RecordSpec);
        Assert.Equal("123456", bn.BanusiCode);
        Assert.Equal("テストオーナー", bn.BanusiName);
        Assert.Equal("赤・白縦縞", bn.Fukusyoku);
        Assert.Equal("2025", bn.HonRuikeiSetYear[0]);
        Assert.Equal(12, bn.HonRuikeiChakuKaisu.Length);
        Assert.Equal("000010", bn.HonRuikeiChakuKaisu[0]);
    }
}
