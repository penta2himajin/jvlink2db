using System;
using Jvlink2Db.Parser.Encoding;
using Jvlink2Db.Parser.Records;
using Jvlink2Db.Parser.Tests.Fixtures;
using Xunit;

namespace Jvlink2Db.Parser.Tests.Records;

public class HnDecoderTests
{
    [Fact]
    public void Decode_throws_when_buffer_is_shorter_than_legacy_minimum()
    {
        var ex = Assert.Throws<RecordTooShortException>(() => HnDecoder.Decode(new byte[245]));
        Assert.Equal("HN", ex.RecordSpec);
        Assert.Equal(245, ex.ActualLength);
        Assert.Equal(246, ex.RequiredLength);
    }

    [Fact]
    public void Decode_extracts_pk_and_parents()
    {
        var bytes = new HnFixtureBuilder()
            .HansyokuNum("0099999999")
            .KettoNum("2018104567")
            .Bamei("テスト繁殖馬")
            .HansyokuFNum("0001234567")
            .HansyokuMNum("0007654321")
            .Build();

        var hn = HnDecoder.Decode(bytes);

        Assert.Equal("HN", hn.RecordSpec);
        Assert.Equal("0099999999", hn.HansyokuNum);
        Assert.Equal("2018104567", hn.KettoNum);
        Assert.Equal("テスト繁殖馬", hn.Bamei);
        Assert.Equal("0001234567", hn.HansyokuFNum);
        Assert.Equal("0007654321", hn.HansyokuMNum);
    }

    [Fact]
    public void Decode_handles_legacy_246_byte_layout_with_15_byte_SanchiName()
    {
        // Build a 246-byte buffer mirroring the layout JV-Link returns for
        // pre-spec-bump HN records: SanchiName 15 bytes, then HansyokuFNum
        // at 225, HansyokuMNum at 235, CRLF at 245.
        var buffer = new byte[246];
        Array.Fill(buffer, (byte)0x20);
        buffer[0] = (byte)'H';
        buffer[1] = (byte)'N';
        buffer[244] = 0x0D;
        buffer[245] = 0x0A;

        Put(buffer, 12, 10, "0099999999");
        Put(buffer, 30, 10, "2010104321");
        Put(buffer, 41, 36, "古いテスト繁殖馬");
        Put(buffer, 210, 15, "北海道");                  // 15-byte SanchiName
        Put(buffer, 225, 10, "0001234567");              // HansyokuFNum (legacy offset)
        Put(buffer, 235, 10, "0007654321");              // HansyokuMNum (legacy offset)

        var hn = HnDecoder.Decode(buffer);

        Assert.Equal("HN", hn.RecordSpec);
        Assert.Equal("0099999999", hn.HansyokuNum);
        Assert.Equal("2010104321", hn.KettoNum);
        Assert.Equal("古いテスト繁殖馬", hn.Bamei);
        Assert.Equal("北海道", hn.SanchiName);
        Assert.Equal("0001234567", hn.HansyokuFNum);
        Assert.Equal("0007654321", hn.HansyokuMNum);
    }

    private static void Put(byte[] buffer, int oneBasedOffset, int length, string value)
    {
        var bytes = Sjis.Encoding.GetBytes(value);
        if (bytes.Length > length)
        {
            throw new ArgumentException($"'{value}' is {bytes.Length} bytes, field is {length}.");
        }
        bytes.CopyTo(buffer.AsSpan(oneBasedOffset - 1, bytes.Length));
    }
}
