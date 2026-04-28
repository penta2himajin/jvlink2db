using System;
using Jvlink2Db.Pipeline.Schedule;
using Xunit;

namespace Jvlink2Db.Pipeline.Tests.Schedule;

public class TaskNamingPolicyTests
{
    [Fact]
    public void FullPath_prefixes_folder()
    {
        Assert.Equal(@"\jvlink2db\race-normal", TaskNamingPolicy.FullPath("race-normal"));
    }

    [Theory]
    [InlineData("foo\\bar")]
    [InlineData("foo/bar")]
    public void FullPath_rejects_path_separators(string name)
    {
        Assert.Throws<ArgumentException>(() => TaskNamingPolicy.FullPath(name));
    }

    [Fact]
    public void FullPath_rejects_empty_name()
    {
        Assert.Throws<ArgumentException>(() => TaskNamingPolicy.FullPath(""));
    }
}
