using System;
using Jvlink2Db.Parser.Records;
using Jvlink2Db.Parser.Tests.Fixtures;
using Xunit;

namespace Jvlink2Db.Parser.Tests.Records;

public class BtDecoderTests
{
    [Fact]
    public void Decode_throws_when_buffer_is_too_short()
    {
        var ex = Assert.Throws<RecordTooShortException>(() => BtDecoder.Decode(new byte[6888]));
        Assert.Equal("BT", ex.RecordSpec);
        Assert.Equal(6888, ex.ActualLength);
        Assert.Equal(6889, ex.RequiredLength);
    }

    [Fact]
    public void Decode_extracts_hansyoku_num_and_keito_name()
    {
        var bytes = new BtFixtureBuilder()
            .HansyokuNum("0001234567")
            .KeitoName("テスト系統")
            .Build();

        var bt = BtDecoder.Decode(bytes);

        Assert.Equal("BT", bt.RecordSpec);
        Assert.Equal("0001234567", bt.HansyokuNum);
        Assert.Equal("テスト系統", bt.KeitoName);
    }
}
