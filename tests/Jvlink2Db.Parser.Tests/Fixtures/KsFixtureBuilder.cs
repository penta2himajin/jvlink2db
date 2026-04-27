using System;
using Jvlink2Db.Parser.Encoding;

namespace Jvlink2Db.Parser.Tests.Fixtures;

internal sealed class KsFixtureBuilder
{
    public const int RecordLength = 4173;

    private readonly byte[] _buffer;

    public KsFixtureBuilder()
    {
        _buffer = new byte[RecordLength];
        Array.Fill(_buffer, (byte)0x20);
        With(1, 2, "KS");
        _buffer[RecordLength - 2] = 0x0D;
        _buffer[RecordLength - 1] = 0x0A;
    }

    public KsFixtureBuilder With(int oneBasedOffset, int length, string value)
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

    public KsFixtureBuilder RecordSpec(string value) => With(1, 2, value);
    public KsFixtureBuilder DataKubun(string value) => With(3, 1, value);
    public KsFixtureBuilder MakeDate(string yyyymmdd) => With(4, 8, yyyymmdd);
    public KsFixtureBuilder KisyuCode(string value) => With(12, 5, value);
    public KsFixtureBuilder KisyuName(string value) => With(42, 34, value);
    public KsFixtureBuilder BirthDate(string yyyymmdd) => With(34, 8, yyyymmdd);

    public KsFixtureBuilder HatuKiJyo(int index, string year, string monthDay, string jyoCd, string raceNum, string kettoNum, string bamei)
    {
        var basePos = 265 + (index * 67);
        return With(basePos, 4, year).With(basePos + 4, 4, monthDay).With(basePos + 8, 2, jyoCd)
            .With(basePos + 14, 2, raceNum).With(basePos + 18, 10, kettoNum).With(basePos + 28, 36, bamei);
    }

    public KsFixtureBuilder SaikinJyusyo(int index, string year, string monthDay, string hondai)
    {
        var basePos = 527 + (index * 163);
        return With(basePos, 4, year).With(basePos + 4, 4, monthDay).With(basePos + 16, 60, hondai);
    }

    public byte[] Build() => (byte[])_buffer.Clone();
}
