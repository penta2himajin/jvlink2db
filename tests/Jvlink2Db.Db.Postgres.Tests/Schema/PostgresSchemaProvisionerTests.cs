using System.Threading;
using System.Threading.Tasks;
using Jvlink2Db.Db.Postgres.Schema;
using Jvlink2Db.Db.Postgres.Tests.Fixtures;
using Npgsql;
using Xunit;

namespace Jvlink2Db.Db.Postgres.Tests.Schema;

[Collection(PostgresCollection.Name)]
[Trait("Category", "Database")]
public sealed class PostgresSchemaProvisionerTests
{
    private readonly PostgresFixture _fixture;

    public PostgresSchemaProvisionerTests(PostgresFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task EnsureCreatedAsync_creates_schema_and_ra_table()
    {
        var schemaName = NewSchemaName();
        var sut = new PostgresSchemaProvisioner(_fixture.DataSource, schemaName);

        await sut.EnsureCreatedAsync(CancellationToken.None);

        Assert.True(await SchemaExists(schemaName));
        Assert.True(await TableExists(schemaName, "ra"));
    }

    [Fact]
    public async Task EnsureCreatedAsync_is_idempotent()
    {
        var schemaName = NewSchemaName();
        var sut = new PostgresSchemaProvisioner(_fixture.DataSource, schemaName);

        await sut.EnsureCreatedAsync(CancellationToken.None);
        // second invocation must not throw and must leave the schema unchanged
        await sut.EnsureCreatedAsync(CancellationToken.None);

        Assert.True(await TableExists(schemaName, "ra"));
    }

    [Fact]
    public async Task EnsureCreatedAsync_creates_expected_pk_columns()
    {
        var schemaName = NewSchemaName();
        var sut = new PostgresSchemaProvisioner(_fixture.DataSource, schemaName);

        await sut.EnsureCreatedAsync(CancellationToken.None);

        var pkCols = await PrimaryKeyColumns(schemaName, "ra");
        Assert.Equal(new[] { "year", "month_day", "jyo_cd", "kaiji", "nichiji", "race_num" }, pkCols);
    }

    private static string NewSchemaName() => $"jv_{System.Guid.NewGuid():N}";

    private async Task<bool> SchemaExists(string schemaName)
    {
        await using var conn = await _fixture.DataSource.OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand("SELECT 1 FROM information_schema.schemata WHERE schema_name = @s", conn);
        cmd.Parameters.AddWithValue("s", schemaName);
        return await cmd.ExecuteScalarAsync() is not null;
    }

    private async Task<bool> TableExists(string schemaName, string tableName)
    {
        await using var conn = await _fixture.DataSource.OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand(
            "SELECT 1 FROM information_schema.tables WHERE table_schema = @s AND table_name = @t",
            conn);
        cmd.Parameters.AddWithValue("s", schemaName);
        cmd.Parameters.AddWithValue("t", tableName);
        return await cmd.ExecuteScalarAsync() is not null;
    }

    private async Task<string[]> PrimaryKeyColumns(string schemaName, string tableName)
    {
        await using var conn = await _fixture.DataSource.OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand(@"
SELECT a.attname
FROM   pg_index i
JOIN   pg_class c        ON c.oid = i.indrelid
JOIN   pg_namespace n    ON n.oid = c.relnamespace
JOIN   pg_attribute a    ON a.attrelid = c.oid AND a.attnum = ANY(i.indkey)
WHERE  n.nspname = @s
  AND  c.relname = @t
  AND  i.indisprimary
ORDER  BY array_position(i.indkey, a.attnum)", conn);
        cmd.Parameters.AddWithValue("s", schemaName);
        cmd.Parameters.AddWithValue("t", tableName);

        var result = new System.Collections.Generic.List<string>();
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            result.Add(reader.GetString(0));
        }

        return result.ToArray();
    }
}
