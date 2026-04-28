using System;

namespace Jvlink2Db.Pipeline.Setup;

/// <summary>
/// Progress event emitted by <see cref="SetupRunner"/> at JV-Link
/// file boundaries and per-sink flush boundaries. Consumers (CLI,
/// log file writers, tests) format this however they want; the
/// runner stays format-neutral.
/// </summary>
public abstract record ProgressEvent;

public sealed record FileCompletedEvent(
    string Filename,
    int FileIndex,
    int TotalFiles,
    int RecordsInFile,
    int RecordsTotal,
    TimeSpan Elapsed) : ProgressEvent;

public sealed record FlushStartedEvent(
    string RecordSpec,
    int BufferedRecords) : ProgressEvent;

public sealed record FlushCompletedEvent(
    string RecordSpec,
    long Inserted,
    TimeSpan Elapsed) : ProgressEvent;
