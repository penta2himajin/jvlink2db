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
public sealed class PostgresTkWriterTests
{
    private readonly PostgresFixture _fixture;

    public PostgresTkWriterTests(PostgresFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task WriteAsync_persists_special_race_entries()
    {
        var schemaName = await ProvisionAsync();
        var sut = new PostgresTkWriter(_fixture.DataSource, schemaName);

        await sut.WriteAsync(ToAsync(new[] { TkBuilder.Sample() }), CancellationToken.None);

        await using var conn = await _fixture.DataSource.OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand(
            $@"SELECT hondai, grade_cd, kyori, toroku_tosu,
                      array_length(ketto_num, 1), ketto_num[1], bamei[1], futan[300]
               FROM ""{schemaName}"".tk",
            conn);
        await using var reader = await cmd.ExecuteReaderAsync();
        Assert.True(await reader.ReadAsync());
        Assert.Equal("天皇賞(春)", reader.GetString(0));
        Assert.Equal("A", reader.GetString(1));
        Assert.Equal("3200", reader.GetString(2));
        Assert.Equal("018", reader.GetString(3));
        Assert.Equal(300, reader.GetFieldValue<int>(4));
        Assert.Equal("2020100001", reader.GetString(5));
        Assert.Equal("テスト馬1", reader.GetString(6));
        Assert.Equal("580", reader.GetString(7));
    }

    private async Task<string> ProvisionAsync()
    {
        var schemaName = $"jv_{Guid.NewGuid():N}";
        await new PostgresSchemaProvisioner(_fixture.DataSource, schemaName)
            .EnsureCreatedAsync(CancellationToken.None);
        return schemaName;
    }

    private static async IAsyncEnumerable<Tk> ToAsync(IEnumerable<Tk> source)
    {
        foreach (var item in source)
        {
            yield return item;
            await Task.Yield();
        }
    }
}
