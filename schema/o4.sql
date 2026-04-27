-- jv.o4: O4 (馬単オッズ) records.
-- JV-Data 4.9.0.1 §JV_O4_ODDS_UMATAN (4031 bytes).

CREATE TABLE IF NOT EXISTS o4 (
    record_spec      text     NOT NULL,
    data_kubun       text     NOT NULL,
    make_date        date,

    year             text     NOT NULL,
    month_day        text     NOT NULL,
    jyo_cd           text     NOT NULL,
    kaiji            text     NOT NULL,
    nichiji          text     NOT NULL,
    race_num         text     NOT NULL,
    happyo_tm        text     NOT NULL,

    toroku_tosu      smallint,
    syusso_tosu      smallint,
    umatan_flag      text,

    kumi             text[],     -- 306 entries
    odds             integer[],
    ninki            smallint[],

    total_hyosu_umatan bigint,

    PRIMARY KEY (year, month_day, jyo_cd, kaiji, nichiji, race_num, happyo_tm)
);

COMMENT ON COLUMN o4.record_spec IS 'RecordSpec — always "O4"';
COMMENT ON COLUMN o4.data_kubun  IS 'DataKubun';
COMMENT ON COLUMN o4.make_date   IS 'MakeDate';
COMMENT ON COLUMN o4.year        IS 'Year — RACE_ID';
COMMENT ON COLUMN o4.month_day   IS 'MonthDay — RACE_ID';
COMMENT ON COLUMN o4.jyo_cd      IS 'JyoCD — RACE_ID';
COMMENT ON COLUMN o4.kaiji       IS 'Kaiji — RACE_ID';
COMMENT ON COLUMN o4.nichiji     IS 'Nichiji — RACE_ID';
COMMENT ON COLUMN o4.race_num    IS 'RaceNum — RACE_ID';
COMMENT ON COLUMN o4.happyo_tm   IS 'HappyoTime (MMDDHHMM)';
COMMENT ON COLUMN o4.toroku_tosu IS 'TorokuTosu';
COMMENT ON COLUMN o4.syusso_tosu IS 'SyussoTosu';
COMMENT ON COLUMN o4.umatan_flag IS 'UmatanFlag';
COMMENT ON COLUMN o4.kumi        IS 'OddsUmatanInfo[306].Kumi';
COMMENT ON COLUMN o4.odds        IS 'OddsUmatanInfo[306].Odds';
COMMENT ON COLUMN o4.ninki       IS 'OddsUmatanInfo[306].Ninki';
COMMENT ON COLUMN o4.total_hyosu_umatan IS 'TotalHyosuUmatan';
