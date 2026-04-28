using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Jvlink2Db.Core.Jvlink;
using Jvlink2Db.Core.Persistence;
using Jvlink2Db.Db.Postgres.Operational;
using Jvlink2Db.Db.Postgres.Schema;
using Jvlink2Db.Jvlink.Com;
using Jvlink2Db.Pipeline.Setup;
using Npgsql;

namespace Jvlink2Db.Cli.Commands;

internal enum ResumeBehavior
{
    None,
    NormalIncremental,  // option=1 — read/write acquisition_state.last_fromtime
    SetupIncremental,   // option=3/4 — read last_filename and JVSkip past, write new last_filename
}

internal sealed record RunDescriptor(
    string Mode,
    string Connection,
    string Schema,
    string OperationalSchema,
    string Sid,
    string Dataspec,
    int Option,
    string Fromtime,
    ResumeBehavior Resume,
    bool Quiet = false);

internal static class ModeRunner
{
    public static Option<string> Connection() => new(
        name: "--connection",
        description: "Npgsql connection string for the target PostgreSQL.")
    { IsRequired = true };

    public static Option<string> Schema() => new(
        name: "--schema",
        description: "Database schema name. Created if it does not exist.",
        getDefaultValue: () => PostgresSchemaProvisioner.DefaultSchemaName);

    public static Option<string> OperationalSchema() => new(
        name: "--operational-schema",
        description: "Schema for acquisition_state and run_history.",
        getDefaultValue: () => PostgresOperationalSchemaProvisioner.DefaultSchemaName);

    public static Option<string> Dataspec() => new(
        name: "--dataspec",
        description: "Dataspec ID, e.g. RACE, DIFN, BLOD.")
    { IsRequired = true };

    public static Option<string> Sid() => new(
        name: "--sid",
        description: "Software identifier passed to JVInit.",
        getDefaultValue: () => "jvlink2db/0.1");

    public static Option<bool> Quiet() => new(
        name: "--quiet",
        description: "Suppress per-file and per-flush progress lines on stderr.");

    /// <summary>
    /// Reads <c>acquisition_state.last_fromtime</c> for normal-mode resume,
    /// or <c>last_filename</c> for setup-mode resume. Provisions the
    /// operational schema first so the lookup never fails on a fresh DB.
    /// </summary>
    public static async Task<AcquisitionState?> LookupStateAsync(
        string connection,
        string operationalSchema,
        string dataspec,
        int option,
        CancellationToken cancellationToken)
    {
        await using var dataSource = NpgsqlDataSource.Create(connection);
        await new PostgresOperationalSchemaProvisioner(dataSource, operationalSchema)
            .EnsureCreatedAsync(cancellationToken).ConfigureAwait(false);
        return await new PostgresAcquisitionStateStore(dataSource, operationalSchema)
            .GetAsync(dataspec, option, cancellationToken).ConfigureAwait(false);
    }

    public static async Task ExecuteAsync(
        System.CommandLine.Invocation.InvocationContext ctx,
        RunDescriptor run)
    {
        var token = ctx.GetCancellationToken();
        await using var dataSource = NpgsqlDataSource.Create(run.Connection);
        using var jv = new ComJvLink();

        var dataProvisioner = new PostgresSchemaProvisioner(dataSource, run.Schema);
        var operationalProvisioner = new PostgresOperationalSchemaProvisioner(dataSource, run.OperationalSchema);
        var stateStore = new PostgresAcquisitionStateStore(dataSource, run.OperationalSchema);
        var historyStore = new PostgresRunHistoryStore(dataSource, run.OperationalSchema);

        await operationalProvisioner.EnsureCreatedAsync(token).ConfigureAwait(false);

        string? resumeFromFilename = null;
        if (run.Resume == ResumeBehavior.SetupIncremental)
        {
            var saved = await stateStore.GetAsync(run.Dataspec, run.Option, token).ConfigureAwait(false);
            resumeFromFilename = saved?.LastFilename;
        }

        var startedAt = DateTimeOffset.UtcNow;
        var historyId = await historyStore.StartAsync(
            new RunHistoryStart(run.Mode, run.Dataspec, run.Option, run.Fromtime, startedAt),
            token).ConfigureAwait(false);

        var sinks = PostgresSinkFactory.CreateAll(dataSource, run.Schema);
        var progress = run.Quiet ? null : (Action<ProgressEvent>)WriteProgress;
        var runner = new SetupRunner(jv, dataProvisioner, sinks, progress: progress);

        try
        {
            var report = await runner.RunAsync(
                new SetupOptions(run.Sid, run.Dataspec, run.Fromtime, run.Option, resumeFromFilename),
                token).ConfigureAwait(false);

            await PersistResumeStateAsync(stateStore, run, report, token).ConfigureAwait(false);

            await historyStore.FinishAsync(historyId, new RunHistoryFinish(
                FinishedAt: DateTimeOffset.UtcNow,
                Outcome: "success",
                OpenReturnCode: report.OpenReturnCode,
                ReadCount: report.ReadCount,
                DownloadCount: report.DownloadCount,
                LastFileTimestamp: report.LastFileTimestamp,
                RecordCounts: report.RecordCountsById,
                RecordsInserted: report.RecordsInsertedById,
                ErrorMessage: null), token).ConfigureAwait(false);

            PrintReport(report, run.Schema);
            ctx.ExitCode = 0;
        }
        catch (JvLinkException ex)
        {
            await historyStore.FinishAsync(historyId, FailureFinish(ex.Message, openReturnCode: null), token).ConfigureAwait(false);
            Console.Error.WriteLine($"JV-Link error: {ex.Method} returned {ex.ReturnCode}");
            ctx.ExitCode = ExitCodeFor(ex.ReturnCode);
        }
        catch (Exception ex)
        {
            await historyStore.FinishAsync(historyId, FailureFinish(ex.Message, openReturnCode: null), token).ConfigureAwait(false);
            throw;
        }
    }

