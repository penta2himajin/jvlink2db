using System;
using Jvlink2Db.Parser.Records;
using Jvlink2Db.Parser.Tests.Fixtures;
using Xunit;

namespace Jvlink2Db.Parser.Tests.Records;

public class CkDecoderTests
{
    [Fact]
    public void Decode_throws_when_buffer_is_too_short()
    {
        var ex = Assert.Throws<ArgumentException>(() => CkDecoder.Decode(new byte[6869]));
        Assert.Contains("6870", ex.Message);
    }

    [Fact]
    public void Decode_extracts_subject_codes_and_uma_chaku_scalars()
    {
        var bytes = new CkFixtureBuilder()
            .Year("2026")
            .RaceNum("11")
            .KettoNum("2020100123")
            .Bamei("テスト馬")
            .RuikeiHonsyoHeiti("000123450")
            .RaceCount("042")
            .KisyuCode("00001")
            .ChokyosiCode("00099")
            .BanusiCode("000123")
            .BreederCode("00012345")
            .Build();

        var ck = CkDecoder.Decode(bytes);

        Assert.Equal("CK", ck.RecordSpec);
        Assert.Equal("2026", ck.Year);
        Assert.Equal("11", ck.RaceNum);
        Assert.Equal("2020100123", ck.KettoNum);
        Assert.Equal("テスト馬", ck.Bamei);
        Assert.Equal("000123450", ck.RuikeiHonsyoHeiti);
        Assert.Equal("042", ck.RaceCount);
        Assert.Equal(4, ck.Kyakusitu.Length);
        Assert.Equal("00001", ck.KisyuCode);
        Assert.Equal("00099", ck.ChokyosiCode);
        Assert.Equal("000123", ck.BanusiCode);
        Assert.Equal("00012345", ck.BreederCode);
    }
}
