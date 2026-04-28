using System;
using Jvlink2Db.Parser.Encoding;

namespace Jvlink2Db.Parser.Tests.Fixtures;

internal sealed class TkFixtureBuilder
{
    public const int RecordLength = 21657;

    private readonly byte[] _buffer;

    public TkFixtureBuilder()
    {
        _buffer = new byte[RecordLength];
        Array.Fill(_buffer, (byte)0x20);
        With(1, 2, "TK");
        _buffer[RecordLength - 2] = 0x0D;
        _buffer[RecordLength - 1] = 0x0A;
    }

    public TkFixtureBuilder With(int oneBasedOffset, int length, string value)
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

    public TkFixtureBuilder RecordSpec(string value) => With(1, 2, value);
    public TkFixtureBuilder Year(string value) => With(12, 4, value);
    public TkFixtureBuilder MonthDay(string value) => With(16, 4, value);
    public TkFixtureBuilder RaceNum(string value) => With(26, 2, value);
    public TkFixtureBuilder Hondai(string value) => With(33, 60, value);
    public TkFixtureBuilder GradeCD(string value) => With(615, 1, value);
    public TkFixtureBuilder Kyori(string value) => With(637, 4, value);
    public TkFixtureBuilder TorokuTosu(string value) => With(653, 3, value);

    public TkFixtureBuilder TokuKettoNum(int index, string value) =>
        With(656 + (index * 70) + 3, 10, value);

    public TkFixtureBuilder TokuBamei(int index, string value) =>
        With(656 + (index * 70) + 13, 36, value);

    public byte[] Build() => (byte[])_buffer.Clone();
}
