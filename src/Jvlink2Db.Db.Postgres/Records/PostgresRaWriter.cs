using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Jvlink2Db.Core.Persistence;
using Jvlink2Db.Core.Records;
using Npgsql;
using NpgsqlTypes;

namespace Jvlink2Db.Db.Postgres.Records;

/// <summary>
/// Idempotent bulk writer for <see cref="Ra"/> records. Uses the
/// staging-table pattern from docs/04-database-schema.md: COPY the
/// batch into a TEMP table, then dedupe by primary key (last record
/// wins, in JV-Link's emit order) and merge into <c>jv.ra</c> with
/// <c>ON CONFLICT ... DO UPDATE</c>.
/// </summary>
public sealed class PostgresRaWriter : IBulkWriter<Ra>
{
    private readonly NpgsqlDataSource _dataSource;
    private readonly string _schemaName;

    public PostgresRaWriter(NpgsqlDataSource dataSource, string? schemaName = null)
    {
        ArgumentNullException.ThrowIfNull(dataSource);
        _dataSource = dataSource;
        _schemaName = schemaName ?? PostgresSchemaProvisionerSchema;
    }

    private const string PostgresSchemaProvisionerSchema = Schema.PostgresSchemaProvisioner.DefaultSchemaName;

    public async Task<long> WriteAsync(IAsyncEnumerable<Ra> records, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(records);

        var schema = QuoteIdentifier(_schemaName);
        var allCols = string.Join(", ", RaColumns.All);
        var pkCols = string.Join(", ", RaColumns.PrimaryKey);
        var nonPkSet = string.Join(", ", RaColumns.NonPrimaryKey().Select(c => $"{c} = EXCLUDED.{c}"));

        await using var conn = await _dataSource.OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
        await using var tx = await conn.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);

        await using (var cmd = new NpgsqlCommand(
                         $"CREATE TEMP TABLE stg_ra (LIKE {schema}.ra INCLUDING DEFAULTS) ON COMMIT DROP",
                         conn,
                         tx))
        {
            await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }

        await using (var writer = await conn.BeginBinaryImportAsync(
                         $"COPY stg_ra ({allCols}) FROM STDIN BINARY",
                         cancellationToken).ConfigureAwait(false))
        {
            await foreach (var ra in records.WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                await writer.StartRowAsync(cancellationToken).ConfigureAwait(false);
                await WriteRowAsync(writer, ra, cancellationToken).ConfigureAwait(false);
            }

            await writer.CompleteAsync(cancellationToken).ConfigureAwait(false);
        }

        var mergeSql = $@"
INSERT INTO {schema}.ra ({allCols})
SELECT DISTINCT ON ({pkCols}) {allCols}
FROM stg_ra
ORDER BY {pkCols}, ctid DESC
ON CONFLICT ({pkCols}) DO UPDATE SET {nonPkSet}";

        long rows;
        await using (var cmd = new NpgsqlCommand(mergeSql, conn, tx))
        {
            rows = await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }

