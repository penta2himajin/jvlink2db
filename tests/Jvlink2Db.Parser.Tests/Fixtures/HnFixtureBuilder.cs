using System;
using Jvlink2Db.Parser.Encoding;

namespace Jvlink2Db.Parser.Tests.Fixtures;

internal sealed class HnFixtureBuilder
{
    public const int RecordLength = 251;

    private readonly byte[] _buffer;

    public HnFixtureBuilder()
    {
        _buffer = new byte[RecordLength];
        Array.Fill(_buffer, (byte)0x20);
        With(1, 2, "HN");
        _buffer[RecordLength - 2] = 0x0D;
        _buffer[RecordLength - 1] = 0x0A;
    }

    public HnFixtureBuilder With(int oneBasedOffset, int length, string value)
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

    public HnFixtureBuilder RecordSpec(string value) => With(1, 2, value);
    public HnFixtureBuilder HansyokuNum(string value) => With(12, 10, value);
    public HnFixtureBuilder KettoNum(string value) => With(30, 10, value);
    public HnFixtureBuilder Bamei(string value) => With(41, 36, value);
    public HnFixtureBuilder HansyokuFNum(string value) => With(230, 10, value);
    public HnFixtureBuilder HansyokuMNum(string value) => With(240, 10, value);

    public byte[] Build() => (byte[])_buffer.Clone();
}
