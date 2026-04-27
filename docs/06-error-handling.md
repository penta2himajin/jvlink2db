# Error-code handling

The full code table lives in §3 of the JV-Link Interface Specification.
This document records jvlink2db's response policy for each class of
code. Codes not listed here are treated as fatal; the pipeline aborts
and surfaces the original code to the caller.

Return-value classes are determined by the leading digit:

- `-1xx`: caller-side parameter or environment problems. **Fatal.**
- `-2xx`: API-usage / state errors. **Fatal.** Indicates a bug in this
  project.
- `-3xx`: authentication and licence. **Fatal**, except `-301` which is
  retried (see below).
- `-4xx`: server / data integrity. **Mostly transient**, retried.
- `-5xx`: server maintenance, disk, or download failures. **Mostly
  transient**, retried.
- `0` and positive: success.

## `JVOpen` and `JVRTOpen`

| Code | Meaning | Policy |
|---|---|---|
| `0` | Success | Continue. |
| `-1` | No matching data | Continue: nothing to read for this window. Call `JVClose`. |
| `-2` | User cancelled the setup dialog | Treat as fatal for unattended runs. |
| `-111` | `dataspec` parameter invalid | Fatal. Configuration bug. |
| `-112` | `fromtime` start invalid | Fatal. Configuration bug. |
| `-113` | `fromtime` end invalid | Fatal. Configuration bug. |
| `-114` | `key` parameter invalid (real-time only) | Fatal. |
| `-115` | `option` parameter invalid | Fatal. Configuration bug. |
| `-116` | `dataspec` / `option` combination invalid | Fatal. Configuration bug. |
| `-201` | `JVInit` not called | Fatal. Implementation bug. |
| `-202` | Previous open not closed | Fatal. Implementation bug. |
| `-211` | Registry corrupted | Fatal. Operator must repair JV-Link. |
| `-301` | Authentication transient | Retry with backoff (3 attempts: 1 s, 5 s, 30 s). Then fatal. |
| `-302` | Service-key expired | Fatal. Operator must renew. |
| `-303` | Service-key not configured | Fatal. Operator must configure JV-Link. |
| `-305` | Terms-of-service not accepted | Fatal. Operator must accept ToS in JV-Link. |
| `-401` | JV-Link internal error | Fatal. |
| `-411` … `-431` | HTTP errors | Retry with backoff (3 attempts: 5 s, 30 s, 5 min). |
| `-501` | Setup-kit invalid | Fatal. The setup-kit option is no longer relevant in 2026 (CD/DVD support ended in 2022). |
| `-504` | Server under maintenance | Retry with backoff up to 1 hour. |

## `JVStatus`

| Code | Meaning | Policy |
|---|---|---|
| `>= 0` | Files downloaded so far | Continue polling until the value reaches `downloadcount`. |
| `-201` | `JVInit` not called | Fatal. |
| `-203` | `JVOpen` not called | Fatal. |
| `-502` | Download failed (network, disk) | Abort the current `JVOpen`, close, retry the open with backoff. |

## `JVRead` / `JVGets`

| Code | Meaning | Policy |
|---|---|---|
| `> 0` | Bytes read into the buffer | Decode and dispatch the record. |
| `0` | EOF for the whole acquisition | Exit the read loop, call `JVClose`. |
| `-1` | End of one file, more files to come | Update bookkeeping (last filename) and continue. |
| `-3` | File still downloading | Sleep briefly and retry the same call. |
| `-201` | `JVInit` / `JVOpen` not called | Fatal. |
| `-202` | Previous open not closed | Fatal. |
| `-203` | `JVOpen` not called | Fatal. |
| `-402` | Downloaded file is empty | Delete the file via `JVFiledelete`, abort the open, retry. |
| `-403` | Downloaded file is corrupt | Same as `-402`. |
| `-502` | Download failure | Same as `JVStatus` `-502`. |
| `-503` | File missing on disk | Abort the open, retry. |

## `JVClose`

`JVClose` returns `0` on success. It is called in `finally` blocks so
that any error in the read loop still releases JV-Link state.

## Fatal-error reporting

Fatal errors propagate as a typed exception that carries the original
code, the JV-Link method, the dataspec, and the `fromtime`. The CLI
exits with a non-zero status code derived from the JV-Link code class
(`1` for caller errors, `2` for state errors, `3` for auth, `4` for
server / file errors). The exact mapping is documented next to the
exception type and exercised in unit tests.
