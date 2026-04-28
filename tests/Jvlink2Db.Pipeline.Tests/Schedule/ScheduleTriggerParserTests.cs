using System;
using Jvlink2Db.Pipeline.Schedule;
using Xunit;

namespace Jvlink2Db.Pipeline.Tests.Schedule;

public class ScheduleTriggerParserTests
{
    [Fact]
    public void Parse_returns_DailyAt_when_only_daily_is_set()
    {
        var trigger = ScheduleTriggerParser.Parse(daily: "06:30", every: null);

        var daily = Assert.IsType<DailyAtTrigger>(trigger);
        Assert.Equal(new TimeSpan(6, 30, 0), daily.TimeOfDay);
    }

    [Fact]
    public void Parse_returns_EveryInterval_when_only_every_is_set()
    {
        var trigger = ScheduleTriggerParser.Parse(daily: null, every: "1h");

        var every = Assert.IsType<EveryIntervalTrigger>(trigger);
        Assert.Equal(TimeSpan.FromHours(1), every.Interval);
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData("", "")]
    [InlineData("06:30", "1h")]
    public void Parse_throws_when_neither_or_both_are_set(string? daily, string? every)
    {
        Assert.Throws<ArgumentException>(() => ScheduleTriggerParser.Parse(daily, every));
    }

    [Theory]
    [InlineData("25:00")]
    [InlineData("not-a-time")]
    public void Parse_rejects_invalid_daily_value(string daily)
    {
        Assert.Throws<ArgumentException>(() => ScheduleTriggerParser.Parse(daily, null));
    }

    [Fact]
    public void DailyAtTrigger_rejects_24_hours()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new DailyAtTrigger(TimeSpan.FromDays(1)));
    }

    [Fact]
    public void EveryIntervalTrigger_rejects_zero_or_negative()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new EveryIntervalTrigger(TimeSpan.Zero));
        Assert.Throws<ArgumentOutOfRangeException>(() => new EveryIntervalTrigger(TimeSpan.FromMinutes(-1)));
    }
}
