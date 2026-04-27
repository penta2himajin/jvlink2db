using System.Threading;
using System.Threading.Tasks;
using Jvlink2Db.Core.Records;
using Npgsql;

namespace Jvlink2Db.Db.Postgres.Records;

public sealed class PostgresChWriter : PostgresBulkWriter<Ch>
{
    public PostgresChWriter(NpgsqlDataSource dataSource, string? schemaName = null)
        : base(dataSource, schemaName, "ch", ChColumns.All, ChColumns.PrimaryKey)
    {
    }

    protected override async Task WriteRowAsync(NpgsqlBinaryImporter writer, Ch r, CancellationToken ct)
    {
        await WriteText(writer, r.RecordSpec, ct).ConfigureAwait(false);
        await WriteText(writer, r.DataKubun, ct).ConfigureAwait(false);
        await WriteDate(writer, r.MakeDate, ct).ConfigureAwait(false);

        await WriteText(writer, r.ChokyosiCode, ct).ConfigureAwait(false);
        await WriteText(writer, r.DelKubun, ct).ConfigureAwait(false);
        await WriteDate(writer, r.IssueDate, ct).ConfigureAwait(false);
        await WriteDate(writer, r.DelDate, ct).ConfigureAwait(false);
        await WriteDate(writer, r.BirthDate, ct).ConfigureAwait(false);
        await WriteText(writer, r.ChokyosiName, ct).ConfigureAwait(false);
        await WriteText(writer, r.ChokyosiNameKana, ct).ConfigureAwait(false);
        await WriteText(writer, r.ChokyosiRyakusyo, ct).ConfigureAwait(false);
        await WriteText(writer, r.ChokyosiNameEng, ct).ConfigureAwait(false);
        await WriteText(writer, r.SexCD, ct).ConfigureAwait(false);
        await WriteText(writer, r.TozaiCD, ct).ConfigureAwait(false);
        await WriteText(writer, r.Syotai, ct).ConfigureAwait(false);

        await WriteTextArray(writer, r.SaikinJyusyoYear, ct).ConfigureAwait(false);
        await WriteTextArray(writer, r.SaikinJyusyoMonthDay, ct).ConfigureAwait(false);
        await WriteTextArray(writer, r.SaikinJyusyoJyoCD, ct).ConfigureAwait(false);
        await WriteTextArray(writer, r.SaikinJyusyoKaiji, ct).ConfigureAwait(false);
        await WriteTextArray(writer, r.SaikinJyusyoNichiji, ct).ConfigureAwait(false);
        await WriteTextArray(writer, r.SaikinJyusyoRaceNum, ct).ConfigureAwait(false);
        await WriteTextArray(writer, r.SaikinJyusyoHondai, ct).ConfigureAwait(false);
        await WriteTextArray(writer, r.SaikinJyusyoRyakusyo10, ct).ConfigureAwait(false);
        await WriteTextArray(writer, r.SaikinJyusyoRyakusyo6, ct).ConfigureAwait(false);
        await WriteTextArray(writer, r.SaikinJyusyoRyakusyo3, ct).ConfigureAwait(false);
        await WriteTextArray(writer, r.SaikinJyusyoGradeCD, ct).ConfigureAwait(false);
        await WriteTextArray(writer, r.SaikinJyusyoSyussoTosu, ct).ConfigureAwait(false);
        await WriteTextArray(writer, r.SaikinJyusyoKettoNum, ct).ConfigureAwait(false);
        await WriteTextArray(writer, r.SaikinJyusyoBamei, ct).ConfigureAwait(false);
    }
}
