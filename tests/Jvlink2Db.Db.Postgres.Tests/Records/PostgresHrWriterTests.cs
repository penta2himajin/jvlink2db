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
public sealed class PostgresHrWriterTests
{
    private readonly PostgresFixture _fixture;

    public PostgresHrWriterTests(PostgresFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task WriteAsync_persists_typed_arrays()
    {
        var schemaName = await ProvisionAsync();
        var sut = new PostgresHrWriter(_fixture.DataSource, schemaName);

        var record = HrBuilder.Sample(raceNum: "11", tansyoPay: "000000345");

        await sut.WriteAsync(ToAsync(new[] { record }), CancellationToken.None);

        await using var conn = await _fixture.DataSource.OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand(
            $@"SELECT toroku_tosu, pay_tansyo_umaban, pay_tansyo_pay, pay_tansyo_ninki
               FROM ""{schemaName}"".hr WHERE race_num = '11'",
            conn);
        await using var reader = await cmd.ExecuteReaderAsync();
        Assert.True(await reader.ReadAsync());

        Assert.Equal((short)16, reader.GetFieldValue<short>(0));

        var umaban = reader.GetFieldValue<string?[]>(1);
        Assert.Equal(3, umaban.Length);
        Assert.Equal("05", umaban[0]);
        Assert.Null(umaban[1]);

        var pay = reader.GetFieldValue<int?[]>(2);
        Assert.Equal(345, pay[0]);
        Assert.Null(pay[1]);

        var ninki = reader.GetFieldValue<short?[]>(3);
        Assert.Equal((short)1, ninki[0]);
        Assert.Null(ninki[1]);
    }

    [Fact]
    public async Task WriteAsync_is_idempotent_on_re_insert()
    {
        var schemaName = await ProvisionAsync();
        var sut = new PostgresHrWriter(_fixture.DataSource, schemaName);

        var record = HrBuilder.Sample(raceNum: "11");

        await sut.WriteAsync(ToAsync(new[] { record }), CancellationToken.None);
        await sut.WriteAsync(ToAsync(new[] { record }), CancellationToken.None);

        await using var conn = await _fixture.DataSource.OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand(
            $@"SELECT count(*) FROM ""{schemaName}"".hr",
            conn);
        Assert.Equal(1L, Convert.ToInt64(await cmd.ExecuteScalarAsync()));
    }

    private async Task<string> ProvisionAsync()
    {
        var schemaName = $"jv_{Guid.NewGuid():N}";
        await new PostgresSchemaProvisioner(_fixture.DataSource, schemaName)
            .EnsureCreatedAsync(CancellationToken.None);
        return schemaName;
    }

    private static async IAsyncEnumerable<Hr> ToAsync(IEnumerable<Hr> source)
    {
        foreach (var item in source)
        {
            yield return item;
            await Task.Yield();
        }
    }
}
