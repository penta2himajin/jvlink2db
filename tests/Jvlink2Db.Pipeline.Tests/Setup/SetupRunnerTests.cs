using System;
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

public class SetupRunnerTests
{
    [Fact]
    public async Task RunAsync_provisions_schema_before_running_protocol()
    {
        var (jv, prov, writer) = NewFakes();
        var runner = NewRunner(jv, prov, writer);

        await runner.RunAsync(NewOptions(), CancellationToken.None);

        Assert.Equal(1, prov.CallCount);
        Assert.True(jv.Initialized);
        Assert.True(jv.Opened);
        Assert.True(jv.Closed);
    }

    [Fact]
    public async Task RunAsync_decodes_RA_records_and_writes_them()
    {
        var open = new JvLinkOpenResult(0, 3, 1, "20260331235959000");
        var jv = new FakeJvLink(
            openResult: open,
            statuses: new[] { 1 },
            reads: new[]
            {
                JvLinkReadResult.Record(TestBuffers.Ra("01"), "race1.dat"),
                JvLinkReadResult.Record(TestBuffers.Ra("02"), "race1.dat"),
                JvLinkReadResult.EndOfFile("race1.dat"),
                JvLinkReadResult.Record(TestBuffers.Ra("03"), "race2.dat"),
                JvLinkReadResult.EndOfFile("race2.dat"),
                JvLinkReadResult.EndOfData,
            });
        var prov = new FakeSchemaProvisioner();
        var writer = new FakeBulkWriter<Ra>();
        var runner = NewRunner(jv, prov, writer);

        var report = await runner.RunAsync(NewOptions(), CancellationToken.None);

        Assert.Equal(3, writer.Written.Count);
        Assert.Equal(new[] { "01", "02", "03" }, writer.Written.Select(r => r.RaceNum).ToArray());
        Assert.Equal(3, report.RaInserted);
        Assert.Equal(3, report.RecordCountsById["RA"]);
        Assert.Equal(3, report.RecordsInsertedById["RA"]);
    }

    [Fact]
    public async Task RunAsync_skips_unregistered_record_specs_but_counts_them()
    {
        var open = new JvLinkOpenResult(0, 1, 1, "20260331235959000");
        var jv = new FakeJvLink(
            openResult: open,
            statuses: new[] { 1 },
            reads: new[]
            {
                JvLinkReadResult.Record(TestBuffers.Ra("01"), "f.dat"),
                JvLinkReadResult.Record(TestBuffers.NonRa("ZZ"), "f.dat"),
                JvLinkReadResult.Record(TestBuffers.NonRa("ZZ"), "f.dat"),
                JvLinkReadResult.Record(TestBuffers.NonRa("YY"), "f.dat"),
                JvLinkReadResult.EndOfFile("f.dat"),
                JvLinkReadResult.EndOfData,
            });
        var prov = new FakeSchemaProvisioner();
        var writer = new FakeBulkWriter<Ra>();
        var runner = NewRunner(jv, prov, writer);

        var report = await runner.RunAsync(NewOptions(), CancellationToken.None);

        Assert.Single(writer.Written);
        Assert.Equal(1, report.RecordCountsById["RA"]);
        Assert.Equal(2, report.RecordCountsById["ZZ"]);
        Assert.Equal(1, report.RecordCountsById["YY"]);
        Assert.False(report.RecordsInsertedById.ContainsKey("ZZ"));
        Assert.False(report.RecordsInsertedById.ContainsKey("YY"));
    }

    [Fact]
    public async Task RunAsync_returns_empty_report_when_open_returns_minus_one()
    {
        var jv = new FakeJvLink(
            openResult: new JvLinkOpenResult(-1, 0, 0, ""),
            statuses: Array.Empty<int>(),
            reads: Array.Empty<JvLinkReadResult>());
        var prov = new FakeSchemaProvisioner();
        var writer = new FakeBulkWriter<Ra>();
        var runner = NewRunner(jv, prov, writer);

        var report = await runner.RunAsync(NewOptions(), CancellationToken.None);

        Assert.Equal(-1, report.OpenReturnCode);
        Assert.Equal(0, report.RaInserted);
        Assert.Empty(writer.Written);
        Assert.Equal(1, prov.CallCount);  // schema is still provisioned
        Assert.True(jv.Closed);
    }

    [Fact]
    public async Task RunAsync_throws_JvLinkException_when_open_fails_and_still_closes()
    {
        var jv = new FakeJvLink(
            openResult: new JvLinkOpenResult(-116, 0, 0, ""),
            statuses: Array.Empty<int>(),
            reads: Array.Empty<JvLinkReadResult>());
        var prov = new FakeSchemaProvisioner();
        var writer = new FakeBulkWriter<Ra>();
        var runner = NewRunner(jv, prov, writer);

        var ex = await Assert.ThrowsAsync<JvLinkException>(() =>
            runner.RunAsync(NewOptions(), CancellationToken.None));

        Assert.Equal(-116, ex.ReturnCode);
        Assert.Equal("JVOpen", ex.Method);
        Assert.True(jv.Closed);
        Assert.Equal(0, writer.CallCount);
    }

