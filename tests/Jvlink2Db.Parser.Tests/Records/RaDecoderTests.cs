using System;
using System.Linq;
using Jvlink2Db.Parser.Records;
using Jvlink2Db.Parser.Tests.Fixtures;
using Xunit;

namespace Jvlink2Db.Parser.Tests.Records;

public class RaDecoderTests
{
    [Fact]
    public void Decode_throws_when_buffer_is_too_short()
    {
        var bytes = new byte[1271];

        var ex = Assert.Throws<ArgumentException>(() => RaDecoder.Decode(bytes));
        Assert.Contains("1272", ex.Message);
    }

    [Fact]
    public void Decode_throws_when_record_spec_is_not_RA()
    {
        var bytes = new RaFixtureBuilder().RecordSpec("SE").Build();

        var ex = Assert.Throws<InvalidOperationException>(() => RaDecoder.Decode(bytes));
        Assert.Contains("RA", ex.Message);
        Assert.Contains("SE", ex.Message);
    }

    [Fact]
    public void Decode_extracts_header_fields()
    {
        var bytes = new RaFixtureBuilder()
            .DataKubun("7")
            .MakeDate("20260331")
            .Build();

        var ra = RaDecoder.Decode(bytes);

        Assert.Equal("RA", ra.RecordSpec);
        Assert.Equal("7", ra.DataKubun);
        Assert.Equal("20260331", ra.MakeDate);
    }

    [Fact]
    public void Decode_extracts_race_id_pk_fields()
    {
        var bytes = new RaFixtureBuilder()
            .RaceId(year: "2026", monthDay: "0331", jyoCd: "06", kaiji: "01", nichiji: "08", raceNum: "11")
            .Build();

        var ra = RaDecoder.Decode(bytes);

        Assert.Equal("2026", ra.Year);
        Assert.Equal("0331", ra.MonthDay);
        Assert.Equal("06", ra.JyoCD);
        Assert.Equal("01", ra.Kaiji);
        Assert.Equal("08", ra.Nichiji);
        Assert.Equal("11", ra.RaceNum);
    }

    [Fact]
    public void Decode_extracts_race_info_fields_including_japanese_text()
    {
        var bytes = new RaFixtureBuilder()
            .YoubiCD("4")
            .TokuNum("0123")
            .Hondai("有馬記念")
            .Fukudai("グランプリ")
            .HondaiEng("ARIMA KINEN")
            .Ryakusyo10("有馬記念")
            .Ryakusyo6("有馬記念")
            .Ryakusyo3("有馬")
            .Kubun("1")
            .Nkai("070")
            .Build();

        var ra = RaDecoder.Decode(bytes);

        Assert.Equal("4", ra.YoubiCD);
        Assert.Equal("0123", ra.TokuNum);
        Assert.Equal("有馬記念", ra.Hondai);
        Assert.Equal("グランプリ", ra.Fukudai);
        Assert.Equal("ARIMA KINEN", ra.HondaiEng);
        Assert.Equal("有馬記念", ra.Ryakusyo10);
        Assert.Equal("有馬記念", ra.Ryakusyo6);
        Assert.Equal("有馬", ra.Ryakusyo3);
        Assert.Equal("1", ra.Kubun);
        Assert.Equal("070", ra.Nkai);
    }

    [Fact]
    public void Decode_right_trims_padding_spaces_but_preserves_internal_spaces()
    {
        var bytes = new RaFixtureBuilder()
            .Hondai("中山 記念")  // internal space must survive
            .Build();

        var ra = RaDecoder.Decode(bytes);

        Assert.Equal("中山 記念", ra.Hondai);
        Assert.False(ra.Hondai.EndsWith(' '), "trailing padding spaces should be trimmed");
    }

    [Fact]
    public void Decode_extracts_grade_track_and_distance_codes()
    {
        var bytes = new RaFixtureBuilder()
            .GradeCD("E")
            .GradeCDBefore("D")
            .Kyori("2500")
            .KyoriBefore("2400")
            .TrackCD("23")
            .TrackCDBefore("22")
            .CourseKubunCD("A1")
            .CourseKubunCDBefore("A2")
            .Build();

        var ra = RaDecoder.Decode(bytes);

        Assert.Equal("E", ra.GradeCD);
        Assert.Equal("D", ra.GradeCDBefore);
        Assert.Equal("2500", ra.Kyori);
        Assert.Equal("2400", ra.KyoriBefore);
        Assert.Equal("23", ra.TrackCD);
        Assert.Equal("22", ra.TrackCDBefore);
        Assert.Equal("A1", ra.CourseKubunCD);
        Assert.Equal("A2", ra.CourseKubunCDBefore);
    }

    [Fact]
    public void Decode_extracts_jyoken_struct_and_name()
    {
        var bytes = new RaFixtureBuilder()
            .SyubetuCD("11")
            .KigoCD("A01")
            .JyuryoCD("3")
            .JyokenCD(0, "005")
            .JyokenCD(1, "010")
            .JyokenCD(2, "015")
            .JyokenCD(3, "020")
            .JyokenCD(4, "999")
            .JyokenName("３歳以上オープン")
            .Build();

        var ra = RaDecoder.Decode(bytes);

        Assert.Equal("11", ra.SyubetuCD);
        Assert.Equal("A01", ra.KigoCD);
        Assert.Equal("3", ra.JyuryoCD);
        Assert.Equal(new[] { "005", "010", "015", "020", "999" }, ra.JyokenCD);
        Assert.Equal("３歳以上オープン", ra.JyokenName);
    }

