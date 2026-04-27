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
public sealed class PostgresChWriterTests
{
    private readonly PostgresFixture _fixture;

    public PostgresChWriterTests(PostgresFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task WriteAsync_persists_trainer_master_with_history_arrays()
    {
        var schemaName = await ProvisionAsync();
        var sut = new PostgresChWriter(_fixture.DataSource, schemaName);

        await sut.WriteAsync(ToAsync(new[] { ChBuilder.Sample() }), CancellationToken.None);

        await using var conn = await _fixture.DataSource.OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand(
            $@"SELECT chokyosi_name, birth_date, array_length(saikin_jyusyo_year, 1)
               FROM ""{schemaName}"".ch",
            conn);
        await using var reader = await cmd.ExecuteReaderAsync();
        Assert.True(await reader.ReadAsync());
        Assert.Equal("テスト調教師", reader.GetString(0));
        Assert.Equal(new DateOnly(1970, 1, 1), reader.GetFieldValue<DateOnly>(1));
        Assert.Equal(3, reader.GetFieldValue<int>(2));
    }

    private async Task<string> ProvisionAsync()
    {
        var schemaName = $"jv_{Guid.NewGuid():N}";
        await new PostgresSchemaProvisioner(_fixture.DataSource, schemaName)
            .EnsureCreatedAsync(CancellationToken.None);
        return schemaName;
    }

    private static async IAsyncEnumerable<Ch> ToAsync(IEnumerable<Ch> source)
    {
        foreach (var item in source)
        {
            yield return item;
            await Task.Yield();
        }
    }
}
