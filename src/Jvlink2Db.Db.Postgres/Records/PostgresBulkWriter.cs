using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Jvlink2Db.Core.Persistence;
using Jvlink2Db.Db.Postgres.Schema;
using Npgsql;
using NpgsqlTypes;

namespace Jvlink2Db.Db.Postgres.Records;

/// <summary>
/// Generic base for per-record bulk writers. Subclasses supply the
/// table name, the column lists (in COPY order), and a row writer
/// that emits one record's column values in the same order. The base
/// handles staging, binary COPY, dedup-and-merge, and transaction
/// boundaries — uniformly for every record type.
/// </summary>
public abstract class PostgresBulkWriter<TRecord> : IBulkWriter<TRecord>
{
    private readonly NpgsqlDataSource _dataSource;
    private readonly string _schemaName;
    private readonly string _tableName;
    private readonly IReadOnlyList<string> _allColumns;
    private readonly IReadOnlyList<string> _pkColumns;

    protected PostgresBulkWriter(
        NpgsqlDataSource dataSource,
        string? schemaName,
        string tableName,
        IReadOnlyList<string> allColumns,
        IReadOnlyList<string> pkColumns)
    {
        ArgumentNullException.ThrowIfNull(dataSource);
        ArgumentException.ThrowIfNullOrEmpty(tableName);
        ArgumentNullException.ThrowIfNull(allColumns);
        ArgumentNullException.ThrowIfNull(pkColumns);
        if (allColumns.Count == 0)
        {
            throw new ArgumentException("All-columns list must not be empty.", nameof(allColumns));
        }

        if (pkColumns.Count == 0)
        {
            throw new ArgumentException("Primary-key list must not be empty.", nameof(pkColumns));
        }

        _dataSource = dataSource;
        _schemaName = schemaName ?? PostgresSchemaProvisioner.DefaultSchemaName;
        _tableName = tableName;
        _allColumns = allColumns;
        _pkColumns = pkColumns;
    }

    public async Task<long> WriteAsync(IAsyncEnumerable<TRecord> records, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(records);

        var schema = QuoteIdentifier(_schemaName);
        var qualifiedTable = $"{schema}.{QuoteIdentifier(_tableName)}";
        var stagingTable = $"stg_{_tableName}";
        var allColsSql = string.Join(", ", _allColumns);
        var pkColsSql = string.Join(", ", _pkColumns);
        var nonPkSet = string.Join(", ", _allColumns.Except(_pkColumns).Select(c => $"{c} = EXCLUDED.{c}"));

        await using var conn = await _dataSource.OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
        await using var tx = await conn.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);

        // Bulk historical loads issue thousands of small transactions —
        // one per JV-Link file boundary per record-spec. Each COMMIT does
        // a WAL fsync at the default synchronous_commit=on. On WSL2 ext4
        // a single fsync is several ms, so the cumulative wait dominates
        // throughput. Switch to async commits at the transaction scope:
        // if the host crashes, we lose the most recent commits, but
        // jvlink2db is idempotent (ON CONFLICT) so a re-run recovers.
        await using (var cmd = new NpgsqlCommand(
                         "SET LOCAL synchronous_commit = OFF",
                         conn,
                         tx))
        {
            await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }

        await using (var cmd = new NpgsqlCommand(
                         $"CREATE TEMP TABLE {stagingTable} (LIKE {qualifiedTable} INCLUDING DEFAULTS) ON COMMIT DROP",
                         conn,
                         tx))
        {
            await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }

        await using (var writer = await conn.BeginBinaryImportAsync(
                         $"COPY {stagingTable} ({allColsSql}) FROM STDIN BINARY",
                         cancellationToken).ConfigureAwait(false))
        {
            await foreach (var record in records.WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                await writer.StartRowAsync(cancellationToken).ConfigureAwait(false);
                await WriteRowAsync(writer, record, cancellationToken).ConfigureAwait(false);
            }

            await writer.CompleteAsync(cancellationToken).ConfigureAwait(false);
        }

        var mergeSql = $@"
INSERT INTO {qualifiedTable} ({allColsSql})
SELECT DISTINCT ON ({pkColsSql}) {allColsSql}
FROM {stagingTable}
ORDER BY {pkColsSql}, ctid DESC
ON CONFLICT ({pkColsSql}) DO UPDATE SET {nonPkSet}";

