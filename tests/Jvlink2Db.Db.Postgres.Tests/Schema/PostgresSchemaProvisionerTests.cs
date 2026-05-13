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

    [Fact]
    public async Task EnsureCreatedAsync_grants_usage_and_select_to_reader_role()
    {
        var schemaName = NewSchemaName();
        var readerRole = NewRoleName();
        await CreateRole(readerRole);
        try
        {
            var sut = new PostgresSchemaProvisioner(_fixture.DataSource, schemaName, readerRole);

            await sut.EnsureCreatedAsync(CancellationToken.None);

            Assert.True(await HasSchemaPrivilege(readerRole, schemaName, "USAGE"));
            Assert.True(await HasTablePrivilege(readerRole, schemaName, "ra", "SELECT"));
        }
        finally
        {
            await DropRole(readerRole);
        }
    }

    [Fact]
    public async Task EnsureCreatedAsync_default_privileges_cover_tables_created_after_provisioning()
    {
        var schemaName = NewSchemaName();
        var readerRole = NewRoleName();
        await CreateRole(readerRole);
        try
        {
            var sut = new PostgresSchemaProvisioner(_fixture.DataSource, schemaName, readerRole);
            await sut.EnsureCreatedAsync(CancellationToken.None);

            // Simulate a future table created by the same role that provisioned
            // the schema (which is what jvlink2db re-runs do for new record
            // types or schema migrations).
            await ExecuteAsync($"CREATE TABLE \"{schemaName}\".future_table (id int)");

            Assert.True(await HasTablePrivilege(readerRole, schemaName, "future_table", "SELECT"));
        }
        finally
        {
            await DropRole(readerRole);
        }
    }

    [Fact]
    public async Task EnsureCreatedAsync_does_not_grant_when_reader_role_is_null()
    {
        var schemaName = NewSchemaName();
        var readerRole = NewRoleName();
        await CreateRole(readerRole);
        try
        {
            var sut = new PostgresSchemaProvisioner(_fixture.DataSource, schemaName, readerRoleName: null);

            await sut.EnsureCreatedAsync(CancellationToken.None);

            Assert.False(await HasSchemaPrivilege(readerRole, schemaName, "USAGE"));
            Assert.False(await HasTablePrivilege(readerRole, schemaName, "ra", "SELECT"));
        }
        finally
        {
            await DropRole(readerRole);
        }
    }

    [Fact]
    public async Task EnsureCreatedAsync_is_idempotent_with_reader_role()
    {
        var schemaName = NewSchemaName();
        var readerRole = NewRoleName();
        await CreateRole(readerRole);
        try
        {
            var sut = new PostgresSchemaProvisioner(_fixture.DataSource, schemaName, readerRole);

            await sut.EnsureCreatedAsync(CancellationToken.None);
            // Second invocation must not throw on the GRANT statements.
            await sut.EnsureCreatedAsync(CancellationToken.None);

            Assert.True(await HasSchemaPrivilege(readerRole, schemaName, "USAGE"));
            Assert.True(await HasTablePrivilege(readerRole, schemaName, "ra", "SELECT"));
        }
        finally
        {
            await DropRole(readerRole);
        }
    }

    private static string NewSchemaName() => $"jv_{System.Guid.NewGuid():N}";

    private static string NewRoleName() => $"reader_{System.Guid.NewGuid():N}";

    private async Task CreateRole(string roleName)
    {
        await ExecuteAsync($"CREATE ROLE \"{roleName}\"");
    }

    private async Task DropRole(string roleName)
    {
        // REASSIGN/DROP OWNED is needed because DEFAULT PRIVILEGES entries
        // pin the role even though the role itself owns no objects.
        try
        {
            await ExecuteAsync($"DROP OWNED BY \"{roleName}\"");
        }
        catch
        {
            // Best-effort; a clean DROP ROLE below will still surface the
            // real error if any objects remain.
        }
        await ExecuteAsync($"DROP ROLE IF EXISTS \"{roleName}\"");
    }

    private async Task ExecuteAsync(string sql)
    {
        await using var conn = await _fixture.DataSource.OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand(sql, conn);
        await cmd.ExecuteNonQueryAsync();
    }

    private async Task<bool> HasSchemaPrivilege(string roleName, string schemaName, string privilege)
    {
        await using var conn = await _fixture.DataSource.OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand(
            "SELECT has_schema_privilege(@r, @s, @p)", conn);
        cmd.Parameters.AddWithValue("r", roleName);
        cmd.Parameters.AddWithValue("s", schemaName);
        cmd.Parameters.AddWithValue("p", privilege);
        return (bool)(await cmd.ExecuteScalarAsync())!;
    }

    private async Task<bool> HasTablePrivilege(string roleName, string schemaName, string tableName, string privilege)
    {
        await using var conn = await _fixture.DataSource.OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand(
            "SELECT has_table_privilege(@r, @qualified, @p)", conn);
        cmd.Parameters.AddWithValue("r", roleName);
        cmd.Parameters.AddWithValue("qualified", $"\"{schemaName}\".\"{tableName}\"");
        cmd.Parameters.AddWithValue("p", privilege);
        return (bool)(await cmd.ExecuteScalarAsync())!;
    }

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
