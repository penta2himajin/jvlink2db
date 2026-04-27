using System;
using Jvlink2Db.Core.Records;
using Jvlink2Db.Parser.Encoding;

namespace Jvlink2Db.Parser.Records;

public static class HrDecoder
{
    public const string RecordSpec = "HR";
    public const int RecordLength = 719;

    public static Hr Decode(byte[] buffer)
    {
        ArgumentNullException.ThrowIfNull(buffer);

        if (buffer.Length < RecordLength)
        {
            throw new ArgumentException(
                $"HR record requires at least {RecordLength} bytes, got {buffer.Length}.",
                nameof(buffer));
        }

        var span = new ReadOnlySpan<byte>(buffer);
        var actualSpec = Read(span, 1, 2);
        if (actualSpec != RecordSpec)
        {
            throw new InvalidOperationException(
                $"Buffer is not an HR record. Expected RecordSpec '{RecordSpec}', got '{actualSpec}'.");
        }

        return new Hr
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

            FuseirituFlag = ReadArray(span, baseOffset: 32, fieldLength: 1, count: 9),
            TokubaraiFlag = ReadArray(span, baseOffset: 41, fieldLength: 1, count: 9),
            HenkanFlag = ReadArray(span, baseOffset: 50, fieldLength: 1, count: 9),
            HenkanUma = ReadArray(span, baseOffset: 59, fieldLength: 1, count: 28),
            HenkanWaku = ReadArray(span, baseOffset: 87, fieldLength: 1, count: 8),
            HenkanDoWaku = ReadArray(span, baseOffset: 95, fieldLength: 1, count: 8),

            // PayTansyo (PAY_INFO1[3]) base offset 103, each 13 bytes
            PayTansyoUmaban = ReadStruct(span, 103, 3, 13, 1, 2),
            PayTansyoPay = ReadStruct(span, 103, 3, 13, 3, 9),
            PayTansyoNinki = ReadStruct(span, 103, 3, 13, 12, 2),

            // PayFukusyo (PAY_INFO1[5]) base offset 142
            PayFukusyoUmaban = ReadStruct(span, 142, 5, 13, 1, 2),
            PayFukusyoPay = ReadStruct(span, 142, 5, 13, 3, 9),
            PayFukusyoNinki = ReadStruct(span, 142, 5, 13, 12, 2),

            // PayWakuren (PAY_INFO1[3]) base offset 207
            PayWakurenUmaban = ReadStruct(span, 207, 3, 13, 1, 2),
            PayWakurenPay = ReadStruct(span, 207, 3, 13, 3, 9),
            PayWakurenNinki = ReadStruct(span, 207, 3, 13, 12, 2),

            // PayUmaren (PAY_INFO2[3]) base offset 246, 16 bytes each
            PayUmarenKumi = ReadStruct(span, 246, 3, 16, 1, 4),
            PayUmarenPay = ReadStruct(span, 246, 3, 16, 5, 9),
            PayUmarenNinki = ReadStruct(span, 246, 3, 16, 14, 3),

            // PayWide (PAY_INFO2[7]) base offset 294
            PayWideKumi = ReadStruct(span, 294, 7, 16, 1, 4),
            PayWidePay = ReadStruct(span, 294, 7, 16, 5, 9),
            PayWideNinki = ReadStruct(span, 294, 7, 16, 14, 3),

            // PayReserved1 (PAY_INFO2[3]) base offset 406
            PayReserved1Kumi = ReadStruct(span, 406, 3, 16, 1, 4),
            PayReserved1Pay = ReadStruct(span, 406, 3, 16, 5, 9),
            PayReserved1Ninki = ReadStruct(span, 406, 3, 16, 14, 3),

            // PayUmatan (PAY_INFO2[6]) base offset 454
            PayUmatanKumi = ReadStruct(span, 454, 6, 16, 1, 4),
            PayUmatanPay = ReadStruct(span, 454, 6, 16, 5, 9),
            PayUmatanNinki = ReadStruct(span, 454, 6, 16, 14, 3),

            // PaySanrenpuku (PAY_INFO3[3]) base offset 550, 18 bytes each
            PaySanrenpukuKumi = ReadStruct(span, 550, 3, 18, 1, 6),
            PaySanrenpukuPay = ReadStruct(span, 550, 3, 18, 7, 9),
            PaySanrenpukuNinki = ReadStruct(span, 550, 3, 18, 16, 3),

            // PaySanrentan (PAY_INFO4[6]) base offset 604, 19 bytes each
            PaySanrentanKumi = ReadStruct(span, 604, 6, 19, 1, 6),
            PaySanrentanPay = ReadStruct(span, 604, 6, 19, 7, 9),
            PaySanrentanNinki = ReadStruct(span, 604, 6, 19, 16, 4),
        };
    }

    private static string Read(ReadOnlySpan<byte> buffer, int oneBasedOffset, int length) =>
        Sjis.Encoding.GetString(buffer.Slice(oneBasedOffset - 1, length)).TrimEnd(' ');

    private static string[] ReadArray(ReadOnlySpan<byte> buffer, int baseOffset, int fieldLength, int count)
    {
        var result = new string[count];
        for (var i = 0; i < count; i++)
        {
            result[i] = Read(buffer, baseOffset + (i * fieldLength), fieldLength);
        }

        return result;
    }

    /// <summary>
    /// Reads one field at a fixed offset within each entry of a
    /// struct array. Returns <paramref name="count"/> values.
    /// </summary>
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
