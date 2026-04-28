using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Jvlink2Db.Core.Jvlink;
using Jvlink2Db.Core.Records;
using Jvlink2Db.Parser.Records;
using Jvlink2Db.Pipeline.Setup;
using Jvlink2Db.Pipeline.Tests.Probe;
using Xunit;

namespace Jvlink2Db.Pipeline.Tests.Setup;

public class ProgressEventTests
{
    [Fact]
    public async Task RunAsync_emits_FileCompleted_per_file_with_per_file_record_counts()
    {
        var jv = new FakeJvLink(
            openResult: new JvLinkOpenResult(0, 3, 3, "ts"),
            statuses: new[] { 3 },
            reads: new[]
            {
                JvLinkReadResult.Record(TestBuffers.Ra("01"), "f1.dat"),
                JvLinkReadResult.Record(TestBuffers.Ra("02"), "f1.dat"),
                JvLinkReadResult.EndOfFile("f1.dat"),
                JvLinkReadResult.Record(TestBuffers.Ra("03"), "f2.dat"),
                JvLinkReadResult.EndOfFile("f2.dat"),
                JvLinkReadResult.Record(TestBuffers.Ra("04"), "f3.dat"),
                JvLinkReadResult.Record(TestBuffers.Ra("05"), "f3.dat"),
                JvLinkReadResult.Record(TestBuffers.Ra("06"), "f3.dat"),
                JvLinkReadResult.EndOfFile("f3.dat"),
                JvLinkReadResult.EndOfData,
            });

        var captured = new List<ProgressEvent>();
        var runner = NewRunner(jv, new FakeSchemaProvisioner(), new FakeBulkWriter<Ra>(), captured.Add);

        await runner.RunAsync(NewOptions(), CancellationToken.None);

        var fileEvents = captured.OfType<FileCompletedEvent>().ToList();
        Assert.Equal(3, fileEvents.Count);
        Assert.Equal(("f1.dat", 1, 3, 2, 2), Tuple(fileEvents[0]));
        Assert.Equal(("f2.dat", 2, 3, 1, 3), Tuple(fileEvents[1]));
        Assert.Equal(("f3.dat", 3, 3, 3, 6), Tuple(fileEvents[2]));
    }

    [Fact]
    public async Task RunAsync_emits_Flush_events_for_each_sink_with_records()
    {
        var jv = new FakeJvLink(
            openResult: new JvLinkOpenResult(0, 1, 1, "ts"),
            statuses: new[] { 1 },
            reads: new[]
            {
                JvLinkReadResult.Record(TestBuffers.Ra("01"), "f.dat"),
                JvLinkReadResult.EndOfFile("f.dat"),
                JvLinkReadResult.EndOfData,
            });

        var captured = new List<ProgressEvent>();
        var runner = NewRunner(jv, new FakeSchemaProvisioner(), new FakeBulkWriter<Ra>(), captured.Add);

        await runner.RunAsync(NewOptions(), CancellationToken.None);

        var started = captured.OfType<FlushStartedEvent>().Single(e => e.RecordSpec == "RA");
        var completed = captured.OfType<FlushCompletedEvent>().Single(e => e.RecordSpec == "RA");
        Assert.Equal(1, started.BufferedRecords);
        Assert.Equal(1, completed.Inserted);
    }

    [Fact]
    public async Task RunAsync_emits_no_events_when_progress_callback_is_null()
    {
        // Implicit guarantee: the runner must not throw when no progress callback is wired.
        var jv = new FakeJvLink(
            openResult: new JvLinkOpenResult(0, 1, 1, "ts"),
            statuses: new[] { 1 },
            reads: new[]
            {
                JvLinkReadResult.Record(TestBuffers.Ra("01"), "f.dat"),
                JvLinkReadResult.EndOfFile("f.dat"),
                JvLinkReadResult.EndOfData,
            });

        var runner = NewRunner(jv, new FakeSchemaProvisioner(), new FakeBulkWriter<Ra>(), progress: null);

        await runner.RunAsync(NewOptions(), CancellationToken.None);
        // No assertion beyond "no throw".
    }

    [Fact]
    public async Task RunAsync_flushes_each_sink_at_every_file_boundary_to_bound_memory()
    {
        var jv = new FakeJvLink(
            openResult: new JvLinkOpenResult(0, 3, 3, "ts"),
            statuses: new[] { 3 },
            reads: new[]
            {
                JvLinkReadResult.Record(TestBuffers.Ra("01"), "f1.dat"),
                JvLinkReadResult.Record(TestBuffers.Ra("02"), "f1.dat"),
                JvLinkReadResult.EndOfFile("f1.dat"),
                JvLinkReadResult.Record(TestBuffers.Ra("03"), "f2.dat"),
                JvLinkReadResult.EndOfFile("f2.dat"),
                JvLinkReadResult.Record(TestBuffers.Ra("04"), "f3.dat"),
                JvLinkReadResult.EndOfFile("f3.dat"),
                JvLinkReadResult.EndOfData,
            });

        var captured = new List<ProgressEvent>();
        var writer = new FakeBulkWriter<Ra>();
        var runner = NewRunner(jv, new FakeSchemaProvisioner(), writer, captured.Add);

        var report = await runner.RunAsync(NewOptions(), CancellationToken.None);

        var flushStarts = captured.OfType<FlushStartedEvent>().Where(e => e.RecordSpec == "RA").ToList();
        Assert.Equal(3, flushStarts.Count);
        Assert.Equal(2, flushStarts[0].BufferedRecords);  // f1: 01 + 02
        Assert.Equal(1, flushStarts[1].BufferedRecords);  // f2: 03
        Assert.Equal(1, flushStarts[2].BufferedRecords);  // f3: 04

        Assert.Equal(4, report.RecordsInsertedById["RA"]);
        Assert.Equal(3, writer.CallCount);  // one WriteAsync per file (no leftover for the safety-net flush)
    }

    private static (string filename, int fileIndex, int totalFiles, int inFile, int total) Tuple(FileCompletedEvent e) =>
        (e.Filename, e.FileIndex, e.TotalFiles, e.RecordsInFile, e.RecordsTotal);

    private static SetupOptions NewOptions() =>
        new("sid", "RACE", "20260101000000", 4);

    private static SetupRunner NewRunner(
        FakeJvLink jv, FakeSchemaProvisioner prov, FakeBulkWriter<Ra> writer,
        Action<ProgressEvent>? progress) =>
        new(jv, prov,
            new IRecordSink[] { new RecordSink<Ra>("RA", RaDecoder.Decode, writer) },
            pollDelay: TimeSpan.Zero,
            progress: progress);
}
