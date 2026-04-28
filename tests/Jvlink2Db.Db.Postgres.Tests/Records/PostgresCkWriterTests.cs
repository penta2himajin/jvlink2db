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
public sealed class PostgresCkWriterTests
{
    private readonly PostgresFixture _fixture;

    public PostgresCkWriterTests(PostgresFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task WriteAsync_persists_chaku_record()
    {
        var schemaName = await ProvisionAsync();
        var sut = new PostgresCkWriter(_fixture.DataSource, schemaName);

        await sut.WriteAsync(ToAsync(new[] { CkBuilder.Sample() }), CancellationToken.None);

        await using var conn = await _fixture.DataSource.OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand(
            $@"SELECT bamei, ruikei_honsyo_heiti, race_count,
                      kisyu_code, chokyosi_code, banusi_code, breeder_code,
                      array_length(kyakusitu, 1), kyakusitu[1]
               FROM ""{schemaName}"".ck",
            conn);
        await using var reader = await cmd.ExecuteReaderAsync();
        Assert.True(await reader.ReadAsync());
        Assert.Equal("テスト馬", reader.GetString(0));
        Assert.Equal("000123450", reader.GetString(1));
        Assert.Equal("042", reader.GetString(2));
        Assert.Equal("00001", reader.GetString(3));
        Assert.Equal("00099", reader.GetString(4));
        Assert.Equal("000123", reader.GetString(5));
        Assert.Equal("00012345", reader.GetString(6));
        Assert.Equal(4, reader.GetFieldValue<int>(7));
        Assert.Equal("050", reader.GetString(8));
    }

    private async Task<string> ProvisionAsync()
    {
        var schemaName = $"jv_{Guid.NewGuid():N}";
        await new PostgresSchemaProvisioner(_fixture.DataSource, schemaName)
            .EnsureCreatedAsync(CancellationToken.None);
        return schemaName;
    }

    private static async IAsyncEnumerable<Ck> ToAsync(IEnumerable<Ck> source)
    {
        foreach (var item in source)
        {
            yield return item;
            await Task.Yield();
        }
    }
}
