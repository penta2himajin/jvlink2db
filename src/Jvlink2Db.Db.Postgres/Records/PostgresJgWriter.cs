using System.Threading;
using System.Threading.Tasks;
using Jvlink2Db.Core.Records;
using Npgsql;

namespace Jvlink2Db.Db.Postgres.Records;

public sealed class PostgresJgWriter : PostgresBulkWriter<Jg>
{
    public PostgresJgWriter(NpgsqlDataSource dataSource, string? schemaName = null)
        : base(dataSource, schemaName, "jg", JgColumns.All, JgColumns.PrimaryKey)
    {
    }

    protected override async Task WriteRowAsync(NpgsqlBinaryImporter writer, Jg jg, CancellationToken ct)
    {
        await WriteText(writer, jg.RecordSpec, ct).ConfigureAwait(false);
        await WriteText(writer, jg.DataKubun, ct).ConfigureAwait(false);
        await WriteDate(writer, jg.MakeDate, ct).ConfigureAwait(false);

        await WriteText(writer, jg.Year, ct).ConfigureAwait(false);
        await WriteText(writer, jg.MonthDay, ct).ConfigureAwait(false);
        await WriteText(writer, jg.JyoCD, ct).ConfigureAwait(false);
        await WriteText(writer, jg.Kaiji, ct).ConfigureAwait(false);
        await WriteText(writer, jg.Nichiji, ct).ConfigureAwait(false);
        await WriteText(writer, jg.RaceNum, ct).ConfigureAwait(false);

        await WriteText(writer, jg.KettoNum, ct).ConfigureAwait(false);
        await WriteText(writer, jg.Bamei, ct).ConfigureAwait(false);
        await WriteShort(writer, jg.ShutsubaTohyoJun, ct).ConfigureAwait(false);
        await WriteText(writer, jg.ShussoKubun, ct).ConfigureAwait(false);
        await WriteText(writer, jg.JogaiJotaiKubun, ct).ConfigureAwait(false);
    }
}
