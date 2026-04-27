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
public sealed class PostgresRaWriterTests
{
    private readonly PostgresFixture _fixture;

    public PostgresRaWriterTests(PostgresFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task WriteAsync_inserts_records()
    {
        var schemaName = await ProvisionAsync();
        var sut = new PostgresRaWriter(_fixture.DataSource, schemaName);

        var records = new[]
        {
            RaBuilder.Sample(raceNum: "11", hondai: "First"),
            RaBuilder.Sample(raceNum: "12", hondai: "Second"),
        };

        var rows = await sut.WriteAsync(ToAsync(records), CancellationToken.None);

        Assert.Equal(2, rows);
        Assert.Equal(2, await CountRows(schemaName));
    }

    [Fact]
    public async Task WriteAsync_persists_typed_columns()
    {
        var schemaName = await ProvisionAsync();
        var sut = new PostgresRaWriter(_fixture.DataSource, schemaName);

        var record = RaBuilder.Sample(raceNum: "11", hondai: "有馬記念", kyori: "2500");

        await sut.WriteAsync(ToAsync(new[] { record }), CancellationToken.None);

        await using var conn = await _fixture.DataSource.OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand(
            $@"SELECT hondai, kyori, make_date, toroku_tosu, honsyokin
               FROM ""{schemaName}"".ra
               WHERE race_num = '11'",
            conn);
        await using var reader = await cmd.ExecuteReaderAsync();
        Assert.True(await reader.ReadAsync());

        Assert.Equal("有馬記念", reader.GetString(0));
        Assert.Equal((short)2500, reader.GetFieldValue<short>(1));
        Assert.Equal(new DateOnly(2026, 3, 31), reader.GetFieldValue<DateOnly>(2));
        Assert.Equal((short)16, reader.GetFieldValue<short>(3));
        var honsyokin = reader.GetFieldValue<int?[]>(4);
        Assert.Equal(7, honsyokin.Length);
        Assert.Equal(100000, honsyokin[0]);
        Assert.Equal(40000, honsyokin[1]);
        Assert.Null(honsyokin[5]);
        Assert.Null(honsyokin[6]);
    }

    [Fact]
    public async Task WriteAsync_is_idempotent_for_same_records()
    {
        var schemaName = await ProvisionAsync();
        var sut = new PostgresRaWriter(_fixture.DataSource, schemaName);
        var record = RaBuilder.Sample(raceNum: "11", hondai: "Same");

        await sut.WriteAsync(ToAsync(new[] { record }), CancellationToken.None);
        await sut.WriteAsync(ToAsync(new[] { record }), CancellationToken.None);

        Assert.Equal(1, await CountRows(schemaName));
    }

    [Fact]
    public async Task WriteAsync_updates_non_pk_fields_on_conflict()
    {
        var schemaName = await ProvisionAsync();
        var sut = new PostgresRaWriter(_fixture.DataSource, schemaName);

        var first = RaBuilder.Sample(raceNum: "11", hondai: "Old", kyori: "1600");
        var updated = RaBuilder.Sample(raceNum: "11", hondai: "New", kyori: "2000");

        await sut.WriteAsync(ToAsync(new[] { first }), CancellationToken.None);
        await sut.WriteAsync(ToAsync(new[] { updated }), CancellationToken.None);

        var (hondai, kyori) = await GetHondaiAndKyori(schemaName, "11");
        Assert.Equal("New", hondai);
        Assert.Equal((short)2000, kyori);
        Assert.Equal(1, await CountRows(schemaName));
    }

    [Fact]
    public async Task WriteAsync_dedupes_within_batch_keeping_last_emitted()
    {
        var schemaName = await ProvisionAsync();
        var sut = new PostgresRaWriter(_fixture.DataSource, schemaName);

        var records = new[]
        {
            RaBuilder.Sample(raceNum: "11", hondai: "First"),
            RaBuilder.Sample(raceNum: "11", hondai: "Last"),
        };

        await sut.WriteAsync(ToAsync(records), CancellationToken.None);

        Assert.Equal(1, await CountRows(schemaName));
        var (hondai, _) = await GetHondaiAndKyori(schemaName, "11");
        Assert.Equal("Last", hondai);
    }

    private async Task<string> ProvisionAsync()
    {
        var schemaName = $"jv_{Guid.NewGuid():N}";
        await new PostgresSchemaProvisioner(_fixture.DataSource, schemaName)
            .EnsureCreatedAsync(CancellationToken.None);
        return schemaName;
    }

    private async Task<long> CountRows(string schemaName)
    {
        await using var conn = await _fixture.DataSource.OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand($@"SELECT COUNT(*) FROM ""{schemaName}"".ra", conn);
        var result = await cmd.ExecuteScalarAsync();
        return Convert.ToInt64(result);
    }

    private async Task<(string Hondai, short Kyori)> GetHondaiAndKyori(string schemaName, string raceNum)
    {
        await using var conn = await _fixture.DataSource.OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand(
            $@"SELECT hondai, kyori FROM ""{schemaName}"".ra WHERE race_num = @r", conn);
        cmd.Parameters.AddWithValue("r", raceNum);
        await using var reader = await cmd.ExecuteReaderAsync();
        Assert.True(await reader.ReadAsync());
        return (reader.GetString(0), reader.GetFieldValue<short>(1));
    }

    private static async IAsyncEnumerable<Ra> ToAsync(IEnumerable<Ra> source)
    {
        foreach (var item in source)
        {
            yield return item;
            await Task.Yield();
        }
    }
}
