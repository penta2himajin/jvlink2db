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
public sealed class PostgresRcWriterTests
{
    private readonly PostgresFixture _fixture;

    public PostgresRcWriterTests(PostgresFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task WriteAsync_persists_record_with_three_uma_entries()
    {
        var schemaName = await ProvisionAsync();
        var sut = new PostgresRcWriter(_fixture.DataSource, schemaName);

        await sut.WriteAsync(ToAsync(new[] { RcBuilder.Sample() }), CancellationToken.None);

        await using var conn = await _fixture.DataSource.OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand(
            $@"SELECT hondai, kyori, rec_time, array_length(rec_uma_ketto_num, 1), rec_uma_futan[1]
               FROM ""{schemaName}"".rc",
            conn);
        await using var reader = await cmd.ExecuteReaderAsync();
        Assert.True(await reader.ReadAsync());
        Assert.Equal("有馬記念", reader.GetString(0));
        Assert.Equal((short)2500, reader.GetFieldValue<short>(1));
        Assert.Equal((short)1485, reader.GetFieldValue<short>(2));
        Assert.Equal(3, reader.GetFieldValue<int>(3));
        Assert.Equal((short)570, reader.GetFieldValue<short>(4));
    }

    private async Task<string> ProvisionAsync()
    {
        var schemaName = $"jv_{Guid.NewGuid():N}";
        await new PostgresSchemaProvisioner(_fixture.DataSource, schemaName)
            .EnsureCreatedAsync(CancellationToken.None);
        return schemaName;
    }

    private static async IAsyncEnumerable<Rc> ToAsync(IEnumerable<Rc> source)
    {
        foreach (var item in source)
        {
            yield return item;
            await Task.Yield();
        }
    }
}
