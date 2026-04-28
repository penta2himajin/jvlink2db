-- jv.hn: HN (繁殖馬マスタ — brood-mare master). JV-Data §JV_HN_HANSYOKU, 251 bytes.

CREATE TABLE IF NOT EXISTS hn (
    record_spec          text NOT NULL,
    data_kubun           text NOT NULL,
    make_date            date,

    hansyoku_num         text NOT NULL,
    reserved             text,
    ketto_num            text,
    del_kubun            text,
    bamei                text,
    bamei_kana           text,
    bamei_eng            text,
    birth_year           text,
    sex_cd               text,
    hinsyu_cd            text,
    keiro_cd             text,
    hansyoku_mochi_kubun text,
    import_year          text,
    sanchi_name          text,
    hansyoku_f_num       text,
    hansyoku_m_num       text,

    PRIMARY KEY (hansyoku_num)
);

COMMENT ON COLUMN hn.record_spec          IS 'RecordSpec — always "HN"';
COMMENT ON COLUMN hn.data_kubun           IS 'DataKubun';
COMMENT ON COLUMN hn.make_date            IS 'MakeDate';
COMMENT ON COLUMN hn.hansyoku_num         IS 'HansyokuNum';
COMMENT ON COLUMN hn.reserved             IS 'reserved';
COMMENT ON COLUMN hn.ketto_num            IS 'KettoNum';
COMMENT ON COLUMN hn.del_kubun            IS 'DelKubun';
COMMENT ON COLUMN hn.bamei                IS 'Bamei';
COMMENT ON COLUMN hn.bamei_kana           IS 'BameiKana';
COMMENT ON COLUMN hn.bamei_eng            IS 'BameiEng';
COMMENT ON COLUMN hn.birth_year           IS 'BirthYear';
COMMENT ON COLUMN hn.sex_cd               IS 'SexCD';
COMMENT ON COLUMN hn.hinsyu_cd            IS 'HinsyuCD';
COMMENT ON COLUMN hn.keiro_cd             IS 'KeiroCD';
COMMENT ON COLUMN hn.hansyoku_mochi_kubun IS 'HansyokuMochiKubun';
COMMENT ON COLUMN hn.import_year          IS 'ImportYear';
COMMENT ON COLUMN hn.sanchi_name          IS 'SanchiName';
COMMENT ON COLUMN hn.hansyoku_f_num       IS 'HansyokuFNum';
COMMENT ON COLUMN hn.hansyoku_m_num       IS 'HansyokuMNum';
