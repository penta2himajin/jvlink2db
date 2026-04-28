-- jv.ck: CK (出走別着度数 — race-entry placement counts).
-- JV-Data §JV_CK_CHAKU, 6870 bytes.
-- Top-level scalars and identifying codes for the per-race subjects (horse,
-- jockey, trainer, owner, breeder). Deeply-nested CHAKUKAISU3/4/5 placement
-- arrays and the 1220-byte HonRuikei[2] annual stat blocks are deferred to a
-- follow-up PR (matches the KS/CH precedent).

CREATE TABLE IF NOT EXISTS ck (
    record_spec        text NOT NULL,
    data_kubun         text NOT NULL,
    make_date          date,

    year               text NOT NULL,
    month_day          text NOT NULL,
    jyo_cd             text NOT NULL,
    kaiji              text NOT NULL,
    nichiji            text NOT NULL,
    race_num           text NOT NULL,

    -- UmaChaku
    ketto_num                  text NOT NULL,
    bamei                      text,
    ruikei_honsyo_heiti        text,
    ruikei_honsyo_syogai       text,
    ruikei_fuka_heichi         text,
    ruikei_fuka_syogai         text,
    ruikei_syutoku_heichi      text,
    ruikei_syutoku_syogai      text,
    kyakusitu                  text[],
    race_count                 text,

    -- KisyuChaku
    kisyu_code         text,
    kisyu_name         text,

    -- ChokyoChaku
    chokyosi_code      text,
    chokyosi_name      text,

    -- BanusiChaku
    banusi_code        text,
    banusi_name_co     text,
    banusi_name        text,

    -- BreederChaku
    breeder_code       text,
    breeder_name_co    text,
    breeder_name       text,

    PRIMARY KEY (year, month_day, jyo_cd, kaiji, nichiji, race_num, ketto_num)
);

COMMENT ON COLUMN ck.record_spec           IS 'RecordSpec — always "CK"';
COMMENT ON COLUMN ck.data_kubun            IS 'DataKubun';
COMMENT ON COLUMN ck.make_date             IS 'MakeDate';
COMMENT ON COLUMN ck.year                  IS 'Year — RACE_ID';
COMMENT ON COLUMN ck.month_day             IS 'MonthDay — RACE_ID';
COMMENT ON COLUMN ck.jyo_cd                IS 'JyoCD — RACE_ID';
COMMENT ON COLUMN ck.kaiji                 IS 'Kaiji — RACE_ID';
COMMENT ON COLUMN ck.nichiji               IS 'Nichiji — RACE_ID';
COMMENT ON COLUMN ck.race_num              IS 'RaceNum — RACE_ID';
COMMENT ON COLUMN ck.ketto_num             IS 'UmaChaku.KettoNum';
COMMENT ON COLUMN ck.bamei                 IS 'UmaChaku.Bamei';
COMMENT ON COLUMN ck.ruikei_honsyo_heiti   IS 'UmaChaku.RuikeiHonsyoHeiti';
COMMENT ON COLUMN ck.ruikei_honsyo_syogai  IS 'UmaChaku.RuikeiHonsyoSyogai';
COMMENT ON COLUMN ck.ruikei_fuka_heichi    IS 'UmaChaku.RuikeiFukaHeichi';
COMMENT ON COLUMN ck.ruikei_fuka_syogai    IS 'UmaChaku.RuikeiFukaSyogai';
COMMENT ON COLUMN ck.ruikei_syutoku_heichi IS 'UmaChaku.RuikeiSyutokuHeichi';
COMMENT ON COLUMN ck.ruikei_syutoku_syogai IS 'UmaChaku.RuikeiSyutokuSyogai';
COMMENT ON COLUMN ck.kyakusitu             IS 'UmaChaku.Kyakusitu[4]';
COMMENT ON COLUMN ck.race_count            IS 'UmaChaku.RaceCount';
COMMENT ON COLUMN ck.kisyu_code            IS 'KisyuChaku.KisyuCode';
COMMENT ON COLUMN ck.kisyu_name            IS 'KisyuChaku.KisyuName';
COMMENT ON COLUMN ck.chokyosi_code         IS 'ChokyoChaku.ChokyosiCode';
COMMENT ON COLUMN ck.chokyosi_name         IS 'ChokyoChaku.ChokyosiName';
COMMENT ON COLUMN ck.banusi_code           IS 'BanusiChaku.BanusiCode';
COMMENT ON COLUMN ck.banusi_name_co        IS 'BanusiChaku.BanusiName_Co';
COMMENT ON COLUMN ck.banusi_name           IS 'BanusiChaku.BanusiName';
COMMENT ON COLUMN ck.breeder_code          IS 'BreederChaku.BreederCode';
COMMENT ON COLUMN ck.breeder_name_co       IS 'BreederChaku.BreederName_Co';
COMMENT ON COLUMN ck.breeder_name          IS 'BreederChaku.BreederName';
