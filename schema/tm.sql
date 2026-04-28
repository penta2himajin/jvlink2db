-- jv.tm: TM (対戦型データマイニング予想). JV-Data §JV_TM_INFO, 141 bytes.

CREATE TABLE IF NOT EXISTS tm (
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
    tm_score    integer[],

    PRIMARY KEY (year, month_day, jyo_cd, kaiji, nichiji, race_num)
);

COMMENT ON COLUMN tm.record_spec IS 'RecordSpec — always "TM"';
COMMENT ON COLUMN tm.data_kubun  IS 'DataKubun';
COMMENT ON COLUMN tm.make_date   IS 'MakeDate';
COMMENT ON COLUMN tm.year        IS 'Year — RACE_ID';
COMMENT ON COLUMN tm.month_day   IS 'MonthDay — RACE_ID';
COMMENT ON COLUMN tm.jyo_cd      IS 'JyoCD — RACE_ID';
COMMENT ON COLUMN tm.kaiji       IS 'Kaiji — RACE_ID';
COMMENT ON COLUMN tm.nichiji     IS 'Nichiji — RACE_ID';
COMMENT ON COLUMN tm.race_num    IS 'RaceNum — RACE_ID';
COMMENT ON COLUMN tm.make_hm     IS 'MakeHM (HHMM)';
COMMENT ON COLUMN tm.umaban      IS 'TMInfo[18].Umaban';
COMMENT ON COLUMN tm.tm_score    IS 'TMInfo[18].TMScore';
