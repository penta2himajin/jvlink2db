using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Jvlink2Db.Core.Persistence;
using Jvlink2Db.Parser.Records;

namespace Jvlink2Db.Pipeline.Setup;

public sealed class RecordSink<TRecord> : IRecordSink
{
    private readonly Func<byte[], TRecord> _decode;
    private readonly IBulkWriter<TRecord> _writer;
    private List<TRecord> _records = new();

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

    public int BufferedCount => _records.Count;

    public int SkippedCount { get; private set; }

    public void Add(byte[] buffer)
    {
        ArgumentNullException.ThrowIfNull(buffer);
        try
        {
            _records.Add(_decode(buffer));
        }
        catch (RecordTooShortException)
        {
            // The decoder doesn't have a layout for buffers this short.
            // Common cause: the upstream service is delivering an older
            // spec version than the SDK we built decoders against. Skip
            // and surface the count in the run report so the user can
            // decide whether the missed rows matter.
            SkippedCount++;
        }
    }

    public void ClearBuffer() => _records.Clear();

    public async Task<long> FlushAsync(CancellationToken cancellationToken)
    {
        if (_records.Count == 0)
        {
            return 0;
        }

        // Snapshot the current buffer so it can be cleared eagerly: the
        // writer can iterate the snapshot while new records (next file)
        // accumulate into _records — bounding memory to two file's worth
        // in the rare overlap window.
        var snapshot = _records;
        _records = new List<TRecord>();

        var inserted = await _writer.WriteAsync(ToAsync(snapshot, cancellationToken), cancellationToken)
            .ConfigureAwait(false);
        return inserted;
    }

    private static async IAsyncEnumerable<TRecord> ToAsync(
        List<TRecord> source,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        foreach (var r in source)
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return r;
            await Task.Yield();
        }
    }
}
