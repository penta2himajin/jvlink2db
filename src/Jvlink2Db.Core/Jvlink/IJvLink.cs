namespace Jvlink2Db.Core.Jvlink;

public interface IJvLink
{
    int Init(string sid);

    JvLinkOpenResult Open(string dataspec, string fromtime, int option);

    int Status();

    JvLinkReadResult Read();

    JvLinkSkipResult Skip();

    /// <summary>
    /// Deletes the named file from JV-Link's local cache. Used to
    /// recover from <c>JVRead</c> codes <c>-402</c> (empty file) and
    /// <c>-403</c> (corrupt file): delete, then re-open and skip past
    /// already-consumed files.
    /// </summary>
    int FileDelete(string filename);

    int Close();
}
