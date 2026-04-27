using System;
using Jvlink2Db.Parser.Records;
using Jvlink2Db.Parser.Tests.Fixtures;
using Xunit;

namespace Jvlink2Db.Parser.Tests.Records;

public class BrDecoderTests
{
    [Fact]
    public void Decode_throws_when_buffer_is_too_short()
    {
        var ex = Assert.Throws<ArgumentException>(() => BrDecoder.Decode(new byte[544]));
        Assert.Contains("545", ex.Message);
    }

    [Fact]
    public void Decode_throws_when_record_spec_is_not_BR()
    {
        var bytes = new BrFixtureBuilder().RecordSpec("RA").Build();
        Assert.Throws<InvalidOperationException>(() => BrDecoder.Decode(bytes));
    }

    [Fact]
    public void Decode_extracts_pk_and_top_level_fields()
    {
        var bytes = new BrFixtureBuilder()
            .DataKubun("1")
            .MakeDate("20260331")
            .BreederCode("00012345")
            .BreederName("テスト牧場")
            .Address("北海道")
            .Build();

        var br = BrDecoder.Decode(bytes);

        Assert.Equal("BR", br.RecordSpec);
        Assert.Equal("00012345", br.BreederCode);
        Assert.Equal("テスト牧場", br.BreederName);
        Assert.Equal("北海道", br.Address);
    }

    [Fact]
    public void Decode_extracts_hon_ruikei_with_flattened_chaku_kaisu()
    {
        var bytes = new BrFixtureBuilder()
            .HonRuikei(0, "2025", "0001000000", "0000200000",
                ["000010", "000005", "000003", "000002", "000001", "000020"])
            .HonRuikei(1, "9999", "0009999999", "0001234567",
                ["000050", "000030", "000020", "000010", "000005", "000100"])
            .Build();

        var br = BrDecoder.Decode(bytes);

        Assert.Equal(new[] { "2025", "9999" }, br.HonRuikeiSetYear);
        Assert.Equal(new[] { "0001000000", "0009999999" }, br.HonRuikeiHonsyokinTotal);
        Assert.Equal(new[] { "0000200000", "0001234567" }, br.HonRuikeiFukaSyokin);

        // Flat 12 elements: year 0 placements [0..5], year 1 placements [6..11]
        Assert.Equal(12, br.HonRuikeiChakuKaisu.Length);
        Assert.Equal("000010", br.HonRuikeiChakuKaisu[0]);
        Assert.Equal("000020", br.HonRuikeiChakuKaisu[5]);
        Assert.Equal("000050", br.HonRuikeiChakuKaisu[6]);
        Assert.Equal("000100", br.HonRuikeiChakuKaisu[11]);
    }
}
