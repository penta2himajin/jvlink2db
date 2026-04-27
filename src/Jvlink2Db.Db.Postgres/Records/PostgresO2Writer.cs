using System.Threading;
using System.Threading.Tasks;
using Jvlink2Db.Core.Records;
using Npgsql;

namespace Jvlink2Db.Db.Postgres.Records;

public sealed class PostgresO2Writer : PostgresBulkWriter<O2>
{
    public PostgresO2Writer(NpgsqlDataSource dataSource, string? schemaName = null)
        : base(dataSource, schemaName, "o2", O2Columns.All, O2Columns.PrimaryKey)
    {
    }

    protected override async Task WriteRowAsync(NpgsqlBinaryImporter writer, O2 r, CancellationToken ct)
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
        await WriteText(writer, r.HappyoTime, ct).ConfigureAwait(false);
        await WriteShort(writer, r.TorokuTosu, ct).ConfigureAwait(false);
        await WriteShort(writer, r.SyussoTosu, ct).ConfigureAwait(false);
        await WriteText(writer, r.UmarenFlag, ct).ConfigureAwait(false);

        await WriteTextArray(writer, r.Kumi, ct).ConfigureAwait(false);
        await WriteIntArray(writer, r.Odds, ct).ConfigureAwait(false);
        await WriteShortArray(writer, r.Ninki, ct).ConfigureAwait(false);

        await WriteLong(writer, r.TotalHyosuUmaren, ct).ConfigureAwait(false);
    }
}
