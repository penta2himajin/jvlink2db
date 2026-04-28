using System;
using Jvlink2Db.Core.Records;
using Jvlink2Db.Parser.Encoding;

namespace Jvlink2Db.Parser.Records;

public static class DmDecoder
{
    public const string RecordSpec = "DM";
    public const int RecordLength = 303;

    public static Dm Decode(byte[] buffer)
    {
        ArgumentNullException.ThrowIfNull(buffer);

        if (buffer.Length < RecordLength)
        {
            throw new ArgumentException(
                $"DM record requires at least {RecordLength} bytes, got {buffer.Length}.", nameof(buffer));
        }

        var span = new ReadOnlySpan<byte>(buffer);
        var actualSpec = Read(span, 1, 2);
        if (actualSpec != RecordSpec)
        {
            throw new InvalidOperationException(
                $"Buffer is not a DM record. Expected '{RecordSpec}', got '{actualSpec}'.");
        }

        return new Dm
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
            MakeHM = Read(span, 28, 4),

            // DM_INFO[18] base 32, entry 15: Umaban(1,2), DMTime(3,5), DMGosaP(8,4), DMGosaM(12,4)
            Umaban = ReadStruct(span, 32, 18, 15, 1, 2),
            DMTime = ReadStruct(span, 32, 18, 15, 3, 5),
            DMGosaP = ReadStruct(span, 32, 18, 15, 8, 4),
            DMGosaM = ReadStruct(span, 32, 18, 15, 12, 4),
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
