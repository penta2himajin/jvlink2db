using System;
using System.Text.RegularExpressions;

namespace Jvlink2Db.Pipeline.Watch;

/// <summary>
/// Parses a watch interval string. Accepts the standard
/// <see cref="TimeSpan"/> form (<c>HH:MM:SS</c>) and a shorthand
/// <c>Ns</c>/<c>Nm</c>/<c>Nh</c>/<c>Nd</c>.
/// </summary>
public static class IntervalParser
{
    private static readonly Regex Shorthand = new(@"^(\d+)([smhd])$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public static TimeSpan Parse(string value)
    {
        ArgumentException.ThrowIfNullOrEmpty(value);

        var match = Shorthand.Match(value);
        if (match.Success)
        {
            var n = int.Parse(match.Groups[1].ValueSpan);
            return match.Groups[2].Value switch
            {
                "s" => TimeSpan.FromSeconds(n),
                "m" => TimeSpan.FromMinutes(n),
                "h" => TimeSpan.FromHours(n),
                "d" => TimeSpan.FromDays(n),
                _ => throw new FormatException($"Interval '{value}' has an unknown unit suffix."),
            };
        }

        if (TimeSpan.TryParse(value, out var ts))
        {
            return ts;
        }

        throw new FormatException(
            $"Interval '{value}' must be a TimeSpan (HH:MM:SS) or a shorthand like 30s / 5m / 1h / 1d.");
    }
}
