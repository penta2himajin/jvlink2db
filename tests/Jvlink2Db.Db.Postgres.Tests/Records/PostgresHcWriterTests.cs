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
public sealed class PostgresHcWriterTests
{
    private readonly PostgresFixture _fixture;

    public PostgresHcWriterTests(PostgresFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task WriteAsync_persists_slope_training_record()
    {
        var schemaName = await ProvisionAsync();
        var sut = new PostgresHcWriter(_fixture.DataSource, schemaName);

        await sut.WriteAsync(ToAsync(new[] { HcBuilder.Sample() }), CancellationToken.None);

        await using var conn = await _fixture.DataSource.OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand(
            $@"SELECT chokyo_date, ketto_num, haron_time_4, lap_time_1 FROM ""{schemaName}"".hc",
            conn);
        await using var reader = await cmd.ExecuteReaderAsync();
        Assert.True(await reader.ReadAsync());
        Assert.Equal(new DateOnly(2026, 4, 15), reader.GetFieldValue<DateOnly>(0));
        Assert.Equal("2020100123", reader.GetString(1));
        Assert.Equal((short)567, reader.GetFieldValue<short>(2));
        Assert.Equal((short)130, reader.GetFieldValue<short>(3));
    }

    private async Task<string> ProvisionAsync()
    {
        var schemaName = $"jv_{Guid.NewGuid():N}";
        await new PostgresSchemaProvisioner(_fixture.DataSource, schemaName)
            .EnsureCreatedAsync(CancellationToken.None);
        return schemaName;
    }

    private static async IAsyncEnumerable<Hc> ToAsync(IEnumerable<Hc> source)
    {
        foreach (var item in source)
        {
            yield return item;
            await Task.Yield();
        }
    }
}
