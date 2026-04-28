namespace Jvlink2Db.Core.Jvlink;

public interface IJvLink
{
    int Init(string sid);

    JvLinkOpenResult Open(string dataspec, string fromtime, int option);

    int Status();

    JvLinkReadResult Read();

    /// <summary>
    /// Skips the rest of the current file. After this call the next
    /// <see cref="Read"/> returns the first record of the next file
    /// (or end-of-data). The COM method is parameterless and returns
    /// void; the caller learns "which file did we just skip" by
    /// inspecting the filename out param of subsequent <see cref="Read"/>
    /// results.
    /// </summary>
    void Skip();

    /// <summary>
    /// Deletes the named file from JV-Link's local cache. Used to
    /// recover from <c>JVRead</c> codes <c>-402</c> (empty file) and
    /// <c>-403</c> (corrupt file): delete, then re-open and skip past
    /// already-consumed files.
    /// </summary>
    int FileDelete(string filename);

    int Close();
}
