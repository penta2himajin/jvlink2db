using System;
using System.Globalization;

namespace Jvlink2Db.Db.Postgres.Records;

/// <summary>
/// Converts the right-trimmed string fields produced by the parser
/// into the typed values that the database layer expects. Empty /
/// spaces-only inputs and the all-zero date sentinel become
/// <c>null</c>. Used by every per-record COPY writer.
/// </summary>
internal static class JvFieldConversions
{
    public static string? AsText(string value) =>
        string.IsNullOrEmpty(value) ? null : value;

    public static DateOnly? AsDate(string yyyymmdd)
    {
        if (string.IsNullOrWhiteSpace(yyyymmdd))
        {
            return null;
        }

        if (yyyymmdd == "00000000")
        {
            return null;
        }

        return DateOnly.TryParseExact(
            yyyymmdd,
            "yyyyMMdd",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out var d)
            ? d
            : null;
    }

    public static short? AsShort(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return short.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out var v)
            ? v
            : null;
    }

    public static int? AsInt(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out var v)
            ? v
            : null;
    }

    public static long? AsLong(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return long.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out var v)
            ? v
            : null;
    }

    public static short?[] AsShortArray(string[] values)
    {
        var result = new short?[values.Length];
        for (var i = 0; i < values.Length; i++)
        {
            result[i] = AsShort(values[i]);
        }

        return result;
    }

    public static int?[] AsIntArray(string[] values)
    {
        var result = new int?[values.Length];
        for (var i = 0; i < values.Length; i++)
        {
            result[i] = AsInt(values[i]);
        }

        return result;
    }

    public static long?[] AsLongArray(string[] values)
    {
        var result = new long?[values.Length];
        for (var i = 0; i < values.Length; i++)
        {
            result[i] = AsLong(values[i]);
        }

        return result;
    }

    public static string?[] AsTextArray(string[] values)
    {
        var result = new string?[values.Length];
        for (var i = 0; i < values.Length; i++)
        {
            result[i] = AsText(values[i]);
        }

        return result;
    }
}
