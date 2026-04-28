using System.Threading;
using System.Threading.Tasks;
using Jvlink2Db.Core.Records;
using Npgsql;

namespace Jvlink2Db.Db.Postgres.Records;

public sealed class PostgresCkWriter : PostgresBulkWriter<Ck>
{
    public PostgresCkWriter(NpgsqlDataSource dataSource, string? schemaName = null)
        : base(dataSource, schemaName, "ck", CkColumns.All, CkColumns.PrimaryKey)
    {
    }

    protected override async Task WriteRowAsync(NpgsqlBinaryImporter writer, Ck r, CancellationToken ct)
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
        await WriteText(writer, r.KettoNum, ct).ConfigureAwait(false);
        await WriteText(writer, r.Bamei, ct).ConfigureAwait(false);
        await WriteText(writer, r.RuikeiHonsyoHeiti, ct).ConfigureAwait(false);
        await WriteText(writer, r.RuikeiHonsyoSyogai, ct).ConfigureAwait(false);
        await WriteText(writer, r.RuikeiFukaHeichi, ct).ConfigureAwait(false);
        await WriteText(writer, r.RuikeiFukaSyogai, ct).ConfigureAwait(false);
        await WriteText(writer, r.RuikeiSyutokuHeichi, ct).ConfigureAwait(false);
        await WriteText(writer, r.RuikeiSyutokuSyogai, ct).ConfigureAwait(false);
        await WriteTextArray(writer, r.Kyakusitu, ct).ConfigureAwait(false);
        await WriteText(writer, r.RaceCount, ct).ConfigureAwait(false);
        await WriteText(writer, r.KisyuCode, ct).ConfigureAwait(false);
        await WriteText(writer, r.KisyuName, ct).ConfigureAwait(false);
        await WriteText(writer, r.ChokyosiCode, ct).ConfigureAwait(false);
        await WriteText(writer, r.ChokyosiName, ct).ConfigureAwait(false);
        await WriteText(writer, r.BanusiCode, ct).ConfigureAwait(false);
        await WriteText(writer, r.BanusiNameCo, ct).ConfigureAwait(false);
        await WriteText(writer, r.BanusiName, ct).ConfigureAwait(false);
        await WriteText(writer, r.BreederCode, ct).ConfigureAwait(false);
        await WriteText(writer, r.BreederNameCo, ct).ConfigureAwait(false);
        await WriteText(writer, r.BreederName, ct).ConfigureAwait(false);
    }
}
