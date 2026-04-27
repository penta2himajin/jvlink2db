using System;
using Jvlink2Db.Parser.Records;
using Jvlink2Db.Parser.Tests.Fixtures;
using Xunit;

namespace Jvlink2Db.Parser.Tests.Records;

public class O1DecoderTests
{
    [Fact]
    public void Decode_throws_when_buffer_is_too_short()
    {
        var ex = Assert.Throws<ArgumentException>(() => O1Decoder.Decode(new byte[961]));
        Assert.Contains("962", ex.Message);
    }

    [Fact]
    public void Decode_throws_when_record_spec_is_not_O1()
    {
        var bytes = new O1FixtureBuilder().RecordSpec("RA").Build();
        Assert.Throws<InvalidOperationException>(() => O1Decoder.Decode(bytes));
    }

    [Fact]
    public void Decode_extracts_pk_and_header()
    {
        var bytes = new O1FixtureBuilder()
            .DataKubun("3")
            .MakeDate("20260331")
            .RaceId("2026", "0331", "06", "01", "08", "11")
            .HappyoTime("03311534")
            .TorokuTosu("16")
            .SyussoTosu("16")
            .Build();

        var o1 = O1Decoder.Decode(bytes);

        Assert.Equal("O1", o1.RecordSpec);
        Assert.Equal("03311534", o1.HappyoTime);
        Assert.Equal("16", o1.TorokuTosu);
    }

    [Fact]
    public void Decode_extracts_three_odds_arrays()
    {
        var bytes = new O1FixtureBuilder()
            .TansyoEntry(0, "01", "0345", "02")
            .TansyoEntry(27, "16", "9999", "16")
            .FukusyoEntry(0, "01", "0150", "0250", "01")
            .FukusyoEntry(27, "16", "0500", "0700", "10")
            .WakurenEntry(0, "12", "01234", "01")
            .WakurenEntry(35, "78", "99999", "36")
            .TotalHyosuTansyo("01000000000")
            .TotalHyosuFukusyo("00800000000")
            .TotalHyosuWakuren("00500000000")
            .Build();

        var o1 = O1Decoder.Decode(bytes);

        Assert.Equal(28, o1.TansyoUmaban.Length);
        Assert.Equal("01", o1.TansyoUmaban[0]);
        Assert.Equal("0345", o1.TansyoOdds[0]);
        Assert.Equal("02", o1.TansyoNinki[0]);
        Assert.Equal("16", o1.TansyoUmaban[27]);

        Assert.Equal(28, o1.FukusyoUmaban.Length);
        Assert.Equal("0150", o1.FukusyoOddsLow[0]);
        Assert.Equal("0250", o1.FukusyoOddsHigh[0]);

        Assert.Equal(36, o1.WakurenKumi.Length);
        Assert.Equal("12", o1.WakurenKumi[0]);
        Assert.Equal("01234", o1.WakurenOdds[0]);
        Assert.Equal("78", o1.WakurenKumi[35]);

        Assert.Equal("01000000000", o1.TotalHyosuTansyo);
        Assert.Equal("00800000000", o1.TotalHyosuFukusyo);
        Assert.Equal("00500000000", o1.TotalHyosuWakuren);
    }

    [Fact]
    public void Decode_returns_empty_strings_for_unset_fields()
    {
        var o1 = O1Decoder.Decode(new O1FixtureBuilder().Build());

        Assert.Equal(28, o1.TansyoUmaban.Length);
        Assert.All(o1.TansyoUmaban, s => Assert.Equal(string.Empty, s));
        Assert.Equal(36, o1.WakurenKumi.Length);
        Assert.Equal(string.Empty, o1.TotalHyosuTansyo);
    }
}
