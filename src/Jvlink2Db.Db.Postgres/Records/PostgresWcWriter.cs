using System.Threading;
using System.Threading.Tasks;
using Jvlink2Db.Core.Records;
using Npgsql;

namespace Jvlink2Db.Db.Postgres.Records;

public sealed class PostgresWcWriter : PostgresBulkWriter<Wc>
{
    public PostgresWcWriter(NpgsqlDataSource dataSource, string? schemaName = null)
        : base(dataSource, schemaName, "wc", WcColumns.All, WcColumns.PrimaryKey)
    {
    }

    protected override async Task WriteRowAsync(NpgsqlBinaryImporter writer, Wc r, CancellationToken ct)
    {
        await WriteText(writer, r.RecordSpec, ct).ConfigureAwait(false);
        await WriteText(writer, r.DataKubun, ct).ConfigureAwait(false);
        await WriteDate(writer, r.MakeDate, ct).ConfigureAwait(false);
        await WriteText(writer, r.TresenKubun, ct).ConfigureAwait(false);
        await WriteDate(writer, r.ChokyoDate, ct).ConfigureAwait(false);
        await WriteText(writer, r.ChokyoTime, ct).ConfigureAwait(false);
        await WriteText(writer, r.KettoNum, ct).ConfigureAwait(false);
        await WriteText(writer, r.Course, ct).ConfigureAwait(false);
        await WriteText(writer, r.BabaAround, ct).ConfigureAwait(false);
        await WriteText(writer, r.Reserved, ct).ConfigureAwait(false);
        await WriteShort(writer, r.HaronTime10, ct).ConfigureAwait(false);
        await WriteShort(writer, r.LapTime10, ct).ConfigureAwait(false);
        await WriteShort(writer, r.HaronTime9, ct).ConfigureAwait(false);
        await WriteShort(writer, r.LapTime9, ct).ConfigureAwait(false);
        await WriteShort(writer, r.HaronTime8, ct).ConfigureAwait(false);
        await WriteShort(writer, r.LapTime8, ct).ConfigureAwait(false);
        await WriteShort(writer, r.HaronTime7, ct).ConfigureAwait(false);
        await WriteShort(writer, r.LapTime7, ct).ConfigureAwait(false);
        await WriteShort(writer, r.HaronTime6, ct).ConfigureAwait(false);
        await WriteShort(writer, r.LapTime6, ct).ConfigureAwait(false);
        await WriteShort(writer, r.HaronTime5, ct).ConfigureAwait(false);
        await WriteShort(writer, r.LapTime5, ct).ConfigureAwait(false);
        await WriteShort(writer, r.HaronTime4, ct).ConfigureAwait(false);
        await WriteShort(writer, r.LapTime4, ct).ConfigureAwait(false);
        await WriteShort(writer, r.HaronTime3, ct).ConfigureAwait(false);
        await WriteShort(writer, r.LapTime3, ct).ConfigureAwait(false);
        await WriteShort(writer, r.HaronTime2, ct).ConfigureAwait(false);
        await WriteShort(writer, r.LapTime2, ct).ConfigureAwait(false);
        await WriteShort(writer, r.LapTime1, ct).ConfigureAwait(false);
    }
}
