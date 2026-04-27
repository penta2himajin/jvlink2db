using System;
using Jvlink2Db.Core.Records;
using Jvlink2Db.Parser.Encoding;

namespace Jvlink2Db.Parser.Records;

public static class ChDecoder
{
    public const string RecordSpec = "CH";
    public const int RecordLength = 3862;

    public static Ch Decode(byte[] buffer)
    {
        ArgumentNullException.ThrowIfNull(buffer);

        if (buffer.Length < RecordLength)
        {
            throw new ArgumentException(
                $"CH record requires at least {RecordLength} bytes, got {buffer.Length}.",
                nameof(buffer));
        }

        var span = new ReadOnlySpan<byte>(buffer);
        var actualSpec = Read(span, 1, 2);
        if (actualSpec != RecordSpec)
        {
            throw new InvalidOperationException(
                $"Buffer is not a CH record. Expected RecordSpec '{RecordSpec}', got '{actualSpec}'.");
        }

        return new Ch
        {
            RecordSpec = actualSpec,
            DataKubun = Read(span, 3, 1),
            MakeDate = Read(span, 4, 8),

            ChokyosiCode = Read(span, 12, 5),
            DelKubun = Read(span, 17, 1),
            IssueDate = Read(span, 18, 8),
            DelDate = Read(span, 26, 8),
            BirthDate = Read(span, 34, 8),
            ChokyosiName = Read(span, 42, 34),
            ChokyosiNameKana = Read(span, 76, 30),
            ChokyosiRyakusyo = Read(span, 106, 8),
            ChokyosiNameEng = Read(span, 114, 80),
            SexCD = Read(span, 194, 1),
            TozaiCD = Read(span, 195, 1),
            Syotai = Read(span, 196, 20),

            // SAIKIN_JYUSYO_INFO[3] base 216, entry 163
            SaikinJyusyoYear = ReadStruct(span, 216, 3, 163, 1, 4),
            SaikinJyusyoMonthDay = ReadStruct(span, 216, 3, 163, 5, 4),
            SaikinJyusyoJyoCD = ReadStruct(span, 216, 3, 163, 9, 2),
            SaikinJyusyoKaiji = ReadStruct(span, 216, 3, 163, 11, 2),
            SaikinJyusyoNichiji = ReadStruct(span, 216, 3, 163, 13, 2),
            SaikinJyusyoRaceNum = ReadStruct(span, 216, 3, 163, 15, 2),
            SaikinJyusyoHondai = ReadStruct(span, 216, 3, 163, 17, 60),
            SaikinJyusyoRyakusyo10 = ReadStruct(span, 216, 3, 163, 77, 20),
            SaikinJyusyoRyakusyo6 = ReadStruct(span, 216, 3, 163, 97, 12),
            SaikinJyusyoRyakusyo3 = ReadStruct(span, 216, 3, 163, 109, 6),
            SaikinJyusyoGradeCD = ReadStruct(span, 216, 3, 163, 115, 1),
            SaikinJyusyoSyussoTosu = ReadStruct(span, 216, 3, 163, 116, 2),
            SaikinJyusyoKettoNum = ReadStruct(span, 216, 3, 163, 118, 10),
            SaikinJyusyoBamei = ReadStruct(span, 216, 3, 163, 128, 36),

            // HonZenRuikei[3] (705-3860, 1052 bytes each) deferred.
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
