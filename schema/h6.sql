-- jv.h6: H6 (票数 3連単) records.
-- One row per race. JV-Data 4.9.0.1 §JV_H6_HYOSU_SANRENTAN (102890 bytes).

CREATE TABLE IF NOT EXISTS h6 (
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

    hatubai_flag     text,
    henkan_uma       text[],     -- 18

    sanrentan_kumi   text[],     -- 4896
    sanrentan_hyo    bigint[],
    sanrentan_ninki  smallint[],

    hyo_total        bigint[],   -- 2

    PRIMARY KEY (year, month_day, jyo_cd, kaiji, nichiji, race_num)
);

COMMENT ON COLUMN h6.record_spec IS 'RecordSpec — always "H6"';
COMMENT ON COLUMN h6.data_kubun  IS 'DataKubun';
COMMENT ON COLUMN h6.make_date   IS 'MakeDate';
COMMENT ON COLUMN h6.year        IS 'Year — RACE_ID';
COMMENT ON COLUMN h6.month_day   IS 'MonthDay — RACE_ID';
COMMENT ON COLUMN h6.jyo_cd      IS 'JyoCD — RACE_ID';
COMMENT ON COLUMN h6.kaiji       IS 'Kaiji — RACE_ID';
COMMENT ON COLUMN h6.nichiji     IS 'Nichiji — RACE_ID';
COMMENT ON COLUMN h6.race_num    IS 'RaceNum — RACE_ID';
COMMENT ON COLUMN h6.toroku_tosu IS 'TorokuTosu';
COMMENT ON COLUMN h6.syusso_tosu IS 'SyussoTosu';
COMMENT ON COLUMN h6.hatubai_flag IS 'HatubaiFlag';
COMMENT ON COLUMN h6.henkan_uma   IS 'HenkanUma[18]';
COMMENT ON COLUMN h6.sanrentan_kumi  IS 'HyoSanrentan[4896].Kumi';
COMMENT ON COLUMN h6.sanrentan_hyo   IS 'HyoSanrentan[4896].Hyo';
COMMENT ON COLUMN h6.sanrentan_ninki IS 'HyoSanrentan[4896].Ninki';
COMMENT ON COLUMN h6.hyo_total    IS 'HyoTotal[2]';
