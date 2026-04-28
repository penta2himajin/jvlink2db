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
    private dynamic? _dynamicInstance;

    public ComJvLink()
    {
        _comType = Type.GetTypeFromProgID(ProgId)
            ?? throw new InvalidOperationException(
                $"COM ProgID '{ProgId}' is not registered. Install JV-Link from the JRA-VAN developer portal.");
        _instance = Activator.CreateInstance(_comType)
            ?? throw new InvalidOperationException(
                $"Failed to create COM instance for '{ProgId}'.");
        // Dynamic alias for the hot path (Read). The DLR's COM binder caches
        // the bound delegate at the first invocation and reuses it for the
        // remaining ~1M+ records, avoiding the reflection-based dispatch that
        // Type.InvokeMember does on every call.
        _dynamicInstance = _instance;
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
        var jv = _dynamicInstance ?? throw new ObjectDisposedException(nameof(ComJvLink));

        // JV-Link writes into the byte array via SAFEARRAY-of-UI1; the
        // initial buffer can be a placeholder that the COM call replaces.
        // ref-typed object boxes participate in IDispatch byref VARIANT
        // marshalling for COM, which the DLR's ComBinder handles end-to-end.
        object bufferArg = new byte[1];
        object filenameArg = string.Empty;
        int rc = (int)jv.JVGets(ref bufferArg, DefaultBufferSize, ref filenameArg);

        var filename = filenameArg as string ?? string.Empty;
        if (rc > 0)
        {
            var buffer = bufferArg as byte[] ?? Array.Empty<byte>();
            return JvLinkReadResult.Record(buffer, filename);
        }

        return new JvLinkReadResult(rc, null, filename);
    }

    public void Skip()
    {
        // JVSkip is a parameterless, void method per the SDK type library.
        // The caller learns "what was just skipped" by reading the filename
        // out param of the next JVGets call.
        InvokeVoid("JVSkip");
    }

    public int FileDelete(string filename) => ToInt(Invoke("JVFiledelete", filename));

    public int Close() => ToInt(Invoke("JVClose"));

    public void Dispose()
    {
        if (_instance is not null)
        {
            _dynamicInstance = null;
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

    private void InvokeVoid(string method, params object?[] args)
    {
        var instance = _instance ?? throw new ObjectDisposedException(nameof(ComJvLink));
        _ = _comType.InvokeMember(method, BindingFlags.InvokeMethod, binder: null, instance, args);
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