    [Fact]
    public async Task RunAsync_calls_close_even_when_writer_fails()
    {
        var open = new JvLinkOpenResult(0, 1, 1, "20260331235959000");
        var jv = new FakeJvLink(
            openResult: open,
            statuses: new[] { 1 },
            reads: new[]
            {
                JvLinkReadResult.Record(TestBuffers.Ra("01"), "f.dat"),
                JvLinkReadResult.EndOfFile("f.dat"),
                JvLinkReadResult.EndOfData,
            });
        var prov = new FakeSchemaProvisioner();
        var writer = new FakeBulkWriter<Ra> { Throws = new InvalidOperationException("db down") };
        var runner = NewRunner(jv, prov, writer);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            runner.RunAsync(NewOptions(), CancellationToken.None));

        Assert.True(jv.Closed);
    }

    [Fact]
    public async Task RunAsync_polls_status_until_caught_up()
    {
        var open = new JvLinkOpenResult(0, 1, 3, "20260331235959000");
        var jv = new FakeJvLink(
            openResult: open,
            statuses: new[] { 1, 2, 3 },
            reads: new[]
            {
                JvLinkReadResult.Record(TestBuffers.Ra("01"), "f.dat"),
                JvLinkReadResult.EndOfFile("f.dat"),
                JvLinkReadResult.EndOfData,
            });
        var prov = new FakeSchemaProvisioner();
        var writer = new FakeBulkWriter<Ra>();
        var runner = NewRunner(jv, prov, writer);

        var report = await runner.RunAsync(NewOptions(), CancellationToken.None);

        Assert.Single(writer.Written);
        Assert.Equal(1, report.RaInserted);
    }

    [Fact]
    public async Task RunAsync_dispatches_to_multiple_sinks_by_record_spec()
    {
        var open = new JvLinkOpenResult(0, 1, 1, "20260331235959000");
        var jv = new FakeJvLink(
            openResult: open,
            statuses: new[] { 1 },
            reads: new[]
            {
                JvLinkReadResult.Record(TestBuffers.Ra("01"), "f.dat"),
                JvLinkReadResult.Record(TestBuffers.NonRa("ZZ"), "f.dat"),
                JvLinkReadResult.Record(TestBuffers.NonRa("ZZ"), "f.dat"),
                JvLinkReadResult.EndOfFile("f.dat"),
                JvLinkReadResult.EndOfData,
            });
        var prov = new FakeSchemaProvisioner();
        var raWriter = new FakeBulkWriter<Ra>();
        var zzWriter = new FakeBulkWriter<byte[]>();
        var sinks = new IRecordSink[]
        {
            new RecordSink<Ra>("RA", RaDecoder.Decode, raWriter),
            new RecordSink<byte[]>("ZZ", b => b, zzWriter),
        };
        var runner = new SetupRunner(jv, prov, sinks, pollDelay: TimeSpan.Zero);

        var report = await runner.RunAsync(NewOptions(), CancellationToken.None);

        Assert.Single(raWriter.Written);
        Assert.Equal(2, zzWriter.Written.Count);
        Assert.Equal(1, report.RecordsInsertedById["RA"]);
        Assert.Equal(2, report.RecordsInsertedById["ZZ"]);
    }

    [Fact]
    public async Task RunAsync_reports_last_consumed_filename()
    {
        var open = new JvLinkOpenResult(0, 2, 2, "20260331235959000");
        var jv = new FakeJvLink(
            openResult: open,
            statuses: new[] { 2 },
            reads: new[]
            {
                JvLinkReadResult.Record(TestBuffers.Ra("01"), "f1.dat"),
                JvLinkReadResult.EndOfFile("f1.dat"),
                JvLinkReadResult.Record(TestBuffers.Ra("02"), "f2.dat"),
                JvLinkReadResult.EndOfFile("f2.dat"),
                JvLinkReadResult.EndOfData,
            });
        var runner = NewRunner(jv, new FakeSchemaProvisioner(), new FakeBulkWriter<Ra>());

        var report = await runner.RunAsync(NewOptions(), CancellationToken.None);

        Assert.Equal("f2.dat", report.LastConsumedFilename);
    }

    [Fact]
    public async Task RunAsync_skips_files_via_JVSkip_when_ResumeFromFilename_is_set()
    {
        var open = new JvLinkOpenResult(0, 3, 3, "20260331235959000");
        var jv = new FakeJvLink(
            openResult: open,
            statuses: new[] { 3 },
            reads: new[]
            {
                JvLinkReadResult.Record(TestBuffers.Ra("03"), "f3.dat"),
                JvLinkReadResult.EndOfFile("f3.dat"),
                JvLinkReadResult.EndOfData,
            },
            skips: new[]
            {
                new JvLinkSkipResult(0, "f1.dat"),
                new JvLinkSkipResult(0, "f2.dat"),  // matches ResumeFromFilename → loop exits
            });
        var writer = new FakeBulkWriter<Ra>();
        var runner = NewRunner(jv, new FakeSchemaProvisioner(), writer);

        var report = await runner.RunAsync(
            new SetupOptions("sid", "RACE", "20260101000000", 4, ResumeFromFilename: "f2.dat"),
            CancellationToken.None);

        Assert.Equal(2, jv.SkipCallCount);
        Assert.Single(writer.Written);
        Assert.Equal("03", writer.Written[0].RaceNum);
        Assert.Equal("f3.dat", report.LastConsumedFilename);
    }

    [Fact]
    public async Task RunAsync_retries_JvOpen_on_transient_codes_via_retry_policy()
    {
        var jv = new FakeJvLink(
            openResults: new[]
            {
                new JvLinkOpenResult(-301, 0, 0, ""),
                new JvLinkOpenResult(-301, 0, 0, ""),
                new JvLinkOpenResult(0, 1, 0, "ts"),
            },
            statuses: new[] { 1 },
            reads: new[]
            {
                JvLinkReadResult.Record(TestBuffers.Ra("01"), "f.dat"),
                JvLinkReadResult.EndOfFile("f.dat"),
                JvLinkReadResult.EndOfData,
            });
        var prov = new FakeSchemaProvisioner();
        var writer = new FakeBulkWriter<Ra>();
        var policy = new Pipeline.Retry.JvLinkRetryPolicy(
            recorder: (_, __) => Task.CompletedTask);

        var runner = new SetupRunner(jv, prov,
            new IRecordSink[] { new RecordSink<Ra>("RA", RaDecoder.Decode, writer) },
            pollDelay: TimeSpan.Zero,
            retryPolicy: policy);

        var report = await runner.RunAsync(NewOptions(), CancellationToken.None);

        Assert.Equal(3, jv.OpenCallCount);
        Assert.Single(writer.Written);
        Assert.Equal(0, report.OpenReturnCode);
    }

    [Fact]
    public async Task RunAsync_throws_after_retry_budget_is_exhausted()
    {
        var jv = new FakeJvLink(
            openResults: new[]
            {
                new JvLinkOpenResult(-301, 0, 0, ""),
                new JvLinkOpenResult(-301, 0, 0, ""),
                new JvLinkOpenResult(-301, 0, 0, ""),
                new JvLinkOpenResult(-301, 0, 0, ""),
            },
            statuses: System.Array.Empty<int>(),
            reads: System.Array.Empty<JvLinkReadResult>());
        var prov = new FakeSchemaProvisioner();
        var writer = new FakeBulkWriter<Ra>();
        var policy = new Pipeline.Retry.JvLinkRetryPolicy(
            recorder: (_, __) => Task.CompletedTask);

        var runner = new SetupRunner(jv, prov,
            new IRecordSink[] { new RecordSink<Ra>("RA", RaDecoder.Decode, writer) },
            pollDelay: TimeSpan.Zero,
            retryPolicy: policy);

        var ex = await Assert.ThrowsAsync<JvLinkException>(() =>
            runner.RunAsync(NewOptions(), CancellationToken.None));

        Assert.Equal(-301, ex.ReturnCode);
        Assert.Equal(4, jv.OpenCallCount);  // initial + 3 retries from default schedule
    }

    private static SetupOptions NewOptions() =>
        new(Sid: "jvlink2db/test", Dataspec: "RACE", Fromtime: "20260101000000-20260331235959", Option: 4);

    private static (FakeJvLink, FakeSchemaProvisioner, FakeBulkWriter<Ra>) NewFakes()
    {
        var jv = new FakeJvLink(
            openResult: new JvLinkOpenResult(0, 0, 0, "20260331235959000"),
            statuses: new[] { 0 },
            reads: new[] { JvLinkReadResult.EndOfData });
        return (jv, new FakeSchemaProvisioner(), new FakeBulkWriter<Ra>());
    }

    private static SetupRunner NewRunner(FakeJvLink jv, FakeSchemaProvisioner prov, FakeBulkWriter<Ra> writer) =>
        new(jv, prov,
            new IRecordSink[] { new RecordSink<Ra>("RA", RaDecoder.Decode, writer) },
            pollDelay: TimeSpan.Zero);
}
