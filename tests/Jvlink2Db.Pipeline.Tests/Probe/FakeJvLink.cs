using System.Collections.Generic;
using Jvlink2Db.Core.Jvlink;

namespace Jvlink2Db.Pipeline.Tests.Probe;

internal sealed class FakeJvLink : IJvLink
{
    private readonly JvLinkOpenResult _openResult;
    private readonly Queue<int> _statuses;
    private readonly Queue<JvLinkReadResult> _reads;

    public bool Initialized { get; private set; }
    public bool Opened { get; private set; }
    public bool Closed { get; private set; }
    public string? LastSid { get; private set; }
    public string? LastDataspec { get; private set; }
    public string? LastFromtime { get; private set; }
    public int LastOption { get; private set; }

    public FakeJvLink(JvLinkOpenResult openResult, IEnumerable<int> statuses, IEnumerable<JvLinkReadResult> reads)
    {
        _openResult = openResult;
        _statuses = new Queue<int>(statuses);
        _reads = new Queue<JvLinkReadResult>(reads);
    }

    public int Init(string sid)
    {
        Initialized = true;
        LastSid = sid;
        return 0;
    }

    public JvLinkOpenResult Open(string dataspec, string fromtime, int option)
    {
        Opened = true;
        LastDataspec = dataspec;
        LastFromtime = fromtime;
        LastOption = option;
        return _openResult;
    }

    public int Status() => _statuses.Count > 0 ? _statuses.Dequeue() : _openResult.DownloadCount;

    public JvLinkReadResult Read() => _reads.Dequeue();

    public int Close()
    {
        Closed = true;
        return 0;
    }
}
