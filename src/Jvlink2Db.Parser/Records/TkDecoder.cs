using System;
using Jvlink2Db.Core.Records;
using Jvlink2Db.Parser.Encoding;

namespace Jvlink2Db.Parser.Records;

public static class TkDecoder
{
    public const string RecordSpec = "TK";
    public const int RecordLength = 21657;

    public static Tk Decode(byte[] buffer)
    {
        ArgumentNullException.ThrowIfNull(buffer);

        if (buffer.Length < RecordLength)
        {
            throw new ArgumentException(
                $"TK record requires at least {RecordLength} bytes, got {buffer.Length}.", nameof(buffer));
        }

        var span = new ReadOnlySpan<byte>(buffer);
        var actualSpec = Read(span, 1, 2);
        if (actualSpec != RecordSpec)
        {
            throw new InvalidOperationException(
                $"Buffer is not a TK record. Expected '{RecordSpec}', got '{actualSpec}'.");
        }

        var jyokenCd = new string[5];
        for (var i = 0; i < 5; i++)
        {
            jyokenCd[i] = Read(span, 622 + (i * 3), 3);
        }

        return new Tk
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

            // RACE_INFO base 28
            YoubiCD = Read(span, 28, 1),
            TokuNum = Read(span, 29, 4),
            Hondai = Read(span, 33, 60),
            Fukudai = Read(span, 93, 60),
            Kakko = Read(span, 153, 60),
            HondaiEng = Read(span, 213, 120),
            FukudaiEng = Read(span, 333, 120),
            KakkoEng = Read(span, 453, 120),
            Ryakusyo10 = Read(span, 573, 20),
            Ryakusyo6 = Read(span, 593, 12),
            Ryakusyo3 = Read(span, 605, 6),
            Kubun = Read(span, 611, 1),
            Nkai = Read(span, 612, 3),

            GradeCD = Read(span, 615, 1),

            // RACE_JYOKEN base 616
            SyubetuCD = Read(span, 616, 2),
            KigoCD = Read(span, 618, 3),
            JyuryoCD = Read(span, 621, 1),
            JyokenCD = jyokenCd,

            Kyori = Read(span, 637, 4),
            TrackCD = Read(span, 641, 2),
            CourseKubunCD = Read(span, 643, 2),
            HandiDate = Read(span, 645, 8),
            TorokuTosu = Read(span, 653, 3),

            // TOKUUMA_INFO[300] base 656, entry 70
            TokuNumSeq = ReadStruct(span, 656, 300, 70, 1, 3),
            KettoNum = ReadStruct(span, 656, 300, 70, 4, 10),
            Bamei = ReadStruct(span, 656, 300, 70, 14, 36),
            UmaKigoCD = ReadStruct(span, 656, 300, 70, 50, 2),
            SexCD = ReadStruct(span, 656, 300, 70, 52, 1),
            TozaiCD = ReadStruct(span, 656, 300, 70, 53, 1),
            ChokyosiCode = ReadStruct(span, 656, 300, 70, 54, 5),
            ChokyosiRyakusyo = ReadStruct(span, 656, 300, 70, 59, 8),
            Futan = ReadStruct(span, 656, 300, 70, 67, 3),
            Koryu = ReadStruct(span, 656, 300, 70, 70, 1),
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
