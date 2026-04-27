using System;

namespace Jvlink2Db.Pipeline.Tests.Setup;

internal static class TestBuffers
{
    /// <summary>
    /// Minimal valid 1272-byte RA buffer; only the record-spec and an
    /// overridable race_num are populated. The rest stays as ASCII
    /// spaces, which the decoder right-trims to empty strings.
    /// </summary>
    public static byte[] Ra(string raceNum = "01")
    {
        var buf = new byte[1272];
        Array.Fill(buf, (byte)0x20);
        buf[0] = (byte)'R';
        buf[1] = (byte)'A';

        // race_num lives at 1-based offset 26 (length 2)
        buf[25] = (byte)raceNum[0];
        buf[26] = (byte)raceNum[1];

        buf[1270] = 0x0D;
        buf[1271] = 0x0A;
        return buf;
    }

    /// <summary>
    /// Non-RA record with the given two-byte spec. SetupRunner skips
    /// these without decoding, so the buffer just needs the prefix.
    /// </summary>
    public static byte[] NonRa(string spec)
    {
        var buf = new byte[64];
        Array.Fill(buf, (byte)0x20);
        buf[0] = (byte)spec[0];
        buf[1] = (byte)spec[1];
        return buf;
    }
}
