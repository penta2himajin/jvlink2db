using System;
using Jvlink2Db.Core.Records;
using Jvlink2Db.Parser.Encoding;

namespace Jvlink2Db.Parser.Records;

public static class O1Decoder
{
    public const string RecordSpec = "O1";
    public const int RecordLength = 962;

    public static O1 Decode(byte[] buffer)
    {
        ArgumentNullException.ThrowIfNull(buffer);

        if (buffer.Length < RecordLength)
        {
            throw new ArgumentException(
                $"O1 record requires at least {RecordLength} bytes, got {buffer.Length}.",
                nameof(buffer));
        }

        var span = new ReadOnlySpan<byte>(buffer);
        var actualSpec = Read(span, 1, 2);
        if (actualSpec != RecordSpec)
        {
            throw new InvalidOperationException(
                $"Buffer is not an O1 record. Expected RecordSpec '{RecordSpec}', got '{actualSpec}'.");
        }

        return new O1
        {
            RecordSpec = actualSpec,
            DataKubun = Read(span, 3, 1),
            MakeDate = Read(span, 4, 8),
            Year = Read(span, 12, 4),
            MonthDay = Read(span, 16, 4),
            JyoCD = Read(span, 20, 2),
            Kaiji = Read(span, 22, 2),
            Nichiji = Read(span, 24, 2),
            RaceNum = Read(span, 26, 2),
            HappyoTime = Read(span, 28, 8),
            TorokuTosu = Read(span, 36, 2),
            SyussoTosu = Read(span, 38, 2),

            TansyoFlag = Read(span, 40, 1),
            FukusyoFlag = Read(span, 41, 1),
            WakurenFlag = Read(span, 42, 1),
            FukuChakuBaraiKey = Read(span, 43, 1),

            // ODDS_TANSYO_INFO[28] base 44, entry 8: Umaban(1,2), Odds(3,4), Ninki(7,2)
            TansyoUmaban = ReadStruct(span, 44, 28, 8, 1, 2),
            TansyoOdds = ReadStruct(span, 44, 28, 8, 3, 4),
            TansyoNinki = ReadStruct(span, 44, 28, 8, 7, 2),

            // ODDS_FUKUSYO_INFO[28] base 268, entry 12: Umaban(1,2), OddsLow(3,4), OddsHigh(7,4), Ninki(11,2)
            FukusyoUmaban = ReadStruct(span, 268, 28, 12, 1, 2),
            FukusyoOddsLow = ReadStruct(span, 268, 28, 12, 3, 4),
            FukusyoOddsHigh = ReadStruct(span, 268, 28, 12, 7, 4),
            FukusyoNinki = ReadStruct(span, 268, 28, 12, 11, 2),

            // ODDS_WAKUREN_INFO[36] base 604, entry 9: Kumi(1,2), Odds(3,5), Ninki(8,2)
            WakurenKumi = ReadStruct(span, 604, 36, 9, 1, 2),
            WakurenOdds = ReadStruct(span, 604, 36, 9, 3, 5),
            WakurenNinki = ReadStruct(span, 604, 36, 9, 8, 2),

            TotalHyosuTansyo = Read(span, 928, 11),
            TotalHyosuFukusyo = Read(span, 939, 11),
            TotalHyosuWakuren = Read(span, 950, 11),
        };
    }

    private static string Read(ReadOnlySpan<byte> buffer, int oneBasedOffset, int length) =>
        Sjis.Encoding.GetString(buffer.Slice(oneBasedOffset - 1, length)).TrimEnd(' ');

    private static string[] ReadStruct(
        ReadOnlySpan<byte> buffer,
        int arrayBaseOffset,
        int count,
        int entryLength,
        int innerOffset,
        int fieldLength)
    {
        var result = new string[count];
        for (var i = 0; i < count; i++)
        {
            var entryStart = arrayBaseOffset + (i * entryLength);
            result[i] = Read(buffer, entryStart + innerOffset - 1, fieldLength);
        }

        return result;
    }
}
