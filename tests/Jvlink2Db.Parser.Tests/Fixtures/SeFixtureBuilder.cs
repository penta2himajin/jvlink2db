using System;
using Jvlink2Db.Parser.Encoding;

namespace Jvlink2Db.Parser.Tests.Fixtures;

internal sealed class SeFixtureBuilder
{
    public const int RecordLength = 555;

    private readonly byte[] _buffer;

    public SeFixtureBuilder()
    {
        _buffer = new byte[RecordLength];
        Array.Fill(_buffer, (byte)0x20);
        With(1, 2, "SE");
        _buffer[RecordLength - 2] = 0x0D;
        _buffer[RecordLength - 1] = 0x0A;
    }

    public SeFixtureBuilder With(int oneBasedOffset, int length, string value)
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

    public SeFixtureBuilder RecordSpec(string value) => With(1, 2, value);
    public SeFixtureBuilder DataKubun(string value) => With(3, 1, value);
    public SeFixtureBuilder MakeDate(string yyyymmdd) => With(4, 8, yyyymmdd);

    public SeFixtureBuilder RaceId(string year, string monthDay, string jyoCd, string kaiji, string nichiji, string raceNum)
        => With(12, 4, year).With(16, 4, monthDay).With(20, 2, jyoCd).With(22, 2, kaiji).With(24, 2, nichiji).With(26, 2, raceNum);

    public SeFixtureBuilder Wakuban(string value) => With(28, 1, value);
    public SeFixtureBuilder Umaban(string value) => With(29, 2, value);
    public SeFixtureBuilder KettoNum(string value) => With(31, 10, value);
    public SeFixtureBuilder Bamei(string value) => With(41, 36, value);
    public SeFixtureBuilder UmaKigoCD(string value) => With(77, 2, value);
    public SeFixtureBuilder SexCD(string value) => With(79, 1, value);
    public SeFixtureBuilder HinsyuCD(string value) => With(80, 1, value);
    public SeFixtureBuilder KeiroCD(string value) => With(81, 2, value);
    public SeFixtureBuilder Barei(string value) => With(83, 2, value);
    public SeFixtureBuilder TozaiCD(string value) => With(85, 1, value);
    public SeFixtureBuilder ChokyosiCode(string value) => With(86, 5, value);
    public SeFixtureBuilder ChokyosiRyakusyo(string value) => With(91, 8, value);
    public SeFixtureBuilder BanusiCode(string value) => With(99, 6, value);
    public SeFixtureBuilder BanusiName(string value) => With(105, 64, value);
    public SeFixtureBuilder Fukusyoku(string value) => With(169, 60, value);
    public SeFixtureBuilder Futan(string value) => With(289, 3, value);
    public SeFixtureBuilder FutanBefore(string value) => With(292, 3, value);
    public SeFixtureBuilder Blinker(string value) => With(295, 1, value);
    public SeFixtureBuilder KisyuCode(string value) => With(297, 5, value);
    public SeFixtureBuilder KisyuCodeBefore(string value) => With(302, 5, value);
    public SeFixtureBuilder KisyuRyakusyo(string value) => With(307, 8, value);
    public SeFixtureBuilder KisyuRyakusyoBefore(string value) => With(315, 8, value);
    public SeFixtureBuilder MinaraiCD(string value) => With(323, 1, value);
    public SeFixtureBuilder MinaraiCDBefore(string value) => With(324, 1, value);
    public SeFixtureBuilder BaTaijyu(string value) => With(325, 3, value);
    public SeFixtureBuilder ZogenFugo(string value) => With(328, 1, value);
    public SeFixtureBuilder ZogenSa(string value) => With(329, 3, value);
    public SeFixtureBuilder IJyoCD(string value) => With(332, 1, value);
    public SeFixtureBuilder NyusenJyuni(string value) => With(333, 2, value);
    public SeFixtureBuilder KakuteiJyuni(string value) => With(335, 2, value);
    public SeFixtureBuilder DochakuKubun(string value) => With(337, 1, value);
    public SeFixtureBuilder DochakuTosu(string value) => With(338, 1, value);
    public SeFixtureBuilder Time(string value) => With(339, 4, value);
    public SeFixtureBuilder ChakusaCD(string value) => With(343, 3, value);
    public SeFixtureBuilder Jyuni1c(string value) => With(352, 2, value);
    public SeFixtureBuilder Jyuni2c(string value) => With(354, 2, value);
    public SeFixtureBuilder Jyuni3c(string value) => With(356, 2, value);
    public SeFixtureBuilder Jyuni4c(string value) => With(358, 2, value);
    public SeFixtureBuilder Odds(string value) => With(360, 4, value);
    public SeFixtureBuilder Ninki(string value) => With(364, 2, value);
    public SeFixtureBuilder Honsyokin(string value) => With(366, 8, value);
    public SeFixtureBuilder Fukasyokin(string value) => With(374, 8, value);
    public SeFixtureBuilder HaronTimeL4(string value) => With(388, 3, value);
    public SeFixtureBuilder HaronTimeL3(string value) => With(391, 3, value);

    public SeFixtureBuilder ChakuUma(int index, string kettoNum, string bamei)
    {
        var basePos = 394 + (index * 46);
        return With(basePos, 10, kettoNum).With(basePos + 10, 36, bamei);
    }

    public SeFixtureBuilder TimeDiff(string value) => With(532, 4, value);
    public SeFixtureBuilder RecordUpKubun(string value) => With(536, 1, value);
    public SeFixtureBuilder DMKubun(string value) => With(537, 1, value);
    public SeFixtureBuilder DMTime(string value) => With(538, 5, value);
    public SeFixtureBuilder DMGosaP(string value) => With(543, 4, value);
    public SeFixtureBuilder DMGosaM(string value) => With(547, 4, value);
    public SeFixtureBuilder DMJyuni(string value) => With(551, 2, value);
    public SeFixtureBuilder KyakusituKubun(string value) => With(553, 1, value);

    public byte[] Build() => (byte[])_buffer.Clone();
}
