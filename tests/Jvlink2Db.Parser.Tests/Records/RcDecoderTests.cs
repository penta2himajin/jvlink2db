using System;
using Jvlink2Db.Parser.Records;
using Jvlink2Db.Parser.Tests.Fixtures;
using Xunit;

namespace Jvlink2Db.Parser.Tests.Records;

public class RcDecoderTests
{
    [Fact]
    public void Decode_throws_when_buffer_is_too_short()
    {
        var ex = Assert.Throws<ArgumentException>(() => RcDecoder.Decode(new byte[500]));
        Assert.Contains("501", ex.Message);
    }

    [Fact]
    public void Decode_throws_when_record_spec_is_not_RC()
    {
        var bytes = new RcFixtureBuilder().RecordSpec("RA").Build();
        Assert.Throws<InvalidOperationException>(() => RcDecoder.Decode(bytes));
    }

    [Fact]
    public void Decode_extracts_pk_and_record_uma_info()
    {
        var bytes = new RcFixtureBuilder()
            .DataKubun("1")
            .MakeDate("20260331")
            .RecInfoKubun("1")
            .RaceId("2026", "0331", "06", "01", "08", "11")
            .Hondai("有馬記念")
            .GradeCD("E")
            .Kyori("2500")
            .TrackCD("23")
            .RecTime("1485")
            .RecUma(0, "2018104567", "テストレコードホースＡ", "テスト騎手Ａ")
            .RecUma(1, "2019100000", "テストレコードホースＢ", "テスト騎手Ｂ")
            .RecUma(2, "2020100000", "テストレコードホースＣ", "テスト騎手Ｃ")
            .Build();

        var rc = RcDecoder.Decode(bytes);

        Assert.Equal("RC", rc.RecordSpec);
        Assert.Equal("1", rc.RecInfoKubun);
        Assert.Equal("2026", rc.Year);
        Assert.Equal("有馬記念", rc.Hondai);
        Assert.Equal("2500", rc.Kyori);
        Assert.Equal("1485", rc.RecTime);

        Assert.Equal(3, rc.RecUmaKettoNum.Length);
        Assert.Equal("2018104567", rc.RecUmaKettoNum[0]);
        Assert.Equal("テストレコードホースＡ", rc.RecUmaBamei[0]);
        Assert.Equal("テスト騎手Ｃ", rc.RecUmaKisyuName[2]);
    }
}
