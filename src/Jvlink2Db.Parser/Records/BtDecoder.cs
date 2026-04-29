using System;
using Jvlink2Db.Core.Records;
using Jvlink2Db.Parser.Encoding;

namespace Jvlink2Db.Parser.Records;

public static class BtDecoder
{
    public const string RecordSpec = "BT";
    public const int RecordLength = 6889;

    public static Bt Decode(byte[] buffer)
    {
        ArgumentNullException.ThrowIfNull(buffer);

        if (buffer.Length < RecordLength)
        {
            throw new RecordTooShortException(RecordSpec, buffer.Length, RecordLength);
        }

        var span = new ReadOnlySpan<byte>(buffer);
        var actualSpec = Read(span, 1, 2);
        if (actualSpec != RecordSpec)
        {
            throw new InvalidOperationException(
                $"Buffer is not a BT record. Expected '{RecordSpec}', got '{actualSpec}'.");
        }

        return new Bt
        {
            RecordSpec = actualSpec,
            DataKubun = Read(span, 3, 1),
            MakeDate = Read(span, 4, 8),

            HansyokuNum = Read(span, 12, 10),
            KeitoId = Read(span, 22, 30),
            KeitoName = Read(span, 52, 36),
            KeitoEx = Read(span, 88, 6800),
        };
    }

    private static string Read(ReadOnlySpan<byte> buffer, int oneBasedOffset, int length) =>
        Sjis.Encoding.GetString(buffer.Slice(oneBasedOffset - 1, length)).TrimEnd(' ');
}
