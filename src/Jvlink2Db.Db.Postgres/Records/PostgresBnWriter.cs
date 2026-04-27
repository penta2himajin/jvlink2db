using System.Threading;
using System.Threading.Tasks;
using Jvlink2Db.Core.Records;
using Npgsql;

namespace Jvlink2Db.Db.Postgres.Records;

public sealed class PostgresBnWriter : PostgresBulkWriter<Bn>
{
    public PostgresBnWriter(NpgsqlDataSource dataSource, string? schemaName = null)
        : base(dataSource, schemaName, "bn", BnColumns.All, BnColumns.PrimaryKey)
    {
    }

    protected override async Task WriteRowAsync(NpgsqlBinaryImporter writer, Bn r, CancellationToken ct)
    {
        await WriteText(writer, r.RecordSpec, ct).ConfigureAwait(false);
        await WriteText(writer, r.DataKubun, ct).ConfigureAwait(false);
        await WriteDate(writer, r.MakeDate, ct).ConfigureAwait(false);

        await WriteText(writer, r.BanusiCode, ct).ConfigureAwait(false);
        await WriteText(writer, r.BanusiNameCo, ct).ConfigureAwait(false);
        await WriteText(writer, r.BanusiName, ct).ConfigureAwait(false);
        await WriteText(writer, r.BanusiNameKana, ct).ConfigureAwait(false);
        await WriteText(writer, r.BanusiNameEng, ct).ConfigureAwait(false);
        await WriteText(writer, r.Fukusyoku, ct).ConfigureAwait(false);

        await WriteTextArray(writer, r.HonRuikeiSetYear, ct).ConfigureAwait(false);
        await WriteLongArray(writer, r.HonRuikeiHonsyokinTotal, ct).ConfigureAwait(false);
        await WriteLongArray(writer, r.HonRuikeiFukaSyokin, ct).ConfigureAwait(false);
        await WriteIntArray(writer, r.HonRuikeiChakuKaisu, ct).ConfigureAwait(false);
    }
}
