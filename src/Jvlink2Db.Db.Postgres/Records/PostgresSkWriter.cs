using System.Threading;
using System.Threading.Tasks;
using Jvlink2Db.Core.Records;
using Npgsql;

namespace Jvlink2Db.Db.Postgres.Records;

public sealed class PostgresSkWriter : PostgresBulkWriter<Sk>
{
    public PostgresSkWriter(NpgsqlDataSource dataSource, string? schemaName = null)
        : base(dataSource, schemaName, "sk", SkColumns.All, SkColumns.PrimaryKey)
    {
    }

    protected override async Task WriteRowAsync(NpgsqlBinaryImporter writer, Sk r, CancellationToken ct)
    {
        await WriteText(writer, r.RecordSpec, ct).ConfigureAwait(false);
        await WriteText(writer, r.DataKubun, ct).ConfigureAwait(false);
        await WriteDate(writer, r.MakeDate, ct).ConfigureAwait(false);
        await WriteText(writer, r.KettoNum, ct).ConfigureAwait(false);
        await WriteDate(writer, r.BirthDate, ct).ConfigureAwait(false);
        await WriteText(writer, r.SexCD, ct).ConfigureAwait(false);
        await WriteText(writer, r.HinsyuCD, ct).ConfigureAwait(false);
        await WriteText(writer, r.KeiroCD, ct).ConfigureAwait(false);
        await WriteText(writer, r.SankuMochiKubun, ct).ConfigureAwait(false);
        await WriteText(writer, r.ImportYear, ct).ConfigureAwait(false);
        await WriteText(writer, r.BreederCode, ct).ConfigureAwait(false);
        await WriteText(writer, r.SanchiName, ct).ConfigureAwait(false);
        await WriteTextArray(writer, r.HansyokuNum, ct).ConfigureAwait(false);
    }
}
