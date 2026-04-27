using System.Threading;
using System.Threading.Tasks;
using Jvlink2Db.Core.Records;
using Npgsql;

namespace Jvlink2Db.Db.Postgres.Records;

public sealed class PostgresHnWriter : PostgresBulkWriter<Hn>
{
    public PostgresHnWriter(NpgsqlDataSource dataSource, string? schemaName = null)
        : base(dataSource, schemaName, "hn", HnColumns.All, HnColumns.PrimaryKey)
    {
    }

    protected override async Task WriteRowAsync(NpgsqlBinaryImporter writer, Hn r, CancellationToken ct)
    {
        await WriteText(writer, r.RecordSpec, ct).ConfigureAwait(false);
        await WriteText(writer, r.DataKubun, ct).ConfigureAwait(false);
        await WriteDate(writer, r.MakeDate, ct).ConfigureAwait(false);
        await WriteText(writer, r.HansyokuNum, ct).ConfigureAwait(false);
        await WriteText(writer, r.Reserved, ct).ConfigureAwait(false);
        await WriteText(writer, r.KettoNum, ct).ConfigureAwait(false);
        await WriteText(writer, r.DelKubun, ct).ConfigureAwait(false);
        await WriteText(writer, r.Bamei, ct).ConfigureAwait(false);
        await WriteText(writer, r.BameiKana, ct).ConfigureAwait(false);
        await WriteText(writer, r.BameiEng, ct).ConfigureAwait(false);
        await WriteText(writer, r.BirthYear, ct).ConfigureAwait(false);
        await WriteText(writer, r.SexCD, ct).ConfigureAwait(false);
        await WriteText(writer, r.HinsyuCD, ct).ConfigureAwait(false);
        await WriteText(writer, r.KeiroCD, ct).ConfigureAwait(false);
        await WriteText(writer, r.HansyokuMochiKubun, ct).ConfigureAwait(false);
        await WriteText(writer, r.ImportYear, ct).ConfigureAwait(false);
        await WriteText(writer, r.SanchiName, ct).ConfigureAwait(false);
        await WriteText(writer, r.HansyokuFNum, ct).ConfigureAwait(false);
        await WriteText(writer, r.HansyokuMNum, ct).ConfigureAwait(false);
    }
}
