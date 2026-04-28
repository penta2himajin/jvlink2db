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

public class MidReadRecoveryTests
{
    [Fact]
    public async Task RunAsync_recovers_from_403_by_deleting_file_reopening_and_skipping()
    {
        // Scenario: 3 files. f1 reads cleanly. f2 throws -403 mid-read.
        // Recovery: delete f2, close, reopen. JV-Link restarts from f1 → we
        // peek f1's first record, see it's the resume target, JVSkip the rest
        // of f1. Next read is f2's first record (fresh re-download), then f3.
        var jv = new FakeJvLink(
            openResults: new[]
            {
                new JvLinkOpenResult(0, 3, 0, "ts"),
                new JvLinkOpenResult(0, 3, 0, "ts"),  // post-recovery reopen
            },
            statuses: new[] { 3, 3 },
            reads: new[]
            {
                JvLinkReadResult.Record(TestBuffers.Ra("01"), "f1.dat"),
                JvLinkReadResult.EndOfFile("f1.dat"),
                // f2 first attempt: a partial record then -403
                JvLinkReadResult.Record(TestBuffers.Ra("02"), "f2.dat"),
                new JvLinkReadResult(-403, null, "f2.dat"),
                // After recovery — skip-past-resume peek of f1, then JVSkip
                JvLinkReadResult.Record(TestBuffers.Ra("01"), "f1.dat"),
                // next reads after JVSkip — f2 (re-downloaded, now good), then f3
                JvLinkReadResult.Record(TestBuffers.Ra("02"), "f2.dat"),
                JvLinkReadResult.EndOfFile("f2.dat"),
                JvLinkReadResult.Record(TestBuffers.Ra("03"), "f3.dat"),
                JvLinkReadResult.EndOfFile("f3.dat"),
                JvLinkReadResult.EndOfData,
            });
        var writer = new FakeBulkWriter<Ra>();
        var runner = new SetupRunner(jv, new FakeSchemaProvisioner(),
            new IRecordSink[] { new RecordSink<Ra>("RA", RaDecoder.Decode, writer) },
            pollDelay: TimeSpan.Zero);

        var report = await runner.RunAsync(NewOptions(), CancellationToken.None);

        Assert.Equal(2, jv.OpenCallCount);                     // initial + 1 recovery reopen
        Assert.Equal(new[] { "f2.dat" }, jv.DeletedFiles);    // corrupt file deleted
        Assert.Equal(1, jv.SkipCallCount);                    // f1 skipped during resume

        // Final result: f1 + f2 + f3 all merged exactly once each (the partial f2 read was discarded).
        Assert.Equal(new[] { "01", "02", "03" }, writer.Written.Select(r => r.RaceNum).ToArray());
        Assert.Equal(3, report.RecordsInsertedById["RA"]);
    }

    [Fact]
    public async Task RunAsync_propagates_403_after_max_retries_per_file()
    {
        // Same file fails -403 across attempts. Budget is 3 retries per file
        // (MaxRecoveryRetriesPerFile in SetupRunner) → 4th -403 re-throws.
        var jv = new FakeJvLink(
            openResults: Enumerable.Repeat(new JvLinkOpenResult(0, 2, 0, "ts"), 5),
            statuses: Enumerable.Repeat(2, 5),
            reads: new[]
            {
                JvLinkReadResult.Record(TestBuffers.Ra("01"), "f1.dat"),
                JvLinkReadResult.EndOfFile("f1.dat"),
                new JvLinkReadResult(-403, null, "f2.dat"),  // attempt 1
                // recovery 1: peek f1, JVSkip
                JvLinkReadResult.Record(TestBuffers.Ra("01"), "f1.dat"),
                new JvLinkReadResult(-403, null, "f2.dat"),  // attempt 2
                // recovery 2: peek f1, JVSkip
                JvLinkReadResult.Record(TestBuffers.Ra("01"), "f1.dat"),
                new JvLinkReadResult(-403, null, "f2.dat"),  // attempt 3
                // recovery 3: peek f1, JVSkip
                JvLinkReadResult.Record(TestBuffers.Ra("01"), "f1.dat"),
                new JvLinkReadResult(-403, null, "f2.dat"),  // attempt 4 — exhausts budget
            });
        var writer = new FakeBulkWriter<Ra>();
        var runner = new SetupRunner(jv, new FakeSchemaProvisioner(),
            new IRecordSink[] { new RecordSink<Ra>("RA", RaDecoder.Decode, writer) },
            pollDelay: TimeSpan.Zero);

        var ex = await Assert.ThrowsAsync<JvLinkException>(() =>
            runner.RunAsync(NewOptions(), CancellationToken.None));

        Assert.Equal(-403, ex.ReturnCode);
        Assert.Equal(3, jv.DeletedFiles.Count);  // 3 successful recoveries before the 4th re-throws
    }

    private static SetupOptions NewOptions() =>
        new("sid", "RACE", "20260101000000", 4);
}
