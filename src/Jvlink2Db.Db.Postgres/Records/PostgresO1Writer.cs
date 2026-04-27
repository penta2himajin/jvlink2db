using System.Threading;
using System.Threading.Tasks;
using Jvlink2Db.Core.Records;
using Npgsql;

namespace Jvlink2Db.Db.Postgres.Records;

public sealed class PostgresO1Writer : PostgresBulkWriter<O1>
{
    public PostgresO1Writer(NpgsqlDataSource dataSource, string? schemaName = null)
        : base(dataSource, schemaName, "o1", O1Columns.All, O1Columns.PrimaryKey)
    {
    }

    protected override async Task WriteRowAsync(NpgsqlBinaryImporter writer, O1 r, CancellationToken ct)
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

        await WriteText(writer, r.TansyoFlag, ct).ConfigureAwait(false);
        await WriteText(writer, r.FukusyoFlag, ct).ConfigureAwait(false);
        await WriteText(writer, r.WakurenFlag, ct).ConfigureAwait(false);
        await WriteText(writer, r.FukuChakuBaraiKey, ct).ConfigureAwait(false);

        await WriteTextArray(writer, r.TansyoUmaban, ct).ConfigureAwait(false);
        await WriteIntArray(writer, r.TansyoOdds, ct).ConfigureAwait(false);
        await WriteShortArray(writer, r.TansyoNinki, ct).ConfigureAwait(false);

        await WriteTextArray(writer, r.FukusyoUmaban, ct).ConfigureAwait(false);
        await WriteIntArray(writer, r.FukusyoOddsLow, ct).ConfigureAwait(false);
        await WriteIntArray(writer, r.FukusyoOddsHigh, ct).ConfigureAwait(false);
        await WriteShortArray(writer, r.FukusyoNinki, ct).ConfigureAwait(false);

        await WriteTextArray(writer, r.WakurenKumi, ct).ConfigureAwait(false);
        await WriteIntArray(writer, r.WakurenOdds, ct).ConfigureAwait(false);
        await WriteShortArray(writer, r.WakurenNinki, ct).ConfigureAwait(false);

        await WriteLong(writer, r.TotalHyosuTansyo, ct).ConfigureAwait(false);
        await WriteLong(writer, r.TotalHyosuFukusyo, ct).ConfigureAwait(false);
        await WriteLong(writer, r.TotalHyosuWakuren, ct).ConfigureAwait(false);
    }
}
