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
public sealed class PostgresYsWriterTests
{
    private readonly PostgresFixture _fixture;

    public PostgresYsWriterTests(PostgresFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task WriteAsync_persists_year_schedule_with_jyusyo_entries()
    {
        var schemaName = await ProvisionAsync();
        var sut = new PostgresYsWriter(_fixture.DataSource, schemaName);

        await sut.WriteAsync(ToAsync(new[] { YsBuilder.Sample() }), CancellationToken.None);

        await using var conn = await _fixture.DataSource.OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand(
            $@"SELECT youbi_cd, array_length(jyusyo_hondai, 1), jyusyo_hondai[1], jyusyo_kyori[1]
               FROM ""{schemaName}"".ys",
            conn);
        await using var reader = await cmd.ExecuteReaderAsync();
        Assert.True(await reader.ReadAsync());
        Assert.Equal("1", reader.GetString(0));
        Assert.Equal(3, reader.GetFieldValue<int>(1));
        Assert.Equal("天皇賞(春)", reader.GetString(2));
        Assert.Equal("3200", reader.GetString(3));
    }

    private async Task<string> ProvisionAsync()
    {
        var schemaName = $"jv_{System.Guid.NewGuid():N}";
        await new PostgresSchemaProvisioner(_fixture.DataSource, schemaName)
            .EnsureCreatedAsync(CancellationToken.None);
        return schemaName;
    }

    private static async IAsyncEnumerable<Ys> ToAsync(IEnumerable<Ys> source)
    {
        foreach (var item in source)
        {
            yield return item;
            await Task.Yield();
        }
    }
}
