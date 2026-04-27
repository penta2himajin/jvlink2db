using System;
using Jvlink2Db.Core.Records;
using Jvlink2Db.Parser.Encoding;

namespace Jvlink2Db.Parser.Records;

/// <summary>
/// Decodes a CP932 byte buffer that JV-Link returned as a single
/// <c>RA</c> record into the neutral <see cref="Ra"/> DTO. Byte
/// offsets follow JV-Data Specification 4.9.0.1; the SDK's
/// <c>JV_RA_RACE</c> struct is the tiebreaker.
/// </summary>
public static class RaDecoder
{
    public const string RecordSpec = "RA";
    public const int RecordLength = 1272;

    public static Ra Decode(byte[] buffer)
    {
        ArgumentNullException.ThrowIfNull(buffer);

        if (buffer.Length < RecordLength)
        {
            throw new ArgumentException(
                $"RA record requires at least {RecordLength} bytes, got {buffer.Length}.",
                nameof(buffer));
        }

        var span = new ReadOnlySpan<byte>(buffer);
        var actualSpec = Read(span, 1, 2);
        if (actualSpec != RecordSpec)
        {
            throw new InvalidOperationException(
                $"Buffer is not an RA record. Expected RecordSpec '{RecordSpec}', got '{actualSpec}'.");
        }

        return new Ra
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
            GradeCDBefore = Read(span, 616, 1),

            SyubetuCD = Read(span, 617, 2),
            KigoCD = Read(span, 619, 3),
            JyuryoCD = Read(span, 622, 1),
            JyokenCD = ReadArray(span, baseOffset: 623, fieldLength: 3, count: 5),

            JyokenName = Read(span, 638, 60),
            Kyori = Read(span, 698, 4),
            KyoriBefore = Read(span, 702, 4),
            TrackCD = Read(span, 706, 2),
            TrackCDBefore = Read(span, 708, 2),
            CourseKubunCD = Read(span, 710, 2),
            CourseKubunCDBefore = Read(span, 712, 2),

            Honsyokin = ReadArray(span, baseOffset: 714, fieldLength: 8, count: 7),
            HonsyokinBefore = ReadArray(span, baseOffset: 770, fieldLength: 8, count: 5),
            Fukasyokin = ReadArray(span, baseOffset: 810, fieldLength: 8, count: 5),
            FukasyokinBefore = ReadArray(span, baseOffset: 850, fieldLength: 8, count: 3),

            HassoTime = Read(span, 874, 4),
            HassoTimeBefore = Read(span, 878, 4),
            TorokuTosu = Read(span, 882, 2),
            SyussoTosu = Read(span, 884, 2),
            NyusenTosu = Read(span, 886, 2),

            TenkoCD = Read(span, 888, 1),
            SibaBabaCD = Read(span, 889, 1),
            DirtBabaCD = Read(span, 890, 1),

            LapTime = ReadArray(span, baseOffset: 891, fieldLength: 3, count: 25),

            SyogaiMileTime = Read(span, 966, 4),
            HaronTimeS3 = Read(span, 970, 3),
            HaronTimeS4 = Read(span, 973, 3),
            HaronTimeL3 = Read(span, 976, 3),
            HaronTimeL4 = Read(span, 979, 3),

            Corners = ReadCorners(span),

            RecordUpKubun = Read(span, 1270, 1),
        };
    }

    private static string Read(ReadOnlySpan<byte> buffer, int oneBasedOffset, int length)
    {
        var slice = buffer.Slice(oneBasedOffset - 1, length);
        return Sjis.Encoding.GetString(slice).TrimEnd(' ');
    }

    private static string[] ReadArray(ReadOnlySpan<byte> buffer, int baseOffset, int fieldLength, int count)
    {
        var result = new string[count];
        for (var i = 0; i < count; i++)
        {
            result[i] = Read(buffer, baseOffset + (i * fieldLength), fieldLength);
        }

        return result;
    }

    private static RaCorner[] ReadCorners(ReadOnlySpan<byte> buffer)
    {
        var corners = new RaCorner[4];
        for (var i = 0; i < 4; i++)
        {
            var basePos = 982 + (i * 72);
            corners[i] = new RaCorner(
                Corner: Read(buffer, basePos, 1),
                Syukaisu: Read(buffer, basePos + 1, 1),
                Jyuni: Read(buffer, basePos + 2, 70));
        }

        return corners;
    }
}
