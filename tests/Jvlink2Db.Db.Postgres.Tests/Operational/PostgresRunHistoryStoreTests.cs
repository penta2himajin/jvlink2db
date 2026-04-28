using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Jvlink2Db.Core.Persistence;
using Jvlink2Db.Db.Postgres.Operational;
using Jvlink2Db.Db.Postgres.Schema;
using Jvlink2Db.Db.Postgres.Tests.Fixtures;
using Npgsql;
using Xunit;

namespace Jvlink2Db.Db.Postgres.Tests.Operational;

[Collection(PostgresCollection.Name)]
[Trait("Category", "Database")]
public sealed class PostgresRunHistoryStoreTests
{
    private readonly PostgresFixture _fixture;

    public PostgresRunHistoryStoreTests(PostgresFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task StartAsync_inserts_running_row_and_returns_id()
    {
        var schema = await ProvisionAsync();
        var sut = new PostgresRunHistoryStore(_fixture.DataSource, schema);

        var id = await sut.StartAsync(
            new RunHistoryStart("setup", "RACE", 4, "20260101000000", DateTimeOffset.UtcNow),
            CancellationToken.None);

        Assert.True(id > 0);

        await using var conn = await _fixture.DataSource.OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand(
            $@"SELECT mode, dataspec, option, fromtime, outcome FROM ""{schema}"".run_history WHERE id = @id",
            conn);
        cmd.Parameters.AddWithValue("id", id);
        await using var reader = await cmd.ExecuteReaderAsync();
        Assert.True(await reader.ReadAsync());
        Assert.Equal("setup", reader.GetString(0));
        Assert.Equal("RACE", reader.GetString(1));
        Assert.Equal((short)4, reader.GetFieldValue<short>(2));
        Assert.Equal("20260101000000", reader.GetString(3));
        Assert.Equal("running", reader.GetString(4));
    }

    [Fact]
    public async Task FinishAsync_writes_outcome_counts_and_jsonb_summaries()
    {
        var schema = await ProvisionAsync();
        var sut = new PostgresRunHistoryStore(_fixture.DataSource, schema);

        var id = await sut.StartAsync(
            new RunHistoryStart("range", "RACE", 4, "20260101000000-20260331235959", DateTimeOffset.UtcNow),
            CancellationToken.None);

        await sut.FinishAsync(id, new RunHistoryFinish(
            FinishedAt: DateTimeOffset.UtcNow,
            Outcome: "success",
            OpenReturnCode: 0,
            ReadCount: 30,
            DownloadCount: 0,
            LastFileTimestamp: "20260302153301",
            RecordCounts: new Dictionary<string, int> { ["RA"] = 1173, ["SE"] = 14664 },
            RecordsInserted: new Dictionary<string, long> { ["RA"] = 1173, ["SE"] = 14664 },
            ErrorMessage: null), CancellationToken.None);

        await using var conn = await _fixture.DataSource.OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand(
            $@"SELECT outcome, read_count, last_file_timestamp,
                      (record_counts ->> 'RA')::int,
                      (records_inserted ->> 'SE')::bigint
               FROM ""{schema}"".run_history WHERE id = @id",
            conn);
        cmd.Parameters.AddWithValue("id", id);
        await using var reader = await cmd.ExecuteReaderAsync();
        Assert.True(await reader.ReadAsync());
        Assert.Equal("success", reader.GetString(0));
        Assert.Equal(30, reader.GetFieldValue<int>(1));
        Assert.Equal("20260302153301", reader.GetString(2));
        Assert.Equal(1173, reader.GetFieldValue<int>(3));
        Assert.Equal(14664L, reader.GetFieldValue<long>(4));
    }

    [Fact]
    public async Task FinishAsync_records_failure_with_error_message()
    {
        var schema = await ProvisionAsync();
        var sut = new PostgresRunHistoryStore(_fixture.DataSource, schema);

        var id = await sut.StartAsync(
            new RunHistoryStart("normal", "DIFN", 1, "20260101000000", DateTimeOffset.UtcNow),
            CancellationToken.None);

        await sut.FinishAsync(id, new RunHistoryFinish(
            FinishedAt: DateTimeOffset.UtcNow,
            Outcome: "failed",
            OpenReturnCode: -301,
            ReadCount: null,
            DownloadCount: null,
            LastFileTimestamp: null,
            RecordCounts: null,
            RecordsInserted: null,
            ErrorMessage: "JV-Link auth transient"), CancellationToken.None);

        await using var conn = await _fixture.DataSource.OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand(
            $@"SELECT outcome, open_return_code, error_message FROM ""{schema}"".run_history WHERE id = @id",
            conn);
        cmd.Parameters.AddWithValue("id", id);
        await using var reader = await cmd.ExecuteReaderAsync();
        Assert.True(await reader.ReadAsync());
        Assert.Equal("failed", reader.GetString(0));
        Assert.Equal(-301, reader.GetFieldValue<int>(1));
        Assert.Equal("JV-Link auth transient", reader.GetString(2));
    }

    private async Task<string> ProvisionAsync()
    {
        var schemaName = $"jvops_{Guid.NewGuid():N}";
        await new PostgresOperationalSchemaProvisioner(_fixture.DataSource, schemaName)
            .EnsureCreatedAsync(CancellationToken.None);
        return schemaName;
    }
}
