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
public sealed class PostgresBrWriterTests
{
    private readonly PostgresFixture _fixture;

    public PostgresBrWriterTests(PostgresFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task WriteAsync_persists_breeder_with_typed_arrays()
    {
        var schemaName = await ProvisionAsync();
        var sut = new PostgresBrWriter(_fixture.DataSource, schemaName);

        await sut.WriteAsync(ToAsync(new[] { BrBuilder.Sample() }), CancellationToken.None);

        await using var conn = await _fixture.DataSource.OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand(
            $@"SELECT breeder_name, address, hon_ruikei_set_year, hon_ruikei_honsyokin_total[1],
                      array_length(hon_ruikei_chaku_kaisu, 1)
               FROM ""{schemaName}"".br",
            conn);
        await using var reader = await cmd.ExecuteReaderAsync();
        Assert.True(await reader.ReadAsync());
        Assert.Equal("テスト牧場", reader.GetString(0));
        Assert.Equal("北海道", reader.GetString(1));
        Assert.Equal(new[] { "2025", "9999" }, reader.GetFieldValue<string?[]>(2));
        Assert.Equal(1000000L, reader.GetFieldValue<long>(3));
        Assert.Equal(12, reader.GetFieldValue<int>(4));
    }

    private async Task<string> ProvisionAsync()
    {
        var schemaName = $"jv_{Guid.NewGuid():N}";
        await new PostgresSchemaProvisioner(_fixture.DataSource, schemaName)
            .EnsureCreatedAsync(CancellationToken.None);
        return schemaName;
    }

    private static async IAsyncEnumerable<Br> ToAsync(IEnumerable<Br> source)
    {
        foreach (var item in source)
        {
            yield return item;
            await Task.Yield();
        }
    }
}
