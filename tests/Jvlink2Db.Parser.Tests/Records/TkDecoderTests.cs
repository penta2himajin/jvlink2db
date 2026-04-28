using System;
using Jvlink2Db.Parser.Records;
using Jvlink2Db.Parser.Tests.Fixtures;
using Xunit;

namespace Jvlink2Db.Parser.Tests.Records;

public class TkDecoderTests
{
    [Fact]
    public void Decode_throws_when_buffer_is_too_short()
    {
        var ex = Assert.Throws<ArgumentException>(() => TkDecoder.Decode(new byte[21656]));
        Assert.Contains("21657", ex.Message);
    }

    [Fact]
    public void Decode_extracts_race_id_grade_and_first_and_last_entry()
    {
        var bytes = new TkFixtureBuilder()
            .Year("2026")
            .MonthDay("0503")
            .RaceNum("11")
            .Hondai("天皇賞(春)")
            .GradeCD("A")
            .Kyori("3200")
            .TorokuTosu("018")
            .TokuKettoNum(0, "2020100123")
            .TokuBamei(0, "テスト馬1")
            .TokuKettoNum(299, "2020999999")
            .TokuBamei(299, "テスト馬末")
            .Build();

        var tk = TkDecoder.Decode(bytes);

        Assert.Equal("TK", tk.RecordSpec);
        Assert.Equal("2026", tk.Year);
        Assert.Equal("11", tk.RaceNum);
        Assert.Equal("天皇賞(春)", tk.Hondai);
        Assert.Equal("A", tk.GradeCD);
        Assert.Equal("3200", tk.Kyori);
        Assert.Equal("018", tk.TorokuTosu);
        Assert.Equal(300, tk.KettoNum.Length);
        Assert.Equal("2020100123", tk.KettoNum[0]);
        Assert.Equal("テスト馬1", tk.Bamei[0]);
        Assert.Equal("2020999999", tk.KettoNum[299]);
        Assert.Equal("テスト馬末", tk.Bamei[299]);
        Assert.Equal(5, tk.JyokenCD.Length);
    }
}
