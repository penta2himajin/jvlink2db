-- jv.tk: TK (特別登録馬 — special-race entries). JV-Data §JV_TK_TOKUUMA, 21657 bytes.

CREATE TABLE IF NOT EXISTS tk (
    record_spec       text NOT NULL,
    data_kubun        text NOT NULL,
    make_date         date,

    year              text NOT NULL,
    month_day         text NOT NULL,
    jyo_cd            text NOT NULL,
    kaiji             text NOT NULL,
    nichiji           text NOT NULL,
    race_num          text NOT NULL,

    -- RACE_INFO
    youbi_cd          text,
    toku_num          text,
    hondai            text,
    fukudai           text,
    kakko             text,
    hondai_eng        text,
    fukudai_eng       text,
    kakko_eng         text,
    ryakusyo10        text,
    ryakusyo6         text,
    ryakusyo3         text,
    kubun             text,
    nkai              text,

    grade_cd          text,

    -- RACE_JYOKEN
    syubetu_cd        text,
    kigo_cd           text,
    jyuryo_cd         text,
    jyoken_cd         text[],

    kyori             text,
    track_cd          text,
    course_kubun_cd   text,
    handi_date        date,
    toroku_tosu       text,

    -- TokuUmaInfo[300]
    toku_num_seq      text[],
    ketto_num         text[],
    bamei             text[],
    uma_kigo_cd       text[],
    sex_cd            text[],
    tozai_cd          text[],
    chokyosi_code     text[],
    chokyosi_ryakusyo text[],
    futan             text[],
    koryu             text[],

    PRIMARY KEY (year, month_day, jyo_cd, kaiji, nichiji, race_num)
);

COMMENT ON COLUMN tk.record_spec       IS 'RecordSpec — always "TK"';
COMMENT ON COLUMN tk.data_kubun        IS 'DataKubun';
COMMENT ON COLUMN tk.make_date         IS 'MakeDate';
COMMENT ON COLUMN tk.year              IS 'Year — RACE_ID';
COMMENT ON COLUMN tk.month_day         IS 'MonthDay — RACE_ID';
COMMENT ON COLUMN tk.jyo_cd            IS 'JyoCD — RACE_ID';
COMMENT ON COLUMN tk.kaiji             IS 'Kaiji — RACE_ID';
COMMENT ON COLUMN tk.nichiji           IS 'Nichiji — RACE_ID';
COMMENT ON COLUMN tk.race_num          IS 'RaceNum — RACE_ID';
COMMENT ON COLUMN tk.youbi_cd          IS 'RaceInfo.YoubiCD';
COMMENT ON COLUMN tk.toku_num          IS 'RaceInfo.TokuNum';
COMMENT ON COLUMN tk.hondai            IS 'RaceInfo.Hondai';
COMMENT ON COLUMN tk.fukudai           IS 'RaceInfo.Fukudai';
COMMENT ON COLUMN tk.kakko             IS 'RaceInfo.Kakko';
COMMENT ON COLUMN tk.hondai_eng        IS 'RaceInfo.HondaiEng';
COMMENT ON COLUMN tk.fukudai_eng       IS 'RaceInfo.FukudaiEng';
COMMENT ON COLUMN tk.kakko_eng         IS 'RaceInfo.KakkoEng';
COMMENT ON COLUMN tk.ryakusyo10        IS 'RaceInfo.Ryakusyo10';
COMMENT ON COLUMN tk.ryakusyo6         IS 'RaceInfo.Ryakusyo6';
COMMENT ON COLUMN tk.ryakusyo3         IS 'RaceInfo.Ryakusyo3';
COMMENT ON COLUMN tk.kubun             IS 'RaceInfo.Kubun';
COMMENT ON COLUMN tk.nkai              IS 'RaceInfo.Nkai';
COMMENT ON COLUMN tk.grade_cd          IS 'GradeCD';
COMMENT ON COLUMN tk.syubetu_cd        IS 'JyokenInfo.SyubetuCD';
COMMENT ON COLUMN tk.kigo_cd           IS 'JyokenInfo.KigoCD';
COMMENT ON COLUMN tk.jyuryo_cd         IS 'JyokenInfo.JyuryoCD';
COMMENT ON COLUMN tk.jyoken_cd         IS 'JyokenInfo.JyokenCD[5]';
COMMENT ON COLUMN tk.kyori             IS 'Kyori';
COMMENT ON COLUMN tk.track_cd          IS 'TrackCD';
COMMENT ON COLUMN tk.course_kubun_cd   IS 'CourseKubunCD';
COMMENT ON COLUMN tk.handi_date        IS 'HandiDate (YYYYMMDD)';
COMMENT ON COLUMN tk.toroku_tosu       IS 'TorokuTosu';
COMMENT ON COLUMN tk.toku_num_seq      IS 'TokuUmaInfo[300].Num';
COMMENT ON COLUMN tk.ketto_num         IS 'TokuUmaInfo[300].KettoNum';
COMMENT ON COLUMN tk.bamei             IS 'TokuUmaInfo[300].Bamei';
COMMENT ON COLUMN tk.uma_kigo_cd       IS 'TokuUmaInfo[300].UmaKigoCD';
COMMENT ON COLUMN tk.sex_cd            IS 'TokuUmaInfo[300].SexCD';
COMMENT ON COLUMN tk.tozai_cd          IS 'TokuUmaInfo[300].TozaiCD';
COMMENT ON COLUMN tk.chokyosi_code     IS 'TokuUmaInfo[300].ChokyosiCode';
COMMENT ON COLUMN tk.chokyosi_ryakusyo IS 'TokuUmaInfo[300].ChokyosiRyakusyo';
COMMENT ON COLUMN tk.futan             IS 'TokuUmaInfo[300].Futan';
COMMENT ON COLUMN tk.koryu             IS 'TokuUmaInfo[300].Koryu';
