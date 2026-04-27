using System;
using Jvlink2Db.Parser.Encoding;

namespace Jvlink2Db.Parser.Tests.Fixtures;

internal sealed class O6FixtureBuilder
{
    public const int RecordLength = 83285;

    private readonly byte[] _buffer;

    public O6FixtureBuilder()
    {
        _buffer = new byte[RecordLength];
        Array.Fill(_buffer, (byte)0x20);
        With(1, 2, "O6");
        _buffer[RecordLength - 2] = 0x0D;
        _buffer[RecordLength - 1] = 0x0A;
    }

    public O6FixtureBuilder With(int oneBasedOffset, int length, string value)
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

    public O6FixtureBuilder RecordSpec(string value) => With(1, 2, value);
    public O6FixtureBuilder HappyoTime(string mmddhhmm) => With(28, 8, mmddhhmm);

    public O6FixtureBuilder Entry(int index, string kumi, string odds, string ninki)
    {
        var basePos = 41 + (index * 17);
        return With(basePos, 6, kumi).With(basePos + 6, 7, odds).With(basePos + 13, 4, ninki);
    }

    public O6FixtureBuilder TotalHyosuSanrentan(string value) => With(83273, 11, value);

    public byte[] Build() => (byte[])_buffer.Clone();
}
