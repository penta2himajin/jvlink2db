using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Jvlink2Db.Core.Records;
using Jvlink2Db.Db.Postgres.Records;
using Jvlink2Db.Db.Postgres.Schema;
using Jvlink2Db.Db.Postgres.Tests.Fixtures;
using Npgsql;
using Xunit;

namespace Jvlink2Db.Db.Postgres.Tests.Records;

[Collection(PostgresCollection.Name)]
[Trait("Category", "Database")]
public sealed class PostgresWhWriterTests
{
    private readonly PostgresFixture _fixture;

    public PostgresWhWriterTests(PostgresFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task WriteAsync_persists_horse_weights()
    {
        var schemaName = await ProvisionAsync();
        var sut = new PostgresWhWriter(_fixture.DataSource, schemaName);

        await sut.WriteAsync(ToAsync(new[] { WhBuilder.Sample() }), CancellationToken.None);

        await using var conn = await _fixture.DataSource.OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand(
            $@"SELECT happyo_time, array_length(umaban, 1), umaban[1], ba_taijyu[1]
               FROM ""{schemaName}"".wh",
            conn);
        await using var reader = await cmd.ExecuteReaderAsync();
        Assert.True(await reader.ReadAsync());
        Assert.Equal("05031040", reader.GetString(0));
        Assert.Equal(18, reader.GetFieldValue<int>(1));
        Assert.Equal("01", reader.GetString(2));
        Assert.Equal("478", reader.GetString(3));
    }

    private async Task<string> ProvisionAsync()
    {
        var schemaName = $"jv_{System.Guid.NewGuid():N}";
        await new PostgresSchemaProvisioner(_fixture.DataSource, schemaName)
            .EnsureCreatedAsync(CancellationToken.None);
        return schemaName;
    }

    private static async IAsyncEnumerable<Wh> ToAsync(IEnumerable<Wh> source)
    {
        foreach (var item in source)
        {
            yield return item;
            await Task.Yield();
        }
    }
}
