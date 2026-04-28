using System.CommandLine;

namespace Jvlink2Db.Cli.Commands;

public static class NormalCommand
{
    public static Command Create()
    {
        var connection = ModeRunner.Connection();
        var schema = ModeRunner.Schema();
        var dataspec = ModeRunner.Dataspec();
        var sid = ModeRunner.Sid();

        var since = new Option<string>(
            name: "--since",
            description: "Start of fromtime, YYYYMMDDhhmmss. Reads everything strictly newer than this instant.")
        { IsRequired = true };

        var cmd = new Command(
            "normal",
            "Incremental update (option=1). Reads anything matching dataspec strictly newer than --since.")
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
                option: 1,
                sid: ctx.ParseResult.GetValueForOption(sid)!));

        return cmd;
    }
}
