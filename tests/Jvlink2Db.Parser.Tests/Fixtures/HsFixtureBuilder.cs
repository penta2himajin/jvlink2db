using System;
using Jvlink2Db.Parser.Encoding;

namespace Jvlink2Db.Parser.Tests.Fixtures;

internal sealed class HsFixtureBuilder
{
    public const int RecordLength = 200;

    private readonly byte[] _buffer;

    public HsFixtureBuilder()
    {
        _buffer = new byte[RecordLength];
        Array.Fill(_buffer, (byte)0x20);
        With(1, 2, "HS");
        _buffer[RecordLength - 2] = 0x0D;
        _buffer[RecordLength - 1] = 0x0A;
    }

    public HsFixtureBuilder With(int oneBasedOffset, int length, string value)
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

    public HsFixtureBuilder RecordSpec(string value) => With(1, 2, value);
    public HsFixtureBuilder KettoNum(string value) => With(12, 10, value);
    public HsFixtureBuilder SaleCode(string value) => With(46, 6, value);
    public HsFixtureBuilder Price(string value) => With(189, 10, value);

    public byte[] Build() => (byte[])_buffer.Clone();
}