        await tx.CommitAsync(cancellationToken).ConfigureAwait(false);
        return rows;
    }

    private static async Task WriteRowAsync(NpgsqlBinaryImporter writer, Ra ra, CancellationToken ct)
    {
        await WriteText(writer, ra.RecordSpec, ct).ConfigureAwait(false);
        await WriteText(writer, ra.DataKubun, ct).ConfigureAwait(false);
        await WriteDate(writer, ra.MakeDate, ct).ConfigureAwait(false);

        await WriteText(writer, ra.Year, ct).ConfigureAwait(false);
        await WriteText(writer, ra.MonthDay, ct).ConfigureAwait(false);
        await WriteText(writer, ra.JyoCD, ct).ConfigureAwait(false);
        await WriteText(writer, ra.Kaiji, ct).ConfigureAwait(false);
        await WriteText(writer, ra.Nichiji, ct).ConfigureAwait(false);
        await WriteText(writer, ra.RaceNum, ct).ConfigureAwait(false);

        await WriteText(writer, ra.YoubiCD, ct).ConfigureAwait(false);
        await WriteText(writer, ra.TokuNum, ct).ConfigureAwait(false);
        await WriteText(writer, ra.Hondai, ct).ConfigureAwait(false);
        await WriteText(writer, ra.Fukudai, ct).ConfigureAwait(false);
        await WriteText(writer, ra.Kakko, ct).ConfigureAwait(false);
        await WriteText(writer, ra.HondaiEng, ct).ConfigureAwait(false);
        await WriteText(writer, ra.FukudaiEng, ct).ConfigureAwait(false);
        await WriteText(writer, ra.KakkoEng, ct).ConfigureAwait(false);
        await WriteText(writer, ra.Ryakusyo10, ct).ConfigureAwait(false);
        await WriteText(writer, ra.Ryakusyo6, ct).ConfigureAwait(false);
        await WriteText(writer, ra.Ryakusyo3, ct).ConfigureAwait(false);
        await WriteText(writer, ra.Kubun, ct).ConfigureAwait(false);
        await WriteShort(writer, ra.Nkai, ct).ConfigureAwait(false);

        await WriteText(writer, ra.GradeCD, ct).ConfigureAwait(false);
        await WriteText(writer, ra.GradeCDBefore, ct).ConfigureAwait(false);

        await WriteText(writer, ra.SyubetuCD, ct).ConfigureAwait(false);
        await WriteText(writer, ra.KigoCD, ct).ConfigureAwait(false);
        await WriteText(writer, ra.JyuryoCD, ct).ConfigureAwait(false);
        await WriteTextArray(writer, ra.JyokenCD, ct).ConfigureAwait(false);

        await WriteText(writer, ra.JyokenName, ct).ConfigureAwait(false);
        await WriteShort(writer, ra.Kyori, ct).ConfigureAwait(false);
        await WriteShort(writer, ra.KyoriBefore, ct).ConfigureAwait(false);
        await WriteText(writer, ra.TrackCD, ct).ConfigureAwait(false);
        await WriteText(writer, ra.TrackCDBefore, ct).ConfigureAwait(false);
        await WriteText(writer, ra.CourseKubunCD, ct).ConfigureAwait(false);
        await WriteText(writer, ra.CourseKubunCDBefore, ct).ConfigureAwait(false);

        await WriteIntArray(writer, ra.Honsyokin, ct).ConfigureAwait(false);
        await WriteIntArray(writer, ra.HonsyokinBefore, ct).ConfigureAwait(false);
        await WriteIntArray(writer, ra.Fukasyokin, ct).ConfigureAwait(false);
        await WriteIntArray(writer, ra.FukasyokinBefore, ct).ConfigureAwait(false);

        await WriteText(writer, ra.HassoTime, ct).ConfigureAwait(false);
        await WriteText(writer, ra.HassoTimeBefore, ct).ConfigureAwait(false);
        await WriteShort(writer, ra.TorokuTosu, ct).ConfigureAwait(false);
        await WriteShort(writer, ra.SyussoTosu, ct).ConfigureAwait(false);
        await WriteShort(writer, ra.NyusenTosu, ct).ConfigureAwait(false);

        await WriteText(writer, ra.TenkoCD, ct).ConfigureAwait(false);
        await WriteText(writer, ra.SibaBabaCD, ct).ConfigureAwait(false);
        await WriteText(writer, ra.DirtBabaCD, ct).ConfigureAwait(false);

        await WriteShortArray(writer, ra.LapTime, ct).ConfigureAwait(false);

        await WriteShort(writer, ra.SyogaiMileTime, ct).ConfigureAwait(false);
        await WriteShort(writer, ra.HaronTimeS3, ct).ConfigureAwait(false);
        await WriteShort(writer, ra.HaronTimeS4, ct).ConfigureAwait(false);
        await WriteShort(writer, ra.HaronTimeL3, ct).ConfigureAwait(false);
        await WriteShort(writer, ra.HaronTimeL4, ct).ConfigureAwait(false);

        for (var i = 0; i < 4; i++)
        {
            var c = ra.Corners[i];
            await WriteText(writer, c.Corner, ct).ConfigureAwait(false);
            await WriteText(writer, c.Syukaisu, ct).ConfigureAwait(false);
            await WriteText(writer, c.Jyuni, ct).ConfigureAwait(false);
        }

        await WriteText(writer, ra.RecordUpKubun, ct).ConfigureAwait(false);
    }

    private static async ValueTask WriteText(NpgsqlBinaryImporter writer, string value, CancellationToken ct)
    {
        var v = RaConversions.AsText(value);
        if (v is null)
        {
            await writer.WriteNullAsync(ct).ConfigureAwait(false);
        }
        else
        {
            await writer.WriteAsync(v, NpgsqlDbType.Text, ct).ConfigureAwait(false);
        }
    }

    private static async ValueTask WriteDate(NpgsqlBinaryImporter writer, string value, CancellationToken ct)
    {
        var v = RaConversions.AsDate(value);
        if (v is null)
        {
            await writer.WriteNullAsync(ct).ConfigureAwait(false);
        }
        else
        {
            await writer.WriteAsync(v.Value, NpgsqlDbType.Date, ct).ConfigureAwait(false);
        }
    }

    private static async ValueTask WriteShort(NpgsqlBinaryImporter writer, string value, CancellationToken ct)
    {
        var v = RaConversions.AsShort(value);
        if (v is null)
        {
            await writer.WriteNullAsync(ct).ConfigureAwait(false);
        }
        else
        {
            await writer.WriteAsync(v.Value, NpgsqlDbType.Smallint, ct).ConfigureAwait(false);
        }
    }

    private static Task WriteShortArray(NpgsqlBinaryImporter writer, string[] values, CancellationToken ct)
    {
        var arr = RaConversions.AsShortArray(values);
        return writer.WriteAsync(arr, NpgsqlDbType.Array | NpgsqlDbType.Smallint, ct);
    }

    private static Task WriteIntArray(NpgsqlBinaryImporter writer, string[] values, CancellationToken ct)
    {
        var arr = RaConversions.AsIntArray(values);
        return writer.WriteAsync(arr, NpgsqlDbType.Array | NpgsqlDbType.Integer, ct);
    }

    private static Task WriteTextArray(NpgsqlBinaryImporter writer, string[] values, CancellationToken ct)
    {
        var arr = RaConversions.AsTextArray(values);
        return writer.WriteAsync(arr, NpgsqlDbType.Array | NpgsqlDbType.Text, ct);
    }

    private static string QuoteIdentifier(string identifier) =>
        $"\"{identifier.Replace("\"", "\"\"", StringComparison.Ordinal)}\"";
}
