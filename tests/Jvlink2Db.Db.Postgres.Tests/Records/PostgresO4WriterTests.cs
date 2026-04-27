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
public sealed class PostgresO4WriterTests
{
    private readonly PostgresFixture _fixture;

    public PostgresO4WriterTests(PostgresFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task WriteAsync_persists_306_entry_arrays()
    {
        var schemaName = await ProvisionAsync();
        var sut = new PostgresO4Writer(_fixture.DataSource, schemaName);

        await sut.WriteAsync(ToAsync(new[] { O4Builder.Sample() }), CancellationToken.None);

        await using var conn = await _fixture.DataSource.OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand(
            $@"SELECT array_length(kumi, 1), total_hyosu_umatan FROM ""{schemaName}"".o4",
            conn);
        await using var reader = await cmd.ExecuteReaderAsync();
        Assert.True(await reader.ReadAsync());
        Assert.Equal(306, reader.GetFieldValue<int>(0));
        Assert.Equal(800000000L, reader.GetFieldValue<long>(1));
    }

    private async Task<string> ProvisionAsync()
    {
        var schemaName = $"jv_{Guid.NewGuid():N}";
        await new PostgresSchemaProvisioner(_fixture.DataSource, schemaName)
            .EnsureCreatedAsync(CancellationToken.None);
        return schemaName;
    }

    private static async IAsyncEnumerable<O4> ToAsync(IEnumerable<O4> source)
    {
        foreach (var item in source)
        {
            yield return item;
            await Task.Yield();
        }
    }
}
