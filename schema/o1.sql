-- jv.o1: O1 (単・複・枠連オッズ) records.
-- One row per (race, happyo_tm). JV-Data 4.9.0.1 §JV_O1_ODDS_TANFUKUWAKU (962 bytes).

CREATE TABLE IF NOT EXISTS o1 (
    record_spec       text     NOT NULL,
    data_kubun        text     NOT NULL,
    make_date         date,

    year              text     NOT NULL,
    month_day         text     NOT NULL,
    jyo_cd            text     NOT NULL,
    kaiji             text     NOT NULL,
    nichiji           text     NOT NULL,
    race_num          text     NOT NULL,
    happyo_tm         text     NOT NULL,

    toroku_tosu       smallint,
    syusso_tosu       smallint,

    tansyo_flag       text,
    fukusyo_flag      text,
    wakuren_flag      text,
    fuku_chaku_barai_key text,

    tansyo_umaban     text[],     -- 28
    tansyo_odds       integer[],
    tansyo_ninki      smallint[],

    fukusyo_umaban    text[],     -- 28
    fukusyo_odds_low  integer[],
    fukusyo_odds_high integer[],
    fukusyo_ninki     smallint[],

    wakuren_kumi      text[],     -- 36
    wakuren_odds      integer[],
    wakuren_ninki     smallint[],

    total_hyosu_tansyo  bigint,
    total_hyosu_fukusyo bigint,
    total_hyosu_wakuren bigint,

    PRIMARY KEY (year, month_day, jyo_cd, kaiji, nichiji, race_num, happyo_tm)
);

COMMENT ON COLUMN o1.record_spec       IS 'RecordSpec — always "O1"';
COMMENT ON COLUMN o1.data_kubun        IS 'DataKubun';
COMMENT ON COLUMN o1.make_date         IS 'MakeDate';
COMMENT ON COLUMN o1.year              IS 'Year — RACE_ID';
COMMENT ON COLUMN o1.month_day         IS 'MonthDay — RACE_ID';
COMMENT ON COLUMN o1.jyo_cd            IS 'JyoCD — RACE_ID';
COMMENT ON COLUMN o1.kaiji             IS 'Kaiji — RACE_ID';
COMMENT ON COLUMN o1.nichiji           IS 'Nichiji — RACE_ID';
COMMENT ON COLUMN o1.race_num          IS 'RaceNum — RACE_ID';
COMMENT ON COLUMN o1.happyo_tm         IS 'HappyoTime (MMDDHHMM)';
COMMENT ON COLUMN o1.toroku_tosu       IS 'TorokuTosu';
COMMENT ON COLUMN o1.syusso_tosu       IS 'SyussoTosu';
COMMENT ON COLUMN o1.tansyo_flag       IS 'TansyoFlag';
COMMENT ON COLUMN o1.fukusyo_flag      IS 'FukusyoFlag';
COMMENT ON COLUMN o1.wakuren_flag      IS 'WakurenFlag';
COMMENT ON COLUMN o1.fuku_chaku_barai_key IS 'FukuChakuBaraiKey';
COMMENT ON COLUMN o1.tansyo_umaban     IS 'OddsTansyoInfo[28].Umaban';
COMMENT ON COLUMN o1.tansyo_odds       IS 'OddsTansyoInfo[28].Odds';
COMMENT ON COLUMN o1.tansyo_ninki      IS 'OddsTansyoInfo[28].Ninki';
COMMENT ON COLUMN o1.fukusyo_umaban    IS 'OddsFukusyoInfo[28].Umaban';
COMMENT ON COLUMN o1.fukusyo_odds_low  IS 'OddsFukusyoInfo[28].OddsLow';
COMMENT ON COLUMN o1.fukusyo_odds_high IS 'OddsFukusyoInfo[28].OddsHigh';
COMMENT ON COLUMN o1.fukusyo_ninki     IS 'OddsFukusyoInfo[28].Ninki';
COMMENT ON COLUMN o1.wakuren_kumi      IS 'OddsWakurenInfo[36].Kumi';
COMMENT ON COLUMN o1.wakuren_odds      IS 'OddsWakurenInfo[36].Odds';
COMMENT ON COLUMN o1.wakuren_ninki     IS 'OddsWakurenInfo[36].Ninki';
COMMENT ON COLUMN o1.total_hyosu_tansyo  IS 'TotalHyosuTansyo';
COMMENT ON COLUMN o1.total_hyosu_fukusyo IS 'TotalHyosuFukusyo';
COMMENT ON COLUMN o1.total_hyosu_wakuren IS 'TotalHyosuWakuren';
