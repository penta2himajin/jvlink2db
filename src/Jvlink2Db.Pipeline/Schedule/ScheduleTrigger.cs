using System;

namespace Jvlink2Db.Pipeline.Schedule;

/// <summary>
/// Schedule trigger spec — a database-/OS-neutral description of when
/// a scheduled task should fire. The CLI's TaskScheduler wrapper maps
/// these to <c>Microsoft.Win32.TaskScheduler.Trigger</c> instances.
/// </summary>
public abstract record ScheduleTrigger;

public sealed record DailyAtTrigger : ScheduleTrigger
{
    public DailyAtTrigger(TimeSpan timeOfDay)
    {
        if (timeOfDay < TimeSpan.Zero || timeOfDay >= TimeSpan.FromDays(1))
        {
            throw new ArgumentOutOfRangeException(nameof(timeOfDay), timeOfDay,
                "Daily time-of-day must be within [00:00:00, 24:00:00).");
        }

        TimeOfDay = timeOfDay;
    }

    public TimeSpan TimeOfDay { get; init; }
}

public sealed record EveryIntervalTrigger : ScheduleTrigger
{
    public EveryIntervalTrigger(TimeSpan interval)
    {
        if (interval <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(interval), interval,
                "Repetition interval must be positive.");
        }

        Interval = interval;
    }

    public TimeSpan Interval { get; init; }
}

public static class ScheduleTriggerParser
{
    /// <summary>
    /// Parses the mutually-exclusive <c>--daily</c> / <c>--every</c>
    /// options into a <see cref="ScheduleTrigger"/>.
    /// </summary>
    public static ScheduleTrigger Parse(string? daily, string? every)
    {
        var dailySet = !string.IsNullOrEmpty(daily);
        var everySet = !string.IsNullOrEmpty(every);

        if (dailySet == everySet)
        {
            throw new ArgumentException(
                "Exactly one of --daily HH:MM or --every <duration> must be provided.");
        }

        if (dailySet)
        {
            if (!TimeSpan.TryParse(daily, out var ts) || ts < TimeSpan.Zero || ts >= TimeSpan.FromDays(1))
            {
                throw new ArgumentException($"--daily expects HH:MM[:SS] within a single day; got '{daily}'.");
            }

            return new DailyAtTrigger(ts);
        }

        return new EveryIntervalTrigger(Watch.IntervalParser.Parse(every!));
    }
}
