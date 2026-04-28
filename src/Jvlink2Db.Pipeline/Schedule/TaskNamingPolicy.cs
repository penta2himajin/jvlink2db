using System;

namespace Jvlink2Db.Pipeline.Schedule;

/// <summary>
/// All scheduled tasks created by jvlink2db live under a single
/// folder so <c>schedule status</c> can list them without scanning
/// the entire Task Scheduler tree.
/// </summary>
public static class TaskNamingPolicy
{
    public const string FolderPath = @"\jvlink2db";

    public static string FullPath(string name)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        if (name.Contains('\\') || name.Contains('/'))
        {
            throw new ArgumentException("Task name must not contain path separators.", nameof(name));
        }

        return $@"{FolderPath}\{name}";
    }
}
