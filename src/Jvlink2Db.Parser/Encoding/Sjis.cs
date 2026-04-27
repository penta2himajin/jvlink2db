using System.Text;

namespace Jvlink2Db.Parser.Encoding;

/// <summary>
/// CP932 (Shift-JIS) helper. JV-Data record bytes are CP932; .NET 8
/// does not include CP932 in the default encoding set, so we register
/// <see cref="CodePagesEncodingProvider"/> here on first access.
/// </summary>
public static class Sjis
{
    private static readonly System.Text.Encoding s_encoding = CreateEncoding();

    public static System.Text.Encoding Encoding => s_encoding;

    private static System.Text.Encoding CreateEncoding()
    {
        System.Text.Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        return System.Text.Encoding.GetEncoding(932);
    }
}
