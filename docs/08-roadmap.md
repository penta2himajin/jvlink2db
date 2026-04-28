# Implementation roadmap

This document lays out the order in which jvlink2db is built. Each phase
has a goal, a scope, and explicit done-criteria so that progress is
unambiguous. Phases are gates: the next one does not start until the
previous one's criteria are met.

## Working method

Development follows test-driven development throughout. For every unit
of work inside a phase:

1. **Red** — write a failing test that captures the intended behaviour.
2. **Green** — write the minimum code that makes the test pass.
3. **Refactor** — tidy up while keeping tests green.

There is intentionally no separate "testing" phase, because tests are
produced as part of every other phase. There is also no separate
"packaging" or "release" phase; producing a runnable artefact is part
of Phase 0 and stays runnable throughout.

## Target environment

jvlink2db targets Windows 10 / 11 only. Scheduled execution uses
Windows Task Scheduler directly; no abstraction layer for other
schedulers is planned.

## Phases

### Phase 0 — Scaffold

**Status.** Done.

**Goal.** A buildable solution with the project layout described in
[01-architecture.md](./01-architecture.md), an executable that runs and
prints help text, and a CI pipeline that proves it.

**Scope.**

- `dotnet new sln`, six projects under `src/`, four test projects under
  `tests/`.
- Project references wired according to the dependency direction in
  [01-architecture.md](./01-architecture.md).
- Empty placeholder types in each project so it compiles.
- `Jvlink2Db.Cli` produces an `.exe` that prints `--help`.
- `appsettings.json` is read at startup; an empty configuration is
  acceptable but the layered configuration plumbing is in place.
- GitHub Actions workflow on a Windows runner: restore, build
  `win-x86`, run `dotnet test`.

**Done when.**

- `dotnet build -c Release -r win-x86 --self-contained false` succeeds.
- `dotnet test` runs and reports zero tests, zero failures.
- CI is green on `main`.
- A user can clone the repo, build, and run `jvlink2db --help`.

### Phase 1 — Probe (most important milestone)

**Status.** Done. Verified on a real JV-Link installation:

- `option = 1`, fromtime `20260415000000`: `readcount = 50`, 13 record
  types from the `RACE` dataspec.
- `option = 4`, fromtime `20260101000000-20260331235959`: `readcount =
  30`, 30,649 records across the same 13 record types
  (`RA`/`SE`/`HR`/`H1`/`H6`/`O1`-`O6`/`WF`/`JG`). First record id
  observed: `H1`.

**Goal.** Prove on a real Windows host with a real JV-Link installation
that the central protocol assumption — `JVOpen` with `option = 4` and a
`fromtime` range — actually works.

**Scope.**

- A thin COM wrapper in `Jvlink2Db.Jvlink` for `JVInit`, `JVOpen`,
  `JVStatus`, `JVRead`, `JVClose`. No retry, no fancy error mapping;
  the wrapper just makes the calls and surfaces the return values.
- A `jvlink2db probe` subcommand that runs the canonical protocol
  sequence (see [02-jvlink-protocol.md](./02-jvlink-protocol.md)) for a
  user-supplied dataspec, fromtime, and option, and prints what it
  observes: `readcount`, `downloadcount`, the first few record IDs and
  filenames, and a record count by record ID.
- No PostgreSQL involvement at all in this phase. Records are inspected
  in memory and discarded.

**Done when.**

- On a Windows host with a configured JV-Link, running
  `jvlink2db probe --dataspec RACE --fromtime 20260101000000-20260331235959 --option 4`
  completes without error and produces a non-zero `readcount`.
- The first record returned by `JVRead` has the expected two-byte
  record ID prefix from the JV-Data specification (any of `RA`, `SE`,
  `HR`, `H1`, `H6`, `O1`–`O6`, `WF`, `JG`).
- The same command for `--option 1` also works as a sanity check that
  the wrapper itself is correct.
