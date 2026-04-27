using System;
using Jvlink2Db.Parser.Encoding;

namespace Jvlink2Db.Parser.Tests.Fixtures;

internal sealed class BnFixtureBuilder
{
    public const int RecordLength = 477;

    private readonly byte[] _buffer;

    public BnFixtureBuilder()
    {
        _buffer = new byte[RecordLength];
        Array.Fill(_buffer, (byte)0x20);
        With(1, 2, "BN");
        _buffer[RecordLength - 2] = 0x0D;
        _buffer[RecordLength - 1] = 0x0A;
    }

    public BnFixtureBuilder With(int oneBasedOffset, int length, string value)
    {
        var bytes = Sjis.Encoding.GetBytes(value);
        if (bytes.Length > length)
        {
            throw new ArgumentException(
                $"value '{value}' is {bytes.Length} bytes; field at offset {oneBasedOffset} only takes {length}.",
                nameof(value));
        }

        var start = oneBasedOffset - 1;
        bytes.CopyTo(_buffer.AsSpan(start, bytes.Length));
        for (var i = start + bytes.Length; i < start + length; i++)
        {
            _buffer[i] = 0x20;
        }

        return this;
    }

    public BnFixtureBuilder RecordSpec(string value) => With(1, 2, value);
    public BnFixtureBuilder DataKubun(string value) => With(3, 1, value);
    public BnFixtureBuilder MakeDate(string yyyymmdd) => With(4, 8, yyyymmdd);
    public BnFixtureBuilder BanusiCode(string value) => With(12, 6, value);
    public BnFixtureBuilder BanusiName(string value) => With(82, 64, value);
    public BnFixtureBuilder Fukusyoku(string value) => With(296, 60, value);

    public BnFixtureBuilder HonRuikei(int yearIndex, string setYear, string honsyokinTotal, string fukaSyokin, string[] chakuKaisu)
    {
        var basePos = 356 + (yearIndex * 60);
        With(basePos, 4, setYear);
        With(basePos + 4, 10, honsyokinTotal);
        With(basePos + 14, 10, fukaSyokin);
        for (var i = 0; i < chakuKaisu.Length; i++)
        {
            With(basePos + 24 + (i * 6), 6, chakuKaisu[i]);
        }

        return this;
    }

    public byte[] Build() => (byte[])_buffer.Clone();
}
