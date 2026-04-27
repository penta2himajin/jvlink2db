using System;
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
public sealed class PostgresHnWriterTests
{
    private readonly PostgresFixture _fixture;

    public PostgresHnWriterTests(PostgresFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task WriteAsync_persists_brood_mare_master()
    {
        var schemaName = await ProvisionAsync();
        var sut = new PostgresHnWriter(_fixture.DataSource, schemaName);

        await sut.WriteAsync(ToAsync(new[] { HnBuilder.Sample() }), CancellationToken.None);

        await using var conn = await _fixture.DataSource.OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand(
            $@"SELECT bamei, hansyoku_f_num, hansyoku_m_num FROM ""{schemaName}"".hn",
            conn);
        await using var reader = await cmd.ExecuteReaderAsync();
        Assert.True(await reader.ReadAsync());
        Assert.Equal("テスト繁殖馬", reader.GetString(0));
        Assert.Equal("0001234567", reader.GetString(1));
        Assert.Equal("0007654321", reader.GetString(2));
    }

    private async Task<string> ProvisionAsync()
    {
        var schemaName = $"jv_{Guid.NewGuid():N}";
        await new PostgresSchemaProvisioner(_fixture.DataSource, schemaName)
            .EnsureCreatedAsync(CancellationToken.None);
        return schemaName;
    }

    private static async IAsyncEnumerable<Hn> ToAsync(IEnumerable<Hn> source)
    {
        foreach (var item in source)
        {
            yield return item;
            await Task.Yield();
        }
    }
}
