-- jv.br: BR (生産者マスタ — breeder master) records.
-- One row per breeder (PK = breeder_code).
-- JV-Data 4.9.0.1 §JV_BR_BREEDER (545 bytes).

CREATE TABLE IF NOT EXISTS br (
    record_spec       text     NOT NULL,
    data_kubun        text     NOT NULL,
    make_date         date,

    breeder_code      text     NOT NULL,
    breeder_name_co   text,
    breeder_name      text,
    breeder_name_kana text,
    breeder_name_eng  text,
    address           text,

    -- HonRuikei[2]: SetYear, HonSyokinTotal, FukaSyokin, ChakuKaisu[6]
    hon_ruikei_set_year         text[],     -- 2
    hon_ruikei_honsyokin_total  bigint[],   -- 2
    hon_ruikei_fuka_syokin      bigint[],   -- 2
    hon_ruikei_chaku_kaisu      integer[],  -- 12 (2 years × 6 placements, indexed [year*6 + placement])

    PRIMARY KEY (breeder_code)
);

COMMENT ON COLUMN br.record_spec       IS 'RecordSpec — always "BR"';
COMMENT ON COLUMN br.data_kubun        IS 'DataKubun';
COMMENT ON COLUMN br.make_date         IS 'MakeDate';
COMMENT ON COLUMN br.breeder_code      IS 'BreederCode';
COMMENT ON COLUMN br.breeder_name_co   IS 'BreederName_Co';
COMMENT ON COLUMN br.breeder_name      IS 'BreederName';
COMMENT ON COLUMN br.breeder_name_kana IS 'BreederNameKana';
COMMENT ON COLUMN br.breeder_name_eng  IS 'BreederNameEng';
COMMENT ON COLUMN br.address           IS 'Address';
COMMENT ON COLUMN br.hon_ruikei_set_year         IS 'HonRuikei[2].SetYear';
COMMENT ON COLUMN br.hon_ruikei_honsyokin_total  IS 'HonRuikei[2].HonSyokinTotal';
COMMENT ON COLUMN br.hon_ruikei_fuka_syokin      IS 'HonRuikei[2].FukaSyokin';
COMMENT ON COLUMN br.hon_ruikei_chaku_kaisu      IS 'HonRuikei[2].ChakuKaisu[6] flattened (year*6 + placement)';
