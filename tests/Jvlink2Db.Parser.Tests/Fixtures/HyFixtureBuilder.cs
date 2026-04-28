using System;
using Jvlink2Db.Parser.Encoding;

namespace Jvlink2Db.Parser.Tests.Fixtures;

internal sealed class HyFixtureBuilder
{
    public const int RecordLength = 123;

    private readonly byte[] _buffer;

    public HyFixtureBuilder()
    {
        _buffer = new byte[RecordLength];
        Array.Fill(_buffer, (byte)0x20);
        With(1, 2, "HY");
        _buffer[RecordLength - 2] = 0x0D;
        _buffer[RecordLength - 1] = 0x0A;
    }

    public HyFixtureBuilder With(int oneBasedOffset, int length, string value)
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

    public HyFixtureBuilder RecordSpec(string value) => With(1, 2, value);
    public HyFixtureBuilder KettoNum(string value) => With(12, 10, value);
    public HyFixtureBuilder Bamei(string value) => With(22, 36, value);
    public HyFixtureBuilder Origin(string value) => With(58, 64, value);

    public byte[] Build() => (byte[])_buffer.Clone();
}
