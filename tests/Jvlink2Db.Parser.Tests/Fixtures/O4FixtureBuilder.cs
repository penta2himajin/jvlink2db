using System;
using Jvlink2Db.Parser.Encoding;

namespace Jvlink2Db.Parser.Tests.Fixtures;

internal sealed class O4FixtureBuilder
{
    public const int RecordLength = 4031;

    private readonly byte[] _buffer;

    public O4FixtureBuilder()
    {
        _buffer = new byte[RecordLength];
        Array.Fill(_buffer, (byte)0x20);
        With(1, 2, "O4");
        _buffer[RecordLength - 2] = 0x0D;
        _buffer[RecordLength - 1] = 0x0A;
    }

    public O4FixtureBuilder With(int oneBasedOffset, int length, string value)
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

    public O4FixtureBuilder RecordSpec(string value) => With(1, 2, value);
    public O4FixtureBuilder HappyoTime(string mmddhhmm) => With(28, 8, mmddhhmm);

    public O4FixtureBuilder Entry(int index, string kumi, string odds, string ninki)
    {
        var basePos = 41 + (index * 13);
        return With(basePos, 4, kumi).With(basePos + 4, 6, odds).With(basePos + 10, 3, ninki);
    }

    public O4FixtureBuilder TotalHyosuUmatan(string value) => With(4019, 11, value);

    public byte[] Build() => (byte[])_buffer.Clone();
}
