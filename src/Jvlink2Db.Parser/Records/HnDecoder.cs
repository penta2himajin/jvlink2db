using System;
using Jvlink2Db.Core.Records;
using Jvlink2Db.Parser.Encoding;

namespace Jvlink2Db.Parser.Records;

public static class HnDecoder
{
    public const string RecordSpec = "HN";

    /// <summary>
    /// Current JV-Data 4.9.0.x layout (251 bytes including 2-byte CRLF).
    /// </summary>
    public const int RecordLength = 251;

    /// <summary>
    /// Older HN layout observed live (2026-04-29) on a non-trivial fraction
    /// of records: <c>SanchiName</c> is 15 bytes instead of 20, shrinking
    /// the rest of the record by 5 bytes. The header / leading fields are
    /// identical; only fields past offset 210 shift.
    /// </summary>
    public const int LegacyRecordLength = 246;

    public static Hn Decode(byte[] buffer)
    {
        ArgumentNullException.ThrowIfNull(buffer);

        if (buffer.Length < LegacyRecordLength)
        {
            throw new RecordTooShortException(RecordSpec, buffer.Length, LegacyRecordLength);
        }

        var span = new ReadOnlySpan<byte>(buffer);
        var actualSpec = Read(span, 1, 2);
        if (actualSpec != RecordSpec)
        {
            throw new InvalidOperationException(
                $"Buffer is not an HN record. Expected '{RecordSpec}', got '{actualSpec}'.");
        }

        // Layout switch: shorter SanchiName in the legacy layout pushes
        // the trailing two HansyokuNum fields up by 5 bytes.
        var isLegacy = buffer.Length < RecordLength;
        var sanchiNameLength = isLegacy ? 15 : 20;
        var hansyokuFNumOffset = 210 + sanchiNameLength;       // 225 (legacy) or 230 (current)
        var hansyokuMNumOffset = hansyokuFNumOffset + 10;      // 235 or 240

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
            SanchiName = Read(span, 210, sanchiNameLength),
            HansyokuFNum = Read(span, hansyokuFNumOffset, 10),
            HansyokuMNum = Read(span, hansyokuMNumOffset, 10),
        };
    }

    private static string Read(ReadOnlySpan<byte> buffer, int oneBasedOffset, int length) =>
        Sjis.Encoding.GetString(buffer.Slice(oneBasedOffset - 1, length)).TrimEnd(' ');
}
