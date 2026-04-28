using System;
using System.Threading;
using System.Threading.Tasks;
using Jvlink2Db.Core.Persistence;
using Npgsql;

namespace Jvlink2Db.Db.Postgres.Operational;

public sealed class PostgresAcquisitionStateStore : IAcquisitionStateStore
{
    private readonly NpgsqlDataSource _dataSource;
    private readonly string _schemaName;

    public PostgresAcquisitionStateStore(NpgsqlDataSource dataSource, string? schemaName = null)
    {
        ArgumentNullException.ThrowIfNull(dataSource);
        _dataSource = dataSource;
        _schemaName = schemaName ?? "jvlink2db";
    }

    public async Task<AcquisitionState?> GetAsync(string dataspec, int option, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(dataspec);

        var sql = $@"SELECT last_fromtime, last_filename
                     FROM ""{_schemaName}"".acquisition_state
                     WHERE dataspec = @dataspec AND option = @option";

        await using var conn = await _dataSource.OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("dataspec", dataspec);
        cmd.Parameters.AddWithValue("option", (short)option);

        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        if (!await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            return null;
        }

        return new AcquisitionState(
            Dataspec: dataspec,
            Option: option,
            LastFromtime: reader.IsDBNull(0) ? null : reader.GetString(0),
            LastFilename: reader.IsDBNull(1) ? null : reader.GetString(1));
    }

    public async Task UpsertAsync(AcquisitionState state, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(state);

        var sql = $@"
            INSERT INTO ""{_schemaName}"".acquisition_state
                (dataspec, option, last_fromtime, last_filename, last_success_at)
            VALUES (@dataspec, @option, @last_fromtime, @last_filename, now())
            ON CONFLICT (dataspec, option) DO UPDATE SET
                last_fromtime   = EXCLUDED.last_fromtime,
                last_filename   = EXCLUDED.last_filename,
                last_success_at = EXCLUDED.last_success_at";

        await using var conn = await _dataSource.OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("dataspec", state.Dataspec);
        cmd.Parameters.AddWithValue("option", (short)state.Option);
        cmd.Parameters.AddWithValue("last_fromtime", (object?)state.LastFromtime ?? DBNull.Value);
        cmd.Parameters.AddWithValue("last_filename", (object?)state.LastFilename ?? DBNull.Value);

        await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }
}
