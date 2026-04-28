using System;
using System.Threading;
using System.Threading.Tasks;

namespace Jvlink2Db.Pipeline.Watch;

/// <summary>
/// Runs a single-shot work function repeatedly with a fixed interval
/// between cycles. Per-cycle errors are reported through <c>onError</c>
/// and do not stop the loop; only cancellation through the supplied
/// <see cref="CancellationToken"/> ends it.
/// </summary>
public sealed class WatchRunner
{
    private readonly Func<TimeSpan, CancellationToken, Task> _delay;
    private readonly Action<int, Exception>? _onError;

    public WatchRunner(
        Func<TimeSpan, CancellationToken, Task>? delay = null,
        Action<int, Exception>? onError = null)
    {
        _delay = delay ?? Task.Delay;
        _onError = onError;
    }

    public async Task RunAsync(
        Func<CancellationToken, Task> runOnce,
        TimeSpan interval,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(runOnce);

        var cycle = 0;
        while (!cancellationToken.IsCancellationRequested)
        {
            cycle++;
            var startedAt = DateTimeOffset.UtcNow;

            try
            {
                await runOnce(cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                return;
            }
            catch (Exception ex)
            {
                _onError?.Invoke(cycle, ex);
            }

            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            var elapsed = DateTimeOffset.UtcNow - startedAt;
            var remaining = interval - elapsed;
            if (remaining <= TimeSpan.Zero)
            {
                continue;
            }

            try
            {
                await _delay(remaining, cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                return;
            }
        }
    }
}
