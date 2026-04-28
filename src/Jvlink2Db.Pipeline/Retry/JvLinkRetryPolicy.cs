using System;
using System.Threading;
using System.Threading.Tasks;
using Jvlink2Db.Core.Jvlink;

namespace Jvlink2Db.Pipeline.Retry;

/// <summary>
/// Retries <c>JVOpen</c> calls that return one of the documented
/// transient codes, sleeping between attempts according to the
/// configured <see cref="IRetrySchedule"/>. The retry budget is
/// bounded — once the schedule's delays are exhausted the last
/// failure is returned, so the caller can decide what to do with it.
/// </summary>
public sealed class JvLinkRetryPolicy
{
    public static IRetrySchedule DefaultSchedule { get; } = new JvLinkRetrySchedule();

    private readonly IRetrySchedule _schedule;
    private readonly Func<TimeSpan, CancellationToken, Task> _recorder;

    public JvLinkRetryPolicy(
        IRetrySchedule? schedule = null,
        Func<TimeSpan, CancellationToken, Task>? recorder = null)
    {
        _schedule = schedule ?? DefaultSchedule;
        _recorder = recorder ?? ((delay, ct) => Task.Delay(delay, ct));
    }

    public async Task<JvLinkOpenResult> ExecuteOpenAsync(
        Func<JvLinkOpenResult> open,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(open);

        var result = open();
        var delays = _schedule.For(result.ReturnCode);
        if (delays is null)
        {
            return result;
        }

        for (var i = 0; i < delays.Count; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await _recorder(delays[i], cancellationToken).ConfigureAwait(false);

            result = open();

            var nextDelays = _schedule.For(result.ReturnCode);
            if (nextDelays is null)
            {
                return result;
            }

            // If the same (or another retryable) code recurs, keep walking
            // through the schedule we started with — the documented budgets
            // are per-call, not per-code-class.
            delays = nextDelays;
        }

        return result;
    }
}
