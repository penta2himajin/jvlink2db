using System;
using Jvlink2Db.Parser.Records;
using Jvlink2Db.Parser.Tests.Fixtures;
using Xunit;

namespace Jvlink2Db.Parser.Tests.Records;

public class WfDecoderTests
{
    [Fact]
    public void Decode_throws_when_buffer_is_too_short()
    {
        var ex = Assert.Throws<ArgumentException>(() => WfDecoder.Decode(new byte[7214]));
        Assert.Contains("7215", ex.Message);
    }

    [Fact]
    public void Decode_throws_when_record_spec_is_not_WF()
    {
        var bytes = new WfFixtureBuilder().RecordSpec("RA").Build();
        Assert.Throws<InvalidOperationException>(() => WfDecoder.Decode(bytes));
    }

    [Fact]
    public void Decode_extracts_header_and_kaisai_date()
    {
        var bytes = new WfFixtureBuilder()
            .DataKubun("1")
            .MakeDate("20260331")
            .KaisaiDate("20260328")
            .Build();

        var wf = WfDecoder.Decode(bytes);

        Assert.Equal("WF", wf.RecordSpec);
        Assert.Equal("1", wf.DataKubun);
        Assert.Equal("20260331", wf.MakeDate);
        Assert.Equal("20260328", wf.KaisaiDate);
    }

    [Fact]
    public void Decode_extracts_5_race_entries_and_yuko_hyo()
    {
        var bytes = new WfFixtureBuilder()
            .RaceInfo(0, "06", "01", "08", "09")
            .RaceInfo(1, "06", "01", "08", "10")
            .RaceInfo(2, "06", "01", "08", "11")
            .RaceInfo(3, "06", "01", "08", "12")
            .RaceInfo(4, "08", "01", "08", "11")
            .HatsubaiHyo("01234567890")
            .YukoHyo(0, "01000000000")
            .YukoHyo(4, "00500000000")
            .Build();

        var wf = WfDecoder.Decode(bytes);

        Assert.Equal(5, wf.RaceJyoCD.Length);
        Assert.Equal("06", wf.RaceJyoCD[0]);
        Assert.Equal("08", wf.RaceJyoCD[4]);
        Assert.Equal("09", wf.RaceNum[0]);
        Assert.Equal("11", wf.RaceNum[4]);
        Assert.Equal("01234567890", wf.HatsubaiHyo);
        Assert.Equal(5, wf.YukoHyo.Length);
        Assert.Equal("01000000000", wf.YukoHyo[0]);
        Assert.Equal("00500000000", wf.YukoHyo[4]);
        Assert.Equal(string.Empty, wf.YukoHyo[2]);
    }

    [Fact]
    public void Decode_extracts_pay_info_at_first_middle_and_last_indices()
    {
        var bytes = new WfFixtureBuilder()
            .PayInfo(0, "0102030405", "000999000", "0000000010")
            .PayInfo(121, "0506070809", "000123000", "0000000005")
            .PayInfo(242, "0708090101", "000050000", "0000000020")
            .Build();

        var wf = WfDecoder.Decode(bytes);

        Assert.Equal(243, wf.PayKumiban.Length);
        Assert.Equal("0102030405", wf.PayKumiban[0]);
        Assert.Equal("000999000", wf.PayAmount[0]);
        Assert.Equal("0000000010", wf.PayTekichuHyo[0]);

        Assert.Equal("0506070809", wf.PayKumiban[121]);
        Assert.Equal("000123000", wf.PayAmount[121]);

        Assert.Equal("0708090101", wf.PayKumiban[242]);
        Assert.Equal("000050000", wf.PayAmount[242]);
        Assert.Equal("0000000020", wf.PayTekichuHyo[242]);
    }

    [Fact]
    public void Decode_returns_empty_strings_for_unset_fields()
    {
        var wf = WfDecoder.Decode(new WfFixtureBuilder().Build());

        Assert.Equal(243, wf.PayKumiban.Length);
        Assert.All(wf.PayKumiban, s => Assert.Equal(string.Empty, s));
        Assert.Equal(string.Empty, wf.HatsubaiHyo);
    }
}
