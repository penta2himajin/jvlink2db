using System;
using Jvlink2Db.Core.Records;
using Jvlink2Db.Parser.Encoding;

namespace Jvlink2Db.Parser.Records;

public static class CkDecoder
{
    public const string RecordSpec = "CK";
    public const int RecordLength = 6870;

    public static Ck Decode(byte[] buffer)
    {
        ArgumentNullException.ThrowIfNull(buffer);

        if (buffer.Length < RecordLength)
        {
            throw new ArgumentException(
                $"CK record requires at least {RecordLength} bytes, got {buffer.Length}.", nameof(buffer));
        }

        var span = new ReadOnlySpan<byte>(buffer);
        var actualSpec = Read(span, 1, 2);
        if (actualSpec != RecordSpec)
        {
            throw new InvalidOperationException(
                $"Buffer is not a CK record. Expected '{RecordSpec}', got '{actualSpec}'.");
        }

        var kyakusitu = new string[4];
        for (var i = 0; i < 4; i++)
        {
            // UmaChaku rel 1343 + 3i → abs 1370 + 3i
            kyakusitu[i] = Read(span, 1370 + (i * 3), 3);
        }

        return new Ck
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

            // UmaChaku base 28
            KettoNum = Read(span, 28, 10),
            Bamei = Read(span, 38, 36),
            RuikeiHonsyoHeiti = Read(span, 74, 9),
            RuikeiHonsyoSyogai = Read(span, 83, 9),
            RuikeiFukaHeichi = Read(span, 92, 9),
            RuikeiFukaSyogai = Read(span, 101, 9),
            RuikeiSyutokuHeichi = Read(span, 110, 9),
            RuikeiSyutokuSyogai = Read(span, 119, 9),
            Kyakusitu = kyakusitu,
            RaceCount = Read(span, 1382, 3),

            // KisyuChaku base 1385
            KisyuCode = Read(span, 1385, 5),
            KisyuName = Read(span, 1390, 34),

            // ChokyoChaku base 3864
            ChokyosiCode = Read(span, 3864, 5),
            ChokyosiName = Read(span, 3869, 34),

            // BanusiChaku base 6343
            BanusiCode = Read(span, 6343, 6),
            BanusiNameCo = Read(span, 6349, 64),
            BanusiName = Read(span, 6413, 64),

            // BreederChaku base 6597
            BreederCode = Read(span, 6597, 8),
            BreederNameCo = Read(span, 6605, 72),
            BreederName = Read(span, 6677, 72),
        };
    }

    private static string Read(ReadOnlySpan<byte> buffer, int oneBasedOffset, int length) =>
        Sjis.Encoding.GetString(buffer.Slice(oneBasedOffset - 1, length)).TrimEnd(' ');
}
