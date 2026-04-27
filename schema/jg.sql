-- jv.jg: JG (競走馬除外情報 — withdrawal info) records.
-- One row per (race, withdrawn horse). JV-Data 4.9.0.1 §JV_JG_JOGAIBA (80 bytes).

CREATE TABLE IF NOT EXISTS jg (
    record_spec       text     NOT NULL,
    data_kubun        text     NOT NULL,
    make_date         date,

    year              text     NOT NULL,
    month_day         text     NOT NULL,
    jyo_cd            text     NOT NULL,
    kaiji             text     NOT NULL,
    nichiji           text     NOT NULL,
    race_num          text     NOT NULL,

    ketto_num         text     NOT NULL,
    bamei             text,
    shutsuba_tohyo_jun smallint,
    shusso_kubun      text,
    jogai_jotai_kubun text,

    PRIMARY KEY (year, month_day, jyo_cd, kaiji, nichiji, race_num, ketto_num)
);

COMMENT ON COLUMN jg.record_spec       IS 'RecordSpec — always "JG"';
COMMENT ON COLUMN jg.data_kubun        IS 'DataKubun';
COMMENT ON COLUMN jg.make_date         IS 'MakeDate';
COMMENT ON COLUMN jg.year              IS 'Year — RACE_ID';
COMMENT ON COLUMN jg.month_day         IS 'MonthDay — RACE_ID';
COMMENT ON COLUMN jg.jyo_cd            IS 'JyoCD — RACE_ID';
COMMENT ON COLUMN jg.kaiji             IS 'Kaiji — RACE_ID';
COMMENT ON COLUMN jg.nichiji           IS 'Nichiji — RACE_ID';
COMMENT ON COLUMN jg.race_num          IS 'RaceNum — RACE_ID';
COMMENT ON COLUMN jg.ketto_num         IS 'KettoNum';
COMMENT ON COLUMN jg.bamei             IS 'Bamei';
COMMENT ON COLUMN jg.shutsuba_tohyo_jun IS 'ShutsubaTohyoJun';
COMMENT ON COLUMN jg.shusso_kubun      IS 'ShussoKubun';
COMMENT ON COLUMN jg.jogai_jotai_kubun IS 'JogaiJotaiKubun';
