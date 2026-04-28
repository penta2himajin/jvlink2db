using System;
using System.CommandLine;
using System.Threading.Tasks;
using Jvlink2Db.Pipeline.Watch;

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

        var watch = new Option<bool>(
            name: "--watch",
            description: "Stay in foreground and re-run every --interval until Ctrl+C / SIGTERM.");

        var interval = new Option<string?>(
            name: "--interval",
            description: "Watch interval. Accepts HH:MM:SS or shorthand Ns/Nm/Nh/Nd. Required when --watch is set.");

        var cmd = new Command(
            "normal",
            "Incremental update (option=1). Resumes from acquisition_state.last_fromtime if --since is omitted; saves the new last_fromtime on success. Pass --watch --interval <duration> to keep running on a recurring cadence.")
        {
            connection, schema, operationalSchema, dataspec, sid, since, watch, interval,
        };

        cmd.SetHandler(async ctx =>
        {
            var token = ctx.GetCancellationToken();
            var connectionValue = ctx.ParseResult.GetValueForOption(connection)!;
            var operationalSchemaValue = ctx.ParseResult.GetValueForOption(operationalSchema)!;
            var dataspecValue = ctx.ParseResult.GetValueForOption(dataspec)!;
            var sinceArg = ctx.ParseResult.GetValueForOption(since);
            var watchValue = ctx.ParseResult.GetValueForOption(watch);
            var intervalArg = ctx.ParseResult.GetValueForOption(interval);

            TimeSpan? intervalValue = null;
            if (watchValue)
            {
                if (string.IsNullOrEmpty(intervalArg))
                {
                    Console.Error.WriteLine("normal --watch requires --interval (e.g. --interval 30m).");
                    ctx.ExitCode = 1;
                    return;
                }

                try
                {
                    intervalValue = IntervalParser.Parse(intervalArg);
                }
                catch (FormatException ex)
                {
                    Console.Error.WriteLine(ex.Message);
                    ctx.ExitCode = 1;
                    return;
                }
            }

            async Task<bool> RunOnceAsync()
            {
                var resolvedSince = sinceArg;
                if (string.IsNullOrEmpty(resolvedSince))
                {
                    var saved = await ModeRunner.LookupStateAsync(connectionValue, operationalSchemaValue, dataspecValue, option: 1, token).ConfigureAwait(false);
                    resolvedSince = saved?.LastFromtime;

                    if (string.IsNullOrEmpty(resolvedSince))
                    {
                        Console.Error.WriteLine("normal: --since is required on the first run for this dataspec (no acquisition_state row yet).");
                        return false;
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
                    Fromtime: resolvedSince,
                    Resume: ResumeBehavior.NormalIncremental)).ConfigureAwait(false);
                return true;
            }

            if (!watchValue)
            {
                if (!await RunOnceAsync().ConfigureAwait(false))
                {
                    ctx.ExitCode = 1;
                }
                return;
            }

            Console.WriteLine($"normal --watch: cycling every {intervalValue}, Ctrl+C to stop.");
            var watchRunner = new WatchRunner(
                onError: (cycle, ex) =>
                    Console.Error.WriteLine($"[watch cycle {cycle}] {ex.GetType().Name}: {ex.Message}"));

            await watchRunner.RunAsync(
                async _ => { await RunOnceAsync().ConfigureAwait(false); },
                intervalValue!.Value,
                token).ConfigureAwait(false);

            ctx.ExitCode = 0;
        });

        return cmd;
    }
}
