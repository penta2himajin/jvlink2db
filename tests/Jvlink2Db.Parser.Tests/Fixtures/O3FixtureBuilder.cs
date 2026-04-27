using System;
using Jvlink2Db.Parser.Encoding;

namespace Jvlink2Db.Parser.Tests.Fixtures;

internal sealed class O3FixtureBuilder
{
    public const int RecordLength = 2654;

    private readonly byte[] _buffer;

    public O3FixtureBuilder()
    {
        _buffer = new byte[RecordLength];
        Array.Fill(_buffer, (byte)0x20);
        With(1, 2, "O3");
        _buffer[RecordLength - 2] = 0x0D;
        _buffer[RecordLength - 1] = 0x0A;
    }

    public O3FixtureBuilder With(int oneBasedOffset, int length, string value)
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

    public O3FixtureBuilder RecordSpec(string value) => With(1, 2, value);
    public O3FixtureBuilder RaceId(string year, string monthDay, string jyoCd, string kaiji, string nichiji, string raceNum)
        => With(12, 4, year).With(16, 4, monthDay).With(20, 2, jyoCd).With(22, 2, kaiji).With(24, 2, nichiji).With(26, 2, raceNum);
    public O3FixtureBuilder HappyoTime(string mmddhhmm) => With(28, 8, mmddhhmm);

    public O3FixtureBuilder Entry(int index, string kumi, string oddsLow, string oddsHigh, string ninki)
    {
        var basePos = 41 + (index * 17);
        return With(basePos, 4, kumi).With(basePos + 4, 5, oddsLow).With(basePos + 9, 5, oddsHigh).With(basePos + 14, 3, ninki);
    }

    public O3FixtureBuilder TotalHyosuWide(string value) => With(2642, 11, value);

    public byte[] Build() => (byte[])_buffer.Clone();
}
