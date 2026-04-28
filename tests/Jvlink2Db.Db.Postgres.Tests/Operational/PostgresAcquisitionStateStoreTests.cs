using System;
using System.Threading;
using System.Threading.Tasks;
using Jvlink2Db.Core.Persistence;
using Jvlink2Db.Db.Postgres.Operational;
using Jvlink2Db.Db.Postgres.Schema;
using Jvlink2Db.Db.Postgres.Tests.Fixtures;
using Xunit;

namespace Jvlink2Db.Db.Postgres.Tests.Operational;

[Collection(PostgresCollection.Name)]
[Trait("Category", "Database")]
public sealed class PostgresAcquisitionStateStoreTests
{
    private readonly PostgresFixture _fixture;

    public PostgresAcquisitionStateStoreTests(PostgresFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task GetAsync_returns_null_when_no_row_exists()
    {
        var schema = await ProvisionAsync();
        var sut = new PostgresAcquisitionStateStore(_fixture.DataSource, schema);

        var result = await sut.GetAsync("RACE", 1, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpsertAsync_inserts_then_GetAsync_returns_the_state()
    {
        var schema = await ProvisionAsync();
        var sut = new PostgresAcquisitionStateStore(_fixture.DataSource, schema);

        await sut.UpsertAsync(
            new AcquisitionState("RACE", 1, "20260101000000", "20260101AB.dat"),
            CancellationToken.None);

        var got = await sut.GetAsync("RACE", 1, CancellationToken.None);

        Assert.NotNull(got);
        Assert.Equal("RACE", got!.Dataspec);
        Assert.Equal(1, got.Option);
        Assert.Equal("20260101000000", got.LastFromtime);
        Assert.Equal("20260101AB.dat", got.LastFilename);
    }

    [Fact]
    public async Task UpsertAsync_overwrites_an_existing_row_with_same_PK()
    {
        var schema = await ProvisionAsync();
        var sut = new PostgresAcquisitionStateStore(_fixture.DataSource, schema);

        await sut.UpsertAsync(new AcquisitionState("RACE", 1, "20260101000000", "f1.dat"), CancellationToken.None);
        await sut.UpsertAsync(new AcquisitionState("RACE", 1, "20260201000000", "f2.dat"), CancellationToken.None);

        var got = await sut.GetAsync("RACE", 1, CancellationToken.None);
        Assert.Equal("20260201000000", got!.LastFromtime);
        Assert.Equal("f2.dat", got.LastFilename);
    }

    [Fact]
    public async Task GetAsync_distinguishes_dataspec_and_option_pairs()
    {
        var schema = await ProvisionAsync();
        var sut = new PostgresAcquisitionStateStore(_fixture.DataSource, schema);

        await sut.UpsertAsync(new AcquisitionState("RACE", 1, "ftA", null), CancellationToken.None);
        await sut.UpsertAsync(new AcquisitionState("RACE", 4, null, "fnB"), CancellationToken.None);
        await sut.UpsertAsync(new AcquisitionState("DIFN", 4, null, "fnC"), CancellationToken.None);

        var raceNormal = await sut.GetAsync("RACE", 1, CancellationToken.None);
        var raceSetup = await sut.GetAsync("RACE", 4, CancellationToken.None);
        var difnSetup = await sut.GetAsync("DIFN", 4, CancellationToken.None);

        Assert.Equal("ftA", raceNormal!.LastFromtime);
        Assert.Null(raceNormal.LastFilename);
        Assert.Equal("fnB", raceSetup!.LastFilename);
        Assert.Equal("fnC", difnSetup!.LastFilename);
    }

    private async Task<string> ProvisionAsync()
    {
        var schemaName = $"jvops_{Guid.NewGuid():N}";
        await new PostgresOperationalSchemaProvisioner(_fixture.DataSource, schemaName)
            .EnsureCreatedAsync(CancellationToken.None);
        return schemaName;
    }
}
