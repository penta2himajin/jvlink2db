-- jv.ks: KS (騎手マスタ — jockey master) records.
-- Primary key is KisyuCode (5 chars).
-- JV-Data 4.9.0.1 §JV_KS_KISYU (4173 bytes).
-- HonZenRuikei[3] (deeply-nested 1052-byte annual stat blocks at offset
-- 1016-4171) is not yet decoded — to be added in a future PR.

CREATE TABLE IF NOT EXISTS ks (
    record_spec       text     NOT NULL,
    data_kubun        text     NOT NULL,
    make_date         date,

    kisyu_code        text     NOT NULL,
    del_kubun         text,
    issue_date        date,
    del_date          date,
    birth_date        date,
    kisyu_name        text,
    reserved          text,
    kisyu_name_kana   text,
    kisyu_ryakusyo    text,
    kisyu_name_eng    text,
    sex_cd            text,
    sikaku_cd         text,
    minarai_cd        text,
    tozai_cd          text,
    syotai            text,
    chokyosi_code     text,
    chokyosi_ryakusyo text,

    -- HatuKiJyo[2]
    hatu_kijyo_year         text[],
    hatu_kijyo_month_day    text[],
    hatu_kijyo_jyo_cd       text[],
    hatu_kijyo_kaiji        text[],
    hatu_kijyo_nichiji      text[],
    hatu_kijyo_race_num     text[],
    hatu_kijyo_syusso_tosu  text[],
    hatu_kijyo_ketto_num    text[],
    hatu_kijyo_bamei        text[],
    hatu_kijyo_kakutei_jyuni text[],
    hatu_kijyo_ijyo_cd      text[],

    -- HatuSyori[2]
    hatu_syori_year         text[],
    hatu_syori_month_day    text[],
    hatu_syori_jyo_cd       text[],
    hatu_syori_kaiji        text[],
    hatu_syori_nichiji      text[],
    hatu_syori_race_num     text[],
    hatu_syori_syusso_tosu  text[],
    hatu_syori_ketto_num    text[],
    hatu_syori_bamei        text[],

    -- SaikinJyusyo[3]
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

    PRIMARY KEY (kisyu_code)
);

