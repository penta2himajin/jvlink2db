using System;
using System.Collections.Generic;

namespace Jvlink2Db.Pipeline.Retry;

/// <summary>
/// Maps a JV-Link return code to the sequence of delays the retry
/// policy should sleep between attempts. Returning <c>null</c> means
/// the code is not retryable (fatal or success).
/// </summary>
public interface IRetrySchedule
{
    IReadOnlyList<TimeSpan>? For(int returnCode);
}
