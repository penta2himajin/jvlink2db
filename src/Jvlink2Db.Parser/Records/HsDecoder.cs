using System;
using Jvlink2Db.Core.Records;
using Jvlink2Db.Parser.Encoding;

namespace Jvlink2Db.Parser.Records;

public static class HsDecoder
{
    public const string RecordSpec = "HS";
    public const int RecordLength = 200;

    public static Hs Decode(byte[] buffer)
    {
        ArgumentNullException.ThrowIfNull(buffer);

        if (buffer.Length < RecordLength)
        {
            throw new ArgumentException(
                $"HS record requires at least {RecordLength} bytes, got {buffer.Length}.", nameof(buffer));
        }

        var span = new ReadOnlySpan<byte>(buffer);
        var actualSpec = Read(span, 1, 2);
        if (actualSpec != RecordSpec)
        {
            throw new InvalidOperationException(
                $"Buffer is not an HS record. Expected '{RecordSpec}', got '{actualSpec}'.");
        }

        return new Hs
        {
            RecordSpec = actualSpec,
            DataKubun = Read(span, 3, 1),
            MakeDate = Read(span, 4, 8),
            KettoNum = Read(span, 12, 10),
            HansyokuFNum = Read(span, 22, 10),
            HansyokuMNum = Read(span, 32, 10),
            BirthYear = Read(span, 42, 4),
            SaleCode = Read(span, 46, 6),
            SaleHostName = Read(span, 52, 40),
            SaleName = Read(span, 92, 80),
            FromDate = Read(span, 172, 8),
            ToDate = Read(span, 180, 8),
            Barei = Read(span, 188, 1),
            Price = Read(span, 189, 10),
        };
    }

    private static string Read(ReadOnlySpan<byte> buffer, int oneBasedOffset, int length) =>
        Sjis.Encoding.GetString(buffer.Slice(oneBasedOffset - 1, length)).TrimEnd(' ');
}
