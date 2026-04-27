using System;
using Jvlink2Db.Parser.Encoding;

namespace Jvlink2Db.Parser.Tests.Fixtures;

internal sealed class H1FixtureBuilder
{
    public const int RecordLength = 28955;

    private readonly byte[] _buffer;

    public H1FixtureBuilder()
    {
        _buffer = new byte[RecordLength];
        Array.Fill(_buffer, (byte)0x20);
        With(1, 2, "H1");
        _buffer[RecordLength - 2] = 0x0D;
        _buffer[RecordLength - 1] = 0x0A;
    }

    public H1FixtureBuilder With(int oneBasedOffset, int length, string value)
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

    public H1FixtureBuilder RecordSpec(string value) => With(1, 2, value);
    public H1FixtureBuilder RaceId(string year, string monthDay, string jyoCd, string kaiji, string nichiji, string raceNum)
        => With(12, 4, year).With(16, 4, monthDay).With(20, 2, jyoCd).With(22, 2, kaiji).With(24, 2, nichiji).With(26, 2, raceNum);
    public H1FixtureBuilder TorokuTosu(string value) => With(28, 2, value);
    public H1FixtureBuilder SyussoTosu(string value) => With(30, 2, value);

    public H1FixtureBuilder TansyoEntry(int index, string umaban, string hyo, string ninki)
    {
        var basePos = 84 + (index * 15);
        return With(basePos, 2, umaban).With(basePos + 2, 11, hyo).With(basePos + 13, 2, ninki);
    }

    public H1FixtureBuilder UmarenEntry(int index, string kumi, string hyo, string ninki)
    {
        var basePos = 1464 + (index * 18);
        return With(basePos, 4, kumi).With(basePos + 4, 11, hyo).With(basePos + 15, 3, ninki);
    }

    public H1FixtureBuilder SanrenpukuEntry(int index, string kumi, string hyo, string ninki)
    {
        var basePos = 12480 + (index * 20);
        return With(basePos, 6, kumi).With(basePos + 6, 11, hyo).With(basePos + 17, 3, ninki);
    }

    public H1FixtureBuilder HyoTotal(int index, string value) => With(28800 + (index * 11), 11, value);

    public byte[] Build() => (byte[])_buffer.Clone();
}
