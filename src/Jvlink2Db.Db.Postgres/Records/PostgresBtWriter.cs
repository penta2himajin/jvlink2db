using System.Threading;
using System.Threading.Tasks;
using Jvlink2Db.Core.Records;
using Npgsql;

namespace Jvlink2Db.Db.Postgres.Records;

public sealed class PostgresBtWriter : PostgresBulkWriter<Bt>
{
    public PostgresBtWriter(NpgsqlDataSource dataSource, string? schemaName = null)
        : base(dataSource, schemaName, "bt", BtColumns.All, BtColumns.PrimaryKey)
    {
    }

    protected override async Task WriteRowAsync(NpgsqlBinaryImporter writer, Bt r, CancellationToken ct)
    {
        await WriteText(writer, r.RecordSpec, ct).ConfigureAwait(false);
        await WriteText(writer, r.DataKubun, ct).ConfigureAwait(false);
        await WriteDate(writer, r.MakeDate, ct).ConfigureAwait(false);
        await WriteText(writer, r.HansyokuNum, ct).ConfigureAwait(false);
        await WriteText(writer, r.KeitoId, ct).ConfigureAwait(false);
        await WriteText(writer, r.KeitoName, ct).ConfigureAwait(false);
        await WriteText(writer, r.KeitoEx, ct).ConfigureAwait(false);
    }
}
