using System.Threading;
using System.Threading.Tasks;
using Jvlink2Db.Core.Records;
using Npgsql;

namespace Jvlink2Db.Db.Postgres.Records;

public sealed class PostgresH1Writer : PostgresBulkWriter<H1>
{
    public PostgresH1Writer(NpgsqlDataSource dataSource, string? schemaName = null)
        : base(dataSource, schemaName, "h1", H1Columns.All, H1Columns.PrimaryKey)
    {
    }

    protected override async Task WriteRowAsync(NpgsqlBinaryImporter writer, H1 r, CancellationToken ct)
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

        await WriteTextArray(writer, r.HatubaiFlag, ct).ConfigureAwait(false);
        await WriteText(writer, r.FukuChakuBaraiKey, ct).ConfigureAwait(false);
        await WriteTextArray(writer, r.HenkanUma, ct).ConfigureAwait(false);
        await WriteTextArray(writer, r.HenkanWaku, ct).ConfigureAwait(false);
        await WriteTextArray(writer, r.HenkanDoWaku, ct).ConfigureAwait(false);

        await WriteTextArray(writer, r.TansyoUmaban, ct).ConfigureAwait(false);
        await WriteLongArray(writer, r.TansyoHyo, ct).ConfigureAwait(false);
        await WriteShortArray(writer, r.TansyoNinki, ct).ConfigureAwait(false);

        await WriteTextArray(writer, r.FukusyoUmaban, ct).ConfigureAwait(false);
        await WriteLongArray(writer, r.FukusyoHyo, ct).ConfigureAwait(false);
        await WriteShortArray(writer, r.FukusyoNinki, ct).ConfigureAwait(false);

        await WriteTextArray(writer, r.WakurenKumi, ct).ConfigureAwait(false);
        await WriteLongArray(writer, r.WakurenHyo, ct).ConfigureAwait(false);
        await WriteShortArray(writer, r.WakurenNinki, ct).ConfigureAwait(false);

        await WriteTextArray(writer, r.UmarenKumi, ct).ConfigureAwait(false);
        await WriteLongArray(writer, r.UmarenHyo, ct).ConfigureAwait(false);
        await WriteShortArray(writer, r.UmarenNinki, ct).ConfigureAwait(false);

        await WriteTextArray(writer, r.WideKumi, ct).ConfigureAwait(false);
        await WriteLongArray(writer, r.WideHyo, ct).ConfigureAwait(false);
        await WriteShortArray(writer, r.WideNinki, ct).ConfigureAwait(false);

        await WriteTextArray(writer, r.UmatanKumi, ct).ConfigureAwait(false);
        await WriteLongArray(writer, r.UmatanHyo, ct).ConfigureAwait(false);
        await WriteShortArray(writer, r.UmatanNinki, ct).ConfigureAwait(false);

        await WriteTextArray(writer, r.SanrenpukuKumi, ct).ConfigureAwait(false);
        await WriteLongArray(writer, r.SanrenpukuHyo, ct).ConfigureAwait(false);
        await WriteShortArray(writer, r.SanrenpukuNinki, ct).ConfigureAwait(false);

        await WriteLongArray(writer, r.HyoTotal, ct).ConfigureAwait(false);
    }
}
