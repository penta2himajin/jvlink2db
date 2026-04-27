using System;
using Jvlink2Db.Parser.Encoding;

namespace Jvlink2Db.Parser.Tests.Fixtures;

internal sealed class UmFixtureBuilder
{
    public const int RecordLength = 1609;

    private readonly byte[] _buffer;

    public UmFixtureBuilder()
    {
        _buffer = new byte[RecordLength];
        Array.Fill(_buffer, (byte)0x20);
        With(1, 2, "UM");
        _buffer[RecordLength - 2] = 0x0D;
        _buffer[RecordLength - 1] = 0x0A;
    }

    public UmFixtureBuilder With(int oneBasedOffset, int length, string value)
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

    public UmFixtureBuilder RecordSpec(string value) => With(1, 2, value);
    public UmFixtureBuilder DataKubun(string value) => With(3, 1, value);
    public UmFixtureBuilder MakeDate(string yyyymmdd) => With(4, 8, yyyymmdd);
    public UmFixtureBuilder KettoNum(string value) => With(12, 10, value);
    public UmFixtureBuilder BirthDate(string yyyymmdd) => With(39, 8, yyyymmdd);
    public UmFixtureBuilder Bamei(string value) => With(47, 36, value);
    public UmFixtureBuilder SexCD(string value) => With(201, 1, value);

    public UmFixtureBuilder Ketto3(int index, string hansyokuNum, string bamei)
    {
        var basePos = 205 + (index * 46);
        return With(basePos, 10, hansyokuNum).With(basePos + 10, 36, bamei);
    }

    public UmFixtureBuilder ChokyosiCode(string value) => With(850, 5, value);
    public UmFixtureBuilder BreederCode(string value) => With(883, 8, value);
    public UmFixtureBuilder BanusiCode(string value) => With(983, 6, value);
    public UmFixtureBuilder RaceCount(string value) => With(1605, 3, value);

    public UmFixtureBuilder ChakuSogo(int placement, string count) => With(1107 + (placement * 3), 3, count);

    public UmFixtureBuilder ChakuKaisuBa(int ba, int placement, string count)
        => With(1143 + (ba * 18) + (placement * 3), 3, count);

    public byte[] Build() => (byte[])_buffer.Clone();
}
