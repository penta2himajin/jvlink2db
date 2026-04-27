using System;
using System.CommandLine;
using System.Linq;
using System.Threading.Tasks;
using Jvlink2Db.Core.Jvlink;
using Jvlink2Db.Db.Postgres.Records;
using Jvlink2Db.Db.Postgres.Schema;
using Jvlink2Db.Jvlink.Com;
using Jvlink2Db.Pipeline.Setup;
using Npgsql;

namespace Jvlink2Db.Cli.Commands;

public static class SetupCommand
{
    public static Command Create()
    {
        var connection = new Option<string>(
            name: "--connection",
            description: "Npgsql connection string for the target PostgreSQL.")
        {
            IsRequired = true,
        };

        var schema = new Option<string>(
            name: "--schema",
            description: "Database schema name. Created if it does not exist.",
            getDefaultValue: () => PostgresSchemaProvisioner.DefaultSchemaName);

        var dataspec = new Option<string>(
            name: "--dataspec",
            description: "Dataspec ID, e.g. RACE.")
        {
            IsRequired = true,
        };

        var fromtime = new Option<string>(
            name: "--fromtime",
            description: "fromtime parameter. YYYYMMDDhhmmss for start-only, or YYYYMMDDhhmmss-YYYYMMDDhhmmss for a range.")
        {
            IsRequired = true,
        };

        var option = new Option<int>(
            name: "--option",
            description: "JVOpen option. 1=normal, 2=this-week, 3=setup-with-dialog, 4=setup-no-dialog.",
            getDefaultValue: () => 4);

        var sid = new Option<string>(
            name: "--sid",
            description: "Software identifier passed to JVInit.",
            getDefaultValue: () => "jvlink2db/0.1");

        var cmd = new Command(
            "setup",
            "Bulk-load JV-Data records (currently RA only) into the target PostgreSQL.")
        {
            connection, schema, dataspec, fromtime, option, sid,
        };

        cmd.SetHandler(async ctx =>
        {
            var connectionValue = ctx.ParseResult.GetValueForOption(connection)!;
            var schemaValue = ctx.ParseResult.GetValueForOption(schema)!;
            var dataspecValue = ctx.ParseResult.GetValueForOption(dataspec)!;
            var fromtimeValue = ctx.ParseResult.GetValueForOption(fromtime)!;
            var optionValue = ctx.ParseResult.GetValueForOption(option);
            var sidValue = ctx.ParseResult.GetValueForOption(sid)!;
            var token = ctx.GetCancellationToken();

            await using var dataSource = NpgsqlDataSource.Create(connectionValue);
            using var jv = new ComJvLink();

            var provisioner = new PostgresSchemaProvisioner(dataSource, schemaValue);
            var writer = new PostgresRaWriter(dataSource, schemaValue);
            var runner = new SetupRunner(jv, provisioner, writer);

            try
            {
                var report = await runner.RunAsync(
                    new SetupOptions(sidValue, dataspecValue, fromtimeValue, optionValue),
                    token).ConfigureAwait(false);

                PrintReport(report, schemaValue);
                ctx.ExitCode = 0;
            }
            catch (JvLinkException ex)
            {
                Console.Error.WriteLine($"JV-Link error: {ex.Method} returned {ex.ReturnCode}");
                ctx.ExitCode = ExitCodeFor(ex.ReturnCode);
            }
        });

        return cmd;
    }

    private static void PrintReport(SetupReport report, string schemaName)
    {
        Console.WriteLine($"Schema             : {schemaName}");
        Console.WriteLine($"OpenReturnCode     : {report.OpenReturnCode}");
        Console.WriteLine($"ReadCount          : {report.ReadCount}");
        Console.WriteLine($"DownloadCount      : {report.DownloadCount}");
        Console.WriteLine($"LastFileTimestamp  : {report.LastFileTimestamp}");
        Console.WriteLine($"RA records inserted: {report.RaInserted}");
        Console.WriteLine();

        Console.WriteLine("Record counts by ID:");
        if (report.RecordCountsById.Count == 0)
        {
            Console.WriteLine("  (no records)");
        }
        else
        {
            foreach (var kv in report.RecordCountsById.OrderByDescending(kv => kv.Value).ThenBy(kv => kv.Key, StringComparer.Ordinal))
            {
                Console.WriteLine($"  {kv.Key}: {kv.Value}");
            }
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
