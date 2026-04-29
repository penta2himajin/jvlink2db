using System;
using Jvlink2Db.Pipeline.Setup;
using Xunit;

namespace Jvlink2Db.Pipeline.Tests.Setup;

public class DataspecParserTests
{
    [Fact]
    public void Split_returns_single_entry_when_no_comma()
    {
        Assert.Equal(new[] { "RACE" }, DataspecParser.Split("RACE"));
    }

    [Fact]
    public void Split_returns_multiple_entries_for_comma_separated_input()
    {
        Assert.Equal(
            new[] { "RACE", "DIFN", "SLOP" },
            DataspecParser.Split("RACE,DIFN,SLOP"));
    }

    [Fact]
    public void Split_trims_whitespace_and_drops_empty_entries()
    {
        Assert.Equal(
            new[] { "RACE", "DIFN" },
            DataspecParser.Split(" RACE , , DIFN "));
    }

    [Theory]
    [InlineData("")]
    [InlineData(",,,")]
    [InlineData("  ")]
    public void Split_throws_when_no_non_blank_entries(string raw)
    {
        Assert.Throws<ArgumentException>(() => DataspecParser.Split(raw));
    }
}
