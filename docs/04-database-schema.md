# Database schema

## Naming

- Default schema: `jv`.
- One table per record type. Table name: `jv.<record_id>` in lower
  case, e.g. `jv.ra`, `jv.se`, `jv.o1`.
- Column names follow the JV-Data field names converted to
  `snake_case`. The original mixed-case field name is preserved in a
  `COMMENT` on the column for traceability back to the specification.

The schema name is configurable; tests use a disposable schema per run.

## Types

- All textual fields are stored as `text`. The on-wire format is fixed-
  width Shift-JIS, but JV-Data treats trailing spaces as padding rather
  than data, so values are right-trimmed before insertion.
- Numeric fields documented as ASCII-digit strings are stored as the
  smallest integer type that fits the documented width: `smallint`,
  `integer`, `bigint`. Empty / spaces-only values become `NULL`.
- Decimal fields documented with implicit decimal places (e.g. odds
  encoded as `00345` meaning `34.5`) are stored as `numeric` with the
  appropriate scale, again `NULL` for empty.
- Date / time fields are stored as `date` or `timestamp` where they
  represent real instants. Race-coded fields (year, month-day, course
  code, meeting, day, race number) are kept as separate columns because
  they are part of composite keys.

## Primary keys

Primary keys follow the field combinations the JV-Data specification
calls out as the natural identity of each record type. Examples:

- `jv.ra`: `(year, monthday, jyocd, kaiji, nichiji, racenum)`
- `jv.se`: `(year, monthday, jyocd, kaiji, nichiji, racenum, umaban)`
- `jv.o1`: `(year, monthday, jyocd, kaiji, nichiji, racenum, happyo_tm)`
- `jv.um`: `(kettonum)`
- `jv.ks`: `(kishucode)`

Where a record type has multiple sub-formats addressed by an internal
discriminator (e.g. cumulative vs incremental), the discriminator is
part of the primary key.

The full primary-key list lives next to the DDL, not in this document,
so that it stays consistent with what the importer actually creates.

## Indexes

Only the primary key is created automatically. No secondary indexes are
added by jvlink2db. Index strategy depends entirely on the workload of
the consumer, who can add indexes after a load completes.

## DDL management

The DDL lives in `schema/<record_id>.sql`, one file per table, hand-
maintained. On startup, the importer compares the live schema against
the expected DDL and:

- creates missing tables;
- refuses to start if an existing table's columns disagree with the
  expected definition; manual migration is required.

DDL is never altered destructively at runtime.

## Bulk-insert strategy

All inserts use Npgsql's binary `COPY` writer, which is the fastest
path PostgreSQL offers.

For an idempotent load over a window that may overlap previously loaded
data, the import sequence per record type is:

```
BEGIN;
CREATE TEMP TABLE stg_<rec> (LIKE jv.<rec> INCLUDING DEFAULTS) ON COMMIT DROP;
COPY stg_<rec> FROM STDIN BINARY;     -- written by Npgsql
INSERT INTO jv.<rec> SELECT * FROM stg_<rec>
  ON CONFLICT (<pk>) DO UPDATE SET <non-pk> = EXCLUDED.<non-pk>;
COMMIT;
```

Notes:

- The `INSERT ... ON CONFLICT` step is the only place duplicates from a
  single staging batch can hurt; the staging table must therefore be
  deduplicated on the primary key before the merge. The deduplication
  rule is *last record wins*, in the order JV-Link returned them.
- For pure first-time loads of an empty target table, the staging step
  is skipped and `COPY` writes directly into `jv.<rec>`. The pipeline
  detects emptiness with a single `SELECT 1 FROM jv.<rec> LIMIT 1`.
- WAL-skipping (`UNLOGGED` tables, `synchronous_commit = off`,
  `wal_level = minimal`) is left to the operator. The importer does not
  manipulate cluster-wide settings.

## State tables

jvlink2db maintains a small operational schema, separate from `jv`:

- `jvlink2db.acquisition_state`: per-`dataspec` last successful
  `fromtime` and last successful filename.
- `jvlink2db.run_history`: one row per CLI invocation: mode, dataspec,
  fromtime, started_at, finished_at, files_read, records_inserted,
  outcome.

These tables are created on first run.
