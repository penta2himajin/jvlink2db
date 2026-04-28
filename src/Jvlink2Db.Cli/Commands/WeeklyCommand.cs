using System.CommandLine;

namespace Jvlink2Db.Cli.Commands;

public static class WeeklyCommand
{
    public static Command Create()
    {
        var connection = ModeRunner.Connection();
        var schema = ModeRunner.Schema();
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
            connection, schema, dataspec, sid, since,
        };

        cmd.SetHandler(ctx =>
            ModeRunner.ExecuteAsync(
                ctx,
                connection: ctx.ParseResult.GetValueForOption(connection)!,
                schema: ctx.ParseResult.GetValueForOption(schema)!,
                dataspec: ctx.ParseResult.GetValueForOption(dataspec)!,
                fromtime: ctx.ParseResult.GetValueForOption(since)!,
                option: 2,
                sid: ctx.ParseResult.GetValueForOption(sid)!));

        return cmd;
    }
}
