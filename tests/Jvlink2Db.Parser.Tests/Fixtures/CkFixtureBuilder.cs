using System;
using Jvlink2Db.Parser.Encoding;

namespace Jvlink2Db.Parser.Tests.Fixtures;

internal sealed class CkFixtureBuilder
{
    public const int RecordLength = 6870;

    private readonly byte[] _buffer;

    public CkFixtureBuilder()
    {
        _buffer = new byte[RecordLength];
        Array.Fill(_buffer, (byte)0x20);
        With(1, 2, "CK");
        _buffer[RecordLength - 2] = 0x0D;
        _buffer[RecordLength - 1] = 0x0A;
    }

    public CkFixtureBuilder With(int oneBasedOffset, int length, string value)
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

    public CkFixtureBuilder RecordSpec(string value) => With(1, 2, value);
    public CkFixtureBuilder Year(string value) => With(12, 4, value);
    public CkFixtureBuilder RaceNum(string value) => With(26, 2, value);
    public CkFixtureBuilder KettoNum(string value) => With(28, 10, value);
    public CkFixtureBuilder Bamei(string value) => With(38, 36, value);
    public CkFixtureBuilder RuikeiHonsyoHeiti(string value) => With(74, 9, value);
    public CkFixtureBuilder RaceCount(string value) => With(1382, 3, value);
    public CkFixtureBuilder KisyuCode(string value) => With(1385, 5, value);
    public CkFixtureBuilder ChokyosiCode(string value) => With(3864, 5, value);
    public CkFixtureBuilder BanusiCode(string value) => With(6343, 6, value);
    public CkFixtureBuilder BreederCode(string value) => With(6597, 8, value);

    public byte[] Build() => (byte[])_buffer.Clone();
}
