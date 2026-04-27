using System.Threading;
using System.Threading.Tasks;
using Jvlink2Db.Core.Records;
using Npgsql;

namespace Jvlink2Db.Db.Postgres.Records;

public sealed class PostgresRcWriter : PostgresBulkWriter<Rc>
{
    public PostgresRcWriter(NpgsqlDataSource dataSource, string? schemaName = null)
        : base(dataSource, schemaName, "rc", RcColumns.All, RcColumns.PrimaryKey)
    {
    }

    protected override async Task WriteRowAsync(NpgsqlBinaryImporter writer, Rc r, CancellationToken ct)
    {
        await WriteText(writer, r.RecordSpec, ct).ConfigureAwait(false);
        await WriteText(writer, r.DataKubun, ct).ConfigureAwait(false);
        await WriteDate(writer, r.MakeDate, ct).ConfigureAwait(false);

        await WriteText(writer, r.RecInfoKubun, ct).ConfigureAwait(false);

        await WriteText(writer, r.Year, ct).ConfigureAwait(false);
        await WriteText(writer, r.MonthDay, ct).ConfigureAwait(false);
        await WriteText(writer, r.JyoCD, ct).ConfigureAwait(false);
        await WriteText(writer, r.Kaiji, ct).ConfigureAwait(false);
        await WriteText(writer, r.Nichiji, ct).ConfigureAwait(false);
        await WriteText(writer, r.RaceNum, ct).ConfigureAwait(false);

        await WriteText(writer, r.TokuNum, ct).ConfigureAwait(false);
        await WriteText(writer, r.Hondai, ct).ConfigureAwait(false);
        await WriteText(writer, r.GradeCD, ct).ConfigureAwait(false);
        await WriteText(writer, r.SyubetuCD, ct).ConfigureAwait(false);
        await WriteShort(writer, r.Kyori, ct).ConfigureAwait(false);
        await WriteText(writer, r.TrackCD, ct).ConfigureAwait(false);
        await WriteText(writer, r.RecKubun, ct).ConfigureAwait(false);
        await WriteShort(writer, r.RecTime, ct).ConfigureAwait(false);

        await WriteText(writer, r.TenkoCD, ct).ConfigureAwait(false);
        await WriteText(writer, r.SibaBabaCD, ct).ConfigureAwait(false);
        await WriteText(writer, r.DirtBabaCD, ct).ConfigureAwait(false);

        await WriteTextArray(writer, r.RecUmaKettoNum, ct).ConfigureAwait(false);
        await WriteTextArray(writer, r.RecUmaBamei, ct).ConfigureAwait(false);
        await WriteTextArray(writer, r.RecUmaUmaKigoCD, ct).ConfigureAwait(false);
        await WriteTextArray(writer, r.RecUmaSexCD, ct).ConfigureAwait(false);
        await WriteTextArray(writer, r.RecUmaChokyosiCode, ct).ConfigureAwait(false);
        await WriteTextArray(writer, r.RecUmaChokyosiName, ct).ConfigureAwait(false);
        await WriteShortArray(writer, r.RecUmaFutan, ct).ConfigureAwait(false);
        await WriteTextArray(writer, r.RecUmaKisyuCode, ct).ConfigureAwait(false);
        await WriteTextArray(writer, r.RecUmaKisyuName, ct).ConfigureAwait(false);
    }
}
