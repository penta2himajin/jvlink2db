using System;
using Jvlink2Db.Parser.Records;
using Jvlink2Db.Parser.Tests.Fixtures;
using Xunit;

namespace Jvlink2Db.Parser.Tests.Records;

public class ChDecoderTests
{
    [Fact]
    public void Decode_throws_when_buffer_is_too_short()
    {
        var ex = Assert.Throws<ArgumentException>(() => ChDecoder.Decode(new byte[3861]));
        Assert.Contains("3862", ex.Message);
    }

    [Fact]
    public void Decode_throws_when_record_spec_is_not_CH()
    {
        var bytes = new ChFixtureBuilder().RecordSpec("RA").Build();
        Assert.Throws<InvalidOperationException>(() => ChDecoder.Decode(bytes));
    }

    [Fact]
    public void Decode_extracts_top_level_and_saikin_jyusyo()
    {
        var bytes = new ChFixtureBuilder()
            .DataKubun("1")
            .MakeDate("20260331")
            .ChokyosiCode("00567")
            .ChokyosiName("テスト調教師")
            .BirthDate("19700101")
            .SaikinJyusyo(0, "2025", "1228", "有馬記念")
            .SaikinJyusyo(2, "2025", "0427", "天皇賞")
            .Build();

        var ch = ChDecoder.Decode(bytes);

        Assert.Equal("CH", ch.RecordSpec);
        Assert.Equal("00567", ch.ChokyosiCode);
        Assert.Equal("テスト調教師", ch.ChokyosiName);
        Assert.Equal("19700101", ch.BirthDate);
        Assert.Equal(3, ch.SaikinJyusyoYear.Length);
        Assert.Equal("有馬記念", ch.SaikinJyusyoHondai[0]);
        Assert.Equal("天皇賞", ch.SaikinJyusyoHondai[2]);
    }
}