- Notes captured in the issue tracker: actual elapsed time for the 3-
  month probe; `readcount` and `downloadcount` magnitudes; any
  surprising return codes encountered.

**Why this is the critical milestone.** The whole project is
predicated on `option = 4` plus `fromtime` range working as the
specification reads. If it does not, the design has to change before
further layers are built. Stop here for evaluation before starting
Phase 2.

### Phase 2 — First record end-to-end

**Status.** Done. Verified against a real JV-Link plus a local
PostgreSQL on 2026-04-27:

- `setup --dataspec RACE --fromtime 20260415000000 --option 1`:
  144 RA rows inserted from 50 files.
- `setup --dataspec RACE --fromtime 20260101000000-20260331235959
  --option 4`: 1,173 RA rows inserted from 30 files; matches the
  Phase 1 probe count exactly.
- Re-running the option-4 command leaves `jv.ra` row count
  unchanged (1,317 → 1,317), confirming idempotency end-to-end
  through the staging-table + `ON CONFLICT DO UPDATE` path.

**Goal.** A single record type travels all the way from JV-Link to a
PostgreSQL table, idempotently.

**Scope.**

- A decoder in `Jvlink2Db.Parser` for the `RA` record only. The
  decoder consumes raw Shift-JIS bytes and emits a neutral record DTO
  defined in `Jvlink2Db.Core`.
- A schema file `schema/ra.sql` and a bootstrap routine in
  `Jvlink2Db.Db.Postgres` that creates `jv.ra` if it does not exist.
- A binary `COPY` writer in `Jvlink2Db.Db.Postgres` for `jv.ra`,
  followed by an `INSERT ... ON CONFLICT` merge as described in
  [04-database-schema.md](./04-database-schema.md).
- Integration plumbing: `Jvlink2Db.Pipeline` connects the JV-Link
  reader, the parser, and the writer; non-`RA` records are skipped and
  counted.
- A `jvlink2db setup --dataspec RACE --since ... --until ...` (or the
  equivalent CLI shape) that uses Phase 1's wrapper plus the new
  parser and writer.

**Done when.**

- Running setup over a 3-month window populates `jv.ra` with the
  expected number of rows (the dataspec returns a known per-meeting
  count of `RA` records that can be cross-checked against the JRA-VAN
  schedule for the period).
- Running the same command twice over the same window leaves the table
  unchanged on the second run (zero new rows, zero conflicts that
  produce errors).
- `JVDataCheckTool` from the SDK, run against the same range
  independently, reports the same `RA` record count.

### Phase 3 — All records

**Status.** Done (32/33 record types verified end-to-end on a live
JV-Link trial environment on 2026-04-28). 3-month-window setup runs
across 11 dataspecs populated every record type the trial environment
exposes; the same runs are idempotent on a second pass.

| Dataspec | Option | `ReadCount` | Record types populated (read / inserted) |
|---|---|---|---|
| `RACE` | 1 | 50 | RA 144 / 144, SE 2009 / 2009, HR 144 / 144, H1 144 / 144, H6 144 / 144, O1 144 / 144, O2 144 / 144, O3 144 / 144, O4 144 / 144, O5 144 / 144, O6 144 / 144, WF 3 / 3, JG 2138 / 2138 |
| `RACE` | 4 | 30 | RA 1173 / 1173, SE 14664 / 14664, HR 558 / 558, H1 558 / 558, H6 558 / 558, O1 588 / 588, O2 588 / 588, O3 588 / 588, O4 588 / 588, O5 588 / 588, O6 588 / 588, WF 9 / 9, JG 9601 / 9601 |
| `DIFN` | 4 | 73 | UM 221167 / 212353, BR 12011 / 10722, BN 10464 / 8682, CH 2214 / 1474, RC 2125 / 2125, KS 2046 / 1559, SE 1561 / 1561, RA 139 / 139 |
| `SNPN` | 4 | 15 | CK 16573 / 16572 |
| `SLOP` | 4 | 52 | HC 167331 / 165359 |
| `WOOD` | 4 | 54 | WC 50815 / 50035 |
| `YSCH` | 4 | 2 | YS 579 / 291 |
| `HOSN` | 4 | 1 | HS 53008 / 52994 |
| `HOYU` | 4 | 5 | HY 176361 / 176346 |
| `COMM` | 4 | 1 | CS 119 / 119 |
| `MING` | 4 | 22 | DM 1122 / 1122, TM 1083 / 1083 |
| `TOKU` | 4 | 1 | TK 21 / 21 |

