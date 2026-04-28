using System;
using Jvlink2Db.Core.Records;
using Jvlink2Db.Parser.Encoding;

namespace Jvlink2Db.Parser.Records;

public static class WcDecoder
{
    public const string RecordSpec = "WC";
    public const int RecordLength = 105;

    public static Wc Decode(byte[] buffer)
    {
        ArgumentNullException.ThrowIfNull(buffer);

        if (buffer.Length < RecordLength)
        {
            throw new ArgumentException(
                $"WC record requires at least {RecordLength} bytes, got {buffer.Length}.", nameof(buffer));
        }

        var span = new ReadOnlySpan<byte>(buffer);
        var actualSpec = Read(span, 1, 2);
        if (actualSpec != RecordSpec)
        {
            throw new InvalidOperationException(
                $"Buffer is not a WC record. Expected '{RecordSpec}', got '{actualSpec}'.");
        }

        return new Wc
        {
            RecordSpec = actualSpec,
            DataKubun = Read(span, 3, 1),
            MakeDate = Read(span, 4, 8),
            TresenKubun = Read(span, 12, 1),
            ChokyoDate = Read(span, 13, 8),
            ChokyoTime = Read(span, 21, 4),
            KettoNum = Read(span, 25, 10),
            Course = Read(span, 35, 1),
            BabaAround = Read(span, 36, 1),
            Reserved = Read(span, 37, 1),
            HaronTime10 = Read(span, 38, 4),
            LapTime10 = Read(span, 42, 3),
            HaronTime9 = Read(span, 45, 4),
            LapTime9 = Read(span, 49, 3),
            HaronTime8 = Read(span, 52, 4),
            LapTime8 = Read(span, 56, 3),
            HaronTime7 = Read(span, 59, 4),
            LapTime7 = Read(span, 63, 3),
            HaronTime6 = Read(span, 66, 4),
            LapTime6 = Read(span, 70, 3),
            HaronTime5 = Read(span, 73, 4),
            LapTime5 = Read(span, 77, 3),
            HaronTime4 = Read(span, 80, 4),
            LapTime4 = Read(span, 84, 3),
            HaronTime3 = Read(span, 87, 4),
            LapTime3 = Read(span, 91, 3),
            HaronTime2 = Read(span, 94, 4),
            LapTime2 = Read(span, 98, 3),
            LapTime1 = Read(span, 101, 3),
        };
    }

    private static string Read(ReadOnlySpan<byte> buffer, int oneBasedOffset, int length) =>
        Sjis.Encoding.GetString(buffer.Slice(oneBasedOffset - 1, length)).TrimEnd(' ');
}
