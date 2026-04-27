using System;
using Jvlink2Db.Core.Records;
using Jvlink2Db.Parser.Encoding;

namespace Jvlink2Db.Parser.Records;

public static class O6Decoder
{
    public const string RecordSpec = "O6";
    public const int RecordLength = 83285;
    private const int EntryCount = 4896;
    private const int EntryLength = 17;

    public static O6 Decode(byte[] buffer)
    {
        ArgumentNullException.ThrowIfNull(buffer);

        if (buffer.Length < RecordLength)
        {
            throw new ArgumentException(
                $"O6 record requires at least {RecordLength} bytes, got {buffer.Length}.",
                nameof(buffer));
        }

        var span = new ReadOnlySpan<byte>(buffer);
        var actualSpec = Read(span, 1, 2);
        if (actualSpec != RecordSpec)
        {
            throw new InvalidOperationException(
                $"Buffer is not an O6 record. Expected RecordSpec '{RecordSpec}', got '{actualSpec}'.");
        }

        return new O6
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
            SanrentanFlag = Read(span, 40, 1),

            // Kumi(1,6), Odds(7,7), Ninki(14,4)
            Kumi = ReadStruct(span, 41, EntryCount, EntryLength, 1, 6),
            Odds = ReadStruct(span, 41, EntryCount, EntryLength, 7, 7),
            Ninki = ReadStruct(span, 41, EntryCount, EntryLength, 14, 4),

            TotalHyosuSanrentan = Read(span, 83273, 11),
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
