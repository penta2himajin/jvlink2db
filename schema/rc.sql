-- jv.rc: RC (コースレコードマスタ — course-record master) records.
-- One row per (rec_info_kubun, race composite). PK chosen so that
-- multiple record categories for the same race coexist.
-- JV-Data 4.9.0.1 §JV_RC_RECORD (501 bytes).

CREATE TABLE IF NOT EXISTS rc (
    record_spec      text     NOT NULL,
    data_kubun       text     NOT NULL,
    make_date        date,

    rec_info_kubun   text     NOT NULL,

    year             text     NOT NULL,
    month_day        text     NOT NULL,
    jyo_cd           text     NOT NULL,
    kaiji            text     NOT NULL,
    nichiji          text     NOT NULL,
    race_num         text     NOT NULL,

    toku_num         text,
    hondai           text,
    grade_cd         text,
    syubetu_cd       text,
    kyori            smallint,
    track_cd         text,
    rec_kubun        text,
    rec_time         smallint,

    tenko_cd         text,
    siba_baba_cd     text,
    dirt_baba_cd     text,

    rec_uma_ketto_num         text[],     -- 3 entries
    rec_uma_bamei             text[],
    rec_uma_uma_kigo_cd       text[],
    rec_uma_sex_cd            text[],
    rec_uma_chokyosi_code     text[],
    rec_uma_chokyosi_name     text[],
    rec_uma_futan             smallint[],
    rec_uma_kisyu_code        text[],
    rec_uma_kisyu_name        text[],

    PRIMARY KEY (rec_info_kubun, year, month_day, jyo_cd, kaiji, nichiji, race_num)
);

COMMENT ON COLUMN rc.record_spec    IS 'RecordSpec — always "RC"';
COMMENT ON COLUMN rc.data_kubun     IS 'DataKubun';
COMMENT ON COLUMN rc.make_date      IS 'MakeDate';
COMMENT ON COLUMN rc.rec_info_kubun IS 'RecInfoKubun';
COMMENT ON COLUMN rc.year           IS 'Year — RACE_ID';
COMMENT ON COLUMN rc.month_day      IS 'MonthDay — RACE_ID';
COMMENT ON COLUMN rc.jyo_cd         IS 'JyoCD — RACE_ID';
COMMENT ON COLUMN rc.kaiji          IS 'Kaiji — RACE_ID';
COMMENT ON COLUMN rc.nichiji        IS 'Nichiji — RACE_ID';
COMMENT ON COLUMN rc.race_num       IS 'RaceNum — RACE_ID';
COMMENT ON COLUMN rc.toku_num       IS 'TokuNum';
COMMENT ON COLUMN rc.hondai         IS 'Hondai';
COMMENT ON COLUMN rc.grade_cd       IS 'GradeCD';
COMMENT ON COLUMN rc.syubetu_cd     IS 'SyubetuCD';
COMMENT ON COLUMN rc.kyori          IS 'Kyori';
COMMENT ON COLUMN rc.track_cd       IS 'TrackCD';
COMMENT ON COLUMN rc.rec_kubun      IS 'RecKubun';
COMMENT ON COLUMN rc.rec_time       IS 'RecTime';
COMMENT ON COLUMN rc.tenko_cd       IS 'TenkoBaba.TenkoCD';
COMMENT ON COLUMN rc.siba_baba_cd   IS 'TenkoBaba.SibaBabaCD';
COMMENT ON COLUMN rc.dirt_baba_cd   IS 'TenkoBaba.DirtBabaCD';
COMMENT ON COLUMN rc.rec_uma_ketto_num     IS 'RecUmaInfo[3].KettoNum';
COMMENT ON COLUMN rc.rec_uma_bamei         IS 'RecUmaInfo[3].Bamei';
COMMENT ON COLUMN rc.rec_uma_uma_kigo_cd   IS 'RecUmaInfo[3].UmaKigoCD';
COMMENT ON COLUMN rc.rec_uma_sex_cd        IS 'RecUmaInfo[3].SexCD';
COMMENT ON COLUMN rc.rec_uma_chokyosi_code IS 'RecUmaInfo[3].ChokyosiCode';
COMMENT ON COLUMN rc.rec_uma_chokyosi_name IS 'RecUmaInfo[3].ChokyosiName';
COMMENT ON COLUMN rc.rec_uma_futan         IS 'RecUmaInfo[3].Futan';
COMMENT ON COLUMN rc.rec_uma_kisyu_code    IS 'RecUmaInfo[3].KisyuCode';
COMMENT ON COLUMN rc.rec_uma_kisyu_name    IS 'RecUmaInfo[3].KisyuName';
