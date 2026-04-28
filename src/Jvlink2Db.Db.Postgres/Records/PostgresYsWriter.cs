using System.Threading;
using System.Threading.Tasks;
using Jvlink2Db.Core.Records;
using Npgsql;

namespace Jvlink2Db.Db.Postgres.Records;

public sealed class PostgresYsWriter : PostgresBulkWriter<Ys>
{
    public PostgresYsWriter(NpgsqlDataSource dataSource, string? schemaName = null)
        : base(dataSource, schemaName, "ys", YsColumns.All, YsColumns.PrimaryKey)
    {
    }

    protected override async Task WriteRowAsync(NpgsqlBinaryImporter writer, Ys r, CancellationToken ct)
    {
        await WriteText(writer, r.RecordSpec, ct).ConfigureAwait(false);
        await WriteText(writer, r.DataKubun, ct).ConfigureAwait(false);
        await WriteDate(writer, r.MakeDate, ct).ConfigureAwait(false);
        await WriteText(writer, r.Year, ct).ConfigureAwait(false);
        await WriteText(writer, r.MonthDay, ct).ConfigureAwait(false);
        await WriteText(writer, r.JyoCD, ct).ConfigureAwait(false);
        await WriteText(writer, r.Kaiji, ct).ConfigureAwait(false);
        await WriteText(writer, r.Nichiji, ct).ConfigureAwait(false);
        await WriteText(writer, r.YoubiCD, ct).ConfigureAwait(false);
        await WriteTextArray(writer, r.JyusyoTokuNum, ct).ConfigureAwait(false);
        await WriteTextArray(writer, r.JyusyoHondai, ct).ConfigureAwait(false);
        await WriteTextArray(writer, r.JyusyoRyakusyo10, ct).ConfigureAwait(false);
        await WriteTextArray(writer, r.JyusyoRyakusyo6, ct).ConfigureAwait(false);
        await WriteTextArray(writer, r.JyusyoRyakusyo3, ct).ConfigureAwait(false);
        await WriteTextArray(writer, r.JyusyoNkai, ct).ConfigureAwait(false);
        await WriteTextArray(writer, r.JyusyoGradeCD, ct).ConfigureAwait(false);
        await WriteTextArray(writer, r.JyusyoSyubetuCD, ct).ConfigureAwait(false);
        await WriteTextArray(writer, r.JyusyoKigoCD, ct).ConfigureAwait(false);
        await WriteTextArray(writer, r.JyusyoJyuryoCD, ct).ConfigureAwait(false);
        await WriteTextArray(writer, r.JyusyoKyori, ct).ConfigureAwait(false);
        await WriteTextArray(writer, r.JyusyoTrackCD, ct).ConfigureAwait(false);
    }
}
