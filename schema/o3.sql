-- jv.o3: O3 (ワイドオッズ) records.
-- JV-Data 4.9.0.1 §JV_O3_ODDS_WIDE (2654 bytes).

CREATE TABLE IF NOT EXISTS o3 (
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
    wide_flag        text,

    kumi             text[],
    odds_low         integer[],
    odds_high        integer[],
    ninki            smallint[],

    total_hyosu_wide bigint,

    PRIMARY KEY (year, month_day, jyo_cd, kaiji, nichiji, race_num, happyo_tm)
);

COMMENT ON COLUMN o3.record_spec IS 'RecordSpec — always "O3"';
COMMENT ON COLUMN o3.data_kubun  IS 'DataKubun';
COMMENT ON COLUMN o3.make_date   IS 'MakeDate';
COMMENT ON COLUMN o3.year        IS 'Year — RACE_ID';
COMMENT ON COLUMN o3.month_day   IS 'MonthDay — RACE_ID';
COMMENT ON COLUMN o3.jyo_cd      IS 'JyoCD — RACE_ID';
COMMENT ON COLUMN o3.kaiji       IS 'Kaiji — RACE_ID';
COMMENT ON COLUMN o3.nichiji     IS 'Nichiji — RACE_ID';
COMMENT ON COLUMN o3.race_num    IS 'RaceNum — RACE_ID';
COMMENT ON COLUMN o3.happyo_tm   IS 'HappyoTime (MMDDHHMM)';
COMMENT ON COLUMN o3.toroku_tosu IS 'TorokuTosu';
COMMENT ON COLUMN o3.syusso_tosu IS 'SyussoTosu';
COMMENT ON COLUMN o3.wide_flag   IS 'WideFlag';
COMMENT ON COLUMN o3.kumi        IS 'OddsWideInfo[153].Kumi';
COMMENT ON COLUMN o3.odds_low    IS 'OddsWideInfo[153].OddsLow';
COMMENT ON COLUMN o3.odds_high   IS 'OddsWideInfo[153].OddsHigh';
COMMENT ON COLUMN o3.ninki       IS 'OddsWideInfo[153].Ninki';
COMMENT ON COLUMN o3.total_hyosu_wide IS 'TotalHyosuWide';
