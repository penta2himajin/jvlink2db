using System;
using Jvlink2Db.Core.Records;
using Jvlink2Db.Parser.Encoding;

namespace Jvlink2Db.Parser.Records;

public static class H1Decoder
{
    public const string RecordSpec = "H1";
    public const int RecordLength = 28955;

    public static H1 Decode(byte[] buffer)
    {
        ArgumentNullException.ThrowIfNull(buffer);

        if (buffer.Length < RecordLength)
        {
            throw new ArgumentException(
                $"H1 record requires at least {RecordLength} bytes, got {buffer.Length}.",
                nameof(buffer));
        }

        var span = new ReadOnlySpan<byte>(buffer);
        var actualSpec = Read(span, 1, 2);
        if (actualSpec != RecordSpec)
        {
            throw new InvalidOperationException(
                $"Buffer is not an H1 record. Expected RecordSpec '{RecordSpec}', got '{actualSpec}'.");
        }

        return new H1
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
            TorokuTosu = Read(span, 28, 2),
            SyussoTosu = Read(span, 30, 2),

            HatubaiFlag = ReadFlatArray(span, 32, 1, 7),
            FukuChakuBaraiKey = Read(span, 39, 1),
            HenkanUma = ReadFlatArray(span, 40, 1, 28),
            HenkanWaku = ReadFlatArray(span, 68, 1, 8),
            HenkanDoWaku = ReadFlatArray(span, 76, 1, 8),

            // HyoTansyo (HYO_INFO1[28]) base 84, entry 15: Umaban(1,2), Hyo(3,11), Ninki(14,2)
            TansyoUmaban = ReadStruct(span, 84, 28, 15, 1, 2),
            TansyoHyo = ReadStruct(span, 84, 28, 15, 3, 11),
            TansyoNinki = ReadStruct(span, 84, 28, 15, 14, 2),

            FukusyoUmaban = ReadStruct(span, 504, 28, 15, 1, 2),
            FukusyoHyo = ReadStruct(span, 504, 28, 15, 3, 11),
            FukusyoNinki = ReadStruct(span, 504, 28, 15, 14, 2),

            WakurenKumi = ReadStruct(span, 924, 36, 15, 1, 2),
            WakurenHyo = ReadStruct(span, 924, 36, 15, 3, 11),
            WakurenNinki = ReadStruct(span, 924, 36, 15, 14, 2),

            // HyoUmaren (HYO_INFO2[153]) base 1464, entry 18: Kumi(1,4), Hyo(5,11), Ninki(16,3)
            UmarenKumi = ReadStruct(span, 1464, 153, 18, 1, 4),
            UmarenHyo = ReadStruct(span, 1464, 153, 18, 5, 11),
            UmarenNinki = ReadStruct(span, 1464, 153, 18, 16, 3),

            WideKumi = ReadStruct(span, 4218, 153, 18, 1, 4),
            WideHyo = ReadStruct(span, 4218, 153, 18, 5, 11),
            WideNinki = ReadStruct(span, 4218, 153, 18, 16, 3),

            UmatanKumi = ReadStruct(span, 6972, 306, 18, 1, 4),
            UmatanHyo = ReadStruct(span, 6972, 306, 18, 5, 11),
            UmatanNinki = ReadStruct(span, 6972, 306, 18, 16, 3),

            // HyoSanrenpuku (HYO_INFO3[816]) base 12480, entry 20: Kumi(1,6), Hyo(7,11), Ninki(18,3)
            SanrenpukuKumi = ReadStruct(span, 12480, 816, 20, 1, 6),
            SanrenpukuHyo = ReadStruct(span, 12480, 816, 20, 7, 11),
            SanrenpukuNinki = ReadStruct(span, 12480, 816, 20, 18, 3),

            HyoTotal = ReadFlatArray(span, 28800, 11, 14),
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
