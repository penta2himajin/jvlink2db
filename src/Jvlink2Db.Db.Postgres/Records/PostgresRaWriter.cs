using System.Threading;
using System.Threading.Tasks;
using Jvlink2Db.Core.Records;
using Npgsql;

namespace Jvlink2Db.Db.Postgres.Records;

/// <summary>
/// Idempotent bulk writer for <see cref="Ra"/> records.
/// </summary>
public sealed class PostgresRaWriter : PostgresBulkWriter<Ra>
{
    public PostgresRaWriter(NpgsqlDataSource dataSource, string? schemaName = null)
        : base(dataSource, schemaName, "ra", RaColumns.All, RaColumns.PrimaryKey)
    {
    }

    protected override async Task WriteRowAsync(NpgsqlBinaryImporter writer, Ra ra, CancellationToken ct)
    {
        await WriteText(writer, ra.RecordSpec, ct).ConfigureAwait(false);
        await WriteText(writer, ra.DataKubun, ct).ConfigureAwait(false);
        await WriteDate(writer, ra.MakeDate, ct).ConfigureAwait(false);

        await WriteText(writer, ra.Year, ct).ConfigureAwait(false);
        await WriteText(writer, ra.MonthDay, ct).ConfigureAwait(false);
        await WriteText(writer, ra.JyoCD, ct).ConfigureAwait(false);
        await WriteText(writer, ra.Kaiji, ct).ConfigureAwait(false);
        await WriteText(writer, ra.Nichiji, ct).ConfigureAwait(false);
        await WriteText(writer, ra.RaceNum, ct).ConfigureAwait(false);

        await WriteText(writer, ra.YoubiCD, ct).ConfigureAwait(false);
        await WriteText(writer, ra.TokuNum, ct).ConfigureAwait(false);
        await WriteText(writer, ra.Hondai, ct).ConfigureAwait(false);
        await WriteText(writer, ra.Fukudai, ct).ConfigureAwait(false);
        await WriteText(writer, ra.Kakko, ct).ConfigureAwait(false);
        await WriteText(writer, ra.HondaiEng, ct).ConfigureAwait(false);
        await WriteText(writer, ra.FukudaiEng, ct).ConfigureAwait(false);
        await WriteText(writer, ra.KakkoEng, ct).ConfigureAwait(false);
        await WriteText(writer, ra.Ryakusyo10, ct).ConfigureAwait(false);
        await WriteText(writer, ra.Ryakusyo6, ct).ConfigureAwait(false);
        await WriteText(writer, ra.Ryakusyo3, ct).ConfigureAwait(false);
        await WriteText(writer, ra.Kubun, ct).ConfigureAwait(false);
        await WriteShort(writer, ra.Nkai, ct).ConfigureAwait(false);

        await WriteText(writer, ra.GradeCD, ct).ConfigureAwait(false);
        await WriteText(writer, ra.GradeCDBefore, ct).ConfigureAwait(false);

        await WriteText(writer, ra.SyubetuCD, ct).ConfigureAwait(false);
        await WriteText(writer, ra.KigoCD, ct).ConfigureAwait(false);
        await WriteText(writer, ra.JyuryoCD, ct).ConfigureAwait(false);
        await WriteTextArray(writer, ra.JyokenCD, ct).ConfigureAwait(false);

        await WriteText(writer, ra.JyokenName, ct).ConfigureAwait(false);
        await WriteShort(writer, ra.Kyori, ct).ConfigureAwait(false);
        await WriteShort(writer, ra.KyoriBefore, ct).ConfigureAwait(false);
        await WriteText(writer, ra.TrackCD, ct).ConfigureAwait(false);
        await WriteText(writer, ra.TrackCDBefore, ct).ConfigureAwait(false);
        await WriteText(writer, ra.CourseKubunCD, ct).ConfigureAwait(false);
        await WriteText(writer, ra.CourseKubunCDBefore, ct).ConfigureAwait(false);

        await WriteIntArray(writer, ra.Honsyokin, ct).ConfigureAwait(false);
        await WriteIntArray(writer, ra.HonsyokinBefore, ct).ConfigureAwait(false);
        await WriteIntArray(writer, ra.Fukasyokin, ct).ConfigureAwait(false);
        await WriteIntArray(writer, ra.FukasyokinBefore, ct).ConfigureAwait(false);

        await WriteText(writer, ra.HassoTime, ct).ConfigureAwait(false);
        await WriteText(writer, ra.HassoTimeBefore, ct).ConfigureAwait(false);
        await WriteShort(writer, ra.TorokuTosu, ct).ConfigureAwait(false);
        await WriteShort(writer, ra.SyussoTosu, ct).ConfigureAwait(false);
        await WriteShort(writer, ra.NyusenTosu, ct).ConfigureAwait(false);

        await WriteText(writer, ra.TenkoCD, ct).ConfigureAwait(false);
        await WriteText(writer, ra.SibaBabaCD, ct).ConfigureAwait(false);
        await WriteText(writer, ra.DirtBabaCD, ct).ConfigureAwait(false);

        await WriteShortArray(writer, ra.LapTime, ct).ConfigureAwait(false);

        await WriteShort(writer, ra.SyogaiMileTime, ct).ConfigureAwait(false);
        await WriteShort(writer, ra.HaronTimeS3, ct).ConfigureAwait(false);
        await WriteShort(writer, ra.HaronTimeS4, ct).ConfigureAwait(false);
        await WriteShort(writer, ra.HaronTimeL3, ct).ConfigureAwait(false);
        await WriteShort(writer, ra.HaronTimeL4, ct).ConfigureAwait(false);

        for (var i = 0; i < 4; i++)
        {
            var c = ra.Corners[i];
            await WriteText(writer, c.Corner, ct).ConfigureAwait(false);
            await WriteText(writer, c.Syukaisu, ct).ConfigureAwait(false);
            await WriteText(writer, c.Jyuni, ct).ConfigureAwait(false);
        }

        await WriteText(writer, ra.RecordUpKubun, ct).ConfigureAwait(false);
    }
}
