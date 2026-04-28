-- jv.sk: SK (産駒マスタ — progeny master). JV-Data §JV_SK_SANKU, 208 bytes.

CREATE TABLE IF NOT EXISTS sk (
    record_spec       text NOT NULL,
    data_kubun        text NOT NULL,
    make_date         date,

    ketto_num         text NOT NULL,
    birth_date        date,
    sex_cd            text,
    hinsyu_cd         text,
    keiro_cd          text,
    sanku_mochi_kubun text,
    import_year       text,
    breeder_code      text,
    sanchi_name       text,

    hansyoku_num      text[],     -- 14

    PRIMARY KEY (ketto_num)
);

COMMENT ON COLUMN sk.record_spec       IS 'RecordSpec — always "SK"';
COMMENT ON COLUMN sk.data_kubun        IS 'DataKubun';
COMMENT ON COLUMN sk.make_date         IS 'MakeDate';
COMMENT ON COLUMN sk.ketto_num         IS 'KettoNum';
COMMENT ON COLUMN sk.birth_date        IS 'BirthDate';
COMMENT ON COLUMN sk.sex_cd            IS 'SexCD';
COMMENT ON COLUMN sk.hinsyu_cd         IS 'HinsyuCD';
COMMENT ON COLUMN sk.keiro_cd          IS 'KeiroCD';
COMMENT ON COLUMN sk.sanku_mochi_kubun IS 'SankuMochiKubun';
COMMENT ON COLUMN sk.import_year       IS 'ImportYear';
COMMENT ON COLUMN sk.breeder_code      IS 'BreederCode';
COMMENT ON COLUMN sk.sanchi_name       IS 'SanchiName';
COMMENT ON COLUMN sk.hansyoku_num      IS 'HansyokuNum[14] (3-generation pedigree)';
