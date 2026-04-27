using System;
using Jvlink2Db.Parser.Records;
using Jvlink2Db.Parser.Tests.Fixtures;
using Xunit;

namespace Jvlink2Db.Parser.Tests.Records;

public class UmDecoderTests
{
    [Fact]
    public void Decode_throws_when_buffer_is_too_short()
    {
        var ex = Assert.Throws<ArgumentException>(() => UmDecoder.Decode(new byte[1608]));
        Assert.Contains("1609", ex.Message);
    }

    [Fact]
    public void Decode_throws_when_record_spec_is_not_UM()
    {
        var bytes = new UmFixtureBuilder().RecordSpec("RA").Build();
        Assert.Throws<InvalidOperationException>(() => UmDecoder.Decode(bytes));
    }

    [Fact]
    public void Decode_extracts_pk_and_horse_identity()
    {
        var bytes = new UmFixtureBuilder()
            .DataKubun("1")
            .MakeDate("20260331")
            .KettoNum("2020100123")
            .BirthDate("20200315")
            .Bamei("テストホース")
            .SexCD("1")
            .ChokyosiCode("01234")
            .BreederCode("00056789")
            .BanusiCode("123456")
            .RaceCount("025")
            .Build();

        var um = UmDecoder.Decode(bytes);

        Assert.Equal("UM", um.RecordSpec);
        Assert.Equal("2020100123", um.KettoNum);
        Assert.Equal("20200315", um.BirthDate);
        Assert.Equal("テストホース", um.Bamei);
        Assert.Equal("1", um.SexCD);
        Assert.Equal("01234", um.ChokyosiCode);
        Assert.Equal("025", um.RaceCount);
    }

    [Fact]
    public void Decode_extracts_three_generation_pedigree()
    {
        var bytes = new UmFixtureBuilder()
            .Ketto3(0, "0001234567", "父馬")
            .Ketto3(1, "0007654321", "母馬")
            .Ketto3(13, "0099999999", "母父父馬")
            .Build();

        var um = UmDecoder.Decode(bytes);

        Assert.Equal(14, um.KettoHansyokuNum.Length);
        Assert.Equal(14, um.KettoBamei.Length);
        Assert.Equal("0001234567", um.KettoHansyokuNum[0]);
        Assert.Equal("父馬", um.KettoBamei[0]);
        Assert.Equal("0007654321", um.KettoHansyokuNum[1]);
        Assert.Equal("母父父馬", um.KettoBamei[13]);
    }

    [Fact]
    public void Decode_extracts_chaku_kaisu_arrays_with_correct_sizes()
    {
        var bytes = new UmFixtureBuilder()
            .ChakuSogo(0, "012")
            .ChakuSogo(5, "099")
            .ChakuKaisuBa(0, 0, "001")
            .ChakuKaisuBa(6, 5, "020")
            .Build();

        var um = UmDecoder.Decode(bytes);

        Assert.Equal(6, um.ChakuSogo.Length);
        Assert.Equal("012", um.ChakuSogo[0]);
        Assert.Equal("099", um.ChakuSogo[5]);

        Assert.Equal(42, um.ChakuKaisuBa.Length);
        Assert.Equal("001", um.ChakuKaisuBa[0]);            // ba 0, placement 0
        Assert.Equal("020", um.ChakuKaisuBa[(6 * 6) + 5]);  // ba 6, placement 5

        Assert.Equal(72, um.ChakuKaisuJyotai.Length);
        Assert.Equal(36, um.ChakuKaisuKyori.Length);
        Assert.Equal(4, um.Kyakusitu.Length);
    }
}
