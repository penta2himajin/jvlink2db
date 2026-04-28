using System;
using Jvlink2Db.Core.Records;
using Jvlink2Db.Parser.Encoding;

namespace Jvlink2Db.Parser.Records;

public static class WhDecoder
{
    public const string RecordSpec = "WH";
    public const int RecordLength = 847;

    public static Wh Decode(byte[] buffer)
    {
        ArgumentNullException.ThrowIfNull(buffer);

        if (buffer.Length < RecordLength)
        {
            throw new ArgumentException(
                $"WH record requires at least {RecordLength} bytes, got {buffer.Length}.", nameof(buffer));
        }

        var span = new ReadOnlySpan<byte>(buffer);
        var actualSpec = Read(span, 1, 2);
        if (actualSpec != RecordSpec)
        {
            throw new InvalidOperationException(
                $"Buffer is not a WH record. Expected '{RecordSpec}', got '{actualSpec}'.");
        }

        return new Wh
        {
            RecordSpec = actualSpec,
            DataKubun = Read(span, 3, 1),
            MakeDate = Read(span, 4, 8),

            // RACE_ID (16 bytes): Year(4), MonthDay(4), JyoCD(2), Kaiji(2), Nichiji(2), RaceNum(2)
            Year = Read(span, 12, 4),
            MonthDay = Read(span, 16, 4),
            JyoCD = Read(span, 20, 2),
            Kaiji = Read(span, 22, 2),
            Nichiji = Read(span, 24, 2),
            RaceNum = Read(span, 26, 2),

            HappyoTime = Read(span, 28, 8),

            // BATAIJYU_INFO[18] base 36, entry 45
            Umaban = ReadStruct(span, 36, 18, 45, 1, 2),
            Bamei = ReadStruct(span, 36, 18, 45, 3, 36),
            BaTaijyu = ReadStruct(span, 36, 18, 45, 39, 3),
            ZogenFugo = ReadStruct(span, 36, 18, 45, 42, 1),
            ZogenSa = ReadStruct(span, 36, 18, 45, 43, 3),
        };
    }

    private static string Read(ReadOnlySpan<byte> buffer, int oneBasedOffset, int length) =>
        Sjis.Encoding.GetString(buffer.Slice(oneBasedOffset - 1, length)).TrimEnd(' ');

    private static string[] ReadStruct(
        ReadOnlySpan<byte> buffer, int arrayBaseOffset, int count, int entryLength, int innerOffset, int fieldLength)
    {
        var result = new string[count];
        for (var i = 0; i < count; i++)
        {
            result[i] = Read(buffer, arrayBaseOffset + (i * entryLength) + innerOffset - 1, fieldLength);
        }

        return result;
    }
}
