using System;
using Jvlink2Db.Core.Records;
using Jvlink2Db.Parser.Encoding;

namespace Jvlink2Db.Parser.Records;

public static class BnDecoder
{
    public const string RecordSpec = "BN";
    public const int RecordLength = 477;

    public static Bn Decode(byte[] buffer)
    {
        ArgumentNullException.ThrowIfNull(buffer);

        if (buffer.Length < RecordLength)
        {
            throw new ArgumentException(
                $"BN record requires at least {RecordLength} bytes, got {buffer.Length}.",
                nameof(buffer));
        }

        var span = new ReadOnlySpan<byte>(buffer);
        var actualSpec = Read(span, 1, 2);
        if (actualSpec != RecordSpec)
        {
            throw new InvalidOperationException(
                $"Buffer is not a BN record. Expected RecordSpec '{RecordSpec}', got '{actualSpec}'.");
        }

        return new Bn
        {
            RecordSpec = actualSpec,
            DataKubun = Read(span, 3, 1),
            MakeDate = Read(span, 4, 8),

            BanusiCode = Read(span, 12, 6),
            BanusiNameCo = Read(span, 18, 64),
            BanusiName = Read(span, 82, 64),
            BanusiNameKana = Read(span, 146, 50),
            BanusiNameEng = Read(span, 196, 100),
            Fukusyoku = Read(span, 296, 60),

            // SEI_RUIKEI_INFO[2] base 356, entry 60
            HonRuikeiSetYear = ReadStruct(span, 356, 2, 60, 1, 4),
            HonRuikeiHonsyokinTotal = ReadStruct(span, 356, 2, 60, 5, 10),
            HonRuikeiFukaSyokin = ReadStruct(span, 356, 2, 60, 15, 10),
            HonRuikeiChakuKaisu = ReadFlatNested(span, 356, 60, 25, 6, 2, 6),
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

    private static string[] ReadFlatNested(
        ReadOnlySpan<byte> buffer, int baseOffset, int entryLength, int innerOffset, int innerFieldLength, int outerCount, int innerCount)
    {
        var result = new string[outerCount * innerCount];
        for (var o = 0; o < outerCount; o++)
        {
            var entryStart = baseOffset + (o * entryLength);
            for (var i = 0; i < innerCount; i++)
            {
                result[(o * innerCount) + i] = Read(buffer, entryStart + innerOffset - 1 + (i * innerFieldLength), innerFieldLength);
            }
        }

        return result;
    }
}
