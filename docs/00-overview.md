# Overview

## Purpose

jvlink2db imports JRA central horse-racing data from the JV-Link COM
component into a PostgreSQL database. It is the data-acquisition layer
that sits between JRA-VAN Data Lab. and any downstream system that wants
to query racing data with SQL.

The goal is a small, transparent tool: read what JV-Link returns, decode
it according to the published JV-Data specification, and store it in
tables that mirror that specification one-to-one. Nothing more.

## Goals

- Faithful reproduction of the JV-Data record layout in PostgreSQL.
  A reader who has the JV-Data specification should be able to write SQL
  against the database without further documentation.
- Practical performance for full-history loads. The expected ceiling is
  set by JV-Link itself, not by the importer.
- Resumable acquisition. Interrupted runs can be continued without
  re-fetching already-loaded data.
- Idempotent loads. Re-running a load over the same window must not
  produce duplicate rows or corrupt existing rows.
- A small, readable codebase. The whole project should be possible to
  audit by reading it.

## Non-goals

- No GUI. Configuration is via command-line flags and a configuration
  file; runtime control is via the CLI.
- No analytics, no prediction, no visualisation. Downstream systems can
  do whatever they like with the loaded data; that work belongs in those
  systems, not here.
- **Initially, PostgreSQL only.** The internal layout is structured so
  that additional RDBMS backends could be added later, but no other
  backend is in scope until there is concrete demand. Each RDBMS has its
  own incompatible bulk-load mechanism (`COPY FROM STDIN BINARY` for
  PostgreSQL, `SqlBulkCopy` for SQL Server, `LOAD DATA LOCAL INFILE` for
  MySQL, the Appender API for DuckDB, and so on), so adding one will
  mean a separate persistence implementation rather than a single shared
  one.
- No NAR (chihou) racing support. The UmaConn data source is a separate
  product and is not addressed by this project.
- No real-time betting integration.
- No bundling or redistribution of the JRA-VAN SDK.

## Audience

Developers and researchers who hold a JRA-VAN Data Lab. subscription and
want the underlying data in PostgreSQL for their own use. The project
assumes the user can read the JV-Data specification and write their own
SQL.

## Design principles

- **The JV-Data specification is the source of truth.** Field names,
  types, lengths, and primary-key choices follow the specification. Where
  the specification is ambiguous, the bundled `JVData_Struct.cs` from the
  SDK is treated as canonical.
- **One record type, one table.** No flattening, no joining at load time,
  no derived columns. Downstream views are the user's responsibility.
- **Bulk over single-row.** All inserts go through PostgreSQL's binary
  `COPY` protocol via Npgsql. Per-row `INSERT` is reserved for tiny
  bookkeeping tables (e.g. acquisition state).
- **Speed first for the supported backend.** Where a choice exists
  between portability and the fastest path on PostgreSQL, the fastest
  path wins. Portability across RDBMS backends is not promised.
- **Small surface area.** A few well-tested entry points are preferred to
  many configurable knobs.
- **No silent recovery from unrecognised records.** Unknown record types
  are logged and skipped, never coerced into the wrong table.

## Status

Initial design. The repository currently contains specifications only.
Implementation has not yet started.
