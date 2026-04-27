using System;
using Jvlink2Db.Parser.Encoding;

namespace Jvlink2Db.Parser.Tests.Fixtures;

internal sealed class WcFixtureBuilder
{
    public const int RecordLength = 105;

    private readonly byte[] _buffer;

    public WcFixtureBuilder()
    {
        _buffer = new byte[RecordLength];
        Array.Fill(_buffer, (byte)0x20);
        With(1, 2, "WC");
        _buffer[RecordLength - 2] = 0x0D;
        _buffer[RecordLength - 1] = 0x0A;
    }

    public WcFixtureBuilder With(int oneBasedOffset, int length, string value)
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

    public WcFixtureBuilder RecordSpec(string value) => With(1, 2, value);
    public WcFixtureBuilder TresenKubun(string value) => With(12, 1, value);
    public WcFixtureBuilder ChokyoDate(string yyyymmdd) => With(13, 8, yyyymmdd);
    public WcFixtureBuilder ChokyoTime(string value) => With(21, 4, value);
    public WcFixtureBuilder KettoNum(string value) => With(25, 10, value);
    public WcFixtureBuilder HaronTime10(string value) => With(38, 4, value);
    public WcFixtureBuilder LapTime1(string value) => With(101, 3, value);

    public byte[] Build() => (byte[])_buffer.Clone();
}
