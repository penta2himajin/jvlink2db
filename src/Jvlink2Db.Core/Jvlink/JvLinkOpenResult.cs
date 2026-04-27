namespace Jvlink2Db.Core.Jvlink;

public readonly record struct JvLinkOpenResult(
    int ReturnCode,
    int ReadCount,
    int DownloadCount,
    string LastFileTimestamp);
