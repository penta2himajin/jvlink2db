using System;

namespace Jvlink2Db.Core.Jvlink;

public sealed class JvLinkException : Exception
{
    public JvLinkException(int returnCode, string method, Exception? innerException = null)
        : base($"JV-Link {method} returned {returnCode}", innerException)
    {
        ReturnCode = returnCode;
        Method = method;
    }

    public int ReturnCode { get; }

    public string Method { get; }
}
