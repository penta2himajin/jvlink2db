using System;
using System.CommandLine;
using System.Linq;
using System.Threading.Tasks;
using Jvlink2Db.Core.Jvlink;
using Jvlink2Db.Db.Postgres.Schema;
using Jvlink2Db.Jvlink.Com;
using Jvlink2Db.Pipeline.Setup;
using Npgsql;

namespace Jvlink2Db.Cli.Commands;

internal static class ModeRunner
{
    public static Option<string> Connection() => new(
        name: "--connection",
        description: "Npgsql connection string for the target PostgreSQL.")
    { IsRequired = true };

    public static Option<string> Schema() => new(
        name: "--schema",
        description: "Database schema name. Created if it does not exist.",
        getDefaultValue: () => PostgresSchemaProvisioner.DefaultSchemaName);

    public static Option<string> Dataspec() => new(
        name: "--dataspec",
        description: "Dataspec ID, e.g. RACE, DIFN, BLOD.")
    { IsRequired = true };

    public static Option<string> Sid() => new(
        name: "--sid",
        description: "Software identifier passed to JVInit.",
        getDefaultValue: () => "jvlink2db/0.1");

    public static async Task ExecuteAsync(
        System.CommandLine.Invocation.InvocationContext ctx,
        string connection,
        string schema,
        string dataspec,
        string fromtime,
        int option,
        string sid)
    {
        var token = ctx.GetCancellationToken();
        await using var dataSource = NpgsqlDataSource.Create(connection);
        using var jv = new ComJvLink();

        var provisioner = new PostgresSchemaProvisioner(dataSource, schema);
        var sinks = PostgresSinkFactory.CreateAll(dataSource, schema);
        var runner = new SetupRunner(jv, provisioner, sinks);

        try
        {
            var report = await runner.RunAsync(
                new SetupOptions(sid, dataspec, fromtime, option),
                token).ConfigureAwait(false);

            PrintReport(report, schema);
            ctx.ExitCode = 0;
        }
        catch (JvLinkException ex)
        {
            Console.Error.WriteLine($"JV-Link error: {ex.Method} returned {ex.ReturnCode}");
            ctx.ExitCode = ExitCodeFor(ex.ReturnCode);
        }
    }

    private static void PrintReport(SetupReport report, string schemaName)
    {
        Console.WriteLine($"Schema             : {schemaName}");
        Console.WriteLine($"OpenReturnCode     : {report.OpenReturnCode}");
        Console.WriteLine($"ReadCount          : {report.ReadCount}");
        Console.WriteLine($"DownloadCount      : {report.DownloadCount}");
        Console.WriteLine($"LastFileTimestamp  : {report.LastFileTimestamp}");
        Console.WriteLine();

        Console.WriteLine("Record counts by ID (read / inserted):");
        if (report.RecordCountsById.Count == 0)
        {
            Console.WriteLine("  (no records)");
            return;
        }

        foreach (var kv in report.RecordCountsById.OrderByDescending(kv => kv.Value).ThenBy(kv => kv.Key, StringComparer.Ordinal))
        {
            var inserted = report.RecordsInsertedById.TryGetValue(kv.Key, out var n) ? n.ToString() : "-";
            Console.WriteLine($"  {kv.Key}: {kv.Value} / {inserted}");
        }
    }

    private static int ExitCodeFor(int returnCode) => returnCode switch
    {
        >= -199 and <= -100 => 1,
        >= -299 and <= -200 => 2,
        >= -399 and <= -300 => 3,
        _ => 4,
    };
}
