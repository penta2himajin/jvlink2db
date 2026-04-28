using System;
using System.Collections.Generic;

namespace Jvlink2Db.Pipeline.Setup;

/// <summary>
/// Validates a dataspec against JV-Link mode constraints. Some
/// dataspecs are delivered as a single all-encompassing snapshot and
/// reject the range form of <c>fromtime</c>; rather than letting the
/// COM call fail at runtime, the pipeline rejects the combination at
/// parse time.
/// </summary>
public static class DataspecValidator
{
    private static readonly HashSet<string> SnapshotOnly = new(StringComparer.OrdinalIgnoreCase)
    {
        "TOKU", "DIFF", "DIFN", "HOSE", "HOSN", "HOYU", "COMM",
    };

    public static void EnsureRangeAllowed(string dataspec)
    {
        ArgumentException.ThrowIfNullOrEmpty(dataspec);

        if (SnapshotOnly.Contains(dataspec))
        {
            throw new DataspecRangeNotSupportedException(dataspec);
        }
    }
}

public sealed class DataspecRangeNotSupportedException : Exception
{
    public DataspecRangeNotSupportedException(string dataspec)
        : base($"Dataspec '{dataspec}' is delivered as a single snapshot and cannot be requested with a range fromtime. Use the 'setup' or 'normal' mode instead.")
    {
        Dataspec = dataspec;
    }

    public string Dataspec { get; }
}
