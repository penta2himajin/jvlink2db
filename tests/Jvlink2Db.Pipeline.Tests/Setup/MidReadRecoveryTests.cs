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
        // After recovery (delete + reopen + skip past f1) we re-read f2 (now good) and then f3.
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
                // After recovery: re-read f2 cleanly, then f3
                JvLinkReadResult.Record(TestBuffers.Ra("02"), "f2.dat"),
                JvLinkReadResult.EndOfFile("f2.dat"),
                JvLinkReadResult.Record(TestBuffers.Ra("03"), "f3.dat"),
                JvLinkReadResult.EndOfFile("f3.dat"),
                JvLinkReadResult.EndOfData,
            },
            skips: new[]
            {
                new JvLinkSkipResult(0, "f1.dat"),  // skip until f1 (the last completed file)
            });
        var writer = new FakeBulkWriter<Ra>();
        var runner = new SetupRunner(jv, new FakeSchemaProvisioner(),
            new IRecordSink[] { new RecordSink<Ra>("RA", RaDecoder.Decode, writer) },
            pollDelay: TimeSpan.Zero);

        var report = await runner.RunAsync(NewOptions(), CancellationToken.None);

        Assert.Equal(2, jv.OpenCallCount);                     // initial + 1 recovery reopen
        Assert.Equal(new[] { "f2.dat" }, jv.DeletedFiles);    // corrupt file deleted
        Assert.Equal(1, jv.SkipCallCount);                    // skipped f1 to resume past it

        // Final result: f1 + f2 + f3 all merged exactly once each (the partial f2 read was discarded).
        Assert.Equal(new[] { "01", "02", "03" }, writer.Written.Select(r => r.RaceNum).ToArray());
        Assert.Equal(3, report.RecordsInsertedById["RA"]);
    }

    [Fact]
    public async Task RunAsync_propagates_403_after_max_retries_per_file()
    {
        // Same file fails -403 even after one recovery → SetupRunner gives up.
        var jv = new FakeJvLink(
            openResults: Enumerable.Repeat(new JvLinkOpenResult(0, 2, 0, "ts"), 5),
            statuses: Enumerable.Repeat(2, 5),
            reads: new[]
            {
                JvLinkReadResult.Record(TestBuffers.Ra("01"), "f1.dat"),
                JvLinkReadResult.EndOfFile("f1.dat"),
                new JvLinkReadResult(-403, null, "f2.dat"),  // attempt 1
                new JvLinkReadResult(-403, null, "f2.dat"),  // attempt 2
                new JvLinkReadResult(-403, null, "f2.dat"),  // attempt 3
                new JvLinkReadResult(-403, null, "f2.dat"),  // attempt 4 — exhausts budget
            },
            skips: Enumerable.Repeat(new JvLinkSkipResult(0, "f1.dat"), 4));
        var writer = new FakeBulkWriter<Ra>();
        var runner = new SetupRunner(jv, new FakeSchemaProvisioner(),
            new IRecordSink[] { new RecordSink<Ra>("RA", RaDecoder.Decode, writer) },
            pollDelay: TimeSpan.Zero);

        var ex = await Assert.ThrowsAsync<JvLinkException>(() =>
            runner.RunAsync(NewOptions(), CancellationToken.None));

        Assert.Equal(-403, ex.ReturnCode);
        Assert.Equal(3, jv.DeletedFiles.Count);  // 3 retries, each one deletes the file before reopening
    }

    private static SetupOptions NewOptions() =>
        new("sid", "RACE", "20260101000000", 4);
}
