using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Jvlink2Db.Core.Persistence;

namespace Jvlink2Db.Pipeline.Setup;

public sealed class RecordSink<TRecord> : IRecordSink
{
    private readonly Func<byte[], TRecord> _decode;
    private readonly IBulkWriter<TRecord> _writer;
    private readonly List<TRecord> _records = new();

    public RecordSink(string recordSpec, Func<byte[], TRecord> decode, IBulkWriter<TRecord> writer)
    {
        ArgumentException.ThrowIfNullOrEmpty(recordSpec);
        ArgumentNullException.ThrowIfNull(decode);
        ArgumentNullException.ThrowIfNull(writer);

        RecordSpec = recordSpec;
        _decode = decode;
        _writer = writer;
    }

    public string RecordSpec { get; }

    public void Add(byte[] buffer)
    {
        ArgumentNullException.ThrowIfNull(buffer);
        _records.Add(_decode(buffer));
    }

    public Task<long> FlushAsync(CancellationToken cancellationToken) =>
        _records.Count == 0
            ? Task.FromResult(0L)
            : _writer.WriteAsync(ToAsync(cancellationToken), cancellationToken);

    private async IAsyncEnumerable<TRecord> ToAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        foreach (var r in _records)
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return r;
            await Task.Yield();
        }
    }
}
