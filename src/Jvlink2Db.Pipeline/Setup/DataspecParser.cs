using System;
using System.Collections.Generic;

namespace Jvlink2Db.Pipeline.Setup;

/// <summary>
/// Parses the <c>--dataspec</c> CLI argument, which accepts a single
/// dataspec or a comma-separated batch (e.g.
/// <c>RACE</c>, <c>RACE,DIFN,SLOP</c>).
/// </summary>
public static class DataspecParser
{
    public static IReadOnlyList<string> Split(string raw)
    {
        ArgumentException.ThrowIfNullOrEmpty(raw);
        var parts = raw.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length == 0)
        {
            throw new ArgumentException("--dataspec must contain at least one non-blank entry.", nameof(raw));
        }
        return parts;
    }
}
