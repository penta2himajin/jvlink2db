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
public sealed class PostgresO1WriterTests
{
    private readonly PostgresFixture _fixture;

    public PostgresO1WriterTests(PostgresFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task WriteAsync_persists_typed_columns()
    {
        var schemaName = await ProvisionAsync();
        var sut = new PostgresO1Writer(_fixture.DataSource, schemaName);

        var record = O1Builder.Sample(raceNum: "11", happyoTm: "03311534");

        await sut.WriteAsync(ToAsync(new[] { record }), CancellationToken.None);

        await using var conn = await _fixture.DataSource.OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand(
            $@"SELECT happyo_tm, toroku_tosu, total_hyosu_tansyo,
                      array_length(tansyo_umaban, 1), array_length(wakuren_kumi, 1)
               FROM ""{schemaName}"".o1",
            conn);
        await using var reader = await cmd.ExecuteReaderAsync();
        Assert.True(await reader.ReadAsync());
        Assert.Equal("03311534", reader.GetString(0));
        Assert.Equal((short)16, reader.GetFieldValue<short>(1));
        Assert.Equal(1000000000L, reader.GetFieldValue<long>(2));
        Assert.Equal(28, reader.GetFieldValue<int>(3));
        Assert.Equal(36, reader.GetFieldValue<int>(4));
    }

    [Fact]
    public async Task WriteAsync_uses_happyo_tm_as_part_of_pk()
    {
        var schemaName = await ProvisionAsync();
        var sut = new PostgresO1Writer(_fixture.DataSource, schemaName);

        // Same race, two different announcements: PK distinguishes them.
        var first = O1Builder.Sample(raceNum: "11", happyoTm: "03311500");
        var second = O1Builder.Sample(raceNum: "11", happyoTm: "03311534");

        await sut.WriteAsync(ToAsync(new[] { first, second }), CancellationToken.None);

        await using var conn = await _fixture.DataSource.OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand(
            $@"SELECT count(*) FROM ""{schemaName}"".o1 WHERE race_num = '11'",
            conn);
        Assert.Equal(2L, Convert.ToInt64(await cmd.ExecuteScalarAsync()));
    }

    private async Task<string> ProvisionAsync()
    {
        var schemaName = $"jv_{Guid.NewGuid():N}";
        await new PostgresSchemaProvisioner(_fixture.DataSource, schemaName)
            .EnsureCreatedAsync(CancellationToken.None);
        return schemaName;
    }

    private static async IAsyncEnumerable<O1> ToAsync(IEnumerable<O1> source)
    {
        foreach (var item in source)
        {
            yield return item;
            await Task.Yield();
        }
    }
}
