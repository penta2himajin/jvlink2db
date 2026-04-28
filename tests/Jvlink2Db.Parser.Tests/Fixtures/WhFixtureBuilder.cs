using System;
using Jvlink2Db.Parser.Encoding;

namespace Jvlink2Db.Parser.Tests.Fixtures;

internal sealed class WhFixtureBuilder
{
    public const int RecordLength = 847;

    private readonly byte[] _buffer;

    public WhFixtureBuilder()
    {
        _buffer = new byte[RecordLength];
        Array.Fill(_buffer, (byte)0x20);
        With(1, 2, "WH");
        _buffer[RecordLength - 2] = 0x0D;
        _buffer[RecordLength - 1] = 0x0A;
    }

    public WhFixtureBuilder With(int oneBasedOffset, int length, string value)
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

    public WhFixtureBuilder RecordSpec(string value) => With(1, 2, value);
    public WhFixtureBuilder Year(string value) => With(12, 4, value);
    public WhFixtureBuilder MonthDay(string value) => With(16, 4, value);
    public WhFixtureBuilder RaceNum(string value) => With(26, 2, value);

    public WhFixtureBuilder Umaban(int index, string value) =>
        With(36 + (index * 45), 2, value);

    public WhFixtureBuilder BaTaijyu(int index, string value) =>
        With(36 + (index * 45) + 38, 3, value);

    public byte[] Build() => (byte[])_buffer.Clone();
}
