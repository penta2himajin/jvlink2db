namespace Jvlink2Db.Pipeline.Probe;

public sealed record ProbeOptions(
    string Sid,
    string Dataspec,
    string Fromtime,
    int Option,
    int MaxSamples = 10);
