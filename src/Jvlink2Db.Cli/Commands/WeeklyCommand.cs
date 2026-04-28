using System.CommandLine;
using System.Threading.Tasks;

namespace Jvlink2Db.Cli.Commands;

public static class WeeklyCommand
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
            description: "fromtime parameter passed to JVOpen, YYYYMMDDhhmmss. JV-Link still requires it for option=2 even though only the current week's data is returned.")
        { IsRequired = true };

        var cmd = new Command(
            "weekly",
            "This-week-only fetch (option=2). Reads only entries plus the previous week's results, not the historical archive.")
        {
            connection, schema, operationalSchema, dataspec, sid, since,
        };

        cmd.SetHandler(async ctx =>
        {
            await ModeRunner.ExecuteAsync(ctx, new RunDescriptor(
                Mode: "weekly",
                Connection: ctx.ParseResult.GetValueForOption(connection)!,
                Schema: ctx.ParseResult.GetValueForOption(schema)!,
                OperationalSchema: ctx.ParseResult.GetValueForOption(operationalSchema)!,
                Sid: ctx.ParseResult.GetValueForOption(sid)!,
                Dataspec: ctx.ParseResult.GetValueForOption(dataspec)!,
                Option: 2,
                Fromtime: ctx.ParseResult.GetValueForOption(since)!,
                Resume: ResumeBehavior.None)).ConfigureAwait(false);
        });

        return cmd;
    }
}
