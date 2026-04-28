using System;
using System.Collections.Generic;

namespace Jvlink2Db.Pipeline.Retry;

/// <summary>
/// Default retry schedule for <c>JVOpen</c> transient codes, per
/// <c>docs/06-error-handling.md</c>:
/// <list type="bullet">
/// <item><c>-301</c> auth transient: 3 retries at 1 s, 5 s, 30 s.</item>
/// <item><c>-411</c>..<c>-431</c> HTTP errors: 3 retries at 5 s, 30 s, 5 min.</item>
/// <item><c>-504</c> server maintenance: 4 retries at 1 min, 5 min, 15 min, 60 min.</item>
/// </list>
/// </summary>
public sealed class JvLinkRetrySchedule : IRetrySchedule
{
    private static readonly IReadOnlyList<TimeSpan> AuthTransient =
        [TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(30)];

    private static readonly IReadOnlyList<TimeSpan> HttpError =
        [TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(30), TimeSpan.FromMinutes(5)];

    private static readonly IReadOnlyList<TimeSpan> ServerMaintenance =
        [TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(15), TimeSpan.FromMinutes(60)];

    public IReadOnlyList<TimeSpan>? For(int returnCode) => returnCode switch
    {
        -301 => AuthTransient,
        >= -431 and <= -411 => HttpError,
        -504 => ServerMaintenance,
        _ => null,
    };
}
