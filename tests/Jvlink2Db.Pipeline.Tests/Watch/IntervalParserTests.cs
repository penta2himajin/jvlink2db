using System;
using Jvlink2Db.Pipeline.Watch;
using Xunit;

namespace Jvlink2Db.Pipeline.Tests.Watch;

public class IntervalParserTests
{
    [Theory]
    [InlineData("30s", 30)]
    [InlineData("90s", 90)]
    public void Parse_seconds_shorthand(string input, int expectedSeconds)
    {
        Assert.Equal(TimeSpan.FromSeconds(expectedSeconds), IntervalParser.Parse(input));
    }

    [Theory]
    [InlineData("5m", 5)]
    [InlineData("60m", 60)]
    public void Parse_minutes_shorthand(string input, int expectedMinutes)
    {
        Assert.Equal(TimeSpan.FromMinutes(expectedMinutes), IntervalParser.Parse(input));
    }

    [Theory]
    [InlineData("1h", 1)]
    [InlineData("12h", 12)]
    public void Parse_hours_shorthand(string input, int expectedHours)
    {
        Assert.Equal(TimeSpan.FromHours(expectedHours), IntervalParser.Parse(input));
    }

    [Fact]
    public void Parse_days_shorthand()
    {
        Assert.Equal(TimeSpan.FromDays(2), IntervalParser.Parse("2d"));
    }

    [Theory]
    [InlineData("00:30:00", 0, 30, 0)]
    [InlineData("01:00:00", 1, 0, 0)]
    [InlineData("00:00:45", 0, 0, 45)]
    public void Parse_HHMMSS_form(string input, int h, int m, int s)
    {
        Assert.Equal(new TimeSpan(h, m, s), IntervalParser.Parse(input));
    }

    [Theory]
    [InlineData("")]
    [InlineData("abc")]
    [InlineData("5x")]
    public void Parse_throws_on_invalid_input(string input)
    {
        Assert.ThrowsAny<Exception>(() => IntervalParser.Parse(input));
    }
}
