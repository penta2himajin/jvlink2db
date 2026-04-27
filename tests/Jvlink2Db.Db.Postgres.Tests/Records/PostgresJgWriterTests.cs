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
public sealed class PostgresJgWriterTests
{
    private readonly PostgresFixture _fixture;

    public PostgresJgWriterTests(PostgresFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task WriteAsync_inserts_and_round_trips()
    {
        var schemaName = await ProvisionAsync();
        var sut = new PostgresJgWriter(_fixture.DataSource, schemaName);

        await sut.WriteAsync(ToAsync(new[]
        {
            JgBuilder.Sample(raceNum: "11", kettoNum: "2020100001", bamei: "除外Ａ"),
            JgBuilder.Sample(raceNum: "11", kettoNum: "2020100002", bamei: "除外Ｂ"),
        }), CancellationToken.None);

        await using var conn = await _fixture.DataSource.OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand(
            $@"SELECT count(*), max(bamei), max(shutsuba_tohyo_jun)
               FROM ""{schemaName}"".jg WHERE race_num = '11'",
            conn);
        await using var reader = await cmd.ExecuteReaderAsync();
        Assert.True(await reader.ReadAsync());

        Assert.Equal(2L, reader.GetFieldValue<long>(0));
        Assert.Equal("除外Ｂ", reader.GetString(1));
        Assert.Equal((short)2, reader.GetFieldValue<short>(2));
    }

    private async Task<string> ProvisionAsync()
    {
        var schemaName = $"jv_{Guid.NewGuid():N}";
        await new PostgresSchemaProvisioner(_fixture.DataSource, schemaName)
            .EnsureCreatedAsync(CancellationToken.None);
        return schemaName;
    }

    private static async IAsyncEnumerable<Jg> ToAsync(IEnumerable<Jg> source)
    {
        foreach (var item in source)
        {
            yield return item;
            await Task.Yield();
        }
    }
}
