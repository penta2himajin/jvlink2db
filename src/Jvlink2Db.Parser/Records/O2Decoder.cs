using System;
using Jvlink2Db.Core.Records;
using Jvlink2Db.Parser.Encoding;

namespace Jvlink2Db.Parser.Records;

public static class O2Decoder
{
    public const string RecordSpec = "O2";
    public const int RecordLength = 2042;
    private const int EntryCount = 153;
    private const int EntryLength = 13;

    public static O2 Decode(byte[] buffer)
    {
        ArgumentNullException.ThrowIfNull(buffer);

        if (buffer.Length < RecordLength)
        {
            throw new ArgumentException(
                $"O2 record requires at least {RecordLength} bytes, got {buffer.Length}.",
                nameof(buffer));
        }

        var span = new ReadOnlySpan<byte>(buffer);
        var actualSpec = Read(span, 1, 2);
        if (actualSpec != RecordSpec)
        {
            throw new InvalidOperationException(
                $"Buffer is not an O2 record. Expected RecordSpec '{RecordSpec}', got '{actualSpec}'.");
        }

        return new O2
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
            UmarenFlag = Read(span, 40, 1),

            // Kumi(1,4), Odds(5,6), Ninki(11,3)
            Kumi = ReadStruct(span, 41, EntryCount, EntryLength, 1, 4),
            Odds = ReadStruct(span, 41, EntryCount, EntryLength, 5, 6),
            Ninki = ReadStruct(span, 41, EntryCount, EntryLength, 11, 3),

            TotalHyosuUmaren = Read(span, 2030, 11),
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
