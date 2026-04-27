-- jv.o2: O2 (馬連オッズ) records.
-- JV-Data 4.9.0.1 §JV_O2_ODDS_UMAREN (2042 bytes).

CREATE TABLE IF NOT EXISTS o2 (
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
    umaren_flag      text,

    kumi             text[],     -- 153 entries
    odds             integer[],
    ninki            smallint[],

    total_hyosu_umaren bigint,

    PRIMARY KEY (year, month_day, jyo_cd, kaiji, nichiji, race_num, happyo_tm)
);

COMMENT ON COLUMN o2.record_spec IS 'RecordSpec — always "O2"';
COMMENT ON COLUMN o2.data_kubun  IS 'DataKubun';
COMMENT ON COLUMN o2.make_date   IS 'MakeDate';
COMMENT ON COLUMN o2.year        IS 'Year — RACE_ID';
COMMENT ON COLUMN o2.month_day   IS 'MonthDay — RACE_ID';
COMMENT ON COLUMN o2.jyo_cd      IS 'JyoCD — RACE_ID';
COMMENT ON COLUMN o2.kaiji       IS 'Kaiji — RACE_ID';
COMMENT ON COLUMN o2.nichiji     IS 'Nichiji — RACE_ID';
COMMENT ON COLUMN o2.race_num    IS 'RaceNum — RACE_ID';
COMMENT ON COLUMN o2.happyo_tm   IS 'HappyoTime (MMDDHHMM)';
COMMENT ON COLUMN o2.toroku_tosu IS 'TorokuTosu';
COMMENT ON COLUMN o2.syusso_tosu IS 'SyussoTosu';
COMMENT ON COLUMN o2.umaren_flag IS 'UmarenFlag';
COMMENT ON COLUMN o2.kumi        IS 'OddsUmarenInfo[153].Kumi';
COMMENT ON COLUMN o2.odds        IS 'OddsUmarenInfo[153].Odds';
COMMENT ON COLUMN o2.ninki       IS 'OddsUmarenInfo[153].Ninki';
COMMENT ON COLUMN o2.total_hyosu_umaren IS 'TotalHyosuUmaren';
