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
  Jvlink2Db.Core/            # Shared domain types: record kinds, error
                             # codes, acquisition state, configuration
                             # objects, and the small interfaces the
                             # pipeline uses to talk to the database
                             # layer.
  Jvlink2Db.Jvlink/          # Thin COM wrapper around JV-Link methods.
                             # No business logic; translates COM calls
                             # into idiomatic .NET (Result types, async
                             # waits).
  Jvlink2Db.Parser/          # Decoders that turn raw Shift-JIS record
                             # bytes into neutral record DTOs, one
                             # decoder per record type (RA, SE, HR, ...).
                             # Has no dependency on any database client.
  Jvlink2Db.Db.Postgres/     # PostgreSQL persistence: schema bootstrap,
                             # Npgsql binary COPY writers, upsert merge
                             # logic. The only project that knows about
                             # Npgsql.
  Jvlink2Db.Pipeline/        # Acquisition orchestration: setup / range /
                             # normal / weekly modes, retry, resume.
                             # Talks to the database layer only through
                             # interfaces defined in Core.
  Jvlink2Db.Cli/             # Command-line entry point.
tests/
  Jvlink2Db.Core.Tests/
  Jvlink2Db.Parser.Tests/
  Jvlink2Db.Db.Postgres.Tests/
  Jvlink2Db.Pipeline.Tests/
schema/                      # Hand-maintained DDL, one file per record
                             # type.
docs/                        # This directory.
```

Projects depend strictly downward in the order listed: `Cli` references
`Pipeline`, `Pipeline` references `Db.Postgres`, `Parser`, and `Jvlink`,
all of those reference `Core`. Tests reference whatever they exercise.

## Database backends

Initial scope is PostgreSQL only, isolated in `Jvlink2Db.Db.Postgres`.
Other RDBMS backends are not planned, but the project is structured so
that adding one is a matter of adding a sibling project
(`Jvlink2Db.Db.Mssql`, `Jvlink2Db.Db.Mysql`, ...) rather than rewriting
existing code. The boundary is enforced by:

- `Jvlink2Db.Parser` produces neutral record DTOs that have no
  dependency on Npgsql or any other database client.
- `Jvlink2Db.Pipeline` references the database layer only through
  interfaces declared in `Jvlink2Db.Core` (e.g. `IBulkWriter<TRecord>`,
  `ISchemaProvisioner`), never through Npgsql types directly.
- `Jvlink2Db.Cli` selects a backend implementation through dependency
  injection at startup; for now there is exactly one implementation to
  select.

A shared abstract bulk-write API beyond what is strictly required will
not be introduced before a real second implementation exists. Each
RDBMS has its own incompatible bulk-load mechanism, and a one-
implementation abstraction is unlikely to fit the second one without
rework. Speed on PostgreSQL takes priority over symmetry with backends
that do not yet exist.

## Key dependencies

- **Npgsql** for PostgreSQL access. Binary `COPY` is the primary write
  path. Pooling and async I/O are kept enabled. Used only inside
  `Jvlink2Db.Db.Postgres`.
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
