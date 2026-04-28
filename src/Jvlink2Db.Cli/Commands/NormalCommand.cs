using System;
using System.CommandLine;
using System.Threading.Tasks;

namespace Jvlink2Db.Cli.Commands;

public static class NormalCommand
{
    public static Command Create()
    {
        var connection = ModeRunner.Connection();
        var schema = ModeRunner.Schema();
        var operationalSchema = ModeRunner.OperationalSchema();
        var dataspec = ModeRunner.Dataspec();
        var sid = ModeRunner.Sid();

        var since = new Option<string?>(
            name: "--since",
            description: "Start of fromtime, YYYYMMDDhhmmss. If omitted, resumes from acquisition_state.last_fromtime.");

        var cmd = new Command(
            "normal",
            "Incremental update (option=1). Resumes from acquisition_state.last_fromtime if --since is omitted; saves the new last_fromtime on success.")
        {
            connection, schema, operationalSchema, dataspec, sid, since,
        };

        cmd.SetHandler(async ctx =>
        {
            var token = ctx.GetCancellationToken();
            var connectionValue = ctx.ParseResult.GetValueForOption(connection)!;
            var operationalSchemaValue = ctx.ParseResult.GetValueForOption(operationalSchema)!;
            var dataspecValue = ctx.ParseResult.GetValueForOption(dataspec)!;
            var sinceValue = ctx.ParseResult.GetValueForOption(since);

            if (string.IsNullOrEmpty(sinceValue))
            {
                var saved = await ModeRunner.LookupStateAsync(connectionValue, operationalSchemaValue, dataspecValue, option: 1, token).ConfigureAwait(false);
                sinceValue = saved?.LastFromtime;

                if (string.IsNullOrEmpty(sinceValue))
                {
                    Console.Error.WriteLine("normal: --since is required on the first run for this dataspec (no acquisition_state row yet).");
                    ctx.ExitCode = 1;
                    return;
                }
            }

            await ModeRunner.ExecuteAsync(ctx, new RunDescriptor(
                Mode: "normal",
                Connection: connectionValue,
                Schema: ctx.ParseResult.GetValueForOption(schema)!,
                OperationalSchema: operationalSchemaValue,
                Sid: ctx.ParseResult.GetValueForOption(sid)!,
                Dataspec: dataspecValue,
                Option: 1,
                Fromtime: sinceValue,
                Resume: ResumeBehavior.NormalIncremental)).ConfigureAwait(false);
        });

        return cmd;
    }
}
