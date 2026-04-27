-- jv.ch: CH (調教師マスタ — trainer master) records.
-- Primary key is ChokyosiCode.
-- JV-Data 4.9.0.1 §JV_CH_CHOKYOSI (3862 bytes).
-- HonZenRuikei[3] (deeply-nested 1052-byte annual stat blocks at offset
-- 705-3860) is not yet decoded — to be added in a future PR.

CREATE TABLE IF NOT EXISTS ch (
    record_spec        text     NOT NULL,
    data_kubun         text     NOT NULL,
    make_date          date,

    chokyosi_code      text     NOT NULL,
    del_kubun          text,
    issue_date         date,
    del_date           date,
    birth_date         date,
    chokyosi_name      text,
    chokyosi_name_kana text,
    chokyosi_ryakusyo  text,
    chokyosi_name_eng  text,
    sex_cd             text,
    tozai_cd           text,
    syotai             text,

    saikin_jyusyo_year         text[],
    saikin_jyusyo_month_day    text[],
    saikin_jyusyo_jyo_cd       text[],
    saikin_jyusyo_kaiji        text[],
    saikin_jyusyo_nichiji      text[],
    saikin_jyusyo_race_num     text[],
    saikin_jyusyo_hondai       text[],
    saikin_jyusyo_ryakusyo10   text[],
    saikin_jyusyo_ryakusyo6    text[],
    saikin_jyusyo_ryakusyo3    text[],
    saikin_jyusyo_grade_cd     text[],
    saikin_jyusyo_syusso_tosu  text[],
    saikin_jyusyo_ketto_num    text[],
    saikin_jyusyo_bamei        text[],

    PRIMARY KEY (chokyosi_code)
);

COMMENT ON COLUMN ch.record_spec        IS 'RecordSpec — always "CH"';
COMMENT ON COLUMN ch.data_kubun         IS 'DataKubun';
COMMENT ON COLUMN ch.make_date          IS 'MakeDate';
COMMENT ON COLUMN ch.chokyosi_code      IS 'ChokyosiCode';
COMMENT ON COLUMN ch.del_kubun          IS 'DelKubun';
COMMENT ON COLUMN ch.issue_date         IS 'IssueDate';
COMMENT ON COLUMN ch.del_date           IS 'DelDate';
COMMENT ON COLUMN ch.birth_date         IS 'BirthDate';
COMMENT ON COLUMN ch.chokyosi_name      IS 'ChokyosiName';
COMMENT ON COLUMN ch.chokyosi_name_kana IS 'ChokyosiNameKana';
COMMENT ON COLUMN ch.chokyosi_ryakusyo  IS 'ChokyosiRyakusyo';
COMMENT ON COLUMN ch.chokyosi_name_eng  IS 'ChokyosiNameEng';
COMMENT ON COLUMN ch.sex_cd             IS 'SexCD';
COMMENT ON COLUMN ch.tozai_cd           IS 'TozaiCD';
COMMENT ON COLUMN ch.syotai             IS 'Syotai';
COMMENT ON COLUMN ch.saikin_jyusyo_year         IS 'SaikinJyusyo[3].SaikinJyusyoid.Year';
COMMENT ON COLUMN ch.saikin_jyusyo_month_day    IS 'SaikinJyusyo[3].SaikinJyusyoid.MonthDay';
COMMENT ON COLUMN ch.saikin_jyusyo_jyo_cd       IS 'SaikinJyusyo[3].SaikinJyusyoid.JyoCD';
COMMENT ON COLUMN ch.saikin_jyusyo_kaiji        IS 'SaikinJyusyo[3].SaikinJyusyoid.Kaiji';
COMMENT ON COLUMN ch.saikin_jyusyo_nichiji      IS 'SaikinJyusyo[3].SaikinJyusyoid.Nichiji';
COMMENT ON COLUMN ch.saikin_jyusyo_race_num     IS 'SaikinJyusyo[3].SaikinJyusyoid.RaceNum';
COMMENT ON COLUMN ch.saikin_jyusyo_hondai       IS 'SaikinJyusyo[3].Hondai';
COMMENT ON COLUMN ch.saikin_jyusyo_ryakusyo10   IS 'SaikinJyusyo[3].Ryakusyo10';
COMMENT ON COLUMN ch.saikin_jyusyo_ryakusyo6    IS 'SaikinJyusyo[3].Ryakusyo6';
COMMENT ON COLUMN ch.saikin_jyusyo_ryakusyo3    IS 'SaikinJyusyo[3].Ryakusyo3';
COMMENT ON COLUMN ch.saikin_jyusyo_grade_cd     IS 'SaikinJyusyo[3].GradeCD';
COMMENT ON COLUMN ch.saikin_jyusyo_syusso_tosu  IS 'SaikinJyusyo[3].SyussoTosu';
COMMENT ON COLUMN ch.saikin_jyusyo_ketto_num    IS 'SaikinJyusyo[3].KettoNum';
COMMENT ON COLUMN ch.saikin_jyusyo_bamei        IS 'SaikinJyusyo[3].Bamei';
