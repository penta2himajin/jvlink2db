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
public sealed class PostgresBtWriterTests
{
    private readonly PostgresFixture _fixture;

    public PostgresBtWriterTests(PostgresFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task WriteAsync_persists_keito_record()
    {
        var schemaName = await ProvisionAsync();
        var sut = new PostgresBtWriter(_fixture.DataSource, schemaName);

        await sut.WriteAsync(ToAsync(new[] { BtBuilder.Sample() }), CancellationToken.None);

        await using var conn = await _fixture.DataSource.OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand(
            $@"SELECT hansyoku_num, keito_name FROM ""{schemaName}"".bt",
            conn);
        await using var reader = await cmd.ExecuteReaderAsync();
        Assert.True(await reader.ReadAsync());
        Assert.Equal("0001234567", reader.GetString(0));
        Assert.Equal("テスト系統", reader.GetString(1));
    }

    private async Task<string> ProvisionAsync()
    {
        var schemaName = $"jv_{System.Guid.NewGuid():N}";
        await new PostgresSchemaProvisioner(_fixture.DataSource, schemaName)
            .EnsureCreatedAsync(CancellationToken.None);
        return schemaName;
    }

    private static async IAsyncEnumerable<Bt> ToAsync(IEnumerable<Bt> source)
    {
        foreach (var item in source)
        {
            yield return item;
            await Task.Yield();
        }
    }
}
