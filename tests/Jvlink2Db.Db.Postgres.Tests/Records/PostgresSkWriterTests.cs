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
public sealed class PostgresSkWriterTests
{
    private readonly PostgresFixture _fixture;

    public PostgresSkWriterTests(PostgresFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task WriteAsync_persists_progeny_with_pedigree()
    {
        var schemaName = await ProvisionAsync();
        var sut = new PostgresSkWriter(_fixture.DataSource, schemaName);

        await sut.WriteAsync(ToAsync(new[] { SkBuilder.Sample() }), CancellationToken.None);

        await using var conn = await _fixture.DataSource.OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand(
            $@"SELECT birth_date, array_length(hansyoku_num, 1), hansyoku_num[1]
               FROM ""{schemaName}"".sk",
            conn);
        await using var reader = await cmd.ExecuteReaderAsync();
        Assert.True(await reader.ReadAsync());
        Assert.Equal(new DateOnly(2020, 3, 15), reader.GetFieldValue<DateOnly>(0));
        Assert.Equal(14, reader.GetFieldValue<int>(1));
        Assert.Equal("0001234567", reader.GetString(2));
    }

    private async Task<string> ProvisionAsync()
    {
        var schemaName = $"jv_{Guid.NewGuid():N}";
        await new PostgresSchemaProvisioner(_fixture.DataSource, schemaName)
            .EnsureCreatedAsync(CancellationToken.None);
        return schemaName;
    }

    private static async IAsyncEnumerable<Sk> ToAsync(IEnumerable<Sk> source)
    {
        foreach (var item in source)
        {
            yield return item;
            await Task.Yield();
        }
    }
}
