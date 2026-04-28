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
public sealed class PostgresCsWriterTests
{
    private readonly PostgresFixture _fixture;

    public PostgresCsWriterTests(PostgresFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task WriteAsync_persists_course_record()
    {
        var schemaName = await ProvisionAsync();
        var sut = new PostgresCsWriter(_fixture.DataSource, schemaName);

        await sut.WriteAsync(ToAsync(new[] { CsBuilder.Sample() }), CancellationToken.None);

        await using var conn = await _fixture.DataSource.OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand(
            $@"SELECT jyo_cd, kyori, track_cd, kaishu_date FROM ""{schemaName}"".cs",
            conn);
        await using var reader = await cmd.ExecuteReaderAsync();
        Assert.True(await reader.ReadAsync());
        Assert.Equal("05", reader.GetString(0));
        Assert.Equal("3200", reader.GetString(1));
        Assert.Equal("10", reader.GetString(2));
        Assert.Equal(new DateOnly(2024, 1, 1), reader.GetFieldValue<DateOnly>(3));
    }

    private async Task<string> ProvisionAsync()
    {
        var schemaName = $"jv_{Guid.NewGuid():N}";
        await new PostgresSchemaProvisioner(_fixture.DataSource, schemaName)
            .EnsureCreatedAsync(CancellationToken.None);
        return schemaName;
    }

    private static async IAsyncEnumerable<Cs> ToAsync(IEnumerable<Cs> source)
    {
        foreach (var item in source)
        {
            yield return item;
            await Task.Yield();
        }
    }
}
