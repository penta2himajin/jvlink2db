# Pipeline modes

jvlink2db exposes four acquisition modes that map directly onto the four
JV-Link `option` values, with one extra distinction (`setup` vs `range`)
for convenience.

## `setup`

Full historical bulk load.

- Underlying call: `JVOpen(spec, "YYYYMMDDhhmmss", 4, ...)`
- Default `fromtime` start: `19860101000000`.
- Reads all data the JV-Link setup endpoint has for the given spec.
- Resumable. The pipeline records the last successfully consumed
  filename per dataspec; on resume it re-issues the same `JVOpen` and
  uses `JVSkip` to skip past completed files.
- Allowed for any dataspec listed under `option = 3, 4` in
  [02-jvlink-protocol.md](./02-jvlink-protocol.md).

## `range`

Bounded historical load.

- Underlying call: `JVOpen(spec, "YYYYMMDDhhmmss-YYYYMMDDhhmmss", 4, ...)`
- Useful for testing on a few months of data without committing to a
  full setup, and for back-filling a specific window after the fact.
- The end-time form of `fromtime` is rejected by JV-Link for the
  following dataspecs, which deliver a single all-encompassing snapshot:
  `TOKU`, `DIFF`, `DIFN`, `HOSE`, `HOSN`, `HOYU`, `COMM`. The pipeline
  refuses these combinations at parse time rather than letting the call
  fail at runtime.

## `normal`

Incremental update of an already-populated database.

- Underlying call: `JVOpen(spec, fromtime, 1, ...)`
- Default `fromtime`: the last successful timestamp recorded in
  `jvlink2db.acquisition_state` for the given spec, or a configured
  starting point on first run.
- Intended to be run on a schedule (e.g. daily / weekly).

## `weekly`

This-week-only fetch.

- Underlying call: `JVOpen(spec, fromtime, 2, ...)`
- Reads only the current week's race-related data (entries plus the
  previous week's results), not the full historical archive.
- Useful when a downstream consumer only needs the upcoming weekend.

## CLI surface

All four modes share a common set of flags (`--connection`, `--schema`,
`--dataspec`, `--sid`). Each mode adds its own time options:

```
jvlink2db setup   --dataspec RACE [--since 19860101000000]
jvlink2db range   --dataspec RACE  --since 20260101000000 --until 20260331235959
jvlink2db normal  --dataspec RACE  --since 20260415000000
jvlink2db weekly  --dataspec RACE  --since 20260415000000
```

`--since` / `--until` are 14-character `YYYYMMDDhhmmss` strings; this is
what JV-Link expects directly without normalisation.

`range` rejects the snapshot-only dataspecs (`TOKU`, `DIFF`, `DIFN`,
`HOSE`, `HOSN`, `HOYU`, `COMM`) at parse time with a typed exception
(`DataspecRangeNotSupportedException`) and exit code `1`, rather than
letting the COM call fail at runtime with `-113`.

Multiple dataspecs in one invocation (`--dataspec RACE,DIFN,SLOP`)
are processed in sequence — one `JVOpen` call each, never combined
into a single call (see the `dataspec` known-issue note in
[02-jvlink-protocol.md](./02-jvlink-protocol.md)). One `JVInit` and
one COM activation are reused across the batch, so first-launch
dialogs JV-Link sometimes triggers per-process don't fire again
between dataspecs.

## Recurring execution

Two complementary mechanisms cover the recurring-acquisition use
case. Pick the one that matches your runtime.

### `--watch` for foreground / containerised use

`normal` and `weekly` accept `--watch --interval <duration>`. The
process stays in the foreground and re-runs the acquisition every
interval until Ctrl+C / SIGTERM. Per-cycle errors are logged to
stderr and do not stop the loop. `--interval` accepts either
`HH:MM:SS` or shorthand `30s` / `5m` / `1h` / `1d`.

```
jvlink2db normal --dataspec RACE --watch --interval 30m \
  --connection "Host=...;Username=postgres;Password=..."
```

This mode is ideal for containers (`docker run --restart=always …`)
and for terminal sessions where the foreground process is the
process supervisor.

### `schedule install` for unattended Windows hosts

The `schedule` subcommand family drives Windows Task Scheduler so
jvlink2db.exe can run unattended on a recurring trigger. All tasks
are registered under the `\jvlink2db\` folder so `schedule status`
and `schedule uninstall` stay scoped.

```
jvlink2db schedule install \
  --name race-normal \
  --mode normal \
  --dataspec RACE \
  --connection "Host=…;…" \
  --daily 06:30

jvlink2db schedule install \
  --name race-weekly \
  --mode weekly \
  --dataspec RACE \
  --connection "Host=…;…" \
  --since 20260415000000 \
  --every 1h

jvlink2db schedule status
jvlink2db schedule uninstall --name race-normal
```

Tasks default to `LogonType = InteractiveToken` (run only when the
user is logged on) with `RunLevel = Highest`, which gives JV-Link
COM access to the user's session without an interactive UAC prompt.
Pass `--always --password <pw>` to switch to `LogonType = Password`
so the task fires whether or not the user is logged on; Task
Scheduler stores the credential.

The action arguments installed by `schedule install` mirror the
`normal` / `weekly` CLI surface — running `schedule install`
followed by inspecting the registered task in `taskschd.msc` shows
the exact command line that will fire.

## Progress and logging

While a run is active the CLI prints, on a single re-written line:

- The current dataspec.
- Files completed / total (`readcount` from `JVOpen`).
- The current filename being decoded.
- Records decoded so far in the run, and rate (records / second).
- Wall time elapsed.

A structured log line is also written for every file boundary so that
run history can be reconstructed after the fact from the log alone.

## Failure handling

If `JVOpen`, `JVStatus`, or `JVRead` returns a fatal error code, the
run is aborted and the partial work is left in the database. The
`acquisition_state` row for that dataspec is *not* advanced, so a
subsequent run will retry the same window.

For recoverable codes (`-301` authentication transient, `-3` mid-
download, etc.) the pipeline retries with bounded exponential backoff;
see [06-error-handling.md](./06-error-handling.md).
