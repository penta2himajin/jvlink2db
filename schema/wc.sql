-- jv.wc: WC (ウッドチップ調教 — wood-chip training) records.
-- JV-Data 4.9.0.1 §JV_WC_WOOD (105 bytes).

CREATE TABLE IF NOT EXISTS wc (
    record_spec    text NOT NULL,
    data_kubun     text NOT NULL,
    make_date      date,

    tresen_kubun   text NOT NULL,
    chokyo_date    date NOT NULL,
    chokyo_time    text NOT NULL,
    ketto_num      text NOT NULL,
    course         text,
    baba_around    text,
    reserved       text,

    haron_time_10  smallint, lap_time_10 smallint,
    haron_time_9   smallint, lap_time_9  smallint,
    haron_time_8   smallint, lap_time_8  smallint,
    haron_time_7   smallint, lap_time_7  smallint,
    haron_time_6   smallint, lap_time_6  smallint,
    haron_time_5   smallint, lap_time_5  smallint,
    haron_time_4   smallint, lap_time_4  smallint,
    haron_time_3   smallint, lap_time_3  smallint,
    haron_time_2   smallint, lap_time_2  smallint,
    lap_time_1     smallint,

    PRIMARY KEY (tresen_kubun, chokyo_date, chokyo_time, ketto_num)
);

COMMENT ON COLUMN wc.record_spec  IS 'RecordSpec — always "WC"';
COMMENT ON COLUMN wc.data_kubun   IS 'DataKubun';
COMMENT ON COLUMN wc.make_date    IS 'MakeDate';
COMMENT ON COLUMN wc.tresen_kubun IS 'TresenKubun';
COMMENT ON COLUMN wc.chokyo_date  IS 'ChokyoDate';
COMMENT ON COLUMN wc.chokyo_time  IS 'ChokyoTime';
COMMENT ON COLUMN wc.ketto_num    IS 'KettoNum';
COMMENT ON COLUMN wc.course       IS 'Course';
COMMENT ON COLUMN wc.baba_around  IS 'BabaAround';
COMMENT ON COLUMN wc.reserved     IS 'reserved';