    private static async Task PersistResumeStateAsync(
        IAcquisitionStateStore store,
        RunDescriptor run,
        SetupReport report,
        CancellationToken cancellationToken)
    {
        if (report.OpenReturnCode != 0)
        {
            return;
        }

        switch (run.Resume)
        {
            case ResumeBehavior.NormalIncremental when !string.IsNullOrEmpty(report.LastFileTimestamp):
                await store.UpsertAsync(
                    new AcquisitionState(run.Dataspec, run.Option, report.LastFileTimestamp, LastFilename: null),
                    cancellationToken).ConfigureAwait(false);
                break;
            case ResumeBehavior.SetupIncremental when !string.IsNullOrEmpty(report.LastConsumedFilename):
                await store.UpsertAsync(
                    new AcquisitionState(run.Dataspec, run.Option, LastFromtime: null, report.LastConsumedFilename),
                    cancellationToken).ConfigureAwait(false);
                break;
        }
    }

    private static RunHistoryFinish FailureFinish(string error, int? openReturnCode) => new(
        FinishedAt: DateTimeOffset.UtcNow,
        Outcome: "failed",
        OpenReturnCode: openReturnCode,
        ReadCount: null,
        DownloadCount: null,
        LastFileTimestamp: null,
        RecordCounts: null,
        RecordsInserted: null,
        ErrorMessage: error);

    private static void PrintReport(SetupReport report, string schemaName)
    {
        Console.WriteLine($"Schema             : {schemaName}");
        Console.WriteLine($"OpenReturnCode     : {report.OpenReturnCode}");
        Console.WriteLine($"ReadCount          : {report.ReadCount}");
        Console.WriteLine($"DownloadCount      : {report.DownloadCount}");
        Console.WriteLine($"LastFileTimestamp  : {report.LastFileTimestamp}");
        if (!string.IsNullOrEmpty(report.LastConsumedFilename))
        {
            Console.WriteLine($"LastConsumedFile   : {report.LastConsumedFilename}");
        }

        if (report.ReadDuration > TimeSpan.Zero)
        {
            var totalFlushDuration = report.FlushDurationByRecordSpec is null
                ? TimeSpan.Zero
                : report.FlushDurationByRecordSpec.Values.Aggregate(TimeSpan.Zero, (acc, d) => acc + d);
            Console.WriteLine($"ReadDuration       : {Format(report.ReadDuration)} (read+decode time, includes per-file flush)");
            Console.WriteLine($"TotalFlushTime     : {Format(totalFlushDuration)} (sum across record-types — wall-clock overlap with read)");
        }
        Console.WriteLine();

        Console.WriteLine("Record counts by ID (read / inserted / flush):");
        if (report.RecordCountsById.Count == 0)
        {
            Console.WriteLine("  (no records)");
            return;
        }

        foreach (var kv in report.RecordCountsById.OrderByDescending(kv => kv.Value).ThenBy(kv => kv.Key, StringComparer.Ordinal))
        {
            var inserted = report.RecordsInsertedById.TryGetValue(kv.Key, out var n) ? n.ToString("N0") : "-";
            var flush = report.FlushDurationByRecordSpec is not null
                        && report.FlushDurationByRecordSpec.TryGetValue(kv.Key, out var d)
                ? Format(d)
                : "-";
            Console.WriteLine($"  {kv.Key}: {kv.Value:N0} / {inserted} / {flush}");
        }
    }

    private static int ExitCodeFor(int returnCode) => returnCode switch
    {
        >= -199 and <= -100 => 1,
        >= -299 and <= -200 => 2,
        >= -399 and <= -300 => 3,
        _ => 4,
    };

    private static void WriteProgress(ProgressEvent ev)
    {
        switch (ev)
        {
            case FileCompletedEvent f:
                Console.Error.WriteLine(
                    $"[progress] file {f.FileIndex}/{f.TotalFiles} {f.Filename} — {f.RecordsInFile:N0} records ({f.RecordsTotal:N0} total, {Format(f.Elapsed)} elapsed)");
                break;
            case FlushStartedEvent s when s.BufferedRecords > 0:
                Console.Error.WriteLine($"[progress] flush {s.RecordSpec} starting — {s.BufferedRecords:N0} buffered");
                break;
            case FlushCompletedEvent c when c.Inserted > 0:
                Console.Error.WriteLine($"[progress] flush {c.RecordSpec} done — {c.Inserted:N0} merged in {Format(c.Elapsed)}");
                break;
        }
    }

    private static string Format(TimeSpan ts) =>
        ts.TotalHours >= 1
            ? $"{(int)ts.TotalHours}:{ts.Minutes:D2}:{ts.Seconds:D2}"
            : $"{ts.Minutes:D2}:{ts.Seconds:D2}";
}
