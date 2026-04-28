using System;
using Jvlink2Db.Core.Records;
using Jvlink2Db.Parser.Encoding;

namespace Jvlink2Db.Parser.Records;

public static class HcDecoder
{
    public const string RecordSpec = "HC";
    public const int RecordLength = 60;

    public static Hc Decode(byte[] buffer)
    {
        ArgumentNullException.ThrowIfNull(buffer);

        if (buffer.Length < RecordLength)
        {
            throw new ArgumentException(
                $"HC record requires at least {RecordLength} bytes, got {buffer.Length}.", nameof(buffer));
        }

        var span = new ReadOnlySpan<byte>(buffer);
        var actualSpec = Read(span, 1, 2);
        if (actualSpec != RecordSpec)
        {
            throw new InvalidOperationException(
                $"Buffer is not an HC record. Expected '{RecordSpec}', got '{actualSpec}'.");
        }

        return new Hc
        {
            RecordSpec = actualSpec,
            DataKubun = Read(span, 3, 1),
            MakeDate = Read(span, 4, 8),
            TresenKubun = Read(span, 12, 1),
            ChokyoDate = Read(span, 13, 8),
            ChokyoTime = Read(span, 21, 4),
            KettoNum = Read(span, 25, 10),
            HaronTime4 = Read(span, 35, 4),
            LapTime4 = Read(span, 39, 3),
            HaronTime3 = Read(span, 42, 4),
            LapTime3 = Read(span, 46, 3),
            HaronTime2 = Read(span, 49, 4),
            LapTime2 = Read(span, 53, 3),
            LapTime1 = Read(span, 56, 3),
        };
    }

    private static string Read(ReadOnlySpan<byte> buffer, int oneBasedOffset, int length) =>
        Sjis.Encoding.GetString(buffer.Slice(oneBasedOffset - 1, length)).TrimEnd(' ');
}
