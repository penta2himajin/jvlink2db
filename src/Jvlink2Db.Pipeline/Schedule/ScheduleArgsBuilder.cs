using System;
using System.Collections.Generic;
using System.Text;

namespace Jvlink2Db.Pipeline.Schedule;

/// <summary>
/// Builds the argument string passed to <c>jvlink2db.exe</c> when a
/// scheduled task fires. Keeps quoting and ordering consistent so the
/// installed task can be diffed against the install command.
/// </summary>
public static class ScheduleArgsBuilder
{
    public sealed record InstallSpec(
        string Mode,
        string Connection,
        string Schema,
        string OperationalSchema,
        string Dataspec,
        string Sid,
        string? Since);

    public static string Build(InstallSpec spec)
    {
        ArgumentNullException.ThrowIfNull(spec);
        if (spec.Mode != "normal" && spec.Mode != "weekly")
        {
            throw new ArgumentException(
                $"schedule install only supports 'normal' or 'weekly'; got '{spec.Mode}'.", nameof(spec));
        }

        if (spec.Mode == "weekly" && string.IsNullOrEmpty(spec.Since))
        {
            throw new ArgumentException("schedule install --mode weekly requires --since.", nameof(spec));
        }

        var sb = new StringBuilder();
        sb.Append(spec.Mode);
        AppendOption(sb, "--connection", spec.Connection);
        AppendOption(sb, "--schema", spec.Schema);
        AppendOption(sb, "--operational-schema", spec.OperationalSchema);
        AppendOption(sb, "--dataspec", spec.Dataspec);
        AppendOption(sb, "--sid", spec.Sid);
        if (!string.IsNullOrEmpty(spec.Since))
        {
            AppendOption(sb, "--since", spec.Since);
        }

        return sb.ToString();
    }

    private static void AppendOption(StringBuilder sb, string name, string value)
    {
        ArgumentException.ThrowIfNullOrEmpty(value);
        sb.Append(' ').Append(name).Append(' ').Append(Quote(value));
    }

    private static string Quote(string value)
    {
        if (value.Length > 0 && !ContainsSpecial(value))
        {
            return value;
        }

        var escaped = value.Replace("\"", "\\\"", StringComparison.Ordinal);
        return $"\"{escaped}\"";
    }

    private static bool ContainsSpecial(string value)
    {
        foreach (var c in value)
        {
            if (char.IsWhiteSpace(c) || c is '"' or ';' or '=')
            {
                return true;
            }
        }

        return false;
    }
}
