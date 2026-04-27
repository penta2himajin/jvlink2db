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
public sealed class PostgresWfWriterTests
{
    private readonly PostgresFixture _fixture;

    public PostgresWfWriterTests(PostgresFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task WriteAsync_persists_pay_arrays_of_243_with_typed_columns()
    {
        var schemaName = await ProvisionAsync();
        var sut = new PostgresWfWriter(_fixture.DataSource, schemaName);

        var record = WfBuilder.Sample(kaisaiDate: "20260328", hatsubaiHyo: "01234567890");

        await sut.WriteAsync(ToAsync(new[] { record }), CancellationToken.None);

        await using var conn = await _fixture.DataSource.OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand(
            $@"SELECT kaisai_date, hatsubai_hyo, array_length(pay_kumiban, 1),
                      pay_kumiban[1], pay_amount[1], pay_tekichu_hyo[1],
                      pay_kumiban[243], pay_amount[243], pay_tekichu_hyo[243]
               FROM ""{schemaName}"".wf",
            conn);
        await using var reader = await cmd.ExecuteReaderAsync();
        Assert.True(await reader.ReadAsync());

        Assert.Equal(new DateOnly(2026, 3, 28), reader.GetFieldValue<DateOnly>(0));
        Assert.Equal(1234567890L, reader.GetFieldValue<long>(1));
        Assert.Equal(243, reader.GetFieldValue<int>(2));

        Assert.Equal("0102030405", reader.GetString(3));
        Assert.Equal(999000, reader.GetFieldValue<int>(4));
        Assert.Equal(10L, reader.GetFieldValue<long>(5));

        Assert.Equal("0708090101", reader.GetString(6));
        Assert.Equal(50000, reader.GetFieldValue<int>(7));
        Assert.Equal(20L, reader.GetFieldValue<long>(8));
    }

    [Fact]
    public async Task WriteAsync_is_idempotent_on_re_insert()
    {
        var schemaName = await ProvisionAsync();
        var sut = new PostgresWfWriter(_fixture.DataSource, schemaName);

        var record = WfBuilder.Sample(kaisaiDate: "20260328");

        await sut.WriteAsync(ToAsync(new[] { record }), CancellationToken.None);
        await sut.WriteAsync(ToAsync(new[] { record }), CancellationToken.None);

        await using var conn = await _fixture.DataSource.OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand($@"SELECT count(*) FROM ""{schemaName}"".wf", conn);
        Assert.Equal(1L, Convert.ToInt64(await cmd.ExecuteScalarAsync()));
    }

    private async Task<string> ProvisionAsync()
    {
        var schemaName = $"jv_{Guid.NewGuid():N}";
        await new PostgresSchemaProvisioner(_fixture.DataSource, schemaName)
            .EnsureCreatedAsync(CancellationToken.None);
        return schemaName;
    }

    private static async IAsyncEnumerable<Wf> ToAsync(IEnumerable<Wf> source)
    {
        foreach (var item in source)
        {
            yield return item;
            await Task.Yield();
        }
    }
}
