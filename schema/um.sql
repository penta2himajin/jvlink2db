-- jv.um: UM (競走馬マスタ — horse master) records.
-- Primary key is KettoNum (10-char pedigree registration number).
-- JV-Data 4.9.0.1 §JV_UM_UMA (1609 bytes).

CREATE TABLE IF NOT EXISTS um (
    record_spec       text     NOT NULL,
    data_kubun        text     NOT NULL,
    make_date         date,

    ketto_num         text     NOT NULL,
    del_kubun         text,
    reg_date          date,
    del_date          date,
    birth_date        date,
    bamei             text,
    bamei_kana        text,
    bamei_eng         text,
    zaikyu_flag       text,
    reserved          text,
    uma_kigo_cd       text,
    sex_cd            text,
    hinsyu_cd         text,
    keiro_cd          text,

    ketto_hansyoku_num text[],     -- 14 (3代血統)
    ketto_bamei        text[],     -- 14

    tozai_cd          text,
    chokyosi_code     text,
    chokyosi_ryakusyo text,
    syotai            text,
    breeder_code      text,
    breeder_name      text,
    sanchi_name       text,
    banusi_code       text,
    banusi_name       text,

    ruikei_honsyo_heiti     bigint,
    ruikei_honsyo_syogai    bigint,
    ruikei_fuka_heichi      bigint,
    ruikei_fuka_syogai      bigint,
    ruikei_syutoku_heichi   bigint,
    ruikei_syutoku_syogai   bigint,

    chaku_sogo            integer[],  -- 6 (1着〜5着外 + その他)
    chaku_chuo            integer[],  -- 6
    chaku_kaisu_ba        integer[],  -- 42 (7 ba × 6 placements, indexed [ba*6 + placement])
    chaku_kaisu_jyotai    integer[],  -- 72 (12 jyotai × 6 placements)
    chaku_kaisu_kyori     integer[],  -- 36 (6 kyori × 6 placements)

    kyakusitu             text[],     -- 4
    race_count            smallint,

    PRIMARY KEY (ketto_num)
);

COMMENT ON COLUMN um.record_spec       IS 'RecordSpec — always "UM"';
COMMENT ON COLUMN um.data_kubun        IS 'DataKubun';
COMMENT ON COLUMN um.make_date         IS 'MakeDate';
COMMENT ON COLUMN um.ketto_num         IS 'KettoNum';
COMMENT ON COLUMN um.del_kubun         IS 'DelKubun';
COMMENT ON COLUMN um.reg_date          IS 'RegDate';
COMMENT ON COLUMN um.del_date          IS 'DelDate';
COMMENT ON COLUMN um.birth_date        IS 'BirthDate';
COMMENT ON COLUMN um.bamei             IS 'Bamei';
COMMENT ON COLUMN um.bamei_kana        IS 'BameiKana';
COMMENT ON COLUMN um.bamei_eng         IS 'BameiEng';
COMMENT ON COLUMN um.zaikyu_flag       IS 'ZaikyuFlag';
COMMENT ON COLUMN um.reserved          IS 'Reserved';
COMMENT ON COLUMN um.uma_kigo_cd       IS 'UmaKigoCD';
COMMENT ON COLUMN um.sex_cd            IS 'SexCD';
COMMENT ON COLUMN um.hinsyu_cd         IS 'HinsyuCD';
COMMENT ON COLUMN um.keiro_cd          IS 'KeiroCD';
COMMENT ON COLUMN um.ketto_hansyoku_num IS 'Ketto3Info[14].HansyokuNum';
COMMENT ON COLUMN um.ketto_bamei        IS 'Ketto3Info[14].Bamei';
COMMENT ON COLUMN um.tozai_cd          IS 'TozaiCD';
COMMENT ON COLUMN um.chokyosi_code     IS 'ChokyosiCode';
COMMENT ON COLUMN um.chokyosi_ryakusyo IS 'ChokyosiRyakusyo';
COMMENT ON COLUMN um.syotai            IS 'Syotai';
COMMENT ON COLUMN um.breeder_code      IS 'BreederCode';
COMMENT ON COLUMN um.breeder_name      IS 'BreederName';
COMMENT ON COLUMN um.sanchi_name       IS 'SanchiName';
COMMENT ON COLUMN um.banusi_code       IS 'BanusiCode';
COMMENT ON COLUMN um.banusi_name       IS 'BanusiName';
COMMENT ON COLUMN um.ruikei_honsyo_heiti     IS 'RuikeiHonsyoHeiti';
COMMENT ON COLUMN um.ruikei_honsyo_syogai    IS 'RuikeiHonsyoSyogai';
COMMENT ON COLUMN um.ruikei_fuka_heichi      IS 'RuikeiFukaHeichi';
COMMENT ON COLUMN um.ruikei_fuka_syogai      IS 'RuikeiFukaSyogai';
COMMENT ON COLUMN um.ruikei_syutoku_heichi   IS 'RuikeiSyutokuHeichi';
COMMENT ON COLUMN um.ruikei_syutoku_syogai   IS 'RuikeiSyutokuSyogai';
COMMENT ON COLUMN um.chaku_sogo              IS 'ChakuSogo[6]';
COMMENT ON COLUMN um.chaku_chuo              IS 'ChakuChuo[6]';
COMMENT ON COLUMN um.chaku_kaisu_ba          IS 'ChakuKaisuBa[7]×ChakuKaisu[6] flattened';
COMMENT ON COLUMN um.chaku_kaisu_jyotai      IS 'ChakuKaisuJyotai[12]×ChakuKaisu[6] flattened';
COMMENT ON COLUMN um.chaku_kaisu_kyori       IS 'ChakuKaisuKyori[6]×ChakuKaisu[6] flattened';
COMMENT ON COLUMN um.kyakusitu               IS 'Kyakusitu[4]';
COMMENT ON COLUMN um.race_count              IS 'RaceCount';
