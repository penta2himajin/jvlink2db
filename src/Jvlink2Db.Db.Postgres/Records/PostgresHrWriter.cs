using System.Threading;
using System.Threading.Tasks;
using Jvlink2Db.Core.Records;
using Npgsql;

namespace Jvlink2Db.Db.Postgres.Records;

public sealed class PostgresHrWriter : PostgresBulkWriter<Hr>
{
    public PostgresHrWriter(NpgsqlDataSource dataSource, string? schemaName = null)
        : base(dataSource, schemaName, "hr", HrColumns.All, HrColumns.PrimaryKey)
    {
    }

    protected override async Task WriteRowAsync(NpgsqlBinaryImporter writer, Hr hr, CancellationToken ct)
    {
        await WriteText(writer, hr.RecordSpec, ct).ConfigureAwait(false);
        await WriteText(writer, hr.DataKubun, ct).ConfigureAwait(false);
        await WriteDate(writer, hr.MakeDate, ct).ConfigureAwait(false);
        await WriteText(writer, hr.Year, ct).ConfigureAwait(false);
        await WriteText(writer, hr.MonthDay, ct).ConfigureAwait(false);
        await WriteText(writer, hr.JyoCD, ct).ConfigureAwait(false);
        await WriteText(writer, hr.Kaiji, ct).ConfigureAwait(false);
        await WriteText(writer, hr.Nichiji, ct).ConfigureAwait(false);
        await WriteText(writer, hr.RaceNum, ct).ConfigureAwait(false);

        await WriteShort(writer, hr.TorokuTosu, ct).ConfigureAwait(false);
        await WriteShort(writer, hr.SyussoTosu, ct).ConfigureAwait(false);

        await WriteTextArray(writer, hr.FuseirituFlag, ct).ConfigureAwait(false);
        await WriteTextArray(writer, hr.TokubaraiFlag, ct).ConfigureAwait(false);
        await WriteTextArray(writer, hr.HenkanFlag, ct).ConfigureAwait(false);
        await WriteTextArray(writer, hr.HenkanUma, ct).ConfigureAwait(false);
        await WriteTextArray(writer, hr.HenkanWaku, ct).ConfigureAwait(false);
        await WriteTextArray(writer, hr.HenkanDoWaku, ct).ConfigureAwait(false);

        await WriteTextArray(writer, hr.PayTansyoUmaban, ct).ConfigureAwait(false);
        await WriteIntArray(writer, hr.PayTansyoPay, ct).ConfigureAwait(false);
        await WriteShortArray(writer, hr.PayTansyoNinki, ct).ConfigureAwait(false);

        await WriteTextArray(writer, hr.PayFukusyoUmaban, ct).ConfigureAwait(false);
        await WriteIntArray(writer, hr.PayFukusyoPay, ct).ConfigureAwait(false);
        await WriteShortArray(writer, hr.PayFukusyoNinki, ct).ConfigureAwait(false);

        await WriteTextArray(writer, hr.PayWakurenUmaban, ct).ConfigureAwait(false);
        await WriteIntArray(writer, hr.PayWakurenPay, ct).ConfigureAwait(false);
        await WriteShortArray(writer, hr.PayWakurenNinki, ct).ConfigureAwait(false);

        await WriteTextArray(writer, hr.PayUmarenKumi, ct).ConfigureAwait(false);
        await WriteIntArray(writer, hr.PayUmarenPay, ct).ConfigureAwait(false);
        await WriteShortArray(writer, hr.PayUmarenNinki, ct).ConfigureAwait(false);

        await WriteTextArray(writer, hr.PayWideKumi, ct).ConfigureAwait(false);
        await WriteIntArray(writer, hr.PayWidePay, ct).ConfigureAwait(false);
        await WriteShortArray(writer, hr.PayWideNinki, ct).ConfigureAwait(false);

        await WriteTextArray(writer, hr.PayReserved1Kumi, ct).ConfigureAwait(false);
        await WriteIntArray(writer, hr.PayReserved1Pay, ct).ConfigureAwait(false);
        await WriteShortArray(writer, hr.PayReserved1Ninki, ct).ConfigureAwait(false);

        await WriteTextArray(writer, hr.PayUmatanKumi, ct).ConfigureAwait(false);
        await WriteIntArray(writer, hr.PayUmatanPay, ct).ConfigureAwait(false);
        await WriteShortArray(writer, hr.PayUmatanNinki, ct).ConfigureAwait(false);

        await WriteTextArray(writer, hr.PaySanrenpukuKumi, ct).ConfigureAwait(false);
        await WriteIntArray(writer, hr.PaySanrenpukuPay, ct).ConfigureAwait(false);
        await WriteShortArray(writer, hr.PaySanrenpukuNinki, ct).ConfigureAwait(false);

        await WriteTextArray(writer, hr.PaySanrentanKumi, ct).ConfigureAwait(false);
        await WriteIntArray(writer, hr.PaySanrentanPay, ct).ConfigureAwait(false);
        await WriteShortArray(writer, hr.PaySanrentanNinki, ct).ConfigureAwait(false);
    }
}
