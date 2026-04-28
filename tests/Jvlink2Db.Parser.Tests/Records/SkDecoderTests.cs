using System;
using Jvlink2Db.Parser.Records;
using Jvlink2Db.Parser.Tests.Fixtures;
using Xunit;

namespace Jvlink2Db.Parser.Tests.Records;

public class SkDecoderTests
{
    [Fact]
    public void Decode_throws_when_buffer_is_too_short()
    {
        var ex = Assert.Throws<ArgumentException>(() => SkDecoder.Decode(new byte[207]));
        Assert.Contains("208", ex.Message);
    }

    [Fact]
    public void Decode_extracts_pk_and_pedigree()
    {
        var bytes = new SkFixtureBuilder()
            .KettoNum("2020100123")
            .BirthDate("20200315")
            .Hansyoku(0, "0001234567")
            .Hansyoku(13, "0099999999")
            .Build();

        var sk = SkDecoder.Decode(bytes);

        Assert.Equal("SK", sk.RecordSpec);
        Assert.Equal("2020100123", sk.KettoNum);
        Assert.Equal("20200315", sk.BirthDate);
        Assert.Equal(14, sk.HansyokuNum.Length);
        Assert.Equal("0001234567", sk.HansyokuNum[0]);
        Assert.Equal("0099999999", sk.HansyokuNum[13]);
    }
}
