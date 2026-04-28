using System;
using Jvlink2Db.Parser.Encoding;

namespace Jvlink2Db.Parser.Tests.Fixtures;

internal sealed class CsFixtureBuilder
{
    public const int RecordLength = 6829;

    private readonly byte[] _buffer;

    public CsFixtureBuilder()
    {
        _buffer = new byte[RecordLength];
        Array.Fill(_buffer, (byte)0x20);
        With(1, 2, "CS");
        _buffer[RecordLength - 2] = 0x0D;
        _buffer[RecordLength - 1] = 0x0A;
    }

    public CsFixtureBuilder With(int oneBasedOffset, int length, string value)
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

    public CsFixtureBuilder RecordSpec(string value) => With(1, 2, value);
    public CsFixtureBuilder JyoCD(string value) => With(12, 2, value);
    public CsFixtureBuilder Kyori(string value) => With(14, 4, value);
    public CsFixtureBuilder TrackCD(string value) => With(18, 2, value);
    public CsFixtureBuilder KaishuDate(string value) => With(20, 8, value);

    public byte[] Build() => (byte[])_buffer.Clone();
}
