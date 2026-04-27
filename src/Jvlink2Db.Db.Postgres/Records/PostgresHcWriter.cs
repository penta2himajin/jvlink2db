using System.Threading;
using System.Threading.Tasks;
using Jvlink2Db.Core.Records;
using Npgsql;

namespace Jvlink2Db.Db.Postgres.Records;

public sealed class PostgresHcWriter : PostgresBulkWriter<Hc>
{
    public PostgresHcWriter(NpgsqlDataSource dataSource, string? schemaName = null)
        : base(dataSource, schemaName, "hc", HcColumns.All, HcColumns.PrimaryKey)
    {
    }

    protected override async Task WriteRowAsync(NpgsqlBinaryImporter writer, Hc r, CancellationToken ct)
    {
        await WriteText(writer, r.RecordSpec, ct).ConfigureAwait(false);
        await WriteText(writer, r.DataKubun, ct).ConfigureAwait(false);
        await WriteDate(writer, r.MakeDate, ct).ConfigureAwait(false);
        await WriteText(writer, r.TresenKubun, ct).ConfigureAwait(false);
        await WriteDate(writer, r.ChokyoDate, ct).ConfigureAwait(false);
        await WriteText(writer, r.ChokyoTime, ct).ConfigureAwait(false);
        await WriteText(writer, r.KettoNum, ct).ConfigureAwait(false);
        await WriteShort(writer, r.HaronTime4, ct).ConfigureAwait(false);
        await WriteShort(writer, r.LapTime4, ct).ConfigureAwait(false);
        await WriteShort(writer, r.HaronTime3, ct).ConfigureAwait(false);
        await WriteShort(writer, r.LapTime3, ct).ConfigureAwait(false);
        await WriteShort(writer, r.HaronTime2, ct).ConfigureAwait(false);
        await WriteShort(writer, r.LapTime2, ct).ConfigureAwait(false);
        await WriteShort(writer, r.LapTime1, ct).ConfigureAwait(false);
    }
}
