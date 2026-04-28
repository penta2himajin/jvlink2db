using System;
using System.Threading;
using System.Threading.Tasks;
using Jvlink2Db.Pipeline.Watch;
using Xunit;

namespace Jvlink2Db.Pipeline.Tests.Watch;

public class WatchRunnerTests
{
    [Fact]
    public async Task RunAsync_invokes_runOnce_repeatedly_until_cancelled()
    {
        var calls = 0;
        using var cts = new CancellationTokenSource();
        var runner = new WatchRunner(delay: (_, _) => Task.CompletedTask);

        await runner.RunAsync(
            runOnce: async _ =>
            {
                calls++;
                if (calls >= 3)
                {
                    cts.Cancel();
                }
                await Task.Yield();
            },
            interval: TimeSpan.FromMilliseconds(1),
            cancellationToken: cts.Token);

        Assert.Equal(3, calls);
    }

    [Fact]
    public async Task RunAsync_swallows_cycle_errors_and_continues()
    {
        var calls = 0;
        var errors = 0;
        using var cts = new CancellationTokenSource();
        var runner = new WatchRunner(
            delay: (_, _) => Task.CompletedTask,
            onError: (_, __) => errors++);

        await runner.RunAsync(
            runOnce: async _ =>
            {
                calls++;
                if (calls < 3)
                {
                    throw new InvalidOperationException("simulated transient");
                }
                cts.Cancel();
                await Task.Yield();
            },
            interval: TimeSpan.FromMilliseconds(1),
            cancellationToken: cts.Token);

        Assert.Equal(3, calls);
        Assert.Equal(2, errors);
    }

    [Fact]
    public async Task RunAsync_propagates_OperationCanceledException_from_runOnce()
    {
        using var cts = new CancellationTokenSource();
        var runner = new WatchRunner(delay: (_, _) => Task.CompletedTask);

        var task = runner.RunAsync(
            runOnce: ct => throw new OperationCanceledException(ct),
            interval: TimeSpan.FromMilliseconds(1),
            cancellationToken: cts.Token);

        cts.Cancel();
        await task;  // should complete cleanly, not throw
    }

    [Fact]
    public async Task RunAsync_sleeps_remaining_interval_after_each_cycle()
    {
        var delays = new System.Collections.Generic.List<TimeSpan>();
        var calls = 0;
        using var cts = new CancellationTokenSource();
        var runner = new WatchRunner(delay: (d, _) =>
        {
            delays.Add(d);
            return Task.CompletedTask;
        });

        await runner.RunAsync(
            runOnce: async _ =>
            {
                calls++;
                if (calls >= 3)
                {
                    cts.Cancel();
                }
                await Task.Yield();
            },
            interval: TimeSpan.FromSeconds(60),
            cancellationToken: cts.Token);

        Assert.Equal(3, calls);
        // Delay only between non-final cycles — not after the cycle that triggers cancellation.
        Assert.Equal(2, delays.Count);
        Assert.All(delays, d => Assert.True(d > TimeSpan.Zero && d <= TimeSpan.FromSeconds(60)));
    }
}
