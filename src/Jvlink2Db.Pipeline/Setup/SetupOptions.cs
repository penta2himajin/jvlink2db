namespace Jvlink2Db.Pipeline.Setup;

public sealed record SetupOptions(
    string Sid,
    string Dataspec,
    string Fromtime,
    int Option);
