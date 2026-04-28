using System;
using System.CommandLine;
using System.Threading.Tasks;
using Jvlink2Db.Pipeline.Watch;

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
        var quiet = ModeRunner.Quiet();

        var since = new Option<string>(
            name: "--since",
            description: "fromtime parameter passed to JVOpen, YYYYMMDDhhmmss. JV-Link still requires it for option=2 even though only the current week's data is returned.")
        { IsRequired = true };

        var watch = new Option<bool>(
            name: "--watch",
            description: "Stay in foreground and re-run every --interval until Ctrl+C / SIGTERM.");

        var interval = new Option<string?>(
            name: "--interval",
            description: "Watch interval. Accepts HH:MM:SS or shorthand Ns/Nm/Nh/Nd. Required when --watch is set.");

        var cmd = new Command(
            "weekly",
            "This-week-only fetch (option=2). Reads only entries plus the previous week's results, not the historical archive. Pass --watch --interval <duration> to keep running on a recurring cadence.")
        {
            connection, schema, operationalSchema, dataspec, sid, since, watch, interval, quiet,
        };

        cmd.SetHandler(async ctx =>
        {
            var token = ctx.GetCancellationToken();
            var watchValue = ctx.ParseResult.GetValueForOption(watch);
            var intervalArg = ctx.ParseResult.GetValueForOption(interval);

            TimeSpan? intervalValue = null;
            if (watchValue)
            {
                if (string.IsNullOrEmpty(intervalArg))
                {
                    Console.Error.WriteLine("weekly --watch requires --interval (e.g. --interval 1h).");
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
                Mode: "weekly",
                Connection: ctx.ParseResult.GetValueForOption(connection)!,
                Schema: ctx.ParseResult.GetValueForOption(schema)!,
                OperationalSchema: ctx.ParseResult.GetValueForOption(operationalSchema)!,
                Sid: ctx.ParseResult.GetValueForOption(sid)!,
                Dataspec: ctx.ParseResult.GetValueForOption(dataspec)!,
                Option: 2,
                Fromtime: ctx.ParseResult.GetValueForOption(since)!,
                Resume: ResumeBehavior.None,
                Quiet: ctx.ParseResult.GetValueForOption(quiet));

            if (!watchValue)
            {
                await ModeRunner.ExecuteAsync(ctx, descriptor).ConfigureAwait(false);
                return;
            }

            Console.WriteLine($"weekly --watch: cycling every {intervalValue}, Ctrl+C to stop.");
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
