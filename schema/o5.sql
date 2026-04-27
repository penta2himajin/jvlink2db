-- jv.o5: O5 (3連複オッズ) records.
-- JV-Data 4.9.0.1 §JV_O5_ODDS_SANREN (12293 bytes).

CREATE TABLE IF NOT EXISTS o5 (
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
    sanrenpuku_flag  text,

    kumi             text[],     -- 816 entries
    odds             integer[],
    ninki            smallint[],

    total_hyosu_sanrenpuku bigint,

    PRIMARY KEY (year, month_day, jyo_cd, kaiji, nichiji, race_num, happyo_tm)
);

COMMENT ON COLUMN o5.record_spec IS 'RecordSpec — always "O5"';
COMMENT ON COLUMN o5.data_kubun  IS 'DataKubun';
COMMENT ON COLUMN o5.make_date   IS 'MakeDate';
COMMENT ON COLUMN o5.year        IS 'Year — RACE_ID';
COMMENT ON COLUMN o5.month_day   IS 'MonthDay — RACE_ID';
COMMENT ON COLUMN o5.jyo_cd      IS 'JyoCD — RACE_ID';
COMMENT ON COLUMN o5.kaiji       IS 'Kaiji — RACE_ID';
COMMENT ON COLUMN o5.nichiji     IS 'Nichiji — RACE_ID';
COMMENT ON COLUMN o5.race_num    IS 'RaceNum — RACE_ID';
COMMENT ON COLUMN o5.happyo_tm   IS 'HappyoTime (MMDDHHMM)';
COMMENT ON COLUMN o5.toroku_tosu IS 'TorokuTosu';
COMMENT ON COLUMN o5.syusso_tosu IS 'SyussoTosu';
COMMENT ON COLUMN o5.sanrenpuku_flag IS 'SanrenpukuFlag';
COMMENT ON COLUMN o5.kumi        IS 'OddsSanrenInfo[816].Kumi';
COMMENT ON COLUMN o5.odds        IS 'OddsSanrenInfo[816].Odds';
COMMENT ON COLUMN o5.ninki       IS 'OddsSanrenInfo[816].Ninki';
COMMENT ON COLUMN o5.total_hyosu_sanrenpuku IS 'TotalHyosuSanrenpuku';
