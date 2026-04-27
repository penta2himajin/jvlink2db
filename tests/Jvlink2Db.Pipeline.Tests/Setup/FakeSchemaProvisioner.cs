using System.Threading;
using System.Threading.Tasks;
using Jvlink2Db.Core.Persistence;

namespace Jvlink2Db.Pipeline.Tests.Setup;

internal sealed class FakeSchemaProvisioner : ISchemaProvisioner
{
    public int CallCount { get; private set; }

    public Task EnsureCreatedAsync(CancellationToken cancellationToken)
    {
        CallCount++;
        return Task.CompletedTask;
    }
}
