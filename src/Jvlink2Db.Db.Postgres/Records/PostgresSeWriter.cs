using System.Threading;
using System.Threading.Tasks;
using Jvlink2Db.Core.Records;
using Npgsql;

namespace Jvlink2Db.Db.Postgres.Records;

/// <summary>
/// Idempotent bulk writer for <see cref="Se"/> records.
/// </summary>
public sealed class PostgresSeWriter : PostgresBulkWriter<Se>
{
    public PostgresSeWriter(NpgsqlDataSource dataSource, string? schemaName = null)
        : base(dataSource, schemaName, "se", SeColumns.All, SeColumns.PrimaryKey)
    {
    }

    protected override async Task WriteRowAsync(NpgsqlBinaryImporter writer, Se se, CancellationToken ct)
    {
        await WriteText(writer, se.RecordSpec, ct).ConfigureAwait(false);
        await WriteText(writer, se.DataKubun, ct).ConfigureAwait(false);
        await WriteDate(writer, se.MakeDate, ct).ConfigureAwait(false);

        await WriteText(writer, se.Year, ct).ConfigureAwait(false);
        await WriteText(writer, se.MonthDay, ct).ConfigureAwait(false);
        await WriteText(writer, se.JyoCD, ct).ConfigureAwait(false);
        await WriteText(writer, se.Kaiji, ct).ConfigureAwait(false);
        await WriteText(writer, se.Nichiji, ct).ConfigureAwait(false);
        await WriteText(writer, se.RaceNum, ct).ConfigureAwait(false);
        await WriteText(writer, se.Umaban, ct).ConfigureAwait(false);

        await WriteText(writer, se.Wakuban, ct).ConfigureAwait(false);
        await WriteText(writer, se.KettoNum, ct).ConfigureAwait(false);
        await WriteText(writer, se.Bamei, ct).ConfigureAwait(false);
        await WriteText(writer, se.UmaKigoCD, ct).ConfigureAwait(false);
        await WriteText(writer, se.SexCD, ct).ConfigureAwait(false);
        await WriteText(writer, se.HinsyuCD, ct).ConfigureAwait(false);
        await WriteText(writer, se.KeiroCD, ct).ConfigureAwait(false);
        await WriteShort(writer, se.Barei, ct).ConfigureAwait(false);
        await WriteText(writer, se.TozaiCD, ct).ConfigureAwait(false);
        await WriteText(writer, se.ChokyosiCode, ct).ConfigureAwait(false);
        await WriteText(writer, se.ChokyosiRyakusyo, ct).ConfigureAwait(false);
        await WriteText(writer, se.BanusiCode, ct).ConfigureAwait(false);
        await WriteText(writer, se.BanusiName, ct).ConfigureAwait(false);
        await WriteText(writer, se.Fukusyoku, ct).ConfigureAwait(false);
        await WriteText(writer, se.Reserved1, ct).ConfigureAwait(false);
        await WriteShort(writer, se.Futan, ct).ConfigureAwait(false);
        await WriteShort(writer, se.FutanBefore, ct).ConfigureAwait(false);
        await WriteText(writer, se.Blinker, ct).ConfigureAwait(false);
        await WriteText(writer, se.Reserved2, ct).ConfigureAwait(false);
        await WriteText(writer, se.KisyuCode, ct).ConfigureAwait(false);
        await WriteText(writer, se.KisyuCodeBefore, ct).ConfigureAwait(false);
        await WriteText(writer, se.KisyuRyakusyo, ct).ConfigureAwait(false);
        await WriteText(writer, se.KisyuRyakusyoBefore, ct).ConfigureAwait(false);
        await WriteText(writer, se.MinaraiCD, ct).ConfigureAwait(false);
        await WriteText(writer, se.MinaraiCDBefore, ct).ConfigureAwait(false);
        await WriteShort(writer, se.BaTaijyu, ct).ConfigureAwait(false);
        await WriteText(writer, se.ZogenFugo, ct).ConfigureAwait(false);
        await WriteShort(writer, se.ZogenSa, ct).ConfigureAwait(false);
        await WriteText(writer, se.IJyoCD, ct).ConfigureAwait(false);
        await WriteShort(writer, se.NyusenJyuni, ct).ConfigureAwait(false);
        await WriteShort(writer, se.KakuteiJyuni, ct).ConfigureAwait(false);
        await WriteText(writer, se.DochakuKubun, ct).ConfigureAwait(false);
        await WriteShort(writer, se.DochakuTosu, ct).ConfigureAwait(false);
        await WriteShort(writer, se.Time, ct).ConfigureAwait(false);
        await WriteText(writer, se.ChakusaCD, ct).ConfigureAwait(false);
        await WriteText(writer, se.ChakusaCDP, ct).ConfigureAwait(false);
        await WriteText(writer, se.ChakusaCDPP, ct).ConfigureAwait(false);
        await WriteShort(writer, se.Jyuni1c, ct).ConfigureAwait(false);
        await WriteShort(writer, se.Jyuni2c, ct).ConfigureAwait(false);
        await WriteShort(writer, se.Jyuni3c, ct).ConfigureAwait(false);
        await WriteShort(writer, se.Jyuni4c, ct).ConfigureAwait(false);
        await WriteShort(writer, se.Odds, ct).ConfigureAwait(false);
        await WriteShort(writer, se.Ninki, ct).ConfigureAwait(false);
        await WriteInt(writer, se.Honsyokin, ct).ConfigureAwait(false);
        await WriteInt(writer, se.Fukasyokin, ct).ConfigureAwait(false);
        await WriteText(writer, se.Reserved3, ct).ConfigureAwait(false);
        await WriteText(writer, se.Reserved4, ct).ConfigureAwait(false);
        await WriteShort(writer, se.HaronTimeL4, ct).ConfigureAwait(false);
        await WriteShort(writer, se.HaronTimeL3, ct).ConfigureAwait(false);

        for (var i = 0; i < 3; i++)
        {
            var c = se.ChakuUma[i];
            await WriteText(writer, c.KettoNum, ct).ConfigureAwait(false);
            await WriteText(writer, c.Bamei, ct).ConfigureAwait(false);
        }

        await WriteShort(writer, se.TimeDiff, ct).ConfigureAwait(false);
        await WriteText(writer, se.RecordUpKubun, ct).ConfigureAwait(false);
        await WriteText(writer, se.DMKubun, ct).ConfigureAwait(false);
        await WriteInt(writer, se.DMTime, ct).ConfigureAwait(false);
        await WriteShort(writer, se.DMGosaP, ct).ConfigureAwait(false);
        await WriteShort(writer, se.DMGosaM, ct).ConfigureAwait(false);
        await WriteShort(writer, se.DMJyuni, ct).ConfigureAwait(false);
        await WriteText(writer, se.KyakusituKubun, ct).ConfigureAwait(false);
    }
}
