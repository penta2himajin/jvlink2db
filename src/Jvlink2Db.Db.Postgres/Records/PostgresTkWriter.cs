using System.Threading;
using System.Threading.Tasks;
using Jvlink2Db.Core.Records;
using Npgsql;

namespace Jvlink2Db.Db.Postgres.Records;

public sealed class PostgresTkWriter : PostgresBulkWriter<Tk>
{
    public PostgresTkWriter(NpgsqlDataSource dataSource, string? schemaName = null)
        : base(dataSource, schemaName, "tk", TkColumns.All, TkColumns.PrimaryKey)
    {
    }

    protected override async Task WriteRowAsync(NpgsqlBinaryImporter writer, Tk r, CancellationToken ct)
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
        await WriteText(writer, r.YoubiCD, ct).ConfigureAwait(false);
        await WriteText(writer, r.TokuNum, ct).ConfigureAwait(false);
        await WriteText(writer, r.Hondai, ct).ConfigureAwait(false);
        await WriteText(writer, r.Fukudai, ct).ConfigureAwait(false);
        await WriteText(writer, r.Kakko, ct).ConfigureAwait(false);
        await WriteText(writer, r.HondaiEng, ct).ConfigureAwait(false);
        await WriteText(writer, r.FukudaiEng, ct).ConfigureAwait(false);
        await WriteText(writer, r.KakkoEng, ct).ConfigureAwait(false);
        await WriteText(writer, r.Ryakusyo10, ct).ConfigureAwait(false);
        await WriteText(writer, r.Ryakusyo6, ct).ConfigureAwait(false);
        await WriteText(writer, r.Ryakusyo3, ct).ConfigureAwait(false);
        await WriteText(writer, r.Kubun, ct).ConfigureAwait(false);
        await WriteText(writer, r.Nkai, ct).ConfigureAwait(false);
        await WriteText(writer, r.GradeCD, ct).ConfigureAwait(false);
        await WriteText(writer, r.SyubetuCD, ct).ConfigureAwait(false);
        await WriteText(writer, r.KigoCD, ct).ConfigureAwait(false);
        await WriteText(writer, r.JyuryoCD, ct).ConfigureAwait(false);
        await WriteTextArray(writer, r.JyokenCD, ct).ConfigureAwait(false);
        await WriteText(writer, r.Kyori, ct).ConfigureAwait(false);
        await WriteText(writer, r.TrackCD, ct).ConfigureAwait(false);
        await WriteText(writer, r.CourseKubunCD, ct).ConfigureAwait(false);
        await WriteDate(writer, r.HandiDate, ct).ConfigureAwait(false);
        await WriteText(writer, r.TorokuTosu, ct).ConfigureAwait(false);
        await WriteTextArray(writer, r.TokuNumSeq, ct).ConfigureAwait(false);
        await WriteTextArray(writer, r.KettoNum, ct).ConfigureAwait(false);
        await WriteTextArray(writer, r.Bamei, ct).ConfigureAwait(false);
        await WriteTextArray(writer, r.UmaKigoCD, ct).ConfigureAwait(false);
        await WriteTextArray(writer, r.SexCD, ct).ConfigureAwait(false);
        await WriteTextArray(writer, r.TozaiCD, ct).ConfigureAwait(false);
        await WriteTextArray(writer, r.ChokyosiCode, ct).ConfigureAwait(false);
        await WriteTextArray(writer, r.ChokyosiRyakusyo, ct).ConfigureAwait(false);
        await WriteTextArray(writer, r.Futan, ct).ConfigureAwait(false);
        await WriteTextArray(writer, r.Koryu, ct).ConfigureAwait(false);
    }
}
