-- jv.ys: YS (開催スケジュール — year schedule). JV-Data §JV_YS_SCHEDULE, 382 bytes.

CREATE TABLE IF NOT EXISTS ys (
    record_spec   text NOT NULL,
    data_kubun    text NOT NULL,
    make_date     date,

    year          text NOT NULL,
    month_day     text NOT NULL,
    jyo_cd        text NOT NULL,
    kaiji         text NOT NULL,
    nichiji       text NOT NULL,

    youbi_cd      text,

    -- JyusyoInfo[3]
    jyusyo_toku_num     text[],
    jyusyo_hondai       text[],
    jyusyo_ryakusyo10   text[],
    jyusyo_ryakusyo6    text[],
    jyusyo_ryakusyo3    text[],
    jyusyo_nkai         text[],
    jyusyo_grade_cd     text[],
    jyusyo_syubetu_cd   text[],
    jyusyo_kigo_cd      text[],
    jyusyo_jyuryo_cd    text[],
    jyusyo_kyori        text[],
    jyusyo_track_cd     text[],

    PRIMARY KEY (year, month_day, jyo_cd, kaiji, nichiji)
);

COMMENT ON COLUMN ys.record_spec       IS 'RecordSpec — always "YS"';
COMMENT ON COLUMN ys.data_kubun        IS 'DataKubun';
COMMENT ON COLUMN ys.make_date         IS 'MakeDate';
COMMENT ON COLUMN ys.year              IS 'Year — RACE_ID2';
COMMENT ON COLUMN ys.month_day         IS 'MonthDay — RACE_ID2';
COMMENT ON COLUMN ys.jyo_cd            IS 'JyoCD — RACE_ID2';
COMMENT ON COLUMN ys.kaiji             IS 'Kaiji — RACE_ID2';
COMMENT ON COLUMN ys.nichiji           IS 'Nichiji — RACE_ID2';
COMMENT ON COLUMN ys.youbi_cd          IS 'YoubiCD';
COMMENT ON COLUMN ys.jyusyo_toku_num   IS 'JyusyoInfo[3].TokuNum';
COMMENT ON COLUMN ys.jyusyo_hondai     IS 'JyusyoInfo[3].Hondai';
COMMENT ON COLUMN ys.jyusyo_ryakusyo10 IS 'JyusyoInfo[3].Ryakusyo10';
COMMENT ON COLUMN ys.jyusyo_ryakusyo6  IS 'JyusyoInfo[3].Ryakusyo6';
COMMENT ON COLUMN ys.jyusyo_ryakusyo3  IS 'JyusyoInfo[3].Ryakusyo3';
COMMENT ON COLUMN ys.jyusyo_nkai       IS 'JyusyoInfo[3].Nkai';
COMMENT ON COLUMN ys.jyusyo_grade_cd   IS 'JyusyoInfo[3].GradeCD';
COMMENT ON COLUMN ys.jyusyo_syubetu_cd IS 'JyusyoInfo[3].SyubetuCD';
COMMENT ON COLUMN ys.jyusyo_kigo_cd    IS 'JyusyoInfo[3].KigoCD';
COMMENT ON COLUMN ys.jyusyo_jyuryo_cd  IS 'JyusyoInfo[3].JyuryoCD';
COMMENT ON COLUMN ys.jyusyo_kyori      IS 'JyusyoInfo[3].Kyori';
COMMENT ON COLUMN ys.jyusyo_track_cd   IS 'JyusyoInfo[3].TrackCD';
