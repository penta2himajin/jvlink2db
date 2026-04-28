using System.Threading;
using System.Threading.Tasks;
using Jvlink2Db.Core.Records;
using Npgsql;

namespace Jvlink2Db.Db.Postgres.Records;

public sealed class PostgresHsWriter : PostgresBulkWriter<Hs>
{
    public PostgresHsWriter(NpgsqlDataSource dataSource, string? schemaName = null)
        : base(dataSource, schemaName, "hs", HsColumns.All, HsColumns.PrimaryKey)
    {
    }

    protected override async Task WriteRowAsync(NpgsqlBinaryImporter writer, Hs r, CancellationToken ct)
    {
        await WriteText(writer, r.RecordSpec, ct).ConfigureAwait(false);
        await WriteText(writer, r.DataKubun, ct).ConfigureAwait(false);
        await WriteDate(writer, r.MakeDate, ct).ConfigureAwait(false);
        await WriteText(writer, r.KettoNum, ct).ConfigureAwait(false);
        await WriteText(writer, r.HansyokuFNum, ct).ConfigureAwait(false);
        await WriteText(writer, r.HansyokuMNum, ct).ConfigureAwait(false);
        await WriteText(writer, r.BirthYear, ct).ConfigureAwait(false);
        await WriteText(writer, r.SaleCode, ct).ConfigureAwait(false);
        await WriteText(writer, r.SaleHostName, ct).ConfigureAwait(false);
        await WriteText(writer, r.SaleName, ct).ConfigureAwait(false);
        await WriteDate(writer, r.FromDate, ct).ConfigureAwait(false);
        await WriteDate(writer, r.ToDate, ct).ConfigureAwait(false);
        await WriteText(writer, r.Barei, ct).ConfigureAwait(false);
        await WriteLong(writer, r.Price, ct).ConfigureAwait(false);
    }
}