        long rows;
        await using (var cmd = new NpgsqlCommand(mergeSql, conn, tx))
        {
            rows = await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }

        await tx.CommitAsync(cancellationToken).ConfigureAwait(false);
        return rows;
    }

    /// <summary>
    /// Writes one record's columns in the order declared by the
    /// constructor's <c>allColumns</c> list. Subclasses call the
    /// <c>Write*</c> helpers for each column.
    /// </summary>
    protected abstract Task WriteRowAsync(NpgsqlBinaryImporter writer, TRecord record, CancellationToken cancellationToken);

    protected static async ValueTask WriteText(NpgsqlBinaryImporter writer, string value, CancellationToken cancellationToken)
    {
        var v = JvFieldConversions.AsText(value);
        if (v is null)
        {
            await writer.WriteNullAsync(cancellationToken).ConfigureAwait(false);
        }
        else
        {
            await writer.WriteAsync(v, NpgsqlDbType.Text, cancellationToken).ConfigureAwait(false);
        }
    }

    protected static async ValueTask WriteDate(NpgsqlBinaryImporter writer, string value, CancellationToken cancellationToken)
    {
        var v = JvFieldConversions.AsDate(value);
        if (v is null)
        {
            await writer.WriteNullAsync(cancellationToken).ConfigureAwait(false);
        }
        else
        {
            await writer.WriteAsync(v.Value, NpgsqlDbType.Date, cancellationToken).ConfigureAwait(false);
        }
    }

    protected static async ValueTask WriteShort(NpgsqlBinaryImporter writer, string value, CancellationToken cancellationToken)
    {
        var v = JvFieldConversions.AsShort(value);
        if (v is null)
        {
            await writer.WriteNullAsync(cancellationToken).ConfigureAwait(false);
        }
        else
        {
            await writer.WriteAsync(v.Value, NpgsqlDbType.Smallint, cancellationToken).ConfigureAwait(false);
        }
    }

    protected static async ValueTask WriteInt(NpgsqlBinaryImporter writer, string value, CancellationToken cancellationToken)
    {
        var v = JvFieldConversions.AsInt(value);
        if (v is null)
        {
            await writer.WriteNullAsync(cancellationToken).ConfigureAwait(false);
        }
        else
        {
            await writer.WriteAsync(v.Value, NpgsqlDbType.Integer, cancellationToken).ConfigureAwait(false);
        }
    }

    protected static async ValueTask WriteLong(NpgsqlBinaryImporter writer, string value, CancellationToken cancellationToken)
    {
        var v = JvFieldConversions.AsLong(value);
        if (v is null)
        {
            await writer.WriteNullAsync(cancellationToken).ConfigureAwait(false);
        }
        else
        {
            await writer.WriteAsync(v.Value, NpgsqlDbType.Bigint, cancellationToken).ConfigureAwait(false);
        }
    }

    protected static Task WriteShortArray(NpgsqlBinaryImporter writer, string[] values, CancellationToken cancellationToken) =>
        writer.WriteAsync(JvFieldConversions.AsShortArray(values), NpgsqlDbType.Array | NpgsqlDbType.Smallint, cancellationToken);

    protected static Task WriteIntArray(NpgsqlBinaryImporter writer, string[] values, CancellationToken cancellationToken) =>
        writer.WriteAsync(JvFieldConversions.AsIntArray(values), NpgsqlDbType.Array | NpgsqlDbType.Integer, cancellationToken);

    protected static Task WriteLongArray(NpgsqlBinaryImporter writer, string[] values, CancellationToken cancellationToken) =>
        writer.WriteAsync(JvFieldConversions.AsLongArray(values), NpgsqlDbType.Array | NpgsqlDbType.Bigint, cancellationToken);

    protected static Task WriteTextArray(NpgsqlBinaryImporter writer, string[] values, CancellationToken cancellationToken) =>
        writer.WriteAsync(JvFieldConversions.AsTextArray(values), NpgsqlDbType.Array | NpgsqlDbType.Text, cancellationToken);

    private static string QuoteIdentifier(string identifier) =>
        $"\"{identifier.Replace("\"", "\"\"", StringComparison.Ordinal)}\"";
}
