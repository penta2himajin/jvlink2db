namespace Jvlink2Db.Core.Jvlink;

/// <summary>
/// Result of <see cref="IJvLink.Skip"/>. <c>ReturnCode</c> is 0 on
/// success (a file was skipped); the JV-Link spec defines negative
/// codes for end-of-data and errors. <c>Filename</c> is JV-Link's
/// out parameter — the filename associated with the skip operation.
/// </summary>
public readonly record struct JvLinkSkipResult(int ReturnCode, string Filename);
