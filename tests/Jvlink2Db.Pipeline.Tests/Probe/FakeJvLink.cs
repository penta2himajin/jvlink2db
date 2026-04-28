using System.Collections.Generic;
using Jvlink2Db.Core.Jvlink;

namespace Jvlink2Db.Pipeline.Tests.Probe;

internal sealed class FakeJvLink : IJvLink
{
    private readonly Queue<JvLinkOpenResult> _openResults;
    private readonly Queue<int> _statuses;
    private readonly Queue<JvLinkReadResult> _reads;
    private readonly Queue<JvLinkSkipResult> _skips;

    public bool Initialized { get; private set; }
    public bool Opened { get; private set; }
    public bool Closed { get; private set; }
    public string? LastSid { get; private set; }
    public string? LastDataspec { get; private set; }
    public string? LastFromtime { get; private set; }
    public int LastOption { get; private set; }
    public int OpenCallCount { get; private set; }
    public int SkipCallCount { get; private set; }
    public int CloseCallCount { get; private set; }
    public List<string> DeletedFiles { get; } = new();

    public FakeJvLink(
        JvLinkOpenResult openResult,
        IEnumerable<int> statuses,
        IEnumerable<JvLinkReadResult> reads,
        IEnumerable<JvLinkSkipResult>? skips = null)
        : this(new[] { openResult }, statuses, reads, skips)
    {
    }

    public FakeJvLink(
        IEnumerable<JvLinkOpenResult> openResults,
        IEnumerable<int> statuses,
        IEnumerable<JvLinkReadResult> reads,
        IEnumerable<JvLinkSkipResult>? skips = null)
    {
        _openResults = new Queue<JvLinkOpenResult>(openResults);
        _statuses = new Queue<int>(statuses);
        _reads = new Queue<JvLinkReadResult>(reads);
        _skips = new Queue<JvLinkSkipResult>(skips ?? System.Array.Empty<JvLinkSkipResult>());
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
        OpenCallCount++;
        LastDataspec = dataspec;
        LastFromtime = fromtime;
        LastOption = option;
        return _openResults.Count > 0 ? _openResults.Dequeue() : new JvLinkOpenResult(0, 0, 0, "");
    }

    public int Status() => _statuses.Count > 0 ? _statuses.Dequeue() : 0;

    public JvLinkReadResult Read() => _reads.Dequeue();

    public JvLinkSkipResult Skip()
    {
        SkipCallCount++;
        return _skips.Count > 0 ? _skips.Dequeue() : new JvLinkSkipResult(-1, string.Empty);
    }

    public int Close()
    {
        Closed = true;
        CloseCallCount++;
        return 0;
    }

    public int FileDelete(string filename)
    {
        DeletedFiles.Add(filename);
        return 0;
    }
}
