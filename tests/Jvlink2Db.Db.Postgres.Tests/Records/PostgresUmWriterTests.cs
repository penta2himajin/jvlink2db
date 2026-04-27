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
public sealed class PostgresUmWriterTests
{
    private readonly PostgresFixture _fixture;

    public PostgresUmWriterTests(PostgresFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task WriteAsync_persists_horse_master_with_chaku_arrays()
    {
        var schemaName = await ProvisionAsync();
        var sut = new PostgresUmWriter(_fixture.DataSource, schemaName);

        await sut.WriteAsync(ToAsync(new[] { UmBuilder.Sample() }), CancellationToken.None);

        await using var conn = await _fixture.DataSource.OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand(
            $@"SELECT bamei, birth_date, ruikei_honsyo_heiti,
                      array_length(ketto_hansyoku_num, 1),
                      array_length(chaku_kaisu_ba, 1),
                      array_length(chaku_kaisu_jyotai, 1),
                      array_length(chaku_kaisu_kyori, 1)
               FROM ""{schemaName}"".um",
            conn);
        await using var reader = await cmd.ExecuteReaderAsync();
        Assert.True(await reader.ReadAsync());
        Assert.Equal("テストホース", reader.GetString(0));
        Assert.Equal(new DateOnly(2020, 3, 15), reader.GetFieldValue<DateOnly>(1));
        Assert.Equal(100000000L, reader.GetFieldValue<long>(2));
        Assert.Equal(14, reader.GetFieldValue<int>(3));
        Assert.Equal(42, reader.GetFieldValue<int>(4));
        Assert.Equal(72, reader.GetFieldValue<int>(5));
        Assert.Equal(36, reader.GetFieldValue<int>(6));
    }

    private async Task<string> ProvisionAsync()
    {
        var schemaName = $"jv_{Guid.NewGuid():N}";
        await new PostgresSchemaProvisioner(_fixture.DataSource, schemaName)
            .EnsureCreatedAsync(CancellationToken.None);
        return schemaName;
    }

    private static async IAsyncEnumerable<Um> ToAsync(IEnumerable<Um> source)
    {
        foreach (var item in source)
        {
            yield return item;
            await Task.Yield();
        }
    }
}