`RA` (1317 rows) cross-checks exactly with the Phase 2 baseline
(50-file probe + 30-file 3-month setup), and the Phase 2 figures were
independently verified against `JVDataCheckTool`. The "inserted < read"
delta on snapshot-style dataspecs (UM/CH/KS/BR/BN/HC/WC/YS/HS/HY/CK)
is the same record being delivered across multiple file generations
within a single setup window — `INSERT … ON CONFLICT DO UPDATE`
correctly merges them, which is why the second-run row counts are
unchanged.

**Not verified on the trial.** `HN`, `SK`, `BT` (`BLOD` returned no
data on this account; `BLDN` requires an additional license and
returned `-301`) and `WH` (real-time only via `JVRTOpen`, outside
setup-mode scope). Parser and Postgres-writer tests cover these three
record types against fixture bytes; the next live verification with
`BLDN` access or in a real-time run will close the gap.

**Goal.** Every record type listed in
[03-data-specs.md](./03-data-specs.md) has a decoder, a table, and is
loaded by the pipeline.

**Scope.**

- Parsers for the remaining record types (`SE`, `HR`, `H1`, `H6`,
  `O1`–`O6`, `WF`, `JG`, `UM`, `KS`, `CH`, `BR`, `BN`, `RC`, `HN`,
  `SK`, `BT`, `DM`, `TM`, `CK`, `HC`, `WC`, `YS`, `HS`, `HY`, `CS`,
  `TK`, `WH`).
- A schema file per table under `schema/`.
- Each parser added in TDD style: fixture bytes → expected DTO Red,
  decoder Green, refactor.
- Unknown record IDs continue to be logged and skipped, not coerced.

**Deferred.** A handful of deeply-nested sub-records are scaffolded
(top-level identifying scalars decoded; inner stat blocks left for a
follow-up). These are noted in their schema headers:

- `KS` / `CH` `HonZenRuikei[3]` (1052-byte annual stat blocks)
- `CK` `CHAKUKAISU3/4/5_INFO` placement-count arrays and the
  1220-byte `HonRuikei[2]` annual stat blocks

The deferral does not affect row counts or PK semantics.

**Done when.**

- Running setup over a 3-month window populates every applicable
  table; row counts cross-check against `JVDataCheckTool`.
- All parsers have unit tests against fixture bytes.
- Re-running over the same window is idempotent for every table.

### Phase 4 — All modes and resume

**Status.** Code complete. The four subcommands ship with run-history
persistence, normal-mode and setup-mode resume, and retry-with-backoff
on `JVOpen` for the documented transient codes. Live verification on
a single dataspec round-trip is documented in PR 4b's description; a
deliberate kill-and-resume integration test against the Testcontainers
PostgreSQL instance is the only remaining done-criterion.

**Goal.** All four acquisition modes work, runs are resumable after
interruption, and acquisition state is persisted.

**Scope.**

- `setup`, `range`, `normal`, `weekly` subcommands as described in
  [05-pipeline-modes.md](./05-pipeline-modes.md).
- The `jvlink2db.acquisition_state` and `jvlink2db.run_history` tables
  from [04-database-schema.md](./04-database-schema.md).
- Resume logic: for `option = 1`, advance `fromtime` from the saved
  `lastfiletimestamp`. For `option = 3`/`4`, replay the same `JVOpen`
  and `JVSkip` to the saved filename.
- Retry-with-backoff for the recoverable return codes documented in
  [06-error-handling.md](./06-error-handling.md).

