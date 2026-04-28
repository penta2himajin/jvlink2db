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
            "Bounded historical load (option=4, range fromtime). Snapshot dataspecs (TOKU/DIFF/DIFN/HOSE/HOSN/HOYU/COMM) are rejected.")
        {
            connection, schema, dataspec, sid, since, until,
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

            await ModeRunner.ExecuteAsync(
                ctx,
                connection: ctx.ParseResult.GetValueForOption(connection)!,
                schema: ctx.ParseResult.GetValueForOption(schema)!,
                dataspec: dataspecValue,
                fromtime: $"{sinceValue}-{untilValue}",
                option: 4,
                sid: ctx.ParseResult.GetValueForOption(sid)!).ConfigureAwait(false);
        });

        return cmd;
    }
}
