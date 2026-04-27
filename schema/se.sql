-- jv.se: SE (per-horse race info) records.
-- One row per (race, umaban). See JV-Data Specification 4.9.0.1
-- §JV_SE_RACE_UMA. Column types per docs/04-database-schema.md.

CREATE TABLE IF NOT EXISTS se (
    record_spec      text     NOT NULL,
    data_kubun       text     NOT NULL,
    make_date        date,

    year             text     NOT NULL,
    month_day        text     NOT NULL,
    jyo_cd           text     NOT NULL,
    kaiji            text     NOT NULL,
    nichiji          text     NOT NULL,
    race_num         text     NOT NULL,
    umaban           text     NOT NULL,

    wakuban                  text,
    ketto_num                text,
    bamei                    text,
    uma_kigo_cd              text,
    sex_cd                   text,
    hinsyu_cd                text,
    keiro_cd                 text,
    barei                    smallint,
    tozai_cd                 text,
    chokyosi_code            text,
    chokyosi_ryakusyo        text,
    banusi_code              text,
    banusi_name              text,
    fukusyoku                text,
    reserved1                text,
    futan                    smallint,
    futan_before             smallint,
    blinker                  text,
    reserved2                text,
    kisyu_code               text,
    kisyu_code_before        text,
    kisyu_ryakusyo           text,
    kisyu_ryakusyo_before    text,
    minarai_cd               text,
    minarai_cd_before        text,
    ba_taijyu                smallint,
    zogen_fugo               text,
    zogen_sa                 smallint,
    ijyo_cd                  text,
    nyusen_jyuni             smallint,
    kakutei_jyuni            smallint,
    dochaku_kubun            text,
    dochaku_tosu             smallint,
    "time"                   smallint,
    chakusa_cd               text,
    chakusa_cd_p             text,
    chakusa_cd_pp            text,
    jyuni_1c                 smallint,
    jyuni_2c                 smallint,
    jyuni_3c                 smallint,
    jyuni_4c                 smallint,
    odds                     smallint,
    ninki                    smallint,
    honsyokin                integer,
    fukasyokin               integer,
    reserved3                text,
    reserved4                text,
    haron_time_l4            smallint,
    haron_time_l3            smallint,

    chaku_uma_1_ketto_num    text,
    chaku_uma_1_bamei        text,
    chaku_uma_2_ketto_num    text,
    chaku_uma_2_bamei        text,
    chaku_uma_3_ketto_num    text,
    chaku_uma_3_bamei        text,

    time_diff                smallint,
    record_up_kubun          text,
    dm_kubun                 text,
    dm_time                  integer,
    dm_gosa_p                smallint,
    dm_gosa_m                smallint,
    dm_jyuni                 smallint,
    kyakusitu_kubun          text,

    PRIMARY KEY (year, month_day, jyo_cd, kaiji, nichiji, race_num, umaban)
);

