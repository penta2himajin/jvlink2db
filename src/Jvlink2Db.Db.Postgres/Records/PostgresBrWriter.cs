using System.Threading;
using System.Threading.Tasks;
using Jvlink2Db.Core.Records;
using Npgsql;

namespace Jvlink2Db.Db.Postgres.Records;

public sealed class PostgresBrWriter : PostgresBulkWriter<Br>
{
    public PostgresBrWriter(NpgsqlDataSource dataSource, string? schemaName = null)
        : base(dataSource, schemaName, "br", BrColumns.All, BrColumns.PrimaryKey)
    {
    }

    protected override async Task WriteRowAsync(NpgsqlBinaryImporter writer, Br r, CancellationToken ct)
    {
        await WriteText(writer, r.RecordSpec, ct).ConfigureAwait(false);
        await WriteText(writer, r.DataKubun, ct).ConfigureAwait(false);
        await WriteDate(writer, r.MakeDate, ct).ConfigureAwait(false);

        await WriteText(writer, r.BreederCode, ct).ConfigureAwait(false);
        await WriteText(writer, r.BreederNameCo, ct).ConfigureAwait(false);
        await WriteText(writer, r.BreederName, ct).ConfigureAwait(false);
        await WriteText(writer, r.BreederNameKana, ct).ConfigureAwait(false);
        await WriteText(writer, r.BreederNameEng, ct).ConfigureAwait(false);
        await WriteText(writer, r.Address, ct).ConfigureAwait(false);

        await WriteTextArray(writer, r.HonRuikeiSetYear, ct).ConfigureAwait(false);
        await WriteLongArray(writer, r.HonRuikeiHonsyokinTotal, ct).ConfigureAwait(false);
        await WriteLongArray(writer, r.HonRuikeiFukaSyokin, ct).ConfigureAwait(false);
        await WriteIntArray(writer, r.HonRuikeiChakuKaisu, ct).ConfigureAwait(false);
    }
}
