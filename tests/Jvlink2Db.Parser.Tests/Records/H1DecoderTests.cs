using System;
using Jvlink2Db.Parser.Records;
using Jvlink2Db.Parser.Tests.Fixtures;
using Xunit;

namespace Jvlink2Db.Parser.Tests.Records;

public class H1DecoderTests
{
    [Fact]
    public void Decode_throws_when_buffer_is_too_short()
    {
        var ex = Assert.Throws<ArgumentException>(() => H1Decoder.Decode(new byte[28954]));
        Assert.Contains("28955", ex.Message);
    }

    [Fact]
    public void Decode_throws_when_record_spec_is_not_H1()
    {
        var bytes = new H1FixtureBuilder().RecordSpec("RA").Build();
        Assert.Throws<InvalidOperationException>(() => H1Decoder.Decode(bytes));
    }

    [Fact]
    public void Decode_extracts_pk_and_array_sizes()
    {
        var bytes = new H1FixtureBuilder()
            .RaceId("2026", "0331", "06", "01", "08", "11")
            .TorokuTosu("16")
            .SyussoTosu("16")
            .Build();

        var h1 = H1Decoder.Decode(bytes);

        Assert.Equal("H1", h1.RecordSpec);
        Assert.Equal("11", h1.RaceNum);
        Assert.Equal(28, h1.TansyoUmaban.Length);
        Assert.Equal(36, h1.WakurenKumi.Length);
        Assert.Equal(153, h1.UmarenKumi.Length);
        Assert.Equal(306, h1.UmatanKumi.Length);
        Assert.Equal(816, h1.SanrenpukuKumi.Length);
        Assert.Equal(14, h1.HyoTotal.Length);
    }

    [Fact]
    public void Decode_extracts_pay_arrays_at_boundaries()
    {
        var bytes = new H1FixtureBuilder()
            .TansyoEntry(0, "01", "00000012345", "01")
            .TansyoEntry(27, "16", "00000099999", "16")
            .UmarenEntry(0, "0102", "00000054321", "001")
            .UmarenEntry(152, "1516", "00000000010", "153")
            .SanrenpukuEntry(0, "010203", "00000067890", "001")
            .SanrenpukuEntry(815, "141516", "00000000005", "816")
            .HyoTotal(0, "01000000000")
            .HyoTotal(13, "00000005000")
            .Build();

        var h1 = H1Decoder.Decode(bytes);

        Assert.Equal("01", h1.TansyoUmaban[0]);
        Assert.Equal("00000012345", h1.TansyoHyo[0]);
        Assert.Equal("01", h1.TansyoNinki[0]);
        Assert.Equal("16", h1.TansyoUmaban[27]);

        Assert.Equal("0102", h1.UmarenKumi[0]);
        Assert.Equal("00000054321", h1.UmarenHyo[0]);
        Assert.Equal("1516", h1.UmarenKumi[152]);

        Assert.Equal("010203", h1.SanrenpukuKumi[0]);
        Assert.Equal("141516", h1.SanrenpukuKumi[815]);

        Assert.Equal("01000000000", h1.HyoTotal[0]);
        Assert.Equal("00000005000", h1.HyoTotal[13]);
    }
}
