using System;
using Jvlink2Db.Core.Records;
using Jvlink2Db.Parser.Encoding;

namespace Jvlink2Db.Parser.Records;

public static class UmDecoder
{
    public const string RecordSpec = "UM";
    public const int RecordLength = 1609;

    public static Um Decode(byte[] buffer)
    {
        ArgumentNullException.ThrowIfNull(buffer);

        if (buffer.Length < RecordLength)
        {
            throw new ArgumentException(
                $"UM record requires at least {RecordLength} bytes, got {buffer.Length}.",
                nameof(buffer));
        }

        var span = new ReadOnlySpan<byte>(buffer);
        var actualSpec = Read(span, 1, 2);
        if (actualSpec != RecordSpec)
        {
            throw new InvalidOperationException(
                $"Buffer is not a UM record. Expected RecordSpec '{RecordSpec}', got '{actualSpec}'.");
        }

        return new Um
        {
            RecordSpec = actualSpec,
            DataKubun = Read(span, 3, 1),
            MakeDate = Read(span, 4, 8),

            KettoNum = Read(span, 12, 10),
            DelKubun = Read(span, 22, 1),
            RegDate = Read(span, 23, 8),
            DelDate = Read(span, 31, 8),
            BirthDate = Read(span, 39, 8),
            Bamei = Read(span, 47, 36),
            BameiKana = Read(span, 83, 36),
            BameiEng = Read(span, 119, 60),
            ZaikyuFlag = Read(span, 179, 1),
            Reserved = Read(span, 180, 19),
            UmaKigoCD = Read(span, 199, 2),
            SexCD = Read(span, 201, 1),
            HinsyuCD = Read(span, 202, 1),
            KeiroCD = Read(span, 203, 2),

            // Ketto3Info[14] base 205, entry 46: HansyokuNum(1,10), Bamei(11,36)
            KettoHansyokuNum = ReadStruct(span, 205, 14, 46, 1, 10),
            KettoBamei = ReadStruct(span, 205, 14, 46, 11, 36),

            TozaiCD = Read(span, 849, 1),
            ChokyosiCode = Read(span, 850, 5),
            ChokyosiRyakusyo = Read(span, 855, 8),
            Syotai = Read(span, 863, 20),
            BreederCode = Read(span, 883, 8),
            BreederName = Read(span, 891, 72),
            SanchiName = Read(span, 963, 20),
            BanusiCode = Read(span, 983, 6),
            BanusiName = Read(span, 989, 64),

            RuikeiHonsyoHeiti = Read(span, 1053, 9),
            RuikeiHonsyoSyogai = Read(span, 1062, 9),
            RuikeiFukaHeichi = Read(span, 1071, 9),
            RuikeiFukaSyogai = Read(span, 1080, 9),
            RuikeiSyutokuHeichi = Read(span, 1089, 9),
            RuikeiSyutokuSyogai = Read(span, 1098, 9),

            // CHAKUKAISU3_INFO at 1107 (18 bytes, 6 entries × 3 bytes)
            ChakuSogo = ReadFlatArray(span, 1107, 3, 6),
            ChakuChuo = ReadFlatArray(span, 1125, 3, 6),

            // ChakuKaisuBa[7] base 1143, entry 18: ChakuKaisu[6] of 3 bytes each
            ChakuKaisuBa = ReadFlatNested(span, 1143, 18, 1, 3, 7, 6),
            ChakuKaisuJyotai = ReadFlatNested(span, 1269, 18, 1, 3, 12, 6),
            ChakuKaisuKyori = ReadFlatNested(span, 1485, 18, 1, 3, 6, 6),

            Kyakusitu = ReadFlatArray(span, 1593, 3, 4),
            RaceCount = Read(span, 1605, 3),
        };
    }

    private static string Read(ReadOnlySpan<byte> buffer, int oneBasedOffset, int length) =>
        Sjis.Encoding.GetString(buffer.Slice(oneBasedOffset - 1, length)).TrimEnd(' ');

    private static string[] ReadFlatArray(ReadOnlySpan<byte> buffer, int baseOffset, int fieldLength, int count)
    {
        var result = new string[count];
        for (var i = 0; i < count; i++)
        {
            result[i] = Read(buffer, baseOffset + (i * fieldLength), fieldLength);
        }

        return result;
    }

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
