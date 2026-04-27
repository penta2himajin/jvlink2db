using System;
using Jvlink2Db.Parser.Encoding;

namespace Jvlink2Db.Parser.Tests.Fixtures;

/// <summary>
/// Builds 1272-byte synthetic RA buffers that match the byte layout in
/// JV-Data Specification 4.9.0.1 (cross-checked against the SDK's
/// JVData_Struct.cs <c>JV_RA_RACE</c> struct). Offsets are 1-based to
/// match the spec; the builder converts to 0-based internally.
///
/// Default contents: the buffer is filled with ASCII spaces (0x20),
/// the first two bytes are "RA" (record spec), and bytes 1271..1272
/// hold CR+LF. Every <c>WithXxx</c> method overlays one field value
/// in CP932.
/// </summary>
internal sealed class RaFixtureBuilder
{
    public const int RecordLength = 1272;

    private readonly byte[] _buffer;

    public RaFixtureBuilder()
    {
        _buffer = new byte[RecordLength];
        Array.Fill(_buffer, (byte)0x20);
        WriteAscii(1, 2, "RA");
        _buffer[RecordLength - 2] = 0x0D;
        _buffer[RecordLength - 1] = 0x0A;
    }

    public RaFixtureBuilder With(int oneBasedOffset, int length, string value)
    {
        var bytes = Sjis.Encoding.GetBytes(value);
        if (bytes.Length > length)
        {
            throw new ArgumentException(
                $"value '{value}' is {bytes.Length} bytes; field at offset {oneBasedOffset} only takes {length}.",
                nameof(value));
        }

        var start = oneBasedOffset - 1;
        bytes.CopyTo(_buffer.AsSpan(start, bytes.Length));
        for (var i = start + bytes.Length; i < start + length; i++)
        {
            _buffer[i] = 0x20;
        }

        return this;
    }

    private RaFixtureBuilder WriteAscii(int oneBasedOffset, int length, string value) => With(oneBasedOffset, length, value);

    // RECORD_ID
    public RaFixtureBuilder RecordSpec(string value) => With(1, 2, value);
    public RaFixtureBuilder DataKubun(string value) => With(3, 1, value);
    public RaFixtureBuilder MakeDate(string yyyymmdd) => With(4, 8, yyyymmdd);

    // RACE_ID
    public RaFixtureBuilder RaceId(string year, string monthDay, string jyoCd, string kaiji, string nichiji, string raceNum)
        => With(12, 4, year)
           .With(16, 4, monthDay)
           .With(20, 2, jyoCd)
           .With(22, 2, kaiji)
           .With(24, 2, nichiji)
           .With(26, 2, raceNum);

    // RACE_INFO
    public RaFixtureBuilder YoubiCD(string value) => With(28, 1, value);
    public RaFixtureBuilder TokuNum(string value) => With(29, 4, value);
    public RaFixtureBuilder Hondai(string value) => With(33, 60, value);
    public RaFixtureBuilder Fukudai(string value) => With(93, 60, value);
    public RaFixtureBuilder Kakko(string value) => With(153, 60, value);
    public RaFixtureBuilder HondaiEng(string value) => With(213, 120, value);
    public RaFixtureBuilder FukudaiEng(string value) => With(333, 120, value);
    public RaFixtureBuilder KakkoEng(string value) => With(453, 120, value);
    public RaFixtureBuilder Ryakusyo10(string value) => With(573, 20, value);
    public RaFixtureBuilder Ryakusyo6(string value) => With(593, 12, value);
    public RaFixtureBuilder Ryakusyo3(string value) => With(605, 6, value);
    public RaFixtureBuilder Kubun(string value) => With(611, 1, value);
    public RaFixtureBuilder Nkai(string value) => With(612, 3, value);

    // Grade codes
    public RaFixtureBuilder GradeCD(string value) => With(615, 1, value);
    public RaFixtureBuilder GradeCDBefore(string value) => With(616, 1, value);

    // RACE_JYOKEN
    public RaFixtureBuilder SyubetuCD(string value) => With(617, 2, value);
    public RaFixtureBuilder KigoCD(string value) => With(619, 3, value);
    public RaFixtureBuilder JyuryoCD(string value) => With(622, 1, value);
    public RaFixtureBuilder JyokenCD(int index, string value) => With(623 + (index * 3), 3, value);

    public RaFixtureBuilder JyokenName(string value) => With(638, 60, value);
    public RaFixtureBuilder Kyori(string value) => With(698, 4, value);
    public RaFixtureBuilder KyoriBefore(string value) => With(702, 4, value);
    public RaFixtureBuilder TrackCD(string value) => With(706, 2, value);
    public RaFixtureBuilder TrackCDBefore(string value) => With(708, 2, value);
    public RaFixtureBuilder CourseKubunCD(string value) => With(710, 2, value);
    public RaFixtureBuilder CourseKubunCDBefore(string value) => With(712, 2, value);

    public RaFixtureBuilder Honsyokin(int index, string value) => With(714 + (index * 8), 8, value);
    public RaFixtureBuilder HonsyokinBefore(int index, string value) => With(770 + (index * 8), 8, value);
    public RaFixtureBuilder Fukasyokin(int index, string value) => With(810 + (index * 8), 8, value);
    public RaFixtureBuilder FukasyokinBefore(int index, string value) => With(850 + (index * 8), 8, value);

    public RaFixtureBuilder HassoTime(string value) => With(874, 4, value);
    public RaFixtureBuilder HassoTimeBefore(string value) => With(878, 4, value);
    public RaFixtureBuilder TorokuTosu(string value) => With(882, 2, value);
    public RaFixtureBuilder SyussoTosu(string value) => With(884, 2, value);
    public RaFixtureBuilder NyusenTosu(string value) => With(886, 2, value);

    // TENKO_BABA_INFO
    public RaFixtureBuilder TenkoCD(string value) => With(888, 1, value);
    public RaFixtureBuilder SibaBabaCD(string value) => With(889, 1, value);
    public RaFixtureBuilder DirtBabaCD(string value) => With(890, 1, value);

    public RaFixtureBuilder LapTime(int index, string value) => With(891 + (index * 3), 3, value);

    public RaFixtureBuilder SyogaiMileTime(string value) => With(966, 4, value);
    public RaFixtureBuilder HaronTimeS3(string value) => With(970, 3, value);
    public RaFixtureBuilder HaronTimeS4(string value) => With(973, 3, value);
    public RaFixtureBuilder HaronTimeL3(string value) => With(976, 3, value);
    public RaFixtureBuilder HaronTimeL4(string value) => With(979, 3, value);

    // CORNER_INFO[i]: corner=1, syukaisu=1, jyuni=70
    public RaFixtureBuilder Corner(int index, string corner, string syukaisu, string jyuni)
    {
        var basePos = 982 + (index * 72);
        return With(basePos, 1, corner)
              .With(basePos + 1, 1, syukaisu)
              .With(basePos + 2, 70, jyuni);
    }

    public RaFixtureBuilder RecordUpKubun(string value) => With(1270, 1, value);

    public byte[] Build() => (byte[])_buffer.Clone();
}
