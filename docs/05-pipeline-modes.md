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

All modes share a common CLI shape. The exact flag set is the
configuration class's responsibility; this document shows the intent.

```
jvlink2db setup   --dataspec RACE --since 19860101 [--until 20260331]
jvlink2db range   --dataspec RACE --since 20260101 --until 20260331
jvlink2db normal  --dataspec RACE,DIFF,BLOD
jvlink2db weekly  --dataspec RACE
```

Multiple dataspecs given on the command line are processed in sequence,
one `JVOpen` call per dataspec, never combined into a single call (see
the `dataspec` known-issue note in
[02-jvlink-protocol.md](./02-jvlink-protocol.md)).

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
