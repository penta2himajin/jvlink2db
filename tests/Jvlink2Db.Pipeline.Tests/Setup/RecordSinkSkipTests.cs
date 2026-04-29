using System;
using System.Threading;
using System.Threading.Tasks;
using Jvlink2Db.Parser.Records;
using Jvlink2Db.Pipeline.Setup;
using Xunit;

namespace Jvlink2Db.Pipeline.Tests.Setup;

public class RecordSinkSkipTests
{
    [Fact]
    public void Add_increments_SkippedCount_when_decoder_throws_RecordTooShortException()
    {
        var writer = new FakeBulkWriter<string>();
        var sink = new RecordSink<string>(
            "ZZ",
            buffer => throw new RecordTooShortException("ZZ", buffer.Length, 100),
            writer);

        sink.Add(new byte[10]);
        sink.Add(new byte[20]);

        Assert.Equal(2, sink.SkippedCount);
        Assert.Equal(0, sink.BufferedCount);
    }

    [Fact]
    public void Add_propagates_exceptions_other_than_RecordTooShort()
    {
        var sink = new RecordSink<string>(
            "ZZ",
            _ => throw new InvalidOperationException("not a ZZ record"),
            new FakeBulkWriter<string>());

        Assert.Throws<InvalidOperationException>(() => sink.Add(new byte[10]));
    }

    [Fact]
    public async Task FlushAsync_returns_zero_when_only_skipped_records()
    {
        var writer = new FakeBulkWriter<string>();
        var sink = new RecordSink<string>(
            "ZZ",
            buffer => throw new RecordTooShortException("ZZ", 1, 100),
            writer);

        sink.Add(new byte[1]);

        Assert.Equal(0, await sink.FlushAsync(CancellationToken.None));
        Assert.Equal(0, writer.CallCount);
    }
}
