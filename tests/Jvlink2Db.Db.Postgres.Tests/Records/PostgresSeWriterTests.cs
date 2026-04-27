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
public sealed class PostgresSeWriterTests
{
    private readonly PostgresFixture _fixture;

    public PostgresSeWriterTests(PostgresFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task WriteAsync_inserts_records_with_typed_columns()
    {
        var schemaName = await ProvisionAsync();
        var sut = new PostgresSeWriter(_fixture.DataSource, schemaName);

        var records = new[]
        {
            SeBuilder.Sample(raceNum: "11", umaban: "05", bamei: "テストホース", time: "1325"),
            SeBuilder.Sample(raceNum: "11", umaban: "06", bamei: "セカンドホース", time: "1326"),
        };

        var rows = await sut.WriteAsync(ToAsync(records), CancellationToken.None);

        Assert.Equal(2, rows);

        await using var conn = await _fixture.DataSource.OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand(
            $@"SELECT bamei, ""time"", barei, kakutei_jyuni, honsyokin, chaku_uma_1_ketto_num
               FROM ""{schemaName}"".se WHERE umaban = '05'",
            conn);
        await using var reader = await cmd.ExecuteReaderAsync();
        Assert.True(await reader.ReadAsync());
        Assert.Equal("テストホース", reader.GetString(0));
        Assert.Equal((short)1325, reader.GetFieldValue<short>(1));
        Assert.Equal((short)4, reader.GetFieldValue<short>(2));
        Assert.Equal((short)1, reader.GetFieldValue<short>(3));
        Assert.Equal(40000, reader.GetFieldValue<int>(4));
        Assert.Equal("2020100001", reader.GetString(5));
    }

    [Fact]
    public async Task WriteAsync_is_idempotent_and_updates_on_conflict()
    {
        var schemaName = await ProvisionAsync();
        var sut = new PostgresSeWriter(_fixture.DataSource, schemaName);

        var first = SeBuilder.Sample(raceNum: "11", umaban: "05", bamei: "Old", time: "1300");
        var updated = SeBuilder.Sample(raceNum: "11", umaban: "05", bamei: "New", time: "1325");

        await sut.WriteAsync(ToAsync(new[] { first }), CancellationToken.None);
        await sut.WriteAsync(ToAsync(new[] { updated }), CancellationToken.None);

        await using var conn = await _fixture.DataSource.OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand(
            $@"SELECT count(*), max(bamei), max(""time"")
               FROM ""{schemaName}"".se WHERE umaban = '05'",
            conn);
        await using var reader = await cmd.ExecuteReaderAsync();
        Assert.True(await reader.ReadAsync());
        Assert.Equal(1L, reader.GetFieldValue<long>(0));
        Assert.Equal("New", reader.GetString(1));
        Assert.Equal((short)1325, reader.GetFieldValue<short>(2));
    }

    private async Task<string> ProvisionAsync()
    {
        var schemaName = $"jv_{Guid.NewGuid():N}";
        await new PostgresSchemaProvisioner(_fixture.DataSource, schemaName)
            .EnsureCreatedAsync(CancellationToken.None);
        return schemaName;
    }

    private static async IAsyncEnumerable<Se> ToAsync(IEnumerable<Se> source)
    {
        foreach (var item in source)
        {
            yield return item;
            await Task.Yield();
        }
    }
}
