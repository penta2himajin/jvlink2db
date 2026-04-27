# Dataspec and record types

The JV-Data specification distinguishes two concepts that are easy to
confuse:

- A **dataspec** is the four-character ID passed to `JVOpen` to request
  a *bundle* of records (e.g. `RACE`).
- A **record type** is the two-character ID at the start of every
  individual record returned by `JVRead` (e.g. `RA`, `SE`, `O1`).

One dataspec returns many record types. The mapping below is taken from
the JV-Data Specification 4.9.0.1, §"データ種別一覧".

## Cumulative dataspecs

| dataspec | Contains record types | Description |
|---|---|---|
| `TOKU` | `TK` | Special-registration race info. |
| `RACE` | `RA`, `SE`, `HR`, `H1`, `H6`, `O1`, `O2`, `O3`, `O4`, `O5`, `O6`, `WF`, `JG` | Race details, per-horse race info, payouts, vote counts, fixed odds, WIN5, withdrawals. |
| `DIFF` / `DIFN` | `UM`, `KS`, `CH`, `BR`, `BN`, `RC`, plus `RA` and `SE` for NAR / overseas | Incremental master updates plus NAR and overseas race detail. `DIFN` is the post-2023-08-08 extended-field version. |
| `BLOD` / `BLDN` | `HN`, `SK`, `BT` | Pedigree, progeny, lineage. |
| `SNAP` / `SNPN` | `CK` | Per-runner cumulative-record snapshots. |
| `SLOP` | `HC` | Slope-track (`坂路`) training. |
| `WOOD` | `WC` | Wood-chip training. |
| `YSCH` | `YS` | Year-schedule of meetings. |
| `TOKU` | `TK` | Special-registration entries (already listed). |
| `HOSE` / `HOSN` | `HS` | Yearling-sale prices. |
| `HOYU` | `HY` | Horse-name etymology. |
| `COMM` | `CS` | Course information. |
| `MING` | `DM`, `TM` | Time-type and head-to-head data-mining predictions. |

## Backfill (non-cumulative) dataspecs

These are intended for non-cumulative apps that need the same data set
that a cumulative app would have built up over time.

| dataspec | Contains record types | Description |
|---|---|---|
| `TCOV` / `TCVN` | `UM`, `CH`, `BR`, `BN`, `RC`, plus `RA` and `SE` | Backfill for special-registration entries: full history of every registered horse. |
| `RCOV` / `RCVN` | `UM`, `KS`, `CH`, `BR`, `BN`, `RC`, plus `RA` and `SE` | Backfill for race-info contexts. |

## Real-time dataspecs

Real-time data uses a different entry point (`JVRTOpen`) and a separate
set of dataspec IDs. The full list is in the JV-Data Specification.
The ones jvlink2db cares about initially are listed here; this list will
be expanded as the real-time module is implemented.

| dataspec | Contains record types | Description |
|---|---|---|
| `0B12` | `RA`, `SE`, `HR` | Race info after results are confirmed. |
| `0B15` | `RA`, `SE` | Race info from entries onwards. |
| `0B30`–`0B36` | `O1`–`O6`, `H1`, `H6` | Real-time odds and vote counts. |
| `0B41` | `WH` | Pre-race weight. |

## Record-type catalogue

| Record ID | Name | Notes |
|---|---|---|
| `RA` | Race details | One row per race. |
| `SE` | Per-horse race info | One row per (race × horse). |
| `HR` | Payouts | |
| `H1` | Vote counts (non-trifecta) | |
| `H6` | Vote counts (trifecta) | |
| `O1` | Fixed odds (win / place / bracket) | |
| `O2` | Fixed odds (quinella) | |
| `O3` | Fixed odds (wide) | |
| `O4` | Fixed odds (exacta) | |
| `O5` | Fixed odds (trio) | |
| `O6` | Fixed odds (trifecta) | |
| `WF` | WIN5 | |
| `JG` | Withdrawal info | |
| `UM` | Horse master | |
| `KS` | Jockey master | |
| `CH` | Trainer master | |
| `BR` | Breeder master | |
| `BN` | Owner master | |
| `RC` | Course-record master | |
| `HN` | Brood-mare master | |
| `SK` | Progeny master | |
| `BT` | Lineage info | |
| `DM` | Data-mining prediction (time) | |
| `TM` | Data-mining prediction (head-to-head) | |
| `CK` | Cumulative win/place counts | |
| `HC` | Slope training | |
| `WC` | Wood-chip training | |
| `YS` | Year schedule | |
| `HS` | Yearling-sale price | |
| `HY` | Horse-name etymology | |
| `CS` | Course information | |
| `TK` | Special-registration | |
| `WH` | Pre-race weight (real-time) | |

The authoritative byte layout for every record type is defined in the
bundled `JVData_Struct.cs` (and equivalents for C++, VB, Delphi) shipped
with the SDK.