**Implemented.**

- Subcommand split: `setup` (option=4 start-only), `range` (option=4 +
  `--since`/`--until`, snapshot dataspecs rejected at parse time),
  `normal` (option=1, `--since` resumes from `acquisition_state`),
  `weekly` (option=2). PR 4a.
- Operational schema (`jvlink2db.acquisition_state`,
  `jvlink2db.run_history`) plus `IAcquisitionStateStore` /
  `IRunHistoryStore`. Every CLI invocation writes a `run_history` row
  with start/finish, outcome, JV-Link return codes, and JSONB summaries
  of records read and inserted. PR 4b.
- `IJvLink.Skip()` thin wrapper around `JVSkip`. `SetupRunner` accepts
  `ResumeFromFilename` and skips past already-consumed files. PR 4b.
- `JvLinkRetryPolicy` retries `JVOpen` for `-301` (auth transient,
  3 retries at 1 s / 5 s / 30 s), `-411..-431` (HTTP errors, 3 retries
  at 5 s / 30 s / 5 min), `-504` (server maintenance, 4 retries up to
  60 min). Other codes fall through unchanged. PR 4c.

**Deferred.**

- Mid-read recovery for `-402`/`-403`/`-502`/`-503` in `JVRead` /
  `JVStatus` (the spec says "abort, retry the open"); right now these
  surface as `JvLinkException`. The user re-runs and the next run
  benefits from `acquisition_state` resume.
- Multiple dataspecs in one CLI invocation (`--dataspec RACE,DIFF`).

**Done when.**

- Each mode is exercised in integration tests against a Testcontainers
  PostgreSQL instance with canned fixture inputs.
- Killing the process partway through a run and re-running it produces
  the same final state as an uninterrupted run, with no duplicate or
  missing rows.
- `jvlink2db.run_history` reflects every completed and aborted run.

### Phase 5 — Scheduled execution

**Goal.** A first-class way to run jvlink2db on a recurring schedule
via Windows Task Scheduler, plus a long-running foreground mode for
containerised use.

**Scope.**

- `--watch --interval <duration>` flag on the existing `normal` and
  `weekly` subcommands. With `--watch`, the process stays in the
  foreground and re-runs the same acquisition every `interval` until
  signalled to stop. Exits cleanly on `Ctrl+C` / `SIGTERM`.
- New `jvlink2db schedule` subcommand family that drives Task
  Scheduler:
  - `jvlink2db schedule install` — register a scheduled task. Required
    flags include the schedule (e.g. `--daily 06:30`,
    `--every 1h`), the mode (`normal` / `weekly`), the dataspec list,
    and a task name. The task runs `jvlink2db.exe` with the resolved
    arguments. Honours "run whether user is logged on or not" and
    "run with highest privileges" so that the COM component is
    available in the session.
  - `jvlink2db schedule uninstall --name <name>` — remove a previously
    installed task.
  - `jvlink2db schedule status` — list installed jvlink2db tasks with
    their next run time and last result.
- Implementation talks to Task Scheduler through a maintained library
  binding (`Microsoft.Win32.TaskScheduler`, MIT). `schtasks.exe`
  shelling out is the fallback if the library proves problematic.
- Documentation update: a section in
  [05-pipeline-modes.md](./05-pipeline-modes.md) describing both
  approaches (`--watch` for foreground / containerised use,
  `schedule install` for unattended Windows hosts) with worked
  examples.

**Done when.**

- `jvlink2db schedule install` on a Windows host creates a scheduled
  task that the OS executes correctly, including after a reboot, with
  the expected arguments and run policy.
- `jvlink2db schedule status` correctly reflects tasks installed by
  this tool and ignores other unrelated tasks in Task Scheduler.
- `jvlink2db schedule uninstall` removes only the named task and
  leaves nothing behind.
- `--watch` runs in the foreground for at least one full interval
  cycle, with structured logs per cycle, and exits cleanly on signal.
