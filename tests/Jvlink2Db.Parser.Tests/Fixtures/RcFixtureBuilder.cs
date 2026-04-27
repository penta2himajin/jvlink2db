using System;
using Jvlink2Db.Parser.Encoding;

namespace Jvlink2Db.Parser.Tests.Fixtures;

internal sealed class RcFixtureBuilder
{
    public const int RecordLength = 501;

    private readonly byte[] _buffer;

    public RcFixtureBuilder()
    {
        _buffer = new byte[RecordLength];
        Array.Fill(_buffer, (byte)0x20);
        With(1, 2, "RC");
        _buffer[RecordLength - 2] = 0x0D;
        _buffer[RecordLength - 1] = 0x0A;
    }

    public RcFixtureBuilder With(int oneBasedOffset, int length, string value)
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

    public RcFixtureBuilder RecordSpec(string value) => With(1, 2, value);
    public RcFixtureBuilder DataKubun(string value) => With(3, 1, value);
    public RcFixtureBuilder MakeDate(string yyyymmdd) => With(4, 8, yyyymmdd);
    public RcFixtureBuilder RecInfoKubun(string value) => With(12, 1, value);

    public RcFixtureBuilder RaceId(string year, string monthDay, string jyoCd, string kaiji, string nichiji, string raceNum)
        => With(13, 4, year).With(17, 4, monthDay).With(21, 2, jyoCd).With(23, 2, kaiji).With(25, 2, nichiji).With(27, 2, raceNum);

    public RcFixtureBuilder Hondai(string value) => With(33, 60, value);
    public RcFixtureBuilder GradeCD(string value) => With(93, 1, value);
    public RcFixtureBuilder Kyori(string value) => With(96, 4, value);
    public RcFixtureBuilder TrackCD(string value) => With(100, 2, value);
    public RcFixtureBuilder RecTime(string value) => With(103, 4, value);

    public RcFixtureBuilder RecUma(int index, string kettoNum, string bamei, string kisyuName)
    {
        var basePos = 110 + (index * 130);
        return With(basePos, 10, kettoNum).With(basePos + 10, 36, bamei).With(basePos + 96, 34, kisyuName);
    }

    public byte[] Build() => (byte[])_buffer.Clone();
}
