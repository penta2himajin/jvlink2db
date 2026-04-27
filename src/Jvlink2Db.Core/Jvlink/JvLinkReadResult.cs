namespace Jvlink2Db.Core.Jvlink;

public readonly record struct JvLinkReadResult(int ReturnCode, byte[]? Buffer, string? Filename)
{
    public static JvLinkReadResult Record(byte[] buffer, string filename) =>
        new(buffer.Length, buffer, filename);

    public static JvLinkReadResult EndOfFile(string filename) => new(-1, null, filename);

    public static JvLinkReadResult EndOfData { get; } = new(0, null, null);

    public static JvLinkReadResult StillDownloading { get; } = new(-3, null, null);

    public static JvLinkReadResult Error(int code) => new(code, null, null);
}
