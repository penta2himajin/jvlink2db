using System;
using Jvlink2Db.Parser.Records;
using Jvlink2Db.Parser.Tests.Fixtures;
using Xunit;

namespace Jvlink2Db.Parser.Tests.Records;

public class SeDecoderTests
{
    [Fact]
    public void Decode_throws_when_buffer_is_too_short()
    {
        var bytes = new byte[554];

        var ex = Assert.Throws<ArgumentException>(() => SeDecoder.Decode(bytes));
        Assert.Contains("555", ex.Message);
    }

    [Fact]
    public void Decode_throws_when_record_spec_is_not_SE()
    {
        var bytes = new SeFixtureBuilder().RecordSpec("RA").Build();

        var ex = Assert.Throws<InvalidOperationException>(() => SeDecoder.Decode(bytes));
        Assert.Contains("SE", ex.Message);
        Assert.Contains("RA", ex.Message);
    }

    [Fact]
    public void Decode_extracts_pk_and_horse_identity()
    {
        var bytes = new SeFixtureBuilder()
            .DataKubun("7")
            .MakeDate("20260331")
            .RaceId(year: "2026", monthDay: "0331", jyoCd: "06", kaiji: "01", nichiji: "08", raceNum: "11")
            .Wakuban("3")
            .Umaban("05")
            .KettoNum("2020100123")
            .Bamei("テストホースＡ")
            .Build();

        var se = SeDecoder.Decode(bytes);

        Assert.Equal("SE", se.RecordSpec);
        Assert.Equal("7", se.DataKubun);
        Assert.Equal("20260331", se.MakeDate);
        Assert.Equal("2026", se.Year);
        Assert.Equal("0331", se.MonthDay);
        Assert.Equal("06", se.JyoCD);
        Assert.Equal("01", se.Kaiji);
        Assert.Equal("08", se.Nichiji);
        Assert.Equal("11", se.RaceNum);
        Assert.Equal("3", se.Wakuban);
        Assert.Equal("05", se.Umaban);
        Assert.Equal("2020100123", se.KettoNum);
        Assert.Equal("テストホースＡ", se.Bamei);
    }

    [Fact]
    public void Decode_extracts_horse_attributes_and_connections()
    {
        var bytes = new SeFixtureBuilder()
            .UmaKigoCD("00")
            .SexCD("1")
            .HinsyuCD("1")
            .KeiroCD("01")
            .Barei("04")
            .TozaiCD("1")
            .ChokyosiCode("01234")
            .ChokyosiRyakusyo("テスト調")
            .BanusiCode("123456")
            .BanusiName("テストオーナー")
            .KisyuCode("00123")
            .KisyuRyakusyo("テスト騎")
            .Build();

        var se = SeDecoder.Decode(bytes);

        Assert.Equal("00", se.UmaKigoCD);
        Assert.Equal("1", se.SexCD);
        Assert.Equal("1", se.HinsyuCD);
        Assert.Equal("01", se.KeiroCD);
        Assert.Equal("04", se.Barei);
        Assert.Equal("1", se.TozaiCD);
        Assert.Equal("01234", se.ChokyosiCode);
        Assert.Equal("テスト調", se.ChokyosiRyakusyo);
        Assert.Equal("123456", se.BanusiCode);
        Assert.Equal("テストオーナー", se.BanusiName);
        Assert.Equal("00123", se.KisyuCode);
        Assert.Equal("テスト騎", se.KisyuRyakusyo);
    }

    [Fact]
    public void Decode_extracts_race_result_fields()
    {
        var bytes = new SeFixtureBuilder()
            .Futan("550")
            .BaTaijyu("478")
            .ZogenFugo("+")
            .ZogenSa("002")
            .NyusenJyuni("01")
            .KakuteiJyuni("01")
            .Time("1325")
            .ChakusaCD("003")
            .Jyuni1c("03").Jyuni2c("02").Jyuni3c("02").Jyuni4c("01")
            .Odds("0345")
            .Ninki("02")
            .Honsyokin("00040000")
            .Fukasyokin("00010000")
            .HaronTimeL3("345")
            .HaronTimeL4("466")
            .TimeDiff("0010")
            .Build();

        var se = SeDecoder.Decode(bytes);

        Assert.Equal("550", se.Futan);
        Assert.Equal("478", se.BaTaijyu);
        Assert.Equal("+", se.ZogenFugo);
        Assert.Equal("002", se.ZogenSa);
        Assert.Equal("01", se.NyusenJyuni);
        Assert.Equal("01", se.KakuteiJyuni);
        Assert.Equal("1325", se.Time);
        Assert.Equal("003", se.ChakusaCD);
        Assert.Equal("03", se.Jyuni1c);
        Assert.Equal("02", se.Jyuni2c);
        Assert.Equal("02", se.Jyuni3c);
        Assert.Equal("01", se.Jyuni4c);
        Assert.Equal("0345", se.Odds);
        Assert.Equal("02", se.Ninki);
        Assert.Equal("00040000", se.Honsyokin);
        Assert.Equal("00010000", se.Fukasyokin);
        Assert.Equal("345", se.HaronTimeL3);
        Assert.Equal("466", se.HaronTimeL4);
        Assert.Equal("0010", se.TimeDiff);
    }

    [Fact]
    public void Decode_extracts_three_chaku_uma_entries()
    {
        var bytes = new SeFixtureBuilder()
            .ChakuUma(0, "2020100001", "ウマＡ")
            .ChakuUma(1, "2020100002", "ウマＢ")
            .ChakuUma(2, "2020100003", "ウマＣ")
            .Build();

        var se = SeDecoder.Decode(bytes);

        Assert.Equal(3, se.ChakuUma.Length);
        Assert.Equal("2020100001", se.ChakuUma[0].KettoNum);
        Assert.Equal("ウマＡ", se.ChakuUma[0].Bamei);
        Assert.Equal("2020100003", se.ChakuUma[2].KettoNum);
        Assert.Equal("ウマＣ", se.ChakuUma[2].Bamei);
    }

    [Fact]
    public void Decode_extracts_data_mining_fields()
    {
        var bytes = new SeFixtureBuilder()
            .DMKubun("1")
            .DMTime("13250")
            .DMGosaP("0050")
            .DMGosaM("0040")
            .DMJyuni("03")
            .KyakusituKubun("2")
            .Build();

        var se = SeDecoder.Decode(bytes);

        Assert.Equal("1", se.DMKubun);
        Assert.Equal("13250", se.DMTime);
        Assert.Equal("0050", se.DMGosaP);
        Assert.Equal("0040", se.DMGosaM);
        Assert.Equal("03", se.DMJyuni);
        Assert.Equal("2", se.KyakusituKubun);
    }

    [Fact]
    public void Decode_returns_empty_strings_for_unset_fields()
    {
        var bytes = new SeFixtureBuilder().Build();

        var se = SeDecoder.Decode(bytes);

        Assert.Equal("SE", se.RecordSpec);
        Assert.Equal(string.Empty, se.DataKubun);
        Assert.Equal(string.Empty, se.Bamei);
        Assert.Equal(string.Empty, se.Time);
        Assert.Equal(3, se.ChakuUma.Length);
        Assert.All(se.ChakuUma, c =>
        {
            Assert.Equal(string.Empty, c.KettoNum);
            Assert.Equal(string.Empty, c.Bamei);
        });
    }
}
