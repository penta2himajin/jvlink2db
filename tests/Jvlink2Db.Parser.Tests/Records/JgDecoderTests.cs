using System;
using Jvlink2Db.Parser.Records;
using Jvlink2Db.Parser.Tests.Fixtures;
using Xunit;

namespace Jvlink2Db.Parser.Tests.Records;

public class JgDecoderTests
{
    [Fact]
    public void Decode_throws_when_buffer_is_too_short()
    {
        var ex = Assert.Throws<ArgumentException>(() => JgDecoder.Decode(new byte[79]));
        Assert.Contains("80", ex.Message);
    }

    [Fact]
    public void Decode_throws_when_record_spec_is_not_JG()
    {
        var bytes = new JgFixtureBuilder().RecordSpec("RA").Build();
        Assert.Throws<InvalidOperationException>(() => JgDecoder.Decode(bytes));
    }

    [Fact]
    public void Decode_extracts_all_fields()
    {
        var bytes = new JgFixtureBuilder()
            .DataKubun("1")
            .MakeDate("20260331")
            .RaceId("2026", "0331", "06", "01", "08", "11")
            .KettoNum("2020100123")
            .Bamei("除外馬名")
            .ShutsubaTohyoJun("002")
            .ShussoKubun("1")
            .JogaiJotaiKubun("3")
            .Build();

        var jg = JgDecoder.Decode(bytes);

        Assert.Equal("JG", jg.RecordSpec);
        Assert.Equal("2026", jg.Year);
        Assert.Equal("11", jg.RaceNum);
        Assert.Equal("2020100123", jg.KettoNum);
        Assert.Equal("除外馬名", jg.Bamei);
        Assert.Equal("002", jg.ShutsubaTohyoJun);
        Assert.Equal("1", jg.ShussoKubun);
        Assert.Equal("3", jg.JogaiJotaiKubun);
    }
}
