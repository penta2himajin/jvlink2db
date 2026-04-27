using System;
using Jvlink2Db.Parser.Encoding;

namespace Jvlink2Db.Parser.Tests.Fixtures;

internal sealed class ChFixtureBuilder
{
    public const int RecordLength = 3862;

    private readonly byte[] _buffer;

    public ChFixtureBuilder()
    {
        _buffer = new byte[RecordLength];
        Array.Fill(_buffer, (byte)0x20);
        With(1, 2, "CH");
        _buffer[RecordLength - 2] = 0x0D;
        _buffer[RecordLength - 1] = 0x0A;
    }

    public ChFixtureBuilder With(int oneBasedOffset, int length, string value)
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

    public ChFixtureBuilder RecordSpec(string value) => With(1, 2, value);
    public ChFixtureBuilder DataKubun(string value) => With(3, 1, value);
    public ChFixtureBuilder MakeDate(string yyyymmdd) => With(4, 8, yyyymmdd);
    public ChFixtureBuilder ChokyosiCode(string value) => With(12, 5, value);
    public ChFixtureBuilder ChokyosiName(string value) => With(42, 34, value);
    public ChFixtureBuilder BirthDate(string yyyymmdd) => With(34, 8, yyyymmdd);

    public ChFixtureBuilder SaikinJyusyo(int index, string year, string monthDay, string hondai)
    {
        var basePos = 216 + (index * 163);
        return With(basePos, 4, year).With(basePos + 4, 4, monthDay).With(basePos + 16, 60, hondai);
    }

    public byte[] Build() => (byte[])_buffer.Clone();
}
