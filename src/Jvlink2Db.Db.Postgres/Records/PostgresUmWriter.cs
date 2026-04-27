using System.Threading;
using System.Threading.Tasks;
using Jvlink2Db.Core.Records;
using Npgsql;

namespace Jvlink2Db.Db.Postgres.Records;

public sealed class PostgresUmWriter : PostgresBulkWriter<Um>
{
    public PostgresUmWriter(NpgsqlDataSource dataSource, string? schemaName = null)
        : base(dataSource, schemaName, "um", UmColumns.All, UmColumns.PrimaryKey)
    {
    }

    protected override async Task WriteRowAsync(NpgsqlBinaryImporter writer, Um r, CancellationToken ct)
    {
        await WriteText(writer, r.RecordSpec, ct).ConfigureAwait(false);
        await WriteText(writer, r.DataKubun, ct).ConfigureAwait(false);
        await WriteDate(writer, r.MakeDate, ct).ConfigureAwait(false);

        await WriteText(writer, r.KettoNum, ct).ConfigureAwait(false);
        await WriteText(writer, r.DelKubun, ct).ConfigureAwait(false);
        await WriteDate(writer, r.RegDate, ct).ConfigureAwait(false);
        await WriteDate(writer, r.DelDate, ct).ConfigureAwait(false);
        await WriteDate(writer, r.BirthDate, ct).ConfigureAwait(false);
        await WriteText(writer, r.Bamei, ct).ConfigureAwait(false);
        await WriteText(writer, r.BameiKana, ct).ConfigureAwait(false);
        await WriteText(writer, r.BameiEng, ct).ConfigureAwait(false);
        await WriteText(writer, r.ZaikyuFlag, ct).ConfigureAwait(false);
        await WriteText(writer, r.Reserved, ct).ConfigureAwait(false);
        await WriteText(writer, r.UmaKigoCD, ct).ConfigureAwait(false);
        await WriteText(writer, r.SexCD, ct).ConfigureAwait(false);
        await WriteText(writer, r.HinsyuCD, ct).ConfigureAwait(false);
        await WriteText(writer, r.KeiroCD, ct).ConfigureAwait(false);

        await WriteTextArray(writer, r.KettoHansyokuNum, ct).ConfigureAwait(false);
        await WriteTextArray(writer, r.KettoBamei, ct).ConfigureAwait(false);

        await WriteText(writer, r.TozaiCD, ct).ConfigureAwait(false);
        await WriteText(writer, r.ChokyosiCode, ct).ConfigureAwait(false);
        await WriteText(writer, r.ChokyosiRyakusyo, ct).ConfigureAwait(false);
        await WriteText(writer, r.Syotai, ct).ConfigureAwait(false);
        await WriteText(writer, r.BreederCode, ct).ConfigureAwait(false);
        await WriteText(writer, r.BreederName, ct).ConfigureAwait(false);
        await WriteText(writer, r.SanchiName, ct).ConfigureAwait(false);
        await WriteText(writer, r.BanusiCode, ct).ConfigureAwait(false);
        await WriteText(writer, r.BanusiName, ct).ConfigureAwait(false);

        await WriteLong(writer, r.RuikeiHonsyoHeiti, ct).ConfigureAwait(false);
        await WriteLong(writer, r.RuikeiHonsyoSyogai, ct).ConfigureAwait(false);
        await WriteLong(writer, r.RuikeiFukaHeichi, ct).ConfigureAwait(false);
        await WriteLong(writer, r.RuikeiFukaSyogai, ct).ConfigureAwait(false);
        await WriteLong(writer, r.RuikeiSyutokuHeichi, ct).ConfigureAwait(false);
        await WriteLong(writer, r.RuikeiSyutokuSyogai, ct).ConfigureAwait(false);

        await WriteIntArray(writer, r.ChakuSogo, ct).ConfigureAwait(false);
        await WriteIntArray(writer, r.ChakuChuo, ct).ConfigureAwait(false);
        await WriteIntArray(writer, r.ChakuKaisuBa, ct).ConfigureAwait(false);
        await WriteIntArray(writer, r.ChakuKaisuJyotai, ct).ConfigureAwait(false);
        await WriteIntArray(writer, r.ChakuKaisuKyori, ct).ConfigureAwait(false);

        await WriteTextArray(writer, r.Kyakusitu, ct).ConfigureAwait(false);
        await WriteShort(writer, r.RaceCount, ct).ConfigureAwait(false);
    }
}
