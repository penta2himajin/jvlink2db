using System.Threading;
using System.Threading.Tasks;
using Jvlink2Db.Core.Records;
using Npgsql;

namespace Jvlink2Db.Db.Postgres.Records;

public sealed class PostgresHyWriter : PostgresBulkWriter<Hy>
{
    public PostgresHyWriter(NpgsqlDataSource dataSource, string? schemaName = null)
        : base(dataSource, schemaName, "hy", HyColumns.All, HyColumns.PrimaryKey)
    {
    }

    protected override async Task WriteRowAsync(NpgsqlBinaryImporter writer, Hy r, CancellationToken ct)
    {
        await WriteText(writer, r.RecordSpec, ct).ConfigureAwait(false);
        await WriteText(writer, r.DataKubun, ct).ConfigureAwait(false);
        await WriteDate(writer, r.MakeDate, ct).ConfigureAwait(false);
        await WriteText(writer, r.KettoNum, ct).ConfigureAwait(false);
        await WriteText(writer, r.Bamei, ct).ConfigureAwait(false);
        await WriteText(writer, r.Origin, ct).ConfigureAwait(false);
    }
}
