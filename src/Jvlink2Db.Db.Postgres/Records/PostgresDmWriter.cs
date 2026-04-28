using System.Threading;
using System.Threading.Tasks;
using Jvlink2Db.Core.Records;
using Npgsql;

namespace Jvlink2Db.Db.Postgres.Records;

public sealed class PostgresDmWriter : PostgresBulkWriter<Dm>
{
    public PostgresDmWriter(NpgsqlDataSource dataSource, string? schemaName = null)
        : base(dataSource, schemaName, "dm", DmColumns.All, DmColumns.PrimaryKey)
    {
    }

    protected override async Task WriteRowAsync(NpgsqlBinaryImporter writer, Dm r, CancellationToken ct)
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
        await WriteText(writer, r.MakeHM, ct).ConfigureAwait(false);
        await WriteTextArray(writer, r.Umaban, ct).ConfigureAwait(false);
        await WriteIntArray(writer, r.DMTime, ct).ConfigureAwait(false);
        await WriteShortArray(writer, r.DMGosaP, ct).ConfigureAwait(false);
        await WriteShortArray(writer, r.DMGosaM, ct).ConfigureAwait(false);
    }
}
