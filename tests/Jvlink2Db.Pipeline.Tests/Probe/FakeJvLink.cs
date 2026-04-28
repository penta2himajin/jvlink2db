using System.Collections.Generic;
using Jvlink2Db.Core.Jvlink;

namespace Jvlink2Db.Pipeline.Tests.Probe;

internal sealed class FakeJvLink : IJvLink
{
    private readonly Queue<JvLinkOpenResult> _openResults;
    private readonly Queue<int> _statuses;
    private readonly Queue<JvLinkReadResult> _reads;

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
        IEnumerable<JvLinkReadResult> reads)
        : this(new[] { openResult }, statuses, reads)
    {
    }

    public FakeJvLink(
        IEnumerable<JvLinkOpenResult> openResults,
        IEnumerable<int> statuses,
        IEnumerable<JvLinkReadResult> reads)
    {
        _openResults = new Queue<JvLinkOpenResult>(openResults);
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
        OpenCallCount++;
        LastDataspec = dataspec;
        LastFromtime = fromtime;
        LastOption = option;
        return _openResults.Count > 0 ? _openResults.Dequeue() : new JvLinkOpenResult(0, 0, 0, "");
    }

    public int Status() => _statuses.Count > 0 ? _statuses.Dequeue() : 0;

    public JvLinkReadResult Read() => _reads.Dequeue();

    public void Skip() => SkipCallCount++;

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