COMMENT ON COLUMN se.record_spec   IS 'RecordSpec — always "SE"';
COMMENT ON COLUMN se.data_kubun    IS 'DataKubun';
COMMENT ON COLUMN se.make_date     IS 'MakeDate';
COMMENT ON COLUMN se.year          IS 'Year — RACE_ID';
COMMENT ON COLUMN se.month_day     IS 'MonthDay — RACE_ID';
COMMENT ON COLUMN se.jyo_cd        IS 'JyoCD — RACE_ID';
COMMENT ON COLUMN se.kaiji         IS 'Kaiji — RACE_ID';
COMMENT ON COLUMN se.nichiji       IS 'Nichiji — RACE_ID';
COMMENT ON COLUMN se.race_num      IS 'RaceNum — RACE_ID';
COMMENT ON COLUMN se.umaban        IS 'Umaban';
COMMENT ON COLUMN se.wakuban       IS 'Wakuban';
COMMENT ON COLUMN se.ketto_num     IS 'KettoNum';
COMMENT ON COLUMN se.bamei         IS 'Bamei';
COMMENT ON COLUMN se.uma_kigo_cd   IS 'UmaKigoCD';
COMMENT ON COLUMN se.sex_cd        IS 'SexCD';
COMMENT ON COLUMN se.hinsyu_cd     IS 'HinsyuCD';
COMMENT ON COLUMN se.keiro_cd      IS 'KeiroCD';
COMMENT ON COLUMN se.barei         IS 'Barei';
COMMENT ON COLUMN se.tozai_cd      IS 'TozaiCD';
COMMENT ON COLUMN se.chokyosi_code IS 'ChokyosiCode';
COMMENT ON COLUMN se.chokyosi_ryakusyo IS 'ChokyosiRyakusyo';
COMMENT ON COLUMN se.banusi_code   IS 'BanusiCode';
COMMENT ON COLUMN se.banusi_name   IS 'BanusiName';
COMMENT ON COLUMN se.fukusyoku     IS 'Fukusyoku';
COMMENT ON COLUMN se.reserved1     IS 'reserved1';
COMMENT ON COLUMN se.futan         IS 'Futan';
COMMENT ON COLUMN se.futan_before  IS 'FutanBefore';
COMMENT ON COLUMN se.blinker       IS 'Blinker';
COMMENT ON COLUMN se.reserved2     IS 'reserved2';
COMMENT ON COLUMN se.kisyu_code    IS 'KisyuCode';
COMMENT ON COLUMN se.kisyu_code_before IS 'KisyuCodeBefore';
COMMENT ON COLUMN se.kisyu_ryakusyo IS 'KisyuRyakusyo';
COMMENT ON COLUMN se.kisyu_ryakusyo_before IS 'KisyuRyakusyoBefore';
COMMENT ON COLUMN se.minarai_cd    IS 'MinaraiCD';
COMMENT ON COLUMN se.minarai_cd_before IS 'MinaraiCDBefore';
COMMENT ON COLUMN se.ba_taijyu     IS 'BaTaijyu';
COMMENT ON COLUMN se.zogen_fugo    IS 'ZogenFugo';
COMMENT ON COLUMN se.zogen_sa      IS 'ZogenSa';
COMMENT ON COLUMN se.ijyo_cd       IS 'IJyoCD';
COMMENT ON COLUMN se.nyusen_jyuni  IS 'NyusenJyuni';
COMMENT ON COLUMN se.kakutei_jyuni IS 'KakuteiJyuni';
COMMENT ON COLUMN se.dochaku_kubun IS 'DochakuKubun';
COMMENT ON COLUMN se.dochaku_tosu  IS 'DochakuTosu';
COMMENT ON COLUMN se."time"        IS 'Time';
COMMENT ON COLUMN se.chakusa_cd    IS 'ChakusaCD';
COMMENT ON COLUMN se.chakusa_cd_p  IS 'ChakusaCDP';
COMMENT ON COLUMN se.chakusa_cd_pp IS 'ChakusaCDPP';
COMMENT ON COLUMN se.jyuni_1c      IS 'Jyuni1c';
COMMENT ON COLUMN se.jyuni_2c      IS 'Jyuni2c';
COMMENT ON COLUMN se.jyuni_3c      IS 'Jyuni3c';
COMMENT ON COLUMN se.jyuni_4c      IS 'Jyuni4c';
COMMENT ON COLUMN se.odds          IS 'Odds';
COMMENT ON COLUMN se.ninki         IS 'Ninki';
COMMENT ON COLUMN se.honsyokin     IS 'Honsyokin';
COMMENT ON COLUMN se.fukasyokin    IS 'Fukasyokin';
COMMENT ON COLUMN se.reserved3     IS 'reserved3';
COMMENT ON COLUMN se.reserved4     IS 'reserved4';
COMMENT ON COLUMN se.haron_time_l4 IS 'HaronTimeL4';
COMMENT ON COLUMN se.haron_time_l3 IS 'HaronTimeL3';
COMMENT ON COLUMN se.chaku_uma_1_ketto_num IS 'ChakuUmaInfo[0].KettoNum';
COMMENT ON COLUMN se.chaku_uma_1_bamei     IS 'ChakuUmaInfo[0].Bamei';
COMMENT ON COLUMN se.chaku_uma_2_ketto_num IS 'ChakuUmaInfo[1].KettoNum';
COMMENT ON COLUMN se.chaku_uma_2_bamei     IS 'ChakuUmaInfo[1].Bamei';
COMMENT ON COLUMN se.chaku_uma_3_ketto_num IS 'ChakuUmaInfo[2].KettoNum';
COMMENT ON COLUMN se.chaku_uma_3_bamei     IS 'ChakuUmaInfo[2].Bamei';
COMMENT ON COLUMN se.time_diff     IS 'TimeDiff';
COMMENT ON COLUMN se.record_up_kubun IS 'RecordUpKubun';
COMMENT ON COLUMN se.dm_kubun      IS 'DMKubun';
COMMENT ON COLUMN se.dm_time       IS 'DMTime';
COMMENT ON COLUMN se.dm_gosa_p     IS 'DMGosaP';
COMMENT ON COLUMN se.dm_gosa_m     IS 'DMGosaM';
COMMENT ON COLUMN se.dm_jyuni      IS 'DMJyuni';
COMMENT ON COLUMN se.kyakusitu_kubun IS 'KyakusituKubun';
