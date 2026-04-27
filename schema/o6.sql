-- jv.o6: O6 (3連単オッズ) records.
-- JV-Data 4.9.0.1 §JV_O6_ODDS_SANRENTAN (83285 bytes).

CREATE TABLE IF NOT EXISTS o6 (
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
    sanrentan_flag   text,

    kumi             text[],     -- 4896 entries
    odds             integer[],
    ninki            smallint[],

    total_hyosu_sanrentan bigint,

    PRIMARY KEY (year, month_day, jyo_cd, kaiji, nichiji, race_num, happyo_tm)
);

COMMENT ON COLUMN o6.record_spec IS 'RecordSpec — always "O6"';
COMMENT ON COLUMN o6.data_kubun  IS 'DataKubun';
COMMENT ON COLUMN o6.make_date   IS 'MakeDate';
COMMENT ON COLUMN o6.year        IS 'Year — RACE_ID';
COMMENT ON COLUMN o6.month_day   IS 'MonthDay — RACE_ID';
COMMENT ON COLUMN o6.jyo_cd      IS 'JyoCD — RACE_ID';
COMMENT ON COLUMN o6.kaiji       IS 'Kaiji — RACE_ID';
COMMENT ON COLUMN o6.nichiji     IS 'Nichiji — RACE_ID';
COMMENT ON COLUMN o6.race_num    IS 'RaceNum — RACE_ID';
COMMENT ON COLUMN o6.happyo_tm   IS 'HappyoTime (MMDDHHMM)';
COMMENT ON COLUMN o6.toroku_tosu IS 'TorokuTosu';
COMMENT ON COLUMN o6.syusso_tosu IS 'SyussoTosu';
COMMENT ON COLUMN o6.sanrentan_flag IS 'SanrentanFlag';
COMMENT ON COLUMN o6.kumi        IS 'OddsSanrentanInfo[4896].Kumi';
COMMENT ON COLUMN o6.odds        IS 'OddsSanrentanInfo[4896].Odds';
COMMENT ON COLUMN o6.ninki       IS 'OddsSanrentanInfo[4896].Ninki';
COMMENT ON COLUMN o6.total_hyosu_sanrentan IS 'TotalHyosuSanrentan';
