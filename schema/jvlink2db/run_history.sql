-- jvlink2db.run_history: one row per CLI invocation.
-- Records mode/dataspec/fromtime/timestamps/outcome and a JSONB summary of
-- per-record-type read and inserted counts.

CREATE TABLE IF NOT EXISTS run_history (
    id                bigserial   PRIMARY KEY,
    mode              text        NOT NULL,
    dataspec          text        NOT NULL,
    option            smallint    NOT NULL,
    fromtime          text        NOT NULL,
    started_at        timestamptz NOT NULL,
    finished_at       timestamptz,
    outcome           text        NOT NULL,
    open_return_code  integer,
    read_count        integer,
    download_count    integer,
    last_file_timestamp text,
    record_counts     jsonb,
    records_inserted  jsonb,
    error_message     text
);

COMMENT ON COLUMN run_history.mode                IS 'CLI subcommand (setup/range/normal/weekly).';
COMMENT ON COLUMN run_history.dataspec            IS 'JV-Link dataspec ID.';
COMMENT ON COLUMN run_history.option              IS 'JVOpen option (1/2/3/4).';
COMMENT ON COLUMN run_history.fromtime            IS 'fromtime parameter passed to JVOpen.';
COMMENT ON COLUMN run_history.outcome             IS 'success | failed | aborted.';
COMMENT ON COLUMN run_history.open_return_code    IS 'JVOpen ReturnCode (0 success, -1 no-data, negative for errors).';
COMMENT ON COLUMN run_history.read_count          IS 'Total file count reported by JVOpen.';
COMMENT ON COLUMN run_history.download_count      IS 'Files JV-Link needed to download.';
COMMENT ON COLUMN run_history.last_file_timestamp IS 'Newest matching-file timestamp reported by JVOpen.';
COMMENT ON COLUMN run_history.record_counts       IS 'JSONB map of record-spec → records read.';
COMMENT ON COLUMN run_history.records_inserted    IS 'JSONB map of record-spec → rows inserted/merged.';
COMMENT ON COLUMN run_history.error_message       IS 'Exception message when outcome != success.';

CREATE INDEX IF NOT EXISTS idx_run_history_started_at
    ON run_history (started_at DESC);
