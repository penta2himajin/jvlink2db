using System;
using Jvlink2Db.Core.Records;
using Jvlink2Db.Parser.Encoding;

namespace Jvlink2Db.Parser.Records;

public static class H6Decoder
{
    public const string RecordSpec = "H6";
    public const int RecordLength = 102890;

    public static H6 Decode(byte[] buffer)
    {
        ArgumentNullException.ThrowIfNull(buffer);

        if (buffer.Length < RecordLength)
        {
            throw new ArgumentException(
                $"H6 record requires at least {RecordLength} bytes, got {buffer.Length}.",
                nameof(buffer));
        }

        var span = new ReadOnlySpan<byte>(buffer);
        var actualSpec = Read(span, 1, 2);
        if (actualSpec != RecordSpec)
        {
            throw new InvalidOperationException(
                $"Buffer is not an H6 record. Expected RecordSpec '{RecordSpec}', got '{actualSpec}'.");
        }

        return new H6
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
            HatubaiFlag = Read(span, 32, 1),

            HenkanUma = ReadFlatArray(span, 33, 1, 18),

            // HyoSanrentan (HYO_INFO4[4896]) base 51, entry 21: Kumi(1,6), Hyo(7,11), Ninki(18,4)
            SanrentanKumi = ReadStruct(span, 51, 4896, 21, 1, 6),
            SanrentanHyo = ReadStruct(span, 51, 4896, 21, 7, 11),
            SanrentanNinki = ReadStruct(span, 51, 4896, 21, 18, 4),

            HyoTotal = ReadFlatArray(span, 102867, 11, 2),
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
