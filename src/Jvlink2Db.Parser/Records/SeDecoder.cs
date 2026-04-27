using System;
using Jvlink2Db.Core.Records;
using Jvlink2Db.Parser.Encoding;

namespace Jvlink2Db.Parser.Records;

public static class SeDecoder
{
    public const string RecordSpec = "SE";
    public const int RecordLength = 555;

    public static Se Decode(byte[] buffer)
    {
        ArgumentNullException.ThrowIfNull(buffer);

        if (buffer.Length < RecordLength)
        {
            throw new ArgumentException(
                $"SE record requires at least {RecordLength} bytes, got {buffer.Length}.",
                nameof(buffer));
        }

        var span = new ReadOnlySpan<byte>(buffer);
        var actualSpec = Read(span, 1, 2);
        if (actualSpec != RecordSpec)
        {
            throw new InvalidOperationException(
                $"Buffer is not an SE record. Expected RecordSpec '{RecordSpec}', got '{actualSpec}'.");
        }

        return new Se
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

            Wakuban = Read(span, 28, 1),
            Umaban = Read(span, 29, 2),
            KettoNum = Read(span, 31, 10),
            Bamei = Read(span, 41, 36),
            UmaKigoCD = Read(span, 77, 2),
            SexCD = Read(span, 79, 1),
            HinsyuCD = Read(span, 80, 1),
            KeiroCD = Read(span, 81, 2),
            Barei = Read(span, 83, 2),
            TozaiCD = Read(span, 85, 1),
            ChokyosiCode = Read(span, 86, 5),
            ChokyosiRyakusyo = Read(span, 91, 8),
            BanusiCode = Read(span, 99, 6),
            BanusiName = Read(span, 105, 64),
            Fukusyoku = Read(span, 169, 60),
            Reserved1 = Read(span, 229, 60),
            Futan = Read(span, 289, 3),
            FutanBefore = Read(span, 292, 3),
            Blinker = Read(span, 295, 1),
            Reserved2 = Read(span, 296, 1),
            KisyuCode = Read(span, 297, 5),
            KisyuCodeBefore = Read(span, 302, 5),
            KisyuRyakusyo = Read(span, 307, 8),
            KisyuRyakusyoBefore = Read(span, 315, 8),
            MinaraiCD = Read(span, 323, 1),
            MinaraiCDBefore = Read(span, 324, 1),
            BaTaijyu = Read(span, 325, 3),
            ZogenFugo = Read(span, 328, 1),
            ZogenSa = Read(span, 329, 3),
            IJyoCD = Read(span, 332, 1),
            NyusenJyuni = Read(span, 333, 2),
            KakuteiJyuni = Read(span, 335, 2),
            DochakuKubun = Read(span, 337, 1),
            DochakuTosu = Read(span, 338, 1),
            Time = Read(span, 339, 4),
            ChakusaCD = Read(span, 343, 3),
            ChakusaCDP = Read(span, 346, 3),
            ChakusaCDPP = Read(span, 349, 3),
            Jyuni1c = Read(span, 352, 2),
            Jyuni2c = Read(span, 354, 2),
            Jyuni3c = Read(span, 356, 2),
            Jyuni4c = Read(span, 358, 2),
            Odds = Read(span, 360, 4),
            Ninki = Read(span, 364, 2),
            Honsyokin = Read(span, 366, 8),
            Fukasyokin = Read(span, 374, 8),
            Reserved3 = Read(span, 382, 3),
            Reserved4 = Read(span, 385, 3),
            HaronTimeL4 = Read(span, 388, 3),
            HaronTimeL3 = Read(span, 391, 3),

            ChakuUma = ReadChakuUma(span),

            TimeDiff = Read(span, 532, 4),
            RecordUpKubun = Read(span, 536, 1),
            DMKubun = Read(span, 537, 1),
            DMTime = Read(span, 538, 5),
            DMGosaP = Read(span, 543, 4),
            DMGosaM = Read(span, 547, 4),
            DMJyuni = Read(span, 551, 2),
            KyakusituKubun = Read(span, 553, 1),
        };
    }

    private static SeChakuUma[] ReadChakuUma(ReadOnlySpan<byte> buffer)
    {
        var result = new SeChakuUma[3];
        for (var i = 0; i < 3; i++)
        {
            var basePos = 394 + (i * 46);
            result[i] = new SeChakuUma(
                KettoNum: Read(buffer, basePos, 10),
                Bamei: Read(buffer, basePos + 10, 36));
        }

        return result;
    }

    private static string Read(ReadOnlySpan<byte> buffer, int oneBasedOffset, int length) =>
        Sjis.Encoding.GetString(buffer.Slice(oneBasedOffset - 1, length)).TrimEnd(' ');
}
