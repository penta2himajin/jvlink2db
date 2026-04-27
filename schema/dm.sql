-- jv.dm: DM (タイム型データマイニング予想). JV-Data §JV_DM_INFO, 303 bytes.

CREATE TABLE IF NOT EXISTS dm (
    record_spec text NOT NULL,
    data_kubun  text NOT NULL,
    make_date   date,

    year        text NOT NULL,
    month_day   text NOT NULL,
    jyo_cd      text NOT NULL,
    kaiji       text NOT NULL,
    nichiji     text NOT NULL,
    race_num    text NOT NULL,
    make_hm     text,

    umaban      text[],     -- 18
    dm_time     integer[],
    dm_gosa_p   smallint[],
    dm_gosa_m   smallint[],

    PRIMARY KEY (year, month_day, jyo_cd, kaiji, nichiji, race_num)
);

COMMENT ON COLUMN dm.record_spec IS 'RecordSpec — always "DM"';
COMMENT ON COLUMN dm.data_kubun  IS 'DataKubun';
COMMENT ON COLUMN dm.make_date   IS 'MakeDate';
COMMENT ON COLUMN dm.year        IS 'Year — RACE_ID';
COMMENT ON COLUMN dm.month_day   IS 'MonthDay — RACE_ID';
COMMENT ON COLUMN dm.jyo_cd      IS 'JyoCD — RACE_ID';
COMMENT ON COLUMN dm.kaiji       IS 'Kaiji — RACE_ID';
COMMENT ON COLUMN dm.nichiji     IS 'Nichiji — RACE_ID';
COMMENT ON COLUMN dm.race_num    IS 'RaceNum — RACE_ID';
COMMENT ON COLUMN dm.make_hm     IS 'MakeHM';
COMMENT ON COLUMN dm.umaban      IS 'DMInfo[18].Umaban';
COMMENT ON COLUMN dm.dm_time     IS 'DMInfo[18].DMTime';
COMMENT ON COLUMN dm.dm_gosa_p   IS 'DMInfo[18].DMGosaP';
COMMENT ON COLUMN dm.dm_gosa_m   IS 'DMInfo[18].DMGosaM';
