using System;
using Jvlink2Db.Core.Records;
using Jvlink2Db.Parser.Encoding;

namespace Jvlink2Db.Parser.Records;

public static class KsDecoder
{
    public const string RecordSpec = "KS";
    public const int RecordLength = 4173;

    public static Ks Decode(byte[] buffer)
    {
        ArgumentNullException.ThrowIfNull(buffer);

        if (buffer.Length < RecordLength)
        {
            throw new ArgumentException(
                $"KS record requires at least {RecordLength} bytes, got {buffer.Length}.",
                nameof(buffer));
        }

        var span = new ReadOnlySpan<byte>(buffer);
        var actualSpec = Read(span, 1, 2);
        if (actualSpec != RecordSpec)
        {
            throw new InvalidOperationException(
                $"Buffer is not a KS record. Expected RecordSpec '{RecordSpec}', got '{actualSpec}'.");
        }

        return new Ks
        {
            RecordSpec = actualSpec,
            DataKubun = Read(span, 3, 1),
            MakeDate = Read(span, 4, 8),

            KisyuCode = Read(span, 12, 5),
            DelKubun = Read(span, 17, 1),
            IssueDate = Read(span, 18, 8),
            DelDate = Read(span, 26, 8),
            BirthDate = Read(span, 34, 8),
            KisyuName = Read(span, 42, 34),
            Reserved = Read(span, 76, 34),
            KisyuNameKana = Read(span, 110, 30),
            KisyuRyakusyo = Read(span, 140, 8),
            KisyuNameEng = Read(span, 148, 80),
            SexCD = Read(span, 228, 1),
            SikakuCD = Read(span, 229, 1),
            MinaraiCD = Read(span, 230, 1),
            TozaiCD = Read(span, 231, 1),
            Syotai = Read(span, 232, 20),
            ChokyosiCode = Read(span, 252, 5),
            ChokyosiRyakusyo = Read(span, 257, 8),

            // HATUKIJYO_INFO[2] base 265, entry 67: RaceId(1,16) + SyussoTosu(17,2) + KettoNum(19,10) + Bamei(29,36) + KakuteiJyuni(65,2) + IJyoCD(67,1)
            HatuKiJyoYear = ReadStruct(span, 265, 2, 67, 1, 4),
            HatuKiJyoMonthDay = ReadStruct(span, 265, 2, 67, 5, 4),
            HatuKiJyoJyoCD = ReadStruct(span, 265, 2, 67, 9, 2),
            HatuKiJyoKaiji = ReadStruct(span, 265, 2, 67, 11, 2),
            HatuKiJyoNichiji = ReadStruct(span, 265, 2, 67, 13, 2),
            HatuKiJyoRaceNum = ReadStruct(span, 265, 2, 67, 15, 2),
            HatuKiJyoSyussoTosu = ReadStruct(span, 265, 2, 67, 17, 2),
            HatuKiJyoKettoNum = ReadStruct(span, 265, 2, 67, 19, 10),
            HatuKiJyoBamei = ReadStruct(span, 265, 2, 67, 29, 36),
            HatuKiJyoKakuteiJyuni = ReadStruct(span, 265, 2, 67, 65, 2),
            HatuKiJyoIJyoCD = ReadStruct(span, 265, 2, 67, 67, 1),

            // HATUSYORI_INFO[2] base 399, entry 64
            HatuSyoriYear = ReadStruct(span, 399, 2, 64, 1, 4),
            HatuSyoriMonthDay = ReadStruct(span, 399, 2, 64, 5, 4),
            HatuSyoriJyoCD = ReadStruct(span, 399, 2, 64, 9, 2),
            HatuSyoriKaiji = ReadStruct(span, 399, 2, 64, 11, 2),
            HatuSyoriNichiji = ReadStruct(span, 399, 2, 64, 13, 2),
            HatuSyoriRaceNum = ReadStruct(span, 399, 2, 64, 15, 2),
            HatuSyoriSyussoTosu = ReadStruct(span, 399, 2, 64, 17, 2),
            HatuSyoriKettoNum = ReadStruct(span, 399, 2, 64, 19, 10),
            HatuSyoriBamei = ReadStruct(span, 399, 2, 64, 29, 36),

            // SAIKIN_JYUSYO_INFO[3] base 527, entry 163
            SaikinJyusyoYear = ReadStruct(span, 527, 3, 163, 1, 4),
            SaikinJyusyoMonthDay = ReadStruct(span, 527, 3, 163, 5, 4),
            SaikinJyusyoJyoCD = ReadStruct(span, 527, 3, 163, 9, 2),
            SaikinJyusyoKaiji = ReadStruct(span, 527, 3, 163, 11, 2),
            SaikinJyusyoNichiji = ReadStruct(span, 527, 3, 163, 13, 2),
            SaikinJyusyoRaceNum = ReadStruct(span, 527, 3, 163, 15, 2),
            SaikinJyusyoHondai = ReadStruct(span, 527, 3, 163, 17, 60),
            SaikinJyusyoRyakusyo10 = ReadStruct(span, 527, 3, 163, 77, 20),
            SaikinJyusyoRyakusyo6 = ReadStruct(span, 527, 3, 163, 97, 12),
            SaikinJyusyoRyakusyo3 = ReadStruct(span, 527, 3, 163, 109, 6),
            SaikinJyusyoGradeCD = ReadStruct(span, 527, 3, 163, 115, 1),
            SaikinJyusyoSyussoTosu = ReadStruct(span, 527, 3, 163, 116, 2),
            SaikinJyusyoKettoNum = ReadStruct(span, 527, 3, 163, 118, 10),
            SaikinJyusyoBamei = ReadStruct(span, 527, 3, 163, 128, 36),

            // HonZenRuikei[3] (1016-4171, 1052 bytes each) deferred — not decoded in this PR.
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
