using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Jvlink2Db.Core.Persistence;

namespace Jvlink2Db.Pipeline.Tests.Setup;

internal sealed class FakeBulkWriter<T> : IBulkWriter<T>
{
    public List<T> Written { get; } = new();

    public int CallCount { get; private set; }

    public bool ProvisionerCalledFirst { get; private set; }

    public Func<List<T>, long>? ResultSelector { get; set; }

    public Exception? Throws { get; set; }

    public async Task<long> WriteAsync(IAsyncEnumerable<T> records, CancellationToken cancellationToken)
    {
        CallCount++;
        if (Throws is not null)
        {
            throw Throws;
        }

        var batchStart = Written.Count;
        await foreach (var item in records.WithCancellation(cancellationToken).ConfigureAwait(false))
        {
            Written.Add(item);
        }

        // Production IBulkWriter.WriteAsync returns the rows merged in
        // *this* batch — match that contract so per-file flushes don't
        // double-count by returning the cumulative buffer size.
        return ResultSelector?.Invoke(Written) ?? (Written.Count - batchStart);
    }
}
