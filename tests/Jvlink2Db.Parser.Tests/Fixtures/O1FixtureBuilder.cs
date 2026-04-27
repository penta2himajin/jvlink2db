using System;
using Jvlink2Db.Parser.Encoding;

namespace Jvlink2Db.Parser.Tests.Fixtures;

internal sealed class O1FixtureBuilder
{
    public const int RecordLength = 962;

    private readonly byte[] _buffer;

    public O1FixtureBuilder()
    {
        _buffer = new byte[RecordLength];
        Array.Fill(_buffer, (byte)0x20);
        With(1, 2, "O1");
        _buffer[RecordLength - 2] = 0x0D;
        _buffer[RecordLength - 1] = 0x0A;
    }

    public O1FixtureBuilder With(int oneBasedOffset, int length, string value)
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

    public O1FixtureBuilder RecordSpec(string value) => With(1, 2, value);
    public O1FixtureBuilder DataKubun(string value) => With(3, 1, value);
    public O1FixtureBuilder MakeDate(string yyyymmdd) => With(4, 8, yyyymmdd);
    public O1FixtureBuilder RaceId(string year, string monthDay, string jyoCd, string kaiji, string nichiji, string raceNum)
        => With(12, 4, year).With(16, 4, monthDay).With(20, 2, jyoCd).With(22, 2, kaiji).With(24, 2, nichiji).With(26, 2, raceNum);
    public O1FixtureBuilder HappyoTime(string mmddhhmm) => With(28, 8, mmddhhmm);
    public O1FixtureBuilder TorokuTosu(string value) => With(36, 2, value);
    public O1FixtureBuilder SyussoTosu(string value) => With(38, 2, value);

    public O1FixtureBuilder TansyoEntry(int index, string umaban, string odds, string ninki)
    {
        var basePos = 44 + (index * 8);
        return With(basePos, 2, umaban).With(basePos + 2, 4, odds).With(basePos + 6, 2, ninki);
    }

    public O1FixtureBuilder FukusyoEntry(int index, string umaban, string oddsLow, string oddsHigh, string ninki)
    {
        var basePos = 268 + (index * 12);
        return With(basePos, 2, umaban).With(basePos + 2, 4, oddsLow).With(basePos + 6, 4, oddsHigh).With(basePos + 10, 2, ninki);
    }

    public O1FixtureBuilder WakurenEntry(int index, string kumi, string odds, string ninki)
    {
        var basePos = 604 + (index * 9);
        return With(basePos, 2, kumi).With(basePos + 2, 5, odds).With(basePos + 7, 2, ninki);
    }

    public O1FixtureBuilder TotalHyosuTansyo(string value) => With(928, 11, value);
    public O1FixtureBuilder TotalHyosuFukusyo(string value) => With(939, 11, value);
    public O1FixtureBuilder TotalHyosuWakuren(string value) => With(950, 11, value);

    public byte[] Build() => (byte[])_buffer.Clone();
}
