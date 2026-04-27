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
public sealed class PostgresH1WriterTests
{
    private readonly PostgresFixture _fixture;

    public PostgresH1WriterTests(PostgresFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task WriteAsync_persists_all_pay_arrays_with_expected_sizes()
    {
        var schemaName = await ProvisionAsync();
        var sut = new PostgresH1Writer(_fixture.DataSource, schemaName);

        await sut.WriteAsync(ToAsync(new[] { H1Builder.Sample() }), CancellationToken.None);

        await using var conn = await _fixture.DataSource.OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand(
            $@"SELECT array_length(tansyo_umaban, 1), array_length(wakuren_kumi, 1),
                      array_length(umaren_kumi, 1), array_length(umatan_kumi, 1),
                      array_length(sanrenpuku_kumi, 1), array_length(hyo_total, 1)
               FROM ""{schemaName}"".h1",
            conn);
        await using var reader = await cmd.ExecuteReaderAsync();
        Assert.True(await reader.ReadAsync());
        Assert.Equal(28, reader.GetFieldValue<int>(0));
        Assert.Equal(36, reader.GetFieldValue<int>(1));
        Assert.Equal(153, reader.GetFieldValue<int>(2));
        Assert.Equal(306, reader.GetFieldValue<int>(3));
        Assert.Equal(816, reader.GetFieldValue<int>(4));
        Assert.Equal(14, reader.GetFieldValue<int>(5));
    }

    private async Task<string> ProvisionAsync()
    {
        var schemaName = $"jv_{Guid.NewGuid():N}";
        await new PostgresSchemaProvisioner(_fixture.DataSource, schemaName)
            .EnsureCreatedAsync(CancellationToken.None);
        return schemaName;
    }

    private static async IAsyncEnumerable<H1> ToAsync(IEnumerable<H1> source)
    {
        foreach (var item in source)
        {
            yield return item;
            await Task.Yield();
        }
    }
}
