using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using System.Threading.Tasks;
using Jvlink2Db.Core.Jvlink;
using Jvlink2Db.Jvlink.Com;
using Jvlink2Db.Pipeline.Probe;

namespace Jvlink2Db.Cli.Commands;

public static class ProbeCommand
{
    public static Command Create()
    {
        var dataspec = new Option<string>(
            name: "--dataspec",
            description: "Dataspec ID, e.g. RACE, DIFF, BLOD.")
        {
            IsRequired = true,
        };

        var fromtime = new Option<string>(
            name: "--fromtime",
            description: "fromtime parameter. YYYYMMDDhhmmss for start-only, or YYYYMMDDhhmmss-YYYYMMDDhhmmss for a range.")
        {
            IsRequired = true,
        };

        var option = new Option<int>(
            name: "--option",
            description: "JVOpen option. 1=normal, 2=this-week, 3=setup-with-dialog, 4=setup-no-dialog.",
            getDefaultValue: () => 4);

        var sid = new Option<string>(
            name: "--sid",
            description: "Software identifier passed to JVInit.",
            getDefaultValue: () => "jvlink2db/0.1");

        var maxSamples = new Option<int>(
            name: "--max-samples",
            description: "Number of sample record IDs and filenames to print.",
            getDefaultValue: () => 10);

        var cmd = new Command(
            "probe",
            "Run a JV-Link probe to verify the protocol on a real JV-Link installation.")
        {
            dataspec, fromtime, option, sid, maxSamples,
        };

        cmd.SetHandler(async ctx =>
        {
            var dataspecValue = ctx.ParseResult.GetValueForOption(dataspec)!;
            var fromtimeValue = ctx.ParseResult.GetValueForOption(fromtime)!;
            var optionValue = ctx.ParseResult.GetValueForOption(option);
            var sidValue = ctx.ParseResult.GetValueForOption(sid)!;
            var maxSamplesValue = ctx.ParseResult.GetValueForOption(maxSamples);
            var token = ctx.GetCancellationToken();

            using var jv = new ComJvLink();
            var runner = new ProbeRunner(jv);

            try
            {
                var report = await runner.RunAsync(
                    new ProbeOptions(sidValue, dataspecValue, fromtimeValue, optionValue, maxSamplesValue),
                    token).ConfigureAwait(false);

                PrintReport(report, maxSamplesValue);
                ctx.ExitCode = 0;
            }
            catch (JvLinkException ex)
            {
                Console.Error.WriteLine($"JV-Link error: {ex.Method} returned {ex.ReturnCode}");
                ctx.ExitCode = ExitCodeFor(ex.ReturnCode);
            }
        });

        return cmd;
    }

    private static void PrintReport(ProbeReport report, int maxSamples)
    {
        Console.WriteLine($"OpenReturnCode    : {report.OpenReturnCode}");
        Console.WriteLine($"ReadCount         : {report.ReadCount}");
        Console.WriteLine($"DownloadCount     : {report.DownloadCount}");
        Console.WriteLine($"LastFileTimestamp : {report.LastFileTimestamp}");
        Console.WriteLine();

        Console.WriteLine("Record counts by ID:");
        if (report.RecordCountsById.Count == 0)
        {
            Console.WriteLine("  (no records)");
        }
        else
        {
            foreach (var kv in report.RecordCountsById.OrderByDescending(kv => kv.Value).ThenBy(kv => kv.Key, StringComparer.Ordinal))
            {
                Console.WriteLine($"  {kv.Key}: {kv.Value}");
            }
        }

        Console.WriteLine();
        Console.WriteLine($"First {maxSamples} filenames:");
        var shownFiles = 0;
        foreach (var name in report.Filenames)
        {
            if (shownFiles >= maxSamples)
            {
                break;
            }

            Console.WriteLine($"  {name}");
            shownFiles++;
        }

        if (report.Filenames.Count > maxSamples)
        {
            Console.WriteLine($"  ... and {report.Filenames.Count - maxSamples} more");
        }

        Console.WriteLine();
        Console.WriteLine($"First record IDs : {string.Join(",", report.SampleRecordIds)}");
    }

    private static int ExitCodeFor(int returnCode) => returnCode switch
    {
        >= -199 and <= -100 => 1,
        >= -299 and <= -200 => 2,
        >= -399 and <= -300 => 3,
        _ => 4,
    };
}
