using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Jvlink2Db.Core.Jvlink;
using Jvlink2Db.Pipeline.Retry;
using Xunit;

namespace Jvlink2Db.Pipeline.Tests.Retry;

public class JvLinkRetryPolicyTests
{
    [Fact]
    public async Task ExecuteOpenAsync_returns_immediately_on_success()
    {
        var policy = new JvLinkRetryPolicy(JvLinkRetryPolicy.DefaultSchedule, recorder: NoopDelay);
        var calls = 0;

        var result = await policy.ExecuteOpenAsync(
            () => { calls++; return new JvLinkOpenResult(0, 5, 0, "ts"); },
            CancellationToken.None);

        Assert.Equal(0, result.ReturnCode);
        Assert.Equal(1, calls);
    }

    [Fact]
    public async Task ExecuteOpenAsync_returns_immediately_on_no_data()
    {
        var policy = new JvLinkRetryPolicy(JvLinkRetryPolicy.DefaultSchedule, recorder: NoopDelay);
        var calls = 0;

        var result = await policy.ExecuteOpenAsync(
            () => { calls++; return new JvLinkOpenResult(-1, 0, 0, ""); },
            CancellationToken.None);

        Assert.Equal(-1, result.ReturnCode);
        Assert.Equal(1, calls);
    }

    [Fact]
    public async Task ExecuteOpenAsync_returns_immediately_on_non_retryable_error()
    {
        var policy = new JvLinkRetryPolicy(JvLinkRetryPolicy.DefaultSchedule, recorder: NoopDelay);
        var calls = 0;

        var result = await policy.ExecuteOpenAsync(
            () => { calls++; return new JvLinkOpenResult(-116, 0, 0, ""); },
            CancellationToken.None);

        Assert.Equal(-116, result.ReturnCode);
        Assert.Equal(1, calls);  // -116 (dataspec/option mismatch) is fatal, not retried
    }

    [Theory]
    [InlineData(-301)]
    [InlineData(-411)]
    [InlineData(-431)]
    [InlineData(-504)]
    public async Task ExecuteOpenAsync_retries_transient_codes_and_returns_eventual_success(int transientCode)
    {
        var schedule = new TestSchedule
        {
            { transientCode, new[] { TimeSpan.FromMilliseconds(1), TimeSpan.FromMilliseconds(2), TimeSpan.FromMilliseconds(3) } },
        };
        var delays = new List<TimeSpan>();
        var policy = new JvLinkRetryPolicy(schedule, recorder: (d, _) => { delays.Add(d); return Task.CompletedTask; });

        var sequence = new Queue<JvLinkOpenResult>(new[]
        {
            new JvLinkOpenResult(transientCode, 0, 0, ""),
            new JvLinkOpenResult(transientCode, 0, 0, ""),
            new JvLinkOpenResult(0, 5, 0, "ts"),
        });

        var result = await policy.ExecuteOpenAsync(() => sequence.Dequeue(), CancellationToken.None);

        Assert.Equal(0, result.ReturnCode);
        Assert.Equal(new[] { TimeSpan.FromMilliseconds(1), TimeSpan.FromMilliseconds(2) }, delays);
    }

    [Fact]
    public async Task ExecuteOpenAsync_returns_last_failure_when_all_retries_exhausted()
    {
        var schedule = new TestSchedule
        {
            { -301, new[] { TimeSpan.FromMilliseconds(1), TimeSpan.FromMilliseconds(2) } },
        };
        var policy = new JvLinkRetryPolicy(schedule, recorder: NoopDelay);
        var calls = 0;

        var result = await policy.ExecuteOpenAsync(
            () => { calls++; return new JvLinkOpenResult(-301, 0, 0, ""); },
            CancellationToken.None);

        Assert.Equal(-301, result.ReturnCode);
        Assert.Equal(3, calls);  // initial + 2 retries from the schedule
    }

    [Fact]
    public async Task ExecuteOpenAsync_propagates_cancellation()
    {
        var schedule = new TestSchedule
        {
            { -301, new[] { TimeSpan.FromHours(1) } },
        };
        var policy = new JvLinkRetryPolicy(
            schedule,
            recorder: (d, ct) => Task.Delay(d, ct));

        using var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromMilliseconds(50));

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            policy.ExecuteOpenAsync(
                () => new JvLinkOpenResult(-301, 0, 0, ""),
                cts.Token));
    }

    [Fact]
    public void DefaultSchedule_includes_all_documented_transient_codes()
    {
        Assert.NotNull(JvLinkRetryPolicy.DefaultSchedule.For(-301));
        Assert.NotNull(JvLinkRetryPolicy.DefaultSchedule.For(-411));
        Assert.NotNull(JvLinkRetryPolicy.DefaultSchedule.For(-420));
        Assert.NotNull(JvLinkRetryPolicy.DefaultSchedule.For(-431));
        Assert.NotNull(JvLinkRetryPolicy.DefaultSchedule.For(-504));

        // Fatal codes have no schedule
        Assert.Null(JvLinkRetryPolicy.DefaultSchedule.For(-116));
        Assert.Null(JvLinkRetryPolicy.DefaultSchedule.For(-302));
        Assert.Null(JvLinkRetryPolicy.DefaultSchedule.For(0));
        Assert.Null(JvLinkRetryPolicy.DefaultSchedule.For(-1));
    }

    private static Task NoopDelay(TimeSpan _, CancellationToken __) => Task.CompletedTask;

    private sealed class TestSchedule : IRetrySchedule, IEnumerable<KeyValuePair<int, IReadOnlyList<TimeSpan>>>
    {
        private readonly Dictionary<int, IReadOnlyList<TimeSpan>> _map = new();

        public void Add(int code, IReadOnlyList<TimeSpan> delays) => _map[code] = delays;

        public IReadOnlyList<TimeSpan>? For(int returnCode) =>
            _map.TryGetValue(returnCode, out var v) ? v : null;

        public System.Collections.Generic.IEnumerator<KeyValuePair<int, IReadOnlyList<TimeSpan>>> GetEnumerator()
            => _map.GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            => _map.GetEnumerator();
    }
}
