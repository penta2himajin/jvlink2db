using System;
using Jvlink2Db.Core.Records;
using Jvlink2Db.Parser.Encoding;

namespace Jvlink2Db.Parser.Records;

public static class SkDecoder
{
    public const string RecordSpec = "SK";
    public const int RecordLength = 208;

    public static Sk Decode(byte[] buffer)
    {
        ArgumentNullException.ThrowIfNull(buffer);

        if (buffer.Length < RecordLength)
        {
            throw new ArgumentException(
                $"SK record requires at least {RecordLength} bytes, got {buffer.Length}.", nameof(buffer));
        }

        var span = new ReadOnlySpan<byte>(buffer);
        var actualSpec = Read(span, 1, 2);
        if (actualSpec != RecordSpec)
        {
            throw new InvalidOperationException(
                $"Buffer is not an SK record. Expected '{RecordSpec}', got '{actualSpec}'.");
        }

        var hansyoku = new string[14];
        for (var i = 0; i < 14; i++)
        {
            hansyoku[i] = Read(span, 67 + (i * 10), 10);
        }

        return new Sk
        {
            RecordSpec = actualSpec,
            DataKubun = Read(span, 3, 1),
            MakeDate = Read(span, 4, 8),
            KettoNum = Read(span, 12, 10),
            BirthDate = Read(span, 22, 8),
            SexCD = Read(span, 30, 1),
            HinsyuCD = Read(span, 31, 1),
            KeiroCD = Read(span, 32, 2),
            SankuMochiKubun = Read(span, 34, 1),
            ImportYear = Read(span, 35, 4),
            BreederCode = Read(span, 39, 8),
            SanchiName = Read(span, 47, 20),
            HansyokuNum = hansyoku,
        };
    }

    private static string Read(ReadOnlySpan<byte> buffer, int oneBasedOffset, int length) =>
        Sjis.Encoding.GetString(buffer.Slice(oneBasedOffset - 1, length)).TrimEnd(' ');
}
