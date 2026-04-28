using System;
using Jvlink2Db.Parser.Encoding;

namespace Jvlink2Db.Parser.Tests.Fixtures;

internal sealed class YsFixtureBuilder
{
    public const int RecordLength = 382;

    private readonly byte[] _buffer;

    public YsFixtureBuilder()
    {
        _buffer = new byte[RecordLength];
        Array.Fill(_buffer, (byte)0x20);
        With(1, 2, "YS");
        _buffer[RecordLength - 2] = 0x0D;
        _buffer[RecordLength - 1] = 0x0A;
    }

    public YsFixtureBuilder With(int oneBasedOffset, int length, string value)
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

    public YsFixtureBuilder RecordSpec(string value) => With(1, 2, value);
    public YsFixtureBuilder Year(string value) => With(12, 4, value);
    public YsFixtureBuilder MonthDay(string value) => With(16, 4, value);
    public YsFixtureBuilder JyoCD(string value) => With(20, 2, value);
    public YsFixtureBuilder Kaiji(string value) => With(22, 2, value);
    public YsFixtureBuilder Nichiji(string value) => With(24, 2, value);

    public YsFixtureBuilder JyusyoHondai(int index, string value) =>
        With(27 + (index * 118) + 4, 60, value);

    public YsFixtureBuilder JyusyoTokuNum(int index, string value) =>
        With(27 + (index * 118), 4, value);

    public byte[] Build() => (byte[])_buffer.Clone();
}
