using System;
using Jvlink2Db.Parser.Records;
using Jvlink2Db.Parser.Tests.Fixtures;
using Xunit;

namespace Jvlink2Db.Parser.Tests.Records;

public class KsDecoderTests
{
    [Fact]
    public void Decode_throws_when_buffer_is_too_short()
    {
        var ex = Assert.Throws<ArgumentException>(() => KsDecoder.Decode(new byte[4172]));
        Assert.Contains("4173", ex.Message);
    }

    [Fact]
    public void Decode_throws_when_record_spec_is_not_KS()
    {
        var bytes = new KsFixtureBuilder().RecordSpec("RA").Build();
        Assert.Throws<InvalidOperationException>(() => KsDecoder.Decode(bytes));
    }

    [Fact]
    public void Decode_extracts_top_level_fields()
    {
        var bytes = new KsFixtureBuilder()
            .DataKubun("1")
            .MakeDate("20260331")
            .KisyuCode("00123")
            .KisyuName("テスト騎手")
            .BirthDate("20000401")
            .Build();

        var ks = KsDecoder.Decode(bytes);

        Assert.Equal("KS", ks.RecordSpec);
        Assert.Equal("00123", ks.KisyuCode);
        Assert.Equal("テスト騎手", ks.KisyuName);
        Assert.Equal("20000401", ks.BirthDate);
    }

    [Fact]
    public void Decode_extracts_hatukijyo_and_saikin_jyusyo_arrays()
    {
        var bytes = new KsFixtureBuilder()
            .HatuKiJyo(0, "2020", "0405", "06", "07", "2018104567", "初騎乗馬名")
            .SaikinJyusyo(0, "2025", "1228", "有馬記念")
            .SaikinJyusyo(2, "2025", "0530", "日本ダービー")
            .Build();

        var ks = KsDecoder.Decode(bytes);

        Assert.Equal(2, ks.HatuKiJyoYear.Length);
        Assert.Equal("2020", ks.HatuKiJyoYear[0]);
        Assert.Equal("06", ks.HatuKiJyoJyoCD[0]);
        Assert.Equal("2018104567", ks.HatuKiJyoKettoNum[0]);
        Assert.Equal("初騎乗馬名", ks.HatuKiJyoBamei[0]);

        Assert.Equal(3, ks.SaikinJyusyoYear.Length);
        Assert.Equal("有馬記念", ks.SaikinJyusyoHondai[0]);
        Assert.Equal("日本ダービー", ks.SaikinJyusyoHondai[2]);
    }
}
