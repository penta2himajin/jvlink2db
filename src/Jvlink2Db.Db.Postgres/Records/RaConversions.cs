using System;
using System.Globalization;

namespace Jvlink2Db.Db.Postgres.Records;

/// <summary>
/// Converts the right-trimmed string fields on <see cref="Core.Records.Ra"/>
/// into the typed values that <c>jv.ra</c> expects. Empty / spaces-only
/// inputs and the all-zero date sentinel become <c>null</c>.
/// </summary>
internal static class RaConversions
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
