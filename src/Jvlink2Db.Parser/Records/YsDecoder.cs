using System;
using Jvlink2Db.Core.Records;
using Jvlink2Db.Parser.Encoding;

namespace Jvlink2Db.Parser.Records;

public static class YsDecoder
{
    public const string RecordSpec = "YS";
    public const int RecordLength = 382;

    public static Ys Decode(byte[] buffer)
    {
        ArgumentNullException.ThrowIfNull(buffer);

        if (buffer.Length < RecordLength)
        {
            throw new ArgumentException(
                $"YS record requires at least {RecordLength} bytes, got {buffer.Length}.", nameof(buffer));
        }

        var span = new ReadOnlySpan<byte>(buffer);
        var actualSpec = Read(span, 1, 2);
        if (actualSpec != RecordSpec)
        {
            throw new InvalidOperationException(
                $"Buffer is not a YS record. Expected '{RecordSpec}', got '{actualSpec}'.");
        }

        return new Ys
        {
            RecordSpec = actualSpec,
            DataKubun = Read(span, 3, 1),
            MakeDate = Read(span, 4, 8),

            // RACE_ID2 (14 bytes): Year(4), MonthDay(4), JyoCD(2), Kaiji(2), Nichiji(2)
            Year = Read(span, 12, 4),
            MonthDay = Read(span, 16, 4),
            JyoCD = Read(span, 20, 2),
            Kaiji = Read(span, 22, 2),
            Nichiji = Read(span, 24, 2),
            YoubiCD = Read(span, 26, 1),

            // JYUSYO_INFO[3] base 27, entry 118
            JyusyoTokuNum = ReadStruct(span, 27, 3, 118, 1, 4),
            JyusyoHondai = ReadStruct(span, 27, 3, 118, 5, 60),
            JyusyoRyakusyo10 = ReadStruct(span, 27, 3, 118, 65, 20),
            JyusyoRyakusyo6 = ReadStruct(span, 27, 3, 118, 85, 12),
            JyusyoRyakusyo3 = ReadStruct(span, 27, 3, 118, 97, 6),
            JyusyoNkai = ReadStruct(span, 27, 3, 118, 103, 3),
            JyusyoGradeCD = ReadStruct(span, 27, 3, 118, 106, 1),
            JyusyoSyubetuCD = ReadStruct(span, 27, 3, 118, 107, 2),
            JyusyoKigoCD = ReadStruct(span, 27, 3, 118, 109, 3),
            JyusyoJyuryoCD = ReadStruct(span, 27, 3, 118, 112, 1),
            JyusyoKyori = ReadStruct(span, 27, 3, 118, 113, 4),
            JyusyoTrackCD = ReadStruct(span, 27, 3, 118, 117, 2),
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