COMMENT ON COLUMN ks.record_spec    IS 'RecordSpec — always "KS"';
COMMENT ON COLUMN ks.data_kubun     IS 'DataKubun';
COMMENT ON COLUMN ks.make_date      IS 'MakeDate';
COMMENT ON COLUMN ks.kisyu_code     IS 'KisyuCode';
COMMENT ON COLUMN ks.del_kubun      IS 'DelKubun';
COMMENT ON COLUMN ks.issue_date     IS 'IssueDate';
COMMENT ON COLUMN ks.del_date       IS 'DelDate';
COMMENT ON COLUMN ks.birth_date     IS 'BirthDate';
COMMENT ON COLUMN ks.kisyu_name     IS 'KisyuName';
COMMENT ON COLUMN ks.reserved       IS 'reserved';
COMMENT ON COLUMN ks.kisyu_name_kana   IS 'KisyuNameKana';
COMMENT ON COLUMN ks.kisyu_ryakusyo    IS 'KisyuRyakusyo';
COMMENT ON COLUMN ks.kisyu_name_eng    IS 'KisyuNameEng';
COMMENT ON COLUMN ks.sex_cd            IS 'SexCD';
COMMENT ON COLUMN ks.sikaku_cd         IS 'SikakuCD';
COMMENT ON COLUMN ks.minarai_cd        IS 'MinaraiCD';
COMMENT ON COLUMN ks.tozai_cd          IS 'TozaiCD';
COMMENT ON COLUMN ks.syotai            IS 'Syotai';
COMMENT ON COLUMN ks.chokyosi_code     IS 'ChokyosiCode';
COMMENT ON COLUMN ks.chokyosi_ryakusyo IS 'ChokyosiRyakusyo';
COMMENT ON COLUMN ks.hatu_kijyo_year         IS 'HatuKiJyo[2].Hatukijyoid.Year';
COMMENT ON COLUMN ks.hatu_kijyo_month_day    IS 'HatuKiJyo[2].Hatukijyoid.MonthDay';
COMMENT ON COLUMN ks.hatu_kijyo_jyo_cd       IS 'HatuKiJyo[2].Hatukijyoid.JyoCD';
COMMENT ON COLUMN ks.hatu_kijyo_kaiji        IS 'HatuKiJyo[2].Hatukijyoid.Kaiji';
COMMENT ON COLUMN ks.hatu_kijyo_nichiji      IS 'HatuKiJyo[2].Hatukijyoid.Nichiji';
COMMENT ON COLUMN ks.hatu_kijyo_race_num     IS 'HatuKiJyo[2].Hatukijyoid.RaceNum';
COMMENT ON COLUMN ks.hatu_kijyo_syusso_tosu  IS 'HatuKiJyo[2].SyussoTosu';
COMMENT ON COLUMN ks.hatu_kijyo_ketto_num    IS 'HatuKiJyo[2].KettoNum';
COMMENT ON COLUMN ks.hatu_kijyo_bamei        IS 'HatuKiJyo[2].Bamei';
COMMENT ON COLUMN ks.hatu_kijyo_kakutei_jyuni IS 'HatuKiJyo[2].KakuteiJyuni';
COMMENT ON COLUMN ks.hatu_kijyo_ijyo_cd      IS 'HatuKiJyo[2].IJyoCD';
COMMENT ON COLUMN ks.hatu_syori_year         IS 'HatuSyori[2].Hatukijyoid.Year';
COMMENT ON COLUMN ks.hatu_syori_month_day    IS 'HatuSyori[2].Hatukijyoid.MonthDay';
COMMENT ON COLUMN ks.hatu_syori_jyo_cd       IS 'HatuSyori[2].Hatukijyoid.JyoCD';
COMMENT ON COLUMN ks.hatu_syori_kaiji        IS 'HatuSyori[2].Hatukijyoid.Kaiji';
COMMENT ON COLUMN ks.hatu_syori_nichiji      IS 'HatuSyori[2].Hatukijyoid.Nichiji';
COMMENT ON COLUMN ks.hatu_syori_race_num     IS 'HatuSyori[2].Hatukijyoid.RaceNum';
COMMENT ON COLUMN ks.hatu_syori_syusso_tosu  IS 'HatuSyori[2].SyussoTosu';
COMMENT ON COLUMN ks.hatu_syori_ketto_num    IS 'HatuSyori[2].KettoNum';
COMMENT ON COLUMN ks.hatu_syori_bamei        IS 'HatuSyori[2].Bamei';
COMMENT ON COLUMN ks.saikin_jyusyo_year         IS 'SaikinJyusyo[3].SaikinJyusyoid.Year';
COMMENT ON COLUMN ks.saikin_jyusyo_month_day    IS 'SaikinJyusyo[3].SaikinJyusyoid.MonthDay';
COMMENT ON COLUMN ks.saikin_jyusyo_jyo_cd       IS 'SaikinJyusyo[3].SaikinJyusyoid.JyoCD';
COMMENT ON COLUMN ks.saikin_jyusyo_kaiji        IS 'SaikinJyusyo[3].SaikinJyusyoid.Kaiji';
COMMENT ON COLUMN ks.saikin_jyusyo_nichiji      IS 'SaikinJyusyo[3].SaikinJyusyoid.Nichiji';
COMMENT ON COLUMN ks.saikin_jyusyo_race_num     IS 'SaikinJyusyo[3].SaikinJyusyoid.RaceNum';
COMMENT ON COLUMN ks.saikin_jyusyo_hondai       IS 'SaikinJyusyo[3].Hondai';
COMMENT ON COLUMN ks.saikin_jyusyo_ryakusyo10   IS 'SaikinJyusyo[3].Ryakusyo10';
COMMENT ON COLUMN ks.saikin_jyusyo_ryakusyo6    IS 'SaikinJyusyo[3].Ryakusyo6';
COMMENT ON COLUMN ks.saikin_jyusyo_ryakusyo3    IS 'SaikinJyusyo[3].Ryakusyo3';
COMMENT ON COLUMN ks.saikin_jyusyo_grade_cd     IS 'SaikinJyusyo[3].GradeCD';
COMMENT ON COLUMN ks.saikin_jyusyo_syusso_tosu  IS 'SaikinJyusyo[3].SyussoTosu';
COMMENT ON COLUMN ks.saikin_jyusyo_ketto_num    IS 'SaikinJyusyo[3].KettoNum';
COMMENT ON COLUMN ks.saikin_jyusyo_bamei        IS 'SaikinJyusyo[3].Bamei';
