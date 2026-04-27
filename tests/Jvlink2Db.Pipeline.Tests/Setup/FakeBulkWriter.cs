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

    public Func<List<T>, long> ResultSelector { get; set; } = list => list.Count;

    public Exception? Throws { get; set; }

    public async Task<long> WriteAsync(IAsyncEnumerable<T> records, CancellationToken cancellationToken)
    {
        CallCount++;
        if (Throws is not null)
        {
            throw Throws;
        }

        await foreach (var item in records.WithCancellation(cancellationToken).ConfigureAwait(false))
        {
            Written.Add(item);
        }

        return ResultSelector(Written);
    }
}
