-- jv.h1: H1 (票数 全掛式) records.
-- One row per race. JV-Data 4.9.0.1 §JV_H1_HYOSU_ZENKAKE (28955 bytes).

CREATE TABLE IF NOT EXISTS h1 (
    record_spec      text     NOT NULL,
    data_kubun       text     NOT NULL,
    make_date        date,

    year             text     NOT NULL,
    month_day        text     NOT NULL,
    jyo_cd           text     NOT NULL,
    kaiji            text     NOT NULL,
    nichiji          text     NOT NULL,
    race_num         text     NOT NULL,

    toroku_tosu      smallint,
    syusso_tosu      smallint,

    hatubai_flag        text[],     -- 7
    fuku_chaku_barai_key text,
    henkan_uma          text[],     -- 28
    henkan_waku         text[],     -- 8
    henkan_do_waku      text[],     -- 8

    tansyo_umaban       text[],     -- 28
    tansyo_hyo          bigint[],
    tansyo_ninki        smallint[],

    fukusyo_umaban      text[],     -- 28
    fukusyo_hyo         bigint[],
    fukusyo_ninki       smallint[],

    wakuren_kumi        text[],     -- 36
    wakuren_hyo         bigint[],
    wakuren_ninki       smallint[],

    umaren_kumi         text[],     -- 153
    umaren_hyo          bigint[],
    umaren_ninki        smallint[],

    wide_kumi           text[],     -- 153
    wide_hyo            bigint[],
    wide_ninki          smallint[],

    umatan_kumi         text[],     -- 306
    umatan_hyo          bigint[],
    umatan_ninki        smallint[],

    sanrenpuku_kumi     text[],     -- 816
    sanrenpuku_hyo      bigint[],
    sanrenpuku_ninki    smallint[],

    hyo_total           bigint[],   -- 14

    PRIMARY KEY (year, month_day, jyo_cd, kaiji, nichiji, race_num)
);

COMMENT ON COLUMN h1.record_spec IS 'RecordSpec — always "H1"';
COMMENT ON COLUMN h1.data_kubun  IS 'DataKubun';
COMMENT ON COLUMN h1.make_date   IS 'MakeDate';
COMMENT ON COLUMN h1.year        IS 'Year — RACE_ID';
COMMENT ON COLUMN h1.month_day   IS 'MonthDay — RACE_ID';
COMMENT ON COLUMN h1.jyo_cd      IS 'JyoCD — RACE_ID';
COMMENT ON COLUMN h1.kaiji       IS 'Kaiji — RACE_ID';
COMMENT ON COLUMN h1.nichiji     IS 'Nichiji — RACE_ID';
COMMENT ON COLUMN h1.race_num    IS 'RaceNum — RACE_ID';
COMMENT ON COLUMN h1.toroku_tosu IS 'TorokuTosu';
COMMENT ON COLUMN h1.syusso_tosu IS 'SyussoTosu';
COMMENT ON COLUMN h1.hatubai_flag         IS 'HatubaiFlag[7]';
COMMENT ON COLUMN h1.fuku_chaku_barai_key IS 'FukuChakuBaraiKey';
COMMENT ON COLUMN h1.henkan_uma           IS 'HenkanUma[28]';
COMMENT ON COLUMN h1.henkan_waku          IS 'HenkanWaku[8]';
COMMENT ON COLUMN h1.henkan_do_waku       IS 'HenkanDoWaku[8]';
COMMENT ON COLUMN h1.tansyo_umaban        IS 'HyoTansyo[28].Umaban';
COMMENT ON COLUMN h1.tansyo_hyo           IS 'HyoTansyo[28].Hyo';
COMMENT ON COLUMN h1.tansyo_ninki         IS 'HyoTansyo[28].Ninki';
COMMENT ON COLUMN h1.fukusyo_umaban       IS 'HyoFukusyo[28].Umaban';
COMMENT ON COLUMN h1.fukusyo_hyo          IS 'HyoFukusyo[28].Hyo';
COMMENT ON COLUMN h1.fukusyo_ninki        IS 'HyoFukusyo[28].Ninki';
COMMENT ON COLUMN h1.wakuren_kumi         IS 'HyoWakuren[36].Kumi';
COMMENT ON COLUMN h1.wakuren_hyo          IS 'HyoWakuren[36].Hyo';
COMMENT ON COLUMN h1.wakuren_ninki        IS 'HyoWakuren[36].Ninki';
COMMENT ON COLUMN h1.umaren_kumi          IS 'HyoUmaren[153].Kumi';
COMMENT ON COLUMN h1.umaren_hyo           IS 'HyoUmaren[153].Hyo';
COMMENT ON COLUMN h1.umaren_ninki         IS 'HyoUmaren[153].Ninki';
COMMENT ON COLUMN h1.wide_kumi            IS 'HyoWide[153].Kumi';
COMMENT ON COLUMN h1.wide_hyo             IS 'HyoWide[153].Hyo';
COMMENT ON COLUMN h1.wide_ninki           IS 'HyoWide[153].Ninki';
COMMENT ON COLUMN h1.umatan_kumi          IS 'HyoUmatan[306].Kumi';
COMMENT ON COLUMN h1.umatan_hyo           IS 'HyoUmatan[306].Hyo';
COMMENT ON COLUMN h1.umatan_ninki         IS 'HyoUmatan[306].Ninki';
COMMENT ON COLUMN h1.sanrenpuku_kumi      IS 'HyoSanrenpuku[816].Kumi';
COMMENT ON COLUMN h1.sanrenpuku_hyo       IS 'HyoSanrenpuku[816].Hyo';
COMMENT ON COLUMN h1.sanrenpuku_ninki     IS 'HyoSanrenpuku[816].Ninki';
COMMENT ON COLUMN h1.hyo_total            IS 'HyoTotal[14]';
