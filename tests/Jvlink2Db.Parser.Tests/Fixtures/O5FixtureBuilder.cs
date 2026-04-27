using System;
using Jvlink2Db.Parser.Encoding;

namespace Jvlink2Db.Parser.Tests.Fixtures;

internal sealed class O5FixtureBuilder
{
    public const int RecordLength = 12293;

    private readonly byte[] _buffer;

    public O5FixtureBuilder()
    {
        _buffer = new byte[RecordLength];
        Array.Fill(_buffer, (byte)0x20);
        With(1, 2, "O5");
        _buffer[RecordLength - 2] = 0x0D;
        _buffer[RecordLength - 1] = 0x0A;
    }

    public O5FixtureBuilder With(int oneBasedOffset, int length, string value)
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

    public O5FixtureBuilder RecordSpec(string value) => With(1, 2, value);
    public O5FixtureBuilder HappyoTime(string mmddhhmm) => With(28, 8, mmddhhmm);

    public O5FixtureBuilder Entry(int index, string kumi, string odds, string ninki)
    {
        var basePos = 41 + (index * 15);
        return With(basePos, 6, kumi).With(basePos + 6, 6, odds).With(basePos + 12, 3, ninki);
    }

    public O5FixtureBuilder TotalHyosuSanrenpuku(string value) => With(12281, 11, value);

    public byte[] Build() => (byte[])_buffer.Clone();
}
