using System;
using System.Reflection;
using System.Runtime.InteropServices;
using Jvlink2Db.Core.Jvlink;

namespace Jvlink2Db.Jvlink.Com;

/// <summary>
/// Late-bound JV-Link COM wrapper. The wrapper is intentionally thin —
/// it makes the calls and surfaces the return values without retry,
/// without error mapping, and without state validation. Higher layers
/// own those concerns.
/// </summary>
public sealed class ComJvLink : IJvLink, IDisposable
{
    public const string ProgId = "JVDTLab.JVLink";

    private const int DefaultBufferSize = 110_000;

    private readonly Type _comType;
    private object? _instance;

    public ComJvLink()
    {
        _comType = Type.GetTypeFromProgID(ProgId)
            ?? throw new InvalidOperationException(
                $"COM ProgID '{ProgId}' is not registered. Install JV-Link from the JRA-VAN developer portal.");
        _instance = Activator.CreateInstance(_comType)
            ?? throw new InvalidOperationException(
                $"Failed to create COM instance for '{ProgId}'.");
    }

    public int Init(string sid) => ToInt(Invoke("JVInit", sid));

    public JvLinkOpenResult Open(string dataspec, string fromtime, int option)
    {
        var args = new object?[] { dataspec, fromtime, option, 0, 0, string.Empty };
        var byRef = new ParameterModifier(args.Length);
        byRef[3] = true;
        byRef[4] = true;
        byRef[5] = true;

        var rc = ToInt(InvokeWithModifiers("JVOpen", args, byRef));

        return new JvLinkOpenResult(
            ReturnCode: rc,
            ReadCount: ToInt(args[3]),
            DownloadCount: ToInt(args[4]),
            LastFileTimestamp: args[5] as string ?? string.Empty);
    }

    public int Status() => ToInt(Invoke("JVStatus"));

    public JvLinkReadResult Read()
    {
        // JV-Link writes into the byte array via SAFEARRAY-of-UI1; the
        // initial contents do not matter, only that we hand it a byte[].
        var args = new object?[] { new byte[1], DefaultBufferSize, string.Empty };
        var byRef = new ParameterModifier(args.Length);
        byRef[0] = true;
        byRef[2] = true;

        var rc = ToInt(InvokeWithModifiers("JVGets", args, byRef));
        var filename = args[2] as string ?? string.Empty;

        if (rc > 0)
        {
            var buffer = args[0] as byte[] ?? Array.Empty<byte>();
            return JvLinkReadResult.Record(buffer, filename);
        }

        return new JvLinkReadResult(rc, null, filename);
    }

    public JvLinkSkipResult Skip()
    {
        var args = new object?[] { string.Empty };
        var byRef = new ParameterModifier(args.Length);
        byRef[0] = true;

        var rc = ToInt(InvokeWithModifiers("JVSkip", args, byRef));
        var filename = args[0] as string ?? string.Empty;
        return new JvLinkSkipResult(rc, filename);
    }

    public int Close() => ToInt(Invoke("JVClose"));

    public void Dispose()
    {
        if (_instance is not null)
        {
            Marshal.FinalReleaseComObject(_instance);
            _instance = null;
        }
    }

    private object Invoke(string method, params object?[] args)
    {
        var instance = _instance ?? throw new ObjectDisposedException(nameof(ComJvLink));
        return _comType.InvokeMember(method, BindingFlags.InvokeMethod, binder: null, instance, args)
            ?? throw new InvalidOperationException($"JV-Link {method} returned null.");
    }

    private object InvokeWithModifiers(string method, object?[] args, ParameterModifier modifiers)
    {
        var instance = _instance ?? throw new ObjectDisposedException(nameof(ComJvLink));
        return _comType.InvokeMember(
            method,
            BindingFlags.InvokeMethod,
            binder: null,
            instance,
            args,
            modifiers: new[] { modifiers },
            culture: null,
            namedParameters: null)
            ?? throw new InvalidOperationException($"JV-Link {method} returned null.");
    }

    private static int ToInt(object? value) => value switch
    {
        int i => i,
        long l => (int)l,
        short s => s,
        _ => Convert.ToInt32(value),
    };
}
