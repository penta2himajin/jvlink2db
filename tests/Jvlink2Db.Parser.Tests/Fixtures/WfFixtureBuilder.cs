using System;
using Jvlink2Db.Parser.Encoding;

namespace Jvlink2Db.Parser.Tests.Fixtures;

internal sealed class WfFixtureBuilder
{
    public const int RecordLength = 7215;

    private readonly byte[] _buffer;

    public WfFixtureBuilder()
    {
        _buffer = new byte[RecordLength];
        Array.Fill(_buffer, (byte)0x20);
        With(1, 2, "WF");
        _buffer[RecordLength - 2] = 0x0D;
        _buffer[RecordLength - 1] = 0x0A;
    }

    public WfFixtureBuilder With(int oneBasedOffset, int length, string value)
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

    public WfFixtureBuilder RecordSpec(string value) => With(1, 2, value);
    public WfFixtureBuilder DataKubun(string value) => With(3, 1, value);
    public WfFixtureBuilder MakeDate(string yyyymmdd) => With(4, 8, yyyymmdd);
    public WfFixtureBuilder KaisaiDate(string yyyymmdd) => With(12, 8, yyyymmdd);

    public WfFixtureBuilder RaceInfo(int index, string jyoCd, string kaiji, string nichiji, string raceNum)
    {
        var basePos = 22 + (index * 8);
        return With(basePos, 2, jyoCd).With(basePos + 2, 2, kaiji).With(basePos + 4, 2, nichiji).With(basePos + 6, 2, raceNum);
    }

    public WfFixtureBuilder HatsubaiHyo(string value) => With(68, 11, value);
    public WfFixtureBuilder YukoHyo(int index, string value) => With(79 + (index * 11), 11, value);

    public WfFixtureBuilder HenkanFlag(string value) => With(134, 1, value);
    public WfFixtureBuilder FuseiritsuFlag(string value) => With(135, 1, value);
    public WfFixtureBuilder TekichunashiFlag(string value) => With(136, 1, value);
    public WfFixtureBuilder COShoki(string value) => With(137, 15, value);
    public WfFixtureBuilder COZanDaka(string value) => With(152, 15, value);

    public WfFixtureBuilder PayInfo(int index, string kumiban, string pay, string tekichuHyo)
    {
        var basePos = 167 + (index * 29);
        return With(basePos, 10, kumiban).With(basePos + 10, 9, pay).With(basePos + 19, 10, tekichuHyo);
    }

    public byte[] Build() => (byte[])_buffer.Clone();
}
