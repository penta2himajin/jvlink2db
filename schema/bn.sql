-- jv.bn: BN (馬主マスタ — owner master) records.
-- JV-Data 4.9.0.1 §JV_BN_BANUSI (477 bytes).

CREATE TABLE IF NOT EXISTS bn (
    record_spec       text     NOT NULL,
    data_kubun        text     NOT NULL,
    make_date         date,

    banusi_code       text     NOT NULL,
    banusi_name_co    text,
    banusi_name       text,
    banusi_name_kana  text,
    banusi_name_eng   text,
    fukusyoku         text,

    hon_ruikei_set_year         text[],
    hon_ruikei_honsyokin_total  bigint[],
    hon_ruikei_fuka_syokin      bigint[],
    hon_ruikei_chaku_kaisu      integer[],

    PRIMARY KEY (banusi_code)
);

COMMENT ON COLUMN bn.record_spec      IS 'RecordSpec — always "BN"';
COMMENT ON COLUMN bn.data_kubun       IS 'DataKubun';
COMMENT ON COLUMN bn.make_date        IS 'MakeDate';
COMMENT ON COLUMN bn.banusi_code      IS 'BanusiCode';
COMMENT ON COLUMN bn.banusi_name_co   IS 'BanusiName_Co';
COMMENT ON COLUMN bn.banusi_name      IS 'BanusiName';
COMMENT ON COLUMN bn.banusi_name_kana IS 'BanusiNameKana';
COMMENT ON COLUMN bn.banusi_name_eng  IS 'BanusiNameEng';
COMMENT ON COLUMN bn.fukusyoku        IS 'Fukusyoku';
COMMENT ON COLUMN bn.hon_ruikei_set_year         IS 'HonRuikei[2].SetYear';
COMMENT ON COLUMN bn.hon_ruikei_honsyokin_total  IS 'HonRuikei[2].HonSyokinTotal';
COMMENT ON COLUMN bn.hon_ruikei_fuka_syokin      IS 'HonRuikei[2].FukaSyokin';
COMMENT ON COLUMN bn.hon_ruikei_chaku_kaisu      IS 'HonRuikei[2].ChakuKaisu[6] flattened (year*6 + placement)';
