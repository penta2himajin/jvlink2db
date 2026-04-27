# Architecture

## Language and runtime

- C# on .NET 8.
- Built as `win-x86` (32-bit). JV-Link is a 32-bit COM component, and
  the simplest way to interact with it is to host the importer in a
  32-bit process. Running 64-bit and going through a `DllSurrogate` is
  technically possible but adds operational complexity that this project
  does not need.
- Windows 10 / 11 only at runtime. The build itself is portable.

## Repository layout

```
src/
  Jvlink2Db.Core/        # Shared domain types: record kinds, error codes,
                         # acquisition state, configuration objects.
  Jvlink2Db.Jvlink/      # Thin COM wrapper around JV-Link methods.
                         # No business logic; translates COM calls into
                         # idiomatic .NET (Result types, async waits).
  Jvlink2Db.Parser/      # Decoders that turn raw Shift-JIS record bytes
                         # into strongly-typed records, one decoder per
                         # record type (RA, SE, HR, ...).
  Jvlink2Db.Db/          # Npgsql-based persistence: schema bootstrap,
                         # binary COPY writers, upsert merge logic.
  Jvlink2Db.Pipeline/    # Acquisition orchestration: setup / range /
                         # normal / weekly modes, retry, resume.
  Jvlink2Db.Cli/         # Command-line entry point.
tests/
  Jvlink2Db.Core.Tests/
  Jvlink2Db.Parser.Tests/
  Jvlink2Db.Db.Tests/
  Jvlink2Db.Pipeline.Tests/
schema/                  # Hand-maintained DDL, one file per record type.
docs/                    # This directory.
```

Projects depend strictly downward in the order listed: `Cli` references
`Pipeline`, `Pipeline` references `Db`, `Parser`, and `Jvlink`, all of
those reference `Core`. Tests reference whatever they exercise.

## Key dependencies

- **Npgsql** for PostgreSQL access. Binary `COPY` is the primary write
  path. Pooling and async I/O are kept enabled.
- **System.CommandLine** for the CLI surface.
- **Microsoft.Extensions.Logging** + **Serilog** for structured logging
  to console and rolling file.
- **Microsoft.Extensions.Configuration** for layered configuration.
- **xUnit** + **Testcontainers for .NET** for integration tests against
  a disposable PostgreSQL instance.

Third-party dependencies are kept deliberately small. No ORM. No
mediators. No source generators beyond what comes with the BCL and
Npgsql.

## Process model

A single CLI invocation runs in a single process. Concurrency inside the
process is limited because:

- JV-Link is single-threaded; concurrent calls to `JVOpen`/`JVRead` from
  the same process are not supported by the API.
- The JV-Link Interface Specification notes that combining many
  `dataspec` IDs in one `JVOpen` call slows down `JVRead`. The pipeline
  therefore opens one `dataspec` at a time, sequentially.

Asynchronous work is used only where it does not contradict the JV-Link
threading model: PostgreSQL writes can run on background tasks while the
next batch of records is being decoded.

## Configuration

Layered, in order of increasing priority:

1. Built-in defaults.
2. `appsettings.json` next to the executable.
3. `appsettings.Local.json` (gitignored, for personal overrides).
4. Environment variables prefixed `JVLINK2DB_`.
5. Command-line flags.

The full schema is documented inline as part of the configuration class
in `Jvlink2Db.Core`; the key sections are JV-Link service identity,
PostgreSQL connection, and acquisition defaults (mode, dataspecs,
fromtime).
