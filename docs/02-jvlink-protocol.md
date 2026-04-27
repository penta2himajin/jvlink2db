# JV-Link COM protocol

This document summarises the parts of the JV-Link Interface Specification
4.9.0.1 that jvlink2db actually uses. The full specification is the
authoritative reference; what follows is a working subset.

## Component identity

- ProgID: `JVDTLab.JVLink`
- CLSID: `{2AB1774D-0C41-11D7-916F-0003479BEB3F}`
- Threading: single-threaded apartment.
- Bitness: 32-bit. The DLL cannot be loaded directly into a 64-bit
  process. jvlink2db avoids the issue by running the whole importer as
  `win-x86`.

## Methods used by jvlink2db

| Method | Role |
|---|---|
| `JVInit(sid)` | Initialises the library with the application's software identifier. Must be called before any other call. |
| `JVSetSaveFlag(flag)` | Configures whether downloaded files are kept on disk. Set once at startup. |
| `JVSetSavePath(path)` | Configures the local cache directory. Set once at startup. |
| `JVOpen(dataspec, fromtime, option, &readcount, &downloadcount, &lastfiletimestamp)` | Begins an acquisition. Returns the number of files to read, the number of files that need to be downloaded, and the timestamp of the newest matching file. |
| `JVStatus()` | Returns the count of files downloaded so far. Polled while waiting for the background download thread to catch up. |
| `JVRead(buffer, size, filename)` / `JVGets(buffer, size, filename)` | Reads the next record from the current file. `JVGets` returns a `byte[]` directly and is preferred for binary correctness. |
| `JVSkip()` | Skips the rest of the current file. Used to resume from a known filename. |
| `JVClose()` | Closes the current acquisition. Must be called before the next `JVOpen`. |
| `JVCancel()` | Aborts a running download. |

Movie/streaming methods (`JVMVOpen`, `JVMVPlay`, etc.), shirt-pattern
image methods (`JVFuku*`), and event-mode methods (`JVWatchEvent*`) are
out of scope.

## JVOpen parameters

### `dataspec`

A concatenation of four-character data-type IDs. The total length must
be a multiple of four. Examples: `"RACE"`, `"DIFF"`, `"RACEDIFF"`.

The specification documents a known issue: combining multiple data-type
IDs in a single call can make `JVRead` significantly slower when the
total file count is large. jvlink2db opens one ID per call.

### `fromtime`

Two accepted forms:

- **Start only:** `YYYYMMDDhhmmss`. Reads everything strictly newer than
  the given instant up to the present.
- **Range:** `YYYYMMDDhhmmss-YYYYMMDDhhmmss`. Reads everything in the
  half-open interval `(start, end]`.

Several data-type IDs cannot use the range form because they are
delivered as a single all-encompassing snapshot:

- `TOKU`, `DIFF`, `DIFN`, `HOSE`, `HOSN`, `HOYU`, `COMM`

If a range is supplied for one of these, JV-Link returns `-1` (no data).

### `option`

| Value | Meaning |
|---|---|
| `1` | Normal data. The incremental-update mode for accumulator-style applications. Reads anything matching `dataspec` and `fromtime` from the regular data store. |
| `2` | This-week data. Reads only data relevant to the current week's races (race entries plus the previous week's results). |
| `3` | Setup data, with a dialog. Used for first-time bulk loads. |
| `4` | Setup data, without a dialog after the first call. Recommended for unattended bulk loads. |

The valid `option` / `dataspec` combinations from the specification:

| `option` | Allowed `dataspec` |
|---|---|
| `1` | `TOKU`, `RACE`, `DIFF`, `BLOD`, `SNAP`, `SLOP`, `WOOD`, `YSCH`, `HOSE`, `HOYU`, `DIFN`, `BLDN`, `SNPN`, `HOSN` |
| `2` | `TOKU`, `RACE`, `TCOV`, `RCOV`, `SNAP`, `TCVN`, `RCVN`, `SNPN` |
| `3`, `4` | `TOKU`, `RACE`, `DIFF`, `BLOD`, `SNAP`, `SLOP`, `WOOD`, `YSCH`, `HOSE`, `HOYU`, `COMM`, `MING`, `DIFN`, `BLDN`, `SNPN`, `HOSN` |

Any other combination produces error `-116` (`dataspec` and `option`
combination invalid).

## Acquisition loop

The canonical sequence, reproduced from the SDK sample programs:

```
JVInit("sid")                              // once per process
JVSetSaveFlag(...) ; JVSetSavePath(...)    // once, optional

// per dataspec / per chunk
rc = JVOpen(spec, fromtime, option, ...)
if rc < 0: handle error and stop

while JVStatus() < downloadCount:           // wait for downloader
    sleep a short interval
    if JVStatus() < 0: handle error and stop

loop:
    rc = JVGets(buffer, size, filename)
    if rc > 0: a record was read, dispatch by record-id (first 2 bytes)
    if rc == -1: end of one file, current filename is now `filename`
    if rc == 0:  end of all files, exit loop
    if rc == -3: a file is still downloading, sleep briefly and retry
    if rc < -1 and rc != -3: handle error and stop

JVClose()
```

Resume after interruption:

- For `option = 1` (normal): record the last successfully consumed file's
  timestamp (`m_CurrentFileTimestamp`) and pass it as the next `fromtime`.
- For `option = 3` or `4` (setup): record the last successfully consumed
  filename, then on resume call `JVOpen` with the original parameters and
  call `JVSkip` repeatedly until JV-Link reports the recorded filename.

## Notes on encoding

All record bytes are Shift-JIS. Decoders work on raw bytes, not on
.NET strings, and convert only the fields they need to interpret as
text. Numeric fields are typically ASCII-encoded fixed-width digits
(e.g. `"00018"` for 18) and are parsed without going through string
allocations on the hot path.
