using System.Threading;
using System.Threading.Tasks;
using Jvlink2Db.Core.Records;
using Npgsql;

namespace Jvlink2Db.Db.Postgres.Records;

public sealed class PostgresH6Writer : PostgresBulkWriter<H6>
{
    public PostgresH6Writer(NpgsqlDataSource dataSource, string? schemaName = null)
        : base(dataSource, schemaName, "h6", H6Columns.All, H6Columns.PrimaryKey)
    {
    }

    protected override async Task WriteRowAsync(NpgsqlBinaryImporter writer, H6 r, CancellationToken ct)
    {
        await WriteText(writer, r.RecordSpec, ct).ConfigureAwait(false);
        await WriteText(writer, r.DataKubun, ct).ConfigureAwait(false);
        await WriteDate(writer, r.MakeDate, ct).ConfigureAwait(false);
        await WriteText(writer, r.Year, ct).ConfigureAwait(false);
        await WriteText(writer, r.MonthDay, ct).ConfigureAwait(false);
        await WriteText(writer, r.JyoCD, ct).ConfigureAwait(false);
        await WriteText(writer, r.Kaiji, ct).ConfigureAwait(false);
        await WriteText(writer, r.Nichiji, ct).ConfigureAwait(false);
        await WriteText(writer, r.RaceNum, ct).ConfigureAwait(false);
        await WriteShort(writer, r.TorokuTosu, ct).ConfigureAwait(false);
        await WriteShort(writer, r.SyussoTosu, ct).ConfigureAwait(false);

        await WriteText(writer, r.HatubaiFlag, ct).ConfigureAwait(false);
        await WriteTextArray(writer, r.HenkanUma, ct).ConfigureAwait(false);

        await WriteTextArray(writer, r.SanrentanKumi, ct).ConfigureAwait(false);
        await WriteLongArray(writer, r.SanrentanHyo, ct).ConfigureAwait(false);
        await WriteShortArray(writer, r.SanrentanNinki, ct).ConfigureAwait(false);

        await WriteLongArray(writer, r.HyoTotal, ct).ConfigureAwait(false);
    }
}
