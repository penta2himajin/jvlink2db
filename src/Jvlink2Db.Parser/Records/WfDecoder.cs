using System;
using Jvlink2Db.Core.Records;
using Jvlink2Db.Parser.Encoding;

namespace Jvlink2Db.Parser.Records;

public static class WfDecoder
{
    public const string RecordSpec = "WF";
    public const int RecordLength = 7215;

    public static Wf Decode(byte[] buffer)
    {
        ArgumentNullException.ThrowIfNull(buffer);

        if (buffer.Length < RecordLength)
        {
            throw new ArgumentException(
                $"WF record requires at least {RecordLength} bytes, got {buffer.Length}.",
                nameof(buffer));
        }

        var span = new ReadOnlySpan<byte>(buffer);
        var actualSpec = Read(span, 1, 2);
        if (actualSpec != RecordSpec)
        {
            throw new InvalidOperationException(
                $"Buffer is not a WF record. Expected RecordSpec '{RecordSpec}', got '{actualSpec}'.");
        }

        return new Wf
        {
            RecordSpec = actualSpec,
            DataKubun = Read(span, 3, 1),
            MakeDate = Read(span, 4, 8),
            KaisaiDate = Read(span, 12, 8),
            Reserved1 = Read(span, 20, 2),

            // WFRaceInfo[5] base 22, entry size 8: JyoCD(1,2), Kaiji(3,2), Nichiji(5,2), RaceNum(7,2)
            RaceJyoCD = ReadStruct(span, 22, 5, 8, 1, 2),
            RaceKaiji = ReadStruct(span, 22, 5, 8, 3, 2),
            RaceNichiji = ReadStruct(span, 22, 5, 8, 5, 2),
            RaceNum = ReadStruct(span, 22, 5, 8, 7, 2),

            Reserved2 = Read(span, 62, 6),
            HatsubaiHyo = Read(span, 68, 11),

            // WFYukoHyoInfo[5] base 79, entry size 11: Yuko_Hyo(1,11)
            YukoHyo = ReadStruct(span, 79, 5, 11, 1, 11),

            HenkanFlag = Read(span, 134, 1),
            FuseiritsuFlag = Read(span, 135, 1),
            TekichunashiFlag = Read(span, 136, 1),
            COShoki = Read(span, 137, 15),
            COZanDaka = Read(span, 152, 15),

            // WFPayInfo[243] base 167, entry size 29: Kumiban(1,10), Pay(11,9), Tekichu_Hyo(20,10)
            PayKumiban = ReadStruct(span, 167, 243, 29, 1, 10),
            PayAmount = ReadStruct(span, 167, 243, 29, 11, 9),
            PayTekichuHyo = ReadStruct(span, 167, 243, 29, 20, 10),
        };
    }

    private static string Read(ReadOnlySpan<byte> buffer, int oneBasedOffset, int length) =>
        Sjis.Encoding.GetString(buffer.Slice(oneBasedOffset - 1, length)).TrimEnd(' ');

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
