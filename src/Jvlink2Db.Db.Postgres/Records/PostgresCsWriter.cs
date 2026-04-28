using System.Threading;
using System.Threading.Tasks;
using Jvlink2Db.Core.Records;
using Npgsql;

namespace Jvlink2Db.Db.Postgres.Records;

public sealed class PostgresCsWriter : PostgresBulkWriter<Cs>
{
    public PostgresCsWriter(NpgsqlDataSource dataSource, string? schemaName = null)
        : base(dataSource, schemaName, "cs", CsColumns.All, CsColumns.PrimaryKey)
    {
    }

    protected override async Task WriteRowAsync(NpgsqlBinaryImporter writer, Cs r, CancellationToken ct)
    {
        await WriteText(writer, r.RecordSpec, ct).ConfigureAwait(false);
        await WriteText(writer, r.DataKubun, ct).ConfigureAwait(false);
        await WriteDate(writer, r.MakeDate, ct).ConfigureAwait(false);
        await WriteText(writer, r.JyoCD, ct).ConfigureAwait(false);
        await WriteText(writer, r.Kyori, ct).ConfigureAwait(false);
        await WriteText(writer, r.TrackCD, ct).ConfigureAwait(false);
        await WriteDate(writer, r.KaishuDate, ct).ConfigureAwait(false);
        await WriteText(writer, r.CourseEx, ct).ConfigureAwait(false);
    }
}
