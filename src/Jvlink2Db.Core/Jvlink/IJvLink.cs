namespace Jvlink2Db.Core.Jvlink;

public interface IJvLink
{
    int Init(string sid);

    JvLinkOpenResult Open(string dataspec, string fromtime, int option);

    int Status();

    JvLinkReadResult Read();

    JvLinkSkipResult Skip();

    int Close();
}
