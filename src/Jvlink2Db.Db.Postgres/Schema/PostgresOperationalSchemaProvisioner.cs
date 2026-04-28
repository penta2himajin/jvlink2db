using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Jvlink2Db.Core.Persistence;
using Npgsql;

namespace Jvlink2Db.Db.Postgres.Schema;

/// <summary>
/// Provisions the operational schema (default <c>jvlink2db</c>) that
/// holds <c>acquisition_state</c> and <c>run_history</c>. Kept separate
/// from the data schema so users can keep operational metadata in a
/// fixed location even when the data schema is configurable per run.
/// </summary>
public sealed class PostgresOperationalSchemaProvisioner : ISchemaProvisioner
{
    public const string DefaultSchemaName = "jvlink2db";

    private const string ResourcePrefix = "Jvlink2Db.Db.Postgres.OperationalSchema.";
    private const string ResourceSuffix = ".sql";

    private readonly NpgsqlDataSource _dataSource;
    private readonly string _schemaName;

    public PostgresOperationalSchemaProvisioner(NpgsqlDataSource dataSource, string? schemaName = null)
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

        foreach (var resource in DiscoverDdlResources())
        {
            var ddl = ReadEmbeddedDdl(resource);
            await using var cmd = new NpgsqlCommand(ddl, conn);
            await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    private static IEnumerable<string> DiscoverDdlResources() =>
        typeof(PostgresOperationalSchemaProvisioner).Assembly.GetManifestResourceNames()
            .Where(name => name.StartsWith(ResourcePrefix, StringComparison.Ordinal)
                           && name.EndsWith(ResourceSuffix, StringComparison.Ordinal))
            .OrderBy(name => name, StringComparer.Ordinal);

    private static string ReadEmbeddedDdl(string resourceName)
    {
        var assembly = typeof(PostgresOperationalSchemaProvisioner).Assembly;
        using var stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException(
                $"Embedded DDL '{resourceName}' not found in {assembly.GetName().Name}.");
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    private static string QuoteIdentifier(string identifier) =>
        $"\"{identifier.Replace("\"", "\"\"", StringComparison.Ordinal)}\"";
}
