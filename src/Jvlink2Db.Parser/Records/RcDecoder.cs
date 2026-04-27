using System;
using Jvlink2Db.Core.Records;
using Jvlink2Db.Parser.Encoding;

namespace Jvlink2Db.Parser.Records;

public static class RcDecoder
{
    public const string RecordSpec = "RC";
    public const int RecordLength = 501;

    public static Rc Decode(byte[] buffer)
    {
        ArgumentNullException.ThrowIfNull(buffer);

        if (buffer.Length < RecordLength)
        {
            throw new ArgumentException(
                $"RC record requires at least {RecordLength} bytes, got {buffer.Length}.",
                nameof(buffer));
        }

        var span = new ReadOnlySpan<byte>(buffer);
        var actualSpec = Read(span, 1, 2);
        if (actualSpec != RecordSpec)
        {
            throw new InvalidOperationException(
                $"Buffer is not an RC record. Expected RecordSpec '{RecordSpec}', got '{actualSpec}'.");
        }

        return new Rc
        {
            RecordSpec = actualSpec,
            DataKubun = Read(span, 3, 1),
            MakeDate = Read(span, 4, 8),

            RecInfoKubun = Read(span, 12, 1),

            // RACE_ID at offset 13-28
            Year = Read(span, 13, 4),
            MonthDay = Read(span, 17, 4),
            JyoCD = Read(span, 21, 2),
            Kaiji = Read(span, 23, 2),
            Nichiji = Read(span, 25, 2),
            RaceNum = Read(span, 27, 2),

            TokuNum = Read(span, 29, 4),
            Hondai = Read(span, 33, 60),
            GradeCD = Read(span, 93, 1),
            SyubetuCD = Read(span, 94, 2),
            Kyori = Read(span, 96, 4),
            TrackCD = Read(span, 100, 2),
            RecKubun = Read(span, 102, 1),
            RecTime = Read(span, 103, 4),

            // TENKO_BABA_INFO at 107-109
            TenkoCD = Read(span, 107, 1),
            SibaBabaCD = Read(span, 108, 1),
            DirtBabaCD = Read(span, 109, 1),

            // RECUMA_INFO[3] base 110, entry 130
            RecUmaKettoNum = ReadStruct(span, 110, 3, 130, 1, 10),
            RecUmaBamei = ReadStruct(span, 110, 3, 130, 11, 36),
            RecUmaUmaKigoCD = ReadStruct(span, 110, 3, 130, 47, 2),
            RecUmaSexCD = ReadStruct(span, 110, 3, 130, 49, 1),
            RecUmaChokyosiCode = ReadStruct(span, 110, 3, 130, 50, 5),
            RecUmaChokyosiName = ReadStruct(span, 110, 3, 130, 55, 34),
            RecUmaFutan = ReadStruct(span, 110, 3, 130, 89, 3),
            RecUmaKisyuCode = ReadStruct(span, 110, 3, 130, 92, 5),
            RecUmaKisyuName = ReadStruct(span, 110, 3, 130, 97, 34),
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
