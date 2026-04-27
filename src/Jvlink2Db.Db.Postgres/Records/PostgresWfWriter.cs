using System.Threading;
using System.Threading.Tasks;
using Jvlink2Db.Core.Records;
using Npgsql;

namespace Jvlink2Db.Db.Postgres.Records;

public sealed class PostgresWfWriter : PostgresBulkWriter<Wf>
{
    public PostgresWfWriter(NpgsqlDataSource dataSource, string? schemaName = null)
        : base(dataSource, schemaName, "wf", WfColumns.All, WfColumns.PrimaryKey)
    {
    }

    protected override async Task WriteRowAsync(NpgsqlBinaryImporter writer, Wf wf, CancellationToken ct)
    {
        await WriteText(writer, wf.RecordSpec, ct).ConfigureAwait(false);
        await WriteText(writer, wf.DataKubun, ct).ConfigureAwait(false);
        await WriteDate(writer, wf.MakeDate, ct).ConfigureAwait(false);

        await WriteDate(writer, wf.KaisaiDate, ct).ConfigureAwait(false);
        await WriteText(writer, wf.Reserved1, ct).ConfigureAwait(false);

        await WriteTextArray(writer, wf.RaceJyoCD, ct).ConfigureAwait(false);
        await WriteTextArray(writer, wf.RaceKaiji, ct).ConfigureAwait(false);
        await WriteTextArray(writer, wf.RaceNichiji, ct).ConfigureAwait(false);
        await WriteTextArray(writer, wf.RaceNum, ct).ConfigureAwait(false);

        await WriteText(writer, wf.Reserved2, ct).ConfigureAwait(false);
        await WriteLong(writer, wf.HatsubaiHyo, ct).ConfigureAwait(false);
        await WriteLongArray(writer, wf.YukoHyo, ct).ConfigureAwait(false);

        await WriteText(writer, wf.HenkanFlag, ct).ConfigureAwait(false);
        await WriteText(writer, wf.FuseiritsuFlag, ct).ConfigureAwait(false);
        await WriteText(writer, wf.TekichunashiFlag, ct).ConfigureAwait(false);
        await WriteLong(writer, wf.COShoki, ct).ConfigureAwait(false);
        await WriteLong(writer, wf.COZanDaka, ct).ConfigureAwait(false);

        await WriteTextArray(writer, wf.PayKumiban, ct).ConfigureAwait(false);
        await WriteIntArray(writer, wf.PayAmount, ct).ConfigureAwait(false);
        await WriteLongArray(writer, wf.PayTekichuHyo, ct).ConfigureAwait(false);
    }
}
