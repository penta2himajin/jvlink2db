# jvlink2db

## 概要

jvlink2db is a CLI tool that imports JRA-VAN Data Lab. racing data from
the JV-Link COM component into PostgreSQL using bulk `COPY`. Written in
C# (.NET 8), Windows-only, MIT-licensed. The `docs/` directory is the
source of truth for design and specification; this file only covers the
workflow contracts that are not in `docs/`.

## プロジェクト構造

Details: [`docs/01-architecture.md`](./docs/01-architecture.md). Map:

```
src/
  Jvlink2Db.Core/         shared domain types and database-layer interfaces
  Jvlink2Db.Jvlink/       thin COM wrapper around JV-Link
  Jvlink2Db.Parser/       Shift-JIS bytes -> neutral record DTOs
  Jvlink2Db.Db.Postgres/  Npgsql persistence (PostgreSQL-specific)
  Jvlink2Db.Pipeline/     acquisition orchestration (setup/range/normal/weekly)
  Jvlink2Db.Cli/          command-line entry point
tests/                    one test project per src/ project
schema/                   hand-maintained DDL, one file per record type
docs/                     design specifications
```

## ビルド・テスト

```bash
dotnet restore
dotnet build -c Release -r win-x86 --self-contained false
dotnet test
dotnet run --project src/Jvlink2Db.Cli -- --help
```

`-r win-x86` is required: JV-Link is a 32-bit COM component and cannot
be loaded into a 64-bit process. A 64-bit build will pass `dotnet build`
but fail at runtime.

## 開発原則: TDD (Red→Green→Refactor)

All implementation work proceeds in this cycle:

1. **Red**: write a failing test that captures the intended behaviour;
   confirm it fails for the right reason with `dotnet test`.
2. **Green**: write the minimum code that makes the test pass.
3. **Refactor**: tidy up while keeping tests green.

No implementation commit without an accompanying test. Follow the phase
order in [`docs/08-roadmap.md`](./docs/08-roadmap.md); do not start a
phase before the previous one's done-criteria are met. In particular,
do not start Phase 2 work before Phase 1 (Probe) has been verified on
real hardware.

## アーキテクチャ境界

- **Parser is database-neutral.** `Jvlink2Db.Parser` must not reference
  Npgsql or any other database client.
- **`Db.Postgres` is isolated.** Npgsql types do not leak outside
  `Jvlink2Db.Db.Postgres`. Other projects talk to it through interfaces
  declared in `Jvlink2Db.Core`.
- **JV-Data spec is canonical.** Field names, byte offsets, and types
  follow the JV-Data Specification 4.9.0.1; where ambiguous, the SDK's
  `JVData_Struct.cs` is the tiebreaker. Do not invent field semantics.
- **One dataspec per `JVOpen` call.** The JV-Link Interface Specification
  documents that combining multiple dataspecs slows `JVRead`. Sequential
  per-dataspec calls only.

## 禁止事項

1. **Do not delete or disable existing tests.** If a test fails, fix the
   production code, not the test. Skipping or commenting out tests is
   not acceptable.
2. **Do not commit JV-Link service keys or other credentials.**
   `appsettings.Local.json`, environment files, and anything containing
   a service key stay out of the repository. `.gitignore` already covers
   the standard cases; verify before each commit.
3. **Do not redistribute the JRA-VAN SDK.** `JVDTLab.dll`,
   `JVData_Struct.cs`, the SDK installer, and the bundled tools must
   not be committed. They are referenced as external dependencies that
   the user obtains from the JRA-VAN developer portal.
4. **Do not modify CI configuration without explicit instruction.**
   Files under `.github/workflows/` are not changed without the user
   asking for it.
5. **Do not bypass the phase order.** `docs/08-roadmap.md` defines the
   build sequence and done-criteria. Do not write Phase 2+ code before
   Phase 1 is verified, even if it looks ready.
6. **Do not introduce database backends other than PostgreSQL.** This is
   covered in `docs/00-overview.md`; speed on PostgreSQL is the priority.

## 文字コード

JV-Data record bytes are Shift-JIS (CP932). The SDK's structure files
are also CP932. When `Read`-ing or `Edit`-ing such files, ensure no
UTF-8 round-trip corrupts them. Parser fixture bytes committed under
`tests/fixtures/` are binary; treat them as such (no line-ending
normalisation, no encoding conversion).

## Git 態勢

- **Conventional Commits**: `feat:`, `fix:`, `docs:`, `refactor:`,
  `test:`, `ci:`, `chore:`.
- **Branch naming**: `claude/<topic>` for Claude-driven work.
- **Tests must pass and warnings must be zero before committing.**
- **Append a `Co-Authored-By` trailer** to commits Claude authors, for
  transparency:

  ```
  Co-Authored-By: Claude <noreply@anthropic.com>
  ```

- Documentation-only changes go in `docs:` commits; mixed code +
  documentation changes are split into separate commits where it does
  not break atomicity.

## 長文出力 (Stream idle timeout 対策)

Claude Code cloud sessions occasionally fail with `Stream idle timeout`
when producing long output. To reduce the risk:

1. **Stage long writes.** For long documents or source files, write the
   skeleton (headings, function signatures) first, then fill each
   section in follow-up edits. Avoid single blocks larger than ~200
   lines.
2. **Watch out after large reads.** Reading a big file (e.g.
   `JVData_Struct.cs`, ~130 KB) and then immediately producing long
   output is a common trigger. Split into separate turns or excerpt
   only the relevant portion.
3. **Recover carefully.** A timeout can still leave the file write
   completed. Run `git status` before retrying so the same content is
   not written twice.
