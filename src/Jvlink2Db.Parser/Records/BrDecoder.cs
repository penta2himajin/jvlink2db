using System;
using Jvlink2Db.Core.Records;
using Jvlink2Db.Parser.Encoding;

namespace Jvlink2Db.Parser.Records;

public static class BrDecoder
{
    public const string RecordSpec = "BR";
    public const int RecordLength = 545;

    public static Br Decode(byte[] buffer)
    {
        ArgumentNullException.ThrowIfNull(buffer);

        if (buffer.Length < RecordLength)
        {
            throw new ArgumentException(
                $"BR record requires at least {RecordLength} bytes, got {buffer.Length}.",
                nameof(buffer));
        }

        var span = new ReadOnlySpan<byte>(buffer);
        var actualSpec = Read(span, 1, 2);
        if (actualSpec != RecordSpec)
        {
            throw new InvalidOperationException(
                $"Buffer is not a BR record. Expected RecordSpec '{RecordSpec}', got '{actualSpec}'.");
        }

        return new Br
        {
            RecordSpec = actualSpec,
            DataKubun = Read(span, 3, 1),
            MakeDate = Read(span, 4, 8),

            BreederCode = Read(span, 12, 8),
            BreederNameCo = Read(span, 20, 72),
            BreederName = Read(span, 92, 72),
            BreederNameKana = Read(span, 164, 72),
            BreederNameEng = Read(span, 236, 168),
            Address = Read(span, 404, 20),

            // SEI_RUIKEI_INFO[2] base 424, entry 60: SetYear(1,4), HonSyokinTotal(5,10), FukaSyokin(15,10), ChakuKaisu[6](25, 6 each)
            HonRuikeiSetYear = ReadStruct(span, 424, 2, 60, 1, 4),
            HonRuikeiHonsyokinTotal = ReadStruct(span, 424, 2, 60, 5, 10),
            HonRuikeiFukaSyokin = ReadStruct(span, 424, 2, 60, 15, 10),
            HonRuikeiChakuKaisu = ReadFlatNested(span, baseOffset: 424, entryLength: 60, innerOffset: 25, innerFieldLength: 6, outerCount: 2, innerCount: 6),
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

    /// <summary>
    /// Reads a flat array spanning <paramref name="outerCount"/> outer
    /// entries, each containing an inner sub-array of
    /// <paramref name="innerCount"/> elements. Resulting indexing:
    /// <c>result[outer * innerCount + inner]</c>.
    /// </summary>
    private static string[] ReadFlatNested(
        ReadOnlySpan<byte> buffer,
        int baseOffset,
        int entryLength,
        int innerOffset,
        int innerFieldLength,
        int outerCount,
        int innerCount)
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
