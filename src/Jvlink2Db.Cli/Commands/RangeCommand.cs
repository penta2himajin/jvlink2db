using System;
using System.CommandLine;
using System.Threading.Tasks;
using Jvlink2Db.Pipeline.Setup;

namespace Jvlink2Db.Cli.Commands;

public static class RangeCommand
{
    public static Command Create()
    {
        var connection = ModeRunner.Connection();
        var schema = ModeRunner.Schema();
        var operationalSchema = ModeRunner.OperationalSchema();
        var dataspec = ModeRunner.Dataspec();
        var sid = ModeRunner.Sid();

        var since = new Option<string>(
            name: "--since",
            description: "Range start, YYYYMMDDhhmmss.")
        { IsRequired = true };

        var until = new Option<string>(
            name: "--until",
            description: "Range end, YYYYMMDDhhmmss.")
        { IsRequired = true };

        var cmd = new Command(
            "range",
            "Bounded historical load (option=4, range fromtime). Snapshot dataspecs (TOKU/DIFF/DIFN/HOSE/HOSN/HOYU/COMM) are rejected. No resume — re-running the same window is idempotent through ON CONFLICT.")
        {
            connection, schema, operationalSchema, dataspec, sid, since, until,
        };

        cmd.SetHandler(async ctx =>
        {
            var dataspecValue = ctx.ParseResult.GetValueForOption(dataspec)!;
            try
            {
                DataspecValidator.EnsureRangeAllowed(dataspecValue);
            }
            catch (DataspecRangeNotSupportedException ex)
            {
                Console.Error.WriteLine(ex.Message);
                ctx.ExitCode = 1;
                return;
            }

            var sinceValue = ctx.ParseResult.GetValueForOption(since)!;
            var untilValue = ctx.ParseResult.GetValueForOption(until)!;

            await ModeRunner.ExecuteAsync(ctx, new RunDescriptor(
                Mode: "range",
                Connection: ctx.ParseResult.GetValueForOption(connection)!,
                Schema: ctx.ParseResult.GetValueForOption(schema)!,
                OperationalSchema: ctx.ParseResult.GetValueForOption(operationalSchema)!,
                Sid: ctx.ParseResult.GetValueForOption(sid)!,
                Dataspec: dataspecValue,
                Option: 4,
                Fromtime: $"{sinceValue}-{untilValue}",
                Resume: ResumeBehavior.None)).ConfigureAwait(false);
        });

        return cmd;
    }
}
