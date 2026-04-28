using System.CommandLine;

namespace Jvlink2Db.Cli.Commands;

public static class SetupCommand
{
    private const string DefaultSince = "19860101000000";

    public static Command Create()
    {
        var connection = ModeRunner.Connection();
        var schema = ModeRunner.Schema();
        var operationalSchema = ModeRunner.OperationalSchema();
        var dataspec = ModeRunner.Dataspec();
        var sid = ModeRunner.Sid();
        var quiet = ModeRunner.Quiet();

        var since = new Option<string>(
            name: "--since",
            description: "Start of fromtime, YYYYMMDDhhmmss. Defaults to JRA-VAN epoch.",
            getDefaultValue: () => DefaultSince);

        var cmd = new Command(
            "setup",
            "Full historical bulk load (option=4, start-only fromtime). Resumes from acquisition_state.last_filename via JVSkip.")
        {
            connection, schema, operationalSchema, dataspec, sid, since, quiet,
        };

        cmd.SetHandler(async ctx =>
        {
            await ModeRunner.ExecuteAsync(ctx, new RunDescriptor(
                Mode: "setup",
                Connection: ctx.ParseResult.GetValueForOption(connection)!,
                Schema: ctx.ParseResult.GetValueForOption(schema)!,
                OperationalSchema: ctx.ParseResult.GetValueForOption(operationalSchema)!,
                Sid: ctx.ParseResult.GetValueForOption(sid)!,
                Dataspec: ctx.ParseResult.GetValueForOption(dataspec)!,
                Option: 4,
                Fromtime: ctx.ParseResult.GetValueForOption(since)!,
                Resume: ResumeBehavior.SetupIncremental,
                Quiet: ctx.ParseResult.GetValueForOption(quiet))).ConfigureAwait(false);
        });

        return cmd;
    }
}
