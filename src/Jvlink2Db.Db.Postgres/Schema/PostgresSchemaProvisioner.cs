using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Jvlink2Db.Core.Persistence;
using Npgsql;

namespace Jvlink2Db.Db.Postgres.Schema;

/// <summary>
/// Provisions the PostgreSQL schema by running every embedded DDL
/// file. The DDL files contain unqualified table names; this class
/// wraps each invocation with <c>CREATE SCHEMA IF NOT EXISTS</c> +
/// <c>SET search_path</c> so the same DDL can target any schema name.
/// </summary>
public sealed class PostgresSchemaProvisioner : ISchemaProvisioner
{
    public const string DefaultSchemaName = "jv";

    private const string ResourcePrefix = "Jvlink2Db.Db.Postgres.Schema.";
    private const string ResourceSuffix = ".sql";

    private readonly NpgsqlDataSource _dataSource;
    private readonly string _schemaName;
    private readonly string? _readerRoleName;

    public PostgresSchemaProvisioner(
        NpgsqlDataSource dataSource,
        string? schemaName = null,
        string? readerRoleName = null)
    {
        ArgumentNullException.ThrowIfNull(dataSource);
        _dataSource = dataSource;
        _schemaName = schemaName ?? DefaultSchemaName;
        _readerRoleName = string.IsNullOrWhiteSpace(readerRoleName) ? null : readerRoleName;
    }

    public string SchemaName => _schemaName;

    public async Task EnsureCreatedAsync(CancellationToken cancellationToken)
    {
        await using var conn = await _dataSource.OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
        var quotedSchema = QuoteIdentifier(_schemaName);

        await using (var cmd = new NpgsqlCommand($"CREATE SCHEMA IF NOT EXISTS {quotedSchema}", conn))
        {
            await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }

        await using (var cmd = new NpgsqlCommand($"SET search_path TO {quotedSchema}", conn))
        {
            await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }

        // Set DEFAULT PRIVILEGES BEFORE running the DDL so tables created
        // below inherit SELECT for the reader role automatically. GRANT
        // USAGE here lets the reader enter the schema even before tables
        // exist (useful for list_tables-style discovery).
        if (_readerRoleName is not null)
        {
            var quotedRole = QuoteIdentifier(_readerRoleName);

            await using (var cmd = new NpgsqlCommand(
                $"GRANT USAGE ON SCHEMA {quotedSchema} TO {quotedRole}", conn))
            {
                await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            }

            await using (var cmd = new NpgsqlCommand(
                $"ALTER DEFAULT PRIVILEGES IN SCHEMA {quotedSchema} GRANT SELECT ON TABLES TO {quotedRole}",
                conn))
            {
                await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        foreach (var resource in DiscoverDdlResources())
        {
            var ddl = ReadEmbeddedDdl(resource);
            await using var cmd = new NpgsqlCommand(ddl, conn);
            await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }

        // Cover any tables that pre-existed the DEFAULT PRIVILEGES call
        // above (e.g. legacy schemas provisioned before this feature was
        // added, or tables created by a role other than the current one).
        // Idempotent: a re-GRANT on already-granted tables is a no-op.
        if (_readerRoleName is not null)
        {
            var quotedRole = QuoteIdentifier(_readerRoleName);
            await using var cmd = new NpgsqlCommand(
                $"GRANT SELECT ON ALL TABLES IN SCHEMA {quotedSchema} TO {quotedRole}", conn);
            await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    private static IEnumerable<string> DiscoverDdlResources() =>
        typeof(PostgresSchemaProvisioner).Assembly.GetManifestResourceNames()
            .Where(name => name.StartsWith(ResourcePrefix, StringComparison.Ordinal)
                           && name.EndsWith(ResourceSuffix, StringComparison.Ordinal))
            .OrderBy(name => name, StringComparer.Ordinal);

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
