using System;
using Jvlink2Db.Core.Records;
using Jvlink2Db.Parser.Encoding;

namespace Jvlink2Db.Parser.Records;

public static class HnDecoder
{
    public const string RecordSpec = "HN";
    public const int RecordLength = 251;

    public static Hn Decode(byte[] buffer)
    {
        ArgumentNullException.ThrowIfNull(buffer);

        if (buffer.Length < RecordLength)
        {
            throw new ArgumentException(
                $"HN record requires at least {RecordLength} bytes, got {buffer.Length}.", nameof(buffer));
        }

        var span = new ReadOnlySpan<byte>(buffer);
        var actualSpec = Read(span, 1, 2);
        if (actualSpec != RecordSpec)
        {
            throw new InvalidOperationException(
                $"Buffer is not an HN record. Expected '{RecordSpec}', got '{actualSpec}'.");
        }

        return new Hn
        {
            RecordSpec = actualSpec,
            DataKubun = Read(span, 3, 1),
            MakeDate = Read(span, 4, 8),

            HansyokuNum = Read(span, 12, 10),
            Reserved = Read(span, 22, 8),
            KettoNum = Read(span, 30, 10),
            DelKubun = Read(span, 40, 1),
            Bamei = Read(span, 41, 36),
            BameiKana = Read(span, 77, 40),
            BameiEng = Read(span, 117, 80),
            BirthYear = Read(span, 197, 4),
            SexCD = Read(span, 201, 1),
            HinsyuCD = Read(span, 202, 1),
            KeiroCD = Read(span, 203, 2),
            HansyokuMochiKubun = Read(span, 205, 1),
            ImportYear = Read(span, 206, 4),
            SanchiName = Read(span, 210, 20),
            HansyokuFNum = Read(span, 230, 10),
            HansyokuMNum = Read(span, 240, 10),
        };
    }

    private static string Read(ReadOnlySpan<byte> buffer, int oneBasedOffset, int length) =>
        Sjis.Encoding.GetString(buffer.Slice(oneBasedOffset - 1, length)).TrimEnd(' ');
}
