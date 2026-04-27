using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Jvlink2Db.Core.Jvlink;
using Jvlink2Db.Pipeline.Probe;
using Xunit;

namespace Jvlink2Db.Pipeline.Tests.Probe;

public class ProbeRunnerTests
{
    [Fact]
    public async Task RunAsync_collects_record_counts_and_filenames_for_a_clean_run()
    {
        var open = new JvLinkOpenResult(
            ReturnCode: 0,
            ReadCount: 4,
            DownloadCount: 5,
            LastFileTimestamp: "20260331235959000");

        var fake = new FakeJvLink(
            openResult: open,
            statuses: new[] { 5 },
            reads: new[]
            {
                Record("RA"),
                Record("SE"),
                Record("SE"),
                JvLinkReadResult.EndOfFile("file1.dat"),
                Record("RA"),
                JvLinkReadResult.EndOfFile("file2.dat"),
                JvLinkReadResult.EndOfData,
            });

        var runner = new ProbeRunner(fake);
        var report = await runner.RunAsync(
            new ProbeOptions(Sid: "jvlink2db/test", Dataspec: "RACE", Fromtime: "20260101000000-20260331235959", Option: 4),
            CancellationToken.None);

        Assert.True(fake.Initialized);
        Assert.Equal("jvlink2db/test", fake.LastSid);
        Assert.Equal("RACE", fake.LastDataspec);
        Assert.Equal("20260101000000-20260331235959", fake.LastFromtime);
        Assert.Equal(4, fake.LastOption);
        Assert.True(fake.Closed);

        Assert.Equal(0, report.OpenReturnCode);
        Assert.Equal(4, report.ReadCount);
        Assert.Equal(5, report.DownloadCount);
        Assert.Equal("20260331235959000", report.LastFileTimestamp);
        Assert.Equal(2, report.RecordCountsById["RA"]);
        Assert.Equal(2, report.RecordCountsById["SE"]);
        Assert.Equal(new[] { "file1.dat", "file2.dat" }, report.Filenames);
        Assert.Equal(new[] { "RA", "SE", "SE", "RA" }, report.SampleRecordIds);
    }

    [Fact]
    public async Task RunAsync_retries_read_when_file_is_still_downloading()
    {
        var open = new JvLinkOpenResult(0, 1, 1, "20260101000000000");

        var fake = new FakeJvLink(
            openResult: open,
            statuses: new[] { 1 },
            reads: new[]
            {
                JvLinkReadResult.StillDownloading,
                JvLinkReadResult.StillDownloading,
                Record("RA"),
                JvLinkReadResult.EndOfFile("only.dat"),
                JvLinkReadResult.EndOfData,
            });

        var runner = new ProbeRunner(fake, pollDelay: System.TimeSpan.Zero);
        var report = await runner.RunAsync(
            new ProbeOptions("sid", "RACE", "20260101000000-", 4),
            CancellationToken.None);

        Assert.Equal(1, report.RecordCountsById["RA"]);
        Assert.Single(report.Filenames);
    }

    [Fact]
    public async Task RunAsync_throws_JvLinkException_when_Open_fails()
    {
        var fake = new FakeJvLink(
            openResult: new JvLinkOpenResult(ReturnCode: -116, ReadCount: 0, DownloadCount: 0, LastFileTimestamp: ""),
            statuses: System.Array.Empty<int>(),
            reads: System.Array.Empty<JvLinkReadResult>());

        var runner = new ProbeRunner(fake);
        var ex = await Assert.ThrowsAsync<JvLinkException>(async () =>
            await runner.RunAsync(new ProbeOptions("sid", "RACE", "20260101000000-", 1), CancellationToken.None));

        Assert.Equal(-116, ex.ReturnCode);
        Assert.Equal("JVOpen", ex.Method);
        Assert.True(fake.Closed, "Close must run even when Open returns an error");
    }

    [Fact]
    public async Task RunAsync_returns_empty_report_when_Open_returns_minus_one()
    {
        var fake = new FakeJvLink(
            openResult: new JvLinkOpenResult(ReturnCode: -1, ReadCount: 0, DownloadCount: 0, LastFileTimestamp: ""),
            statuses: System.Array.Empty<int>(),
            reads: System.Array.Empty<JvLinkReadResult>());

        var runner = new ProbeRunner(fake);
        var report = await runner.RunAsync(
            new ProbeOptions("sid", "RACE", "20260101000000-", 1),
            CancellationToken.None);

        Assert.Equal(-1, report.OpenReturnCode);
        Assert.Empty(report.RecordCountsById);
        Assert.Empty(report.Filenames);
        Assert.Empty(report.SampleRecordIds);
        Assert.True(fake.Closed);
    }

    private static JvLinkReadResult Record(string recordId)
    {
        var bytes = new byte[110000];
        Encoding.ASCII.GetBytes(recordId, 0, 2, bytes, 0);
        return JvLinkReadResult.Record(bytes, "active.dat");
    }
}
