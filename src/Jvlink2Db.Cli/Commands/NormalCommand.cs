using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Threading.Tasks;
using Jvlink2Db.Pipeline.Setup;
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
        var quiet = ModeRunner.Quiet();

        var since = new Option<string?>(
            name: "--since",
            description: "Start of fromtime, YYYYMMDDhhmmss. If omitted, each dataspec resumes from acquisition_state.last_fromtime.");

        var watch = new Option<bool>(
            name: "--watch",
            description: "Stay in foreground and re-run every --interval until Ctrl+C / SIGTERM.");

        var interval = new Option<string?>(
            name: "--interval",
            description: "Watch interval. Accepts HH:MM:SS or shorthand Ns/Nm/Nh/Nd. Required when --watch is set.");

        var cmd = new Command(
            "normal",
            "Incremental update (option=1). Each dataspec resumes from its own acquisition_state.last_fromtime if --since is omitted; saves the new last_fromtime on success. Pass --watch --interval <duration> to keep running on a recurring cadence.")
        {
            connection, schema, operationalSchema, dataspec, sid, since, watch, interval, quiet,
        };

        cmd.SetHandler(async ctx =>
        {
            var token = ctx.GetCancellationToken();
            var dataspecValue = ctx.ParseResult.GetValueForOption(dataspec)!;
            var sinceArg = ctx.ParseResult.GetValueForOption(since);
            var watchValue = ctx.ParseResult.GetValueForOption(watch);
            var intervalArg = ctx.ParseResult.GetValueForOption(interval);

            IReadOnlyList<string> dataspecs;
            try
            {
                dataspecs = DataspecParser.Split(dataspecValue);
            }
            catch (ArgumentException ex)
            {
                Console.Error.WriteLine(ex.Message);
                ctx.ExitCode = 1;
                return;
            }

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

            var descriptor = new RunDescriptor(
                Mode: "normal",
                Connection: ctx.ParseResult.GetValueForOption(connection)!,
                Schema: ctx.ParseResult.GetValueForOption(schema)!,
                OperationalSchema: ctx.ParseResult.GetValueForOption(operationalSchema)!,
                Sid: ctx.ParseResult.GetValueForOption(sid)!,
                Dataspecs: dataspecs,
                Option: 1,
                Fromtime: sinceArg,  // null → per-dataspec resume from acquisition_state
                Resume: ResumeBehavior.NormalIncremental,
                Quiet: ctx.ParseResult.GetValueForOption(quiet));

            if (!watchValue)
            {
                await ModeRunner.ExecuteAsync(ctx, descriptor).ConfigureAwait(false);
                return;
            }

            Console.WriteLine($"normal --watch: cycling every {intervalValue}, Ctrl+C to stop.");
            var watchRunner = new WatchRunner(
                onError: (cycle, ex) =>
                    Console.Error.WriteLine($"[watch cycle {cycle}] {ex.GetType().Name}: {ex.Message}"));

            await watchRunner.RunAsync(
                async _ => await ModeRunner.ExecuteAsync(ctx, descriptor).ConfigureAwait(false),
                intervalValue!.Value,
                token).ConfigureAwait(false);

            ctx.ExitCode = 0;
        });

        return cmd;
    }
}
