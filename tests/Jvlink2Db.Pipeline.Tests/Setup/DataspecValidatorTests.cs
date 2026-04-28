using Jvlink2Db.Pipeline.Setup;
using Xunit;

namespace Jvlink2Db.Pipeline.Tests.Setup;

public class DataspecValidatorTests
{
    [Theory]
    [InlineData("TOKU")]
    [InlineData("DIFF")]
    [InlineData("DIFN")]
    [InlineData("HOSE")]
    [InlineData("HOSN")]
    [InlineData("HOYU")]
    [InlineData("COMM")]
    public void EnsureRangeAllowed_throws_for_snapshot_dataspecs(string dataspec)
    {
        var ex = Assert.Throws<DataspecRangeNotSupportedException>(
            () => DataspecValidator.EnsureRangeAllowed(dataspec));

        Assert.Equal(dataspec, ex.Dataspec);
    }

    [Theory]
    [InlineData("RACE")]
    [InlineData("BLOD")]
    [InlineData("BLDN")]
    [InlineData("SNAP")]
    [InlineData("SNPN")]
    [InlineData("SLOP")]
    [InlineData("WOOD")]
    [InlineData("MING")]
    public void EnsureRangeAllowed_passes_for_range_capable_dataspecs(string dataspec)
    {
        DataspecValidator.EnsureRangeAllowed(dataspec);
    }

    [Fact]
    public void EnsureRangeAllowed_is_case_insensitive()
    {
        Assert.Throws<DataspecRangeNotSupportedException>(
            () => DataspecValidator.EnsureRangeAllowed("diff"));
    }
}
