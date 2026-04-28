using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Jvlink2Db.Core.Persistence;
using Npgsql;
using NpgsqlTypes;

namespace Jvlink2Db.Db.Postgres.Operational;

public sealed class PostgresRunHistoryStore : IRunHistoryStore
{
    private readonly NpgsqlDataSource _dataSource;
    private readonly string _schemaName;

    public PostgresRunHistoryStore(NpgsqlDataSource dataSource, string? schemaName = null)
    {
        ArgumentNullException.ThrowIfNull(dataSource);
        _dataSource = dataSource;
        _schemaName = schemaName ?? "jvlink2db";
    }

    public async Task<long> StartAsync(RunHistoryStart start, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(start);

        var sql = $@"
            INSERT INTO ""{_schemaName}"".run_history
                (mode, dataspec, option, fromtime, started_at, outcome)
            VALUES (@mode, @dataspec, @option, @fromtime, @started_at, 'running')
            RETURNING id";

        await using var conn = await _dataSource.OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("mode", start.Mode);
        cmd.Parameters.AddWithValue("dataspec", start.Dataspec);
        cmd.Parameters.AddWithValue("option", (short)start.Option);
        cmd.Parameters.AddWithValue("fromtime", start.Fromtime);
        cmd.Parameters.AddWithValue("started_at", start.StartedAt);

        var id = await cmd.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
        return Convert.ToInt64(id);
    }

    public async Task FinishAsync(long id, RunHistoryFinish finish, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(finish);

        var sql = $@"
            UPDATE ""{_schemaName}"".run_history
            SET finished_at         = @finished_at,
                outcome             = @outcome,
                open_return_code    = @open_return_code,
                read_count          = @read_count,
                download_count      = @download_count,
                last_file_timestamp = @last_file_timestamp,
                record_counts       = @record_counts,
                records_inserted    = @records_inserted,
                error_message       = @error_message
            WHERE id = @id";

        await using var conn = await _dataSource.OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("id", id);
        cmd.Parameters.AddWithValue("finished_at", finish.FinishedAt);
        cmd.Parameters.AddWithValue("outcome", finish.Outcome);
        cmd.Parameters.AddWithValue("open_return_code", (object?)finish.OpenReturnCode ?? DBNull.Value);
        cmd.Parameters.AddWithValue("read_count", (object?)finish.ReadCount ?? DBNull.Value);
        cmd.Parameters.AddWithValue("download_count", (object?)finish.DownloadCount ?? DBNull.Value);
        cmd.Parameters.AddWithValue("last_file_timestamp", (object?)finish.LastFileTimestamp ?? DBNull.Value);
        AddJsonbParameter(cmd, "record_counts", finish.RecordCounts);
        AddJsonbParameter(cmd, "records_inserted", finish.RecordsInserted);
        cmd.Parameters.AddWithValue("error_message", (object?)finish.ErrorMessage ?? DBNull.Value);

        await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }

    private static void AddJsonbParameter(NpgsqlCommand cmd, string name, object? value)
    {
        if (value is null)
        {
            cmd.Parameters.Add(new NpgsqlParameter(name, NpgsqlDbType.Jsonb) { Value = DBNull.Value });
            return;
        }

        var json = JsonSerializer.Serialize(value);
        cmd.Parameters.Add(new NpgsqlParameter(name, NpgsqlDbType.Jsonb) { Value = json });
    }
}
