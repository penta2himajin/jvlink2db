using System;

namespace Jvlink2Db.Parser.Records;

/// <summary>
/// Thrown by a record decoder when the supplied buffer is shorter than
/// the layout it knows how to parse. Distinct from generic
/// <see cref="ArgumentException"/> so the pipeline can skip the record
/// (e.g. older spec format that we don't have a layout for) without
/// crashing the whole bulk load.
/// </summary>
public sealed class RecordTooShortException : ArgumentException
{
    public RecordTooShortException(string recordSpec, int actualLength, int requiredLength)
        : base(
            $"{recordSpec} record requires at least {requiredLength} bytes, got {actualLength}.",
            "buffer")
    {
        RecordSpec = recordSpec;
        ActualLength = actualLength;
        RequiredLength = requiredLength;
    }

    public string RecordSpec { get; }

    public int ActualLength { get; }

    public int RequiredLength { get; }
}
