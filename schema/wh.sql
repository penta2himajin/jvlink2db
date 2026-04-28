-- jv.wh: WH (馬体重 — horse weight). JV-Data §JV_WH_BATAIJYU, 847 bytes.

CREATE TABLE IF NOT EXISTS wh (
    record_spec   text NOT NULL,
    data_kubun    text NOT NULL,
    make_date     date,

    year          text NOT NULL,
    month_day     text NOT NULL,
    jyo_cd        text NOT NULL,
    kaiji         text NOT NULL,
    nichiji       text NOT NULL,
    race_num      text NOT NULL,

    happyo_time   text,

    -- BataijyuInfo[18]
    umaban        text[],
    bamei         text[],
    ba_taijyu     text[],
    zogen_fugo    text[],
    zogen_sa      text[],

    PRIMARY KEY (year, month_day, jyo_cd, kaiji, nichiji, race_num)
);

COMMENT ON COLUMN wh.record_spec IS 'RecordSpec — always "WH"';
COMMENT ON COLUMN wh.data_kubun  IS 'DataKubun';
COMMENT ON COLUMN wh.make_date   IS 'MakeDate';
COMMENT ON COLUMN wh.year        IS 'Year — RACE_ID';
COMMENT ON COLUMN wh.month_day   IS 'MonthDay — RACE_ID';
COMMENT ON COLUMN wh.jyo_cd      IS 'JyoCD — RACE_ID';
COMMENT ON COLUMN wh.kaiji       IS 'Kaiji — RACE_ID';
COMMENT ON COLUMN wh.nichiji     IS 'Nichiji — RACE_ID';
COMMENT ON COLUMN wh.race_num    IS 'RaceNum — RACE_ID';
COMMENT ON COLUMN wh.happyo_time IS 'HappyoTime (MMDDHHMM)';
COMMENT ON COLUMN wh.umaban      IS 'BataijyuInfo[18].Umaban';
COMMENT ON COLUMN wh.bamei       IS 'BataijyuInfo[18].Bamei';
COMMENT ON COLUMN wh.ba_taijyu   IS 'BataijyuInfo[18].BaTaijyu';
COMMENT ON COLUMN wh.zogen_fugo  IS 'BataijyuInfo[18].ZogenFugo';
COMMENT ON COLUMN wh.zogen_sa    IS 'BataijyuInfo[18].ZogenSa';
