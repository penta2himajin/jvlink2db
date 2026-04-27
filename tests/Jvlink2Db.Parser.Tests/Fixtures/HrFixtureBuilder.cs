using System;
using Jvlink2Db.Parser.Encoding;

namespace Jvlink2Db.Parser.Tests.Fixtures;

internal sealed class HrFixtureBuilder
{
    public const int RecordLength = 719;

    private readonly byte[] _buffer;

    public HrFixtureBuilder()
    {
        _buffer = new byte[RecordLength];
        Array.Fill(_buffer, (byte)0x20);
        With(1, 2, "HR");
        _buffer[RecordLength - 2] = 0x0D;
        _buffer[RecordLength - 1] = 0x0A;
    }

    public HrFixtureBuilder With(int oneBasedOffset, int length, string value)
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

    public HrFixtureBuilder RecordSpec(string value) => With(1, 2, value);
    public HrFixtureBuilder DataKubun(string value) => With(3, 1, value);
    public HrFixtureBuilder MakeDate(string yyyymmdd) => With(4, 8, yyyymmdd);

    public HrFixtureBuilder RaceId(string year, string monthDay, string jyoCd, string kaiji, string nichiji, string raceNum)
        => With(12, 4, year).With(16, 4, monthDay).With(20, 2, jyoCd).With(22, 2, kaiji).With(24, 2, nichiji).With(26, 2, raceNum);

    public HrFixtureBuilder TorokuTosu(string value) => With(28, 2, value);
    public HrFixtureBuilder SyussoTosu(string value) => With(30, 2, value);

    // PayTansyo[i]: base 103, entry size 13. Umaban(1,2), Pay(3,9), Ninki(12,2)
    public HrFixtureBuilder PayTansyo(int index, string umaban, string pay, string ninki)
    {
        var basePos = 103 + (index * 13);
        return With(basePos, 2, umaban).With(basePos + 2, 9, pay).With(basePos + 11, 2, ninki);
    }

    // PayFukusyo[i]: base 142, 13 each
    public HrFixtureBuilder PayFukusyo(int index, string umaban, string pay, string ninki)
    {
        var basePos = 142 + (index * 13);
        return With(basePos, 2, umaban).With(basePos + 2, 9, pay).With(basePos + 11, 2, ninki);
    }

    // PayUmaren[i]: base 246, 16 each. Kumi(1,4), Pay(5,9), Ninki(14,3)
    public HrFixtureBuilder PayUmaren(int index, string kumi, string pay, string ninki)
    {
        var basePos = 246 + (index * 16);
        return With(basePos, 4, kumi).With(basePos + 4, 9, pay).With(basePos + 13, 3, ninki);
    }

    // PaySanrentan[i]: base 604, 19 each. Kumi(1,6), Pay(7,9), Ninki(16,4)
    public HrFixtureBuilder PaySanrentan(int index, string kumi, string pay, string ninki)
    {
        var basePos = 604 + (index * 19);
        return With(basePos, 6, kumi).With(basePos + 6, 9, pay).With(basePos + 15, 4, ninki);
    }

    public byte[] Build() => (byte[])_buffer.Clone();
}
