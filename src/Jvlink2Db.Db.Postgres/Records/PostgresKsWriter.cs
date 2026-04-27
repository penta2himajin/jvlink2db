using System.Threading;
using System.Threading.Tasks;
using Jvlink2Db.Core.Records;
using Npgsql;

namespace Jvlink2Db.Db.Postgres.Records;

public sealed class PostgresKsWriter : PostgresBulkWriter<Ks>
{
    public PostgresKsWriter(NpgsqlDataSource dataSource, string? schemaName = null)
        : base(dataSource, schemaName, "ks", KsColumns.All, KsColumns.PrimaryKey)
    {
    }

    protected override async Task WriteRowAsync(NpgsqlBinaryImporter writer, Ks r, CancellationToken ct)
    {
        await WriteText(writer, r.RecordSpec, ct).ConfigureAwait(false);
        await WriteText(writer, r.DataKubun, ct).ConfigureAwait(false);
        await WriteDate(writer, r.MakeDate, ct).ConfigureAwait(false);

        await WriteText(writer, r.KisyuCode, ct).ConfigureAwait(false);
        await WriteText(writer, r.DelKubun, ct).ConfigureAwait(false);
        await WriteDate(writer, r.IssueDate, ct).ConfigureAwait(false);
        await WriteDate(writer, r.DelDate, ct).ConfigureAwait(false);
        await WriteDate(writer, r.BirthDate, ct).ConfigureAwait(false);
        await WriteText(writer, r.KisyuName, ct).ConfigureAwait(false);
        await WriteText(writer, r.Reserved, ct).ConfigureAwait(false);
        await WriteText(writer, r.KisyuNameKana, ct).ConfigureAwait(false);
        await WriteText(writer, r.KisyuRyakusyo, ct).ConfigureAwait(false);
        await WriteText(writer, r.KisyuNameEng, ct).ConfigureAwait(false);
        await WriteText(writer, r.SexCD, ct).ConfigureAwait(false);
        await WriteText(writer, r.SikakuCD, ct).ConfigureAwait(false);
        await WriteText(writer, r.MinaraiCD, ct).ConfigureAwait(false);
        await WriteText(writer, r.TozaiCD, ct).ConfigureAwait(false);
        await WriteText(writer, r.Syotai, ct).ConfigureAwait(false);
        await WriteText(writer, r.ChokyosiCode, ct).ConfigureAwait(false);
        await WriteText(writer, r.ChokyosiRyakusyo, ct).ConfigureAwait(false);

        await WriteTextArray(writer, r.HatuKiJyoYear, ct).ConfigureAwait(false);
        await WriteTextArray(writer, r.HatuKiJyoMonthDay, ct).ConfigureAwait(false);
        await WriteTextArray(writer, r.HatuKiJyoJyoCD, ct).ConfigureAwait(false);
        await WriteTextArray(writer, r.HatuKiJyoKaiji, ct).ConfigureAwait(false);
        await WriteTextArray(writer, r.HatuKiJyoNichiji, ct).ConfigureAwait(false);
        await WriteTextArray(writer, r.HatuKiJyoRaceNum, ct).ConfigureAwait(false);
        await WriteTextArray(writer, r.HatuKiJyoSyussoTosu, ct).ConfigureAwait(false);
        await WriteTextArray(writer, r.HatuKiJyoKettoNum, ct).ConfigureAwait(false);
        await WriteTextArray(writer, r.HatuKiJyoBamei, ct).ConfigureAwait(false);
        await WriteTextArray(writer, r.HatuKiJyoKakuteiJyuni, ct).ConfigureAwait(false);
        await WriteTextArray(writer, r.HatuKiJyoIJyoCD, ct).ConfigureAwait(false);

        await WriteTextArray(writer, r.HatuSyoriYear, ct).ConfigureAwait(false);
        await WriteTextArray(writer, r.HatuSyoriMonthDay, ct).ConfigureAwait(false);
        await WriteTextArray(writer, r.HatuSyoriJyoCD, ct).ConfigureAwait(false);
        await WriteTextArray(writer, r.HatuSyoriKaiji, ct).ConfigureAwait(false);
        await WriteTextArray(writer, r.HatuSyoriNichiji, ct).ConfigureAwait(false);
        await WriteTextArray(writer, r.HatuSyoriRaceNum, ct).ConfigureAwait(false);
        await WriteTextArray(writer, r.HatuSyoriSyussoTosu, ct).ConfigureAwait(false);
        await WriteTextArray(writer, r.HatuSyoriKettoNum, ct).ConfigureAwait(false);
        await WriteTextArray(writer, r.HatuSyoriBamei, ct).ConfigureAwait(false);

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
