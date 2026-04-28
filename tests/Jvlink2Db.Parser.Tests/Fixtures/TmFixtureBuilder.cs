using System;
using Jvlink2Db.Parser.Encoding;

namespace Jvlink2Db.Parser.Tests.Fixtures;

internal sealed class TmFixtureBuilder
{
    public const int RecordLength = 141;

    private readonly byte[] _buffer;

    public TmFixtureBuilder()
    {
        _buffer = new byte[RecordLength];
        Array.Fill(_buffer, (byte)0x20);
        With(1, 2, "TM");
        _buffer[RecordLength - 2] = 0x0D;
        _buffer[RecordLength - 1] = 0x0A;
    }

    public TmFixtureBuilder With(int oneBasedOffset, int length, string value)
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

    public TmFixtureBuilder RecordSpec(string value) => With(1, 2, value);
    public TmFixtureBuilder RaceId(string year, string monthDay, string jyoCd, string kaiji, string nichiji, string raceNum)
        => With(12, 4, year).With(16, 4, monthDay).With(20, 2, jyoCd).With(22, 2, kaiji).With(24, 2, nichiji).With(26, 2, raceNum);

    public TmFixtureBuilder Entry(int index, string umaban, string score)
    {
        var basePos = 32 + (index * 6);
        return With(basePos, 2, umaban).With(basePos + 2, 4, score);
    }

    public byte[] Build() => (byte[])_buffer.Clone();
}
