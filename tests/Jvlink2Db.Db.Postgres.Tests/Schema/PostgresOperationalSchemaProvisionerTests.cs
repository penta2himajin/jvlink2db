using System.Threading;
using System.Threading.Tasks;
using Jvlink2Db.Db.Postgres.Schema;
using Jvlink2Db.Db.Postgres.Tests.Fixtures;
using Npgsql;
using Xunit;

namespace Jvlink2Db.Db.Postgres.Tests.Schema;

[Collection(PostgresCollection.Name)]
[Trait("Category", "Database")]
public sealed class PostgresOperationalSchemaProvisionerTests
{
    private readonly PostgresFixture _fixture;

    public PostgresOperationalSchemaProvisionerTests(PostgresFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task EnsureCreatedAsync_creates_run_history_and_acquisition_state()
    {
        var schemaName = NewSchemaName();
        var sut = new PostgresOperationalSchemaProvisioner(_fixture.DataSource, schemaName);

        await sut.EnsureCreatedAsync(CancellationToken.None);

        Assert.True(await TableExists(schemaName, "run_history"));
        Assert.True(await TableExists(schemaName, "acquisition_state"));
    }

    [Fact]
    public async Task EnsureCreatedAsync_grants_usage_and_select_to_reader_role()
    {
        var schemaName = NewSchemaName();
        var readerRole = NewRoleName();
        await CreateRole(readerRole);
        try
        {
            var sut = new PostgresOperationalSchemaProvisioner(_fixture.DataSource, schemaName, readerRole);

            await sut.EnsureCreatedAsync(CancellationToken.None);

            Assert.True(await HasSchemaPrivilege(readerRole, schemaName, "USAGE"));
            Assert.True(await HasTablePrivilege(readerRole, schemaName, "run_history", "SELECT"));
            Assert.True(await HasTablePrivilege(readerRole, schemaName, "acquisition_state", "SELECT"));
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
            var sut = new PostgresOperationalSchemaProvisioner(_fixture.DataSource, schemaName, readerRoleName: null);

            await sut.EnsureCreatedAsync(CancellationToken.None);

            Assert.False(await HasSchemaPrivilege(readerRole, schemaName, "USAGE"));
        }
        finally
        {
            await DropRole(readerRole);
        }
    }

    private static string NewSchemaName() => $"ops_{System.Guid.NewGuid():N}";

    private static string NewRoleName() => $"reader_{System.Guid.NewGuid():N}";

    private async Task CreateRole(string roleName)
    {
        await ExecuteAsync($"CREATE ROLE \"{roleName}\"");
    }

    private async Task DropRole(string roleName)
    {
        try
        {
            await ExecuteAsync($"DROP OWNED BY \"{roleName}\"");
        }
        catch
        {
            // Best-effort cleanup; the DROP ROLE below surfaces real errors.
        }
        await ExecuteAsync($"DROP ROLE IF EXISTS \"{roleName}\"");
    }

    private async Task ExecuteAsync(string sql)
    {
        await using var conn = await _fixture.DataSource.OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand(sql, conn);
        await cmd.ExecuteNonQueryAsync();
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
}
