using System;
using Jvlink2Db.Parser.Encoding;

namespace Jvlink2Db.Parser.Tests.Fixtures;

internal sealed class DmFixtureBuilder
{
    public const int RecordLength = 303;

    private readonly byte[] _buffer;

    public DmFixtureBuilder()
    {
        _buffer = new byte[RecordLength];
        Array.Fill(_buffer, (byte)0x20);
        With(1, 2, "DM");
        _buffer[RecordLength - 2] = 0x0D;
        _buffer[RecordLength - 1] = 0x0A;
    }

    public DmFixtureBuilder With(int oneBasedOffset, int length, string value)
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

    public DmFixtureBuilder RecordSpec(string value) => With(1, 2, value);
    public DmFixtureBuilder RaceId(string year, string monthDay, string jyoCd, string kaiji, string nichiji, string raceNum)
        => With(12, 4, year).With(16, 4, monthDay).With(20, 2, jyoCd).With(22, 2, kaiji).With(24, 2, nichiji).With(26, 2, raceNum);

    public DmFixtureBuilder Entry(int index, string umaban, string time, string gosaP, string gosaM)
    {
        var basePos = 32 + (index * 15);
        return With(basePos, 2, umaban).With(basePos + 2, 5, time).With(basePos + 7, 4, gosaP).With(basePos + 11, 4, gosaM);
    }

    public byte[] Build() => (byte[])_buffer.Clone();
}
