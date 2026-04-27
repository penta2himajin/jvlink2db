using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Jvlink2Db.Core.Persistence;
using Npgsql;

namespace Jvlink2Db.Db.Postgres.Schema;

/// <summary>
/// Provisions the PostgreSQL schema by running the embedded DDL files.
/// The DDL files contain unqualified table names; this class wraps
/// each invocation with <c>CREATE SCHEMA IF NOT EXISTS</c> +
/// <c>SET search_path</c> so the same DDL can target any schema name.
/// </summary>
public sealed class PostgresSchemaProvisioner : ISchemaProvisioner
{
    public const string DefaultSchemaName = "jv";

    private static readonly string[] s_ddlResources =
    [
        "Jvlink2Db.Db.Postgres.Schema.ra.sql",
    ];

    private readonly NpgsqlDataSource _dataSource;
    private readonly string _schemaName;

    public PostgresSchemaProvisioner(NpgsqlDataSource dataSource, string? schemaName = null)
    {
        ArgumentNullException.ThrowIfNull(dataSource);
        _dataSource = dataSource;
        _schemaName = schemaName ?? DefaultSchemaName;
    }

    public string SchemaName => _schemaName;

    public async Task EnsureCreatedAsync(CancellationToken cancellationToken)
    {
        await using var conn = await _dataSource.OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
        var quoted = QuoteIdentifier(_schemaName);

        await using (var cmd = new NpgsqlCommand($"CREATE SCHEMA IF NOT EXISTS {quoted}", conn))
        {
            await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }

        await using (var cmd = new NpgsqlCommand($"SET search_path TO {quoted}", conn))
        {
            await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }

        foreach (var resource in s_ddlResources)
        {
            var ddl = ReadEmbeddedDdl(resource);
            await using var cmd = new NpgsqlCommand(ddl, conn);
            await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    private static string ReadEmbeddedDdl(string resourceName)
    {
        var assembly = typeof(PostgresSchemaProvisioner).Assembly;
        using var stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException(
                $"Embedded DDL '{resourceName}' not found in {assembly.GetName().Name}.");
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    private static string QuoteIdentifier(string identifier) =>
        $"\"{identifier.Replace("\"", "\"\"", StringComparison.Ordinal)}\"";
}
