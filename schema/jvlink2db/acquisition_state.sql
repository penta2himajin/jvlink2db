-- jvlink2db.acquisition_state: per-(dataspec, option) resume marker.
-- Tracks the last successfully consumed timestamp (for option=1 / normal-mode
-- incremental resume) and last successfully consumed filename (for option=3/4
-- setup-mode resume via JVSkip).

CREATE TABLE IF NOT EXISTS acquisition_state (
    dataspec               text     NOT NULL,
    option                 smallint NOT NULL,
    last_fromtime          text,
    last_filename          text,
    last_success_at        timestamptz NOT NULL DEFAULT now(),

    PRIMARY KEY (dataspec, option)
);

COMMENT ON COLUMN acquisition_state.dataspec        IS 'JV-Link dataspec ID (e.g. RACE, DIFN).';
COMMENT ON COLUMN acquisition_state.option          IS 'JVOpen option (1=normal, 2=weekly, 3/4=setup).';
COMMENT ON COLUMN acquisition_state.last_fromtime   IS 'YYYYMMDDhhmmss of the last successful JVOpen ReturnCode 0 (used as the next --since for option=1).';
COMMENT ON COLUMN acquisition_state.last_filename   IS 'Filename of the last successfully consumed file (used by JVSkip on option=3/4 resume).';
COMMENT ON COLUMN acquisition_state.last_success_at IS 'When the row was last updated.';