    [Fact]
    public void Decode_extracts_prize_money_arrays()
    {
        var fb = new RaFixtureBuilder();
        for (var i = 0; i < 7; i++)
        {
            fb.Honsyokin(i, ((i + 1) * 1000).ToString("D8"));
        }

        for (var i = 0; i < 5; i++)
        {
            fb.HonsyokinBefore(i, ((i + 1) * 100).ToString("D8"));
        }

        for (var i = 0; i < 5; i++)
        {
            fb.Fukasyokin(i, ((i + 1) * 10).ToString("D8"));
        }

        for (var i = 0; i < 3; i++)
        {
            fb.FukasyokinBefore(i, ((i + 1) * 1).ToString("D8"));
        }

        var ra = RaDecoder.Decode(fb.Build());

        Assert.Equal(7, ra.Honsyokin.Length);
        Assert.Equal(5, ra.HonsyokinBefore.Length);
        Assert.Equal(5, ra.Fukasyokin.Length);
        Assert.Equal(3, ra.FukasyokinBefore.Length);
        Assert.Equal("00001000", ra.Honsyokin[0]);
        Assert.Equal("00007000", ra.Honsyokin[6]);
        Assert.Equal("00000100", ra.HonsyokinBefore[0]);
        Assert.Equal("00000010", ra.Fukasyokin[0]);
        Assert.Equal("00000003", ra.FukasyokinBefore[2]);
    }

    [Fact]
    public void Decode_extracts_count_and_time_fields()
    {
        var bytes = new RaFixtureBuilder()
            .HassoTime("1530")
            .HassoTimeBefore("1525")
            .TorokuTosu("16")
            .SyussoTosu("16")
            .NyusenTosu("16")
            .TenkoCD("1")
            .SibaBabaCD("2")
            .DirtBabaCD("0")
            .SyogaiMileTime("0975")
            .HaronTimeS3("345")
            .HaronTimeS4("466")
            .HaronTimeL3("349")
            .HaronTimeL4("470")
            .RecordUpKubun("1")
            .Build();

        var ra = RaDecoder.Decode(bytes);

        Assert.Equal("1530", ra.HassoTime);
        Assert.Equal("1525", ra.HassoTimeBefore);
        Assert.Equal("16", ra.TorokuTosu);
        Assert.Equal("16", ra.SyussoTosu);
        Assert.Equal("16", ra.NyusenTosu);
        Assert.Equal("1", ra.TenkoCD);
        Assert.Equal("2", ra.SibaBabaCD);
        Assert.Equal("0", ra.DirtBabaCD);
        Assert.Equal("0975", ra.SyogaiMileTime);
        Assert.Equal("345", ra.HaronTimeS3);
        Assert.Equal("466", ra.HaronTimeS4);
        Assert.Equal("349", ra.HaronTimeL3);
        Assert.Equal("470", ra.HaronTimeL4);
        Assert.Equal("1", ra.RecordUpKubun);
    }

    [Fact]
    public void Decode_extracts_all_25_lap_times()
    {
        var fb = new RaFixtureBuilder();
        for (var i = 0; i < 25; i++)
        {
            fb.LapTime(i, (120 + i).ToString("D3"));
        }

        var ra = RaDecoder.Decode(fb.Build());

        Assert.Equal(25, ra.LapTime.Length);
        Assert.Equal("120", ra.LapTime[0]);
        Assert.Equal("130", ra.LapTime[10]);
        Assert.Equal("144", ra.LapTime[24]);
    }

    [Fact]
    public void Decode_extracts_four_corner_records()
    {
        var fb = new RaFixtureBuilder();
        for (var i = 0; i < 4; i++)
        {
            fb.Corner(i, ((i % 4) + 1).ToString(), "1", $"corner{i}-jyuni");
        }

        var ra = RaDecoder.Decode(fb.Build());

        Assert.Equal(4, ra.Corners.Length);
        Assert.Equal("1", ra.Corners[0].Corner);
        Assert.Equal("1", ra.Corners[0].Syukaisu);
        Assert.Equal("corner0-jyuni", ra.Corners[0].Jyuni);
        Assert.Equal("4", ra.Corners[3].Corner);
        Assert.Equal("corner3-jyuni", ra.Corners[3].Jyuni);
    }

    [Fact]
    public void Decode_returns_empty_strings_for_unset_padding_fields()
    {
        // a default builder leaves every overlay-field as spaces (padding).
        var bytes = new RaFixtureBuilder().Build();

        var ra = RaDecoder.Decode(bytes);

        Assert.Equal("RA", ra.RecordSpec); // builder still sets the spec
        Assert.Equal(string.Empty, ra.DataKubun);
        Assert.Equal(string.Empty, ra.Hondai);
        Assert.Equal(string.Empty, ra.Kyori);
        Assert.Equal(7, ra.Honsyokin.Length);
        Assert.All(ra.Honsyokin, v => Assert.Equal(string.Empty, v));
        Assert.Equal(4, ra.Corners.Length);
        Assert.All(ra.Corners, c =>
        {
            Assert.Equal(string.Empty, c.Corner);
            Assert.Equal(string.Empty, c.Syukaisu);
            Assert.Equal(string.Empty, c.Jyuni);
        });
    }
}
