using System;
using Jvlink2Db.Core.Records;
using Jvlink2Db.Parser.Encoding;

namespace Jvlink2Db.Parser.Records;

public static class CsDecoder
{
    public const string RecordSpec = "CS";
    public const int RecordLength = 6829;

    public static Cs Decode(byte[] buffer)
    {
        ArgumentNullException.ThrowIfNull(buffer);

        if (buffer.Length < RecordLength)
        {
            throw new ArgumentException(
                $"CS record requires at least {RecordLength} bytes, got {buffer.Length}.", nameof(buffer));
        }

        var span = new ReadOnlySpan<byte>(buffer);
        var actualSpec = Read(span, 1, 2);
        if (actualSpec != RecordSpec)
        {
            throw new InvalidOperationException(
                $"Buffer is not a CS record. Expected '{RecordSpec}', got '{actualSpec}'.");
        }

        return new Cs
        {
            RecordSpec = actualSpec,
            DataKubun = Read(span, 3, 1),
            MakeDate = Read(span, 4, 8),

            JyoCD = Read(span, 12, 2),
            Kyori = Read(span, 14, 4),
            TrackCD = Read(span, 18, 2),
            KaishuDate = Read(span, 20, 8),
            CourseEx = Read(span, 28, 6800),
        };
    }

    private static string Read(ReadOnlySpan<byte> buffer, int oneBasedOffset, int length) =>
        Sjis.Encoding.GetString(buffer.Slice(oneBasedOffset - 1, length)).TrimEnd(' ');
}
