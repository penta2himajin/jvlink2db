-- jv.hc: HC (坂路調教 — slope training) records.
-- JV-Data 4.9.0.1 §JV_HC_HANRO (60 bytes).

CREATE TABLE IF NOT EXISTS hc (
    record_spec    text NOT NULL,
    data_kubun     text NOT NULL,
    make_date      date,

    tresen_kubun   text NOT NULL,
    chokyo_date    date NOT NULL,
    chokyo_time    text NOT NULL,
    ketto_num      text NOT NULL,
    haron_time_4   smallint,
    lap_time_4     smallint,
    haron_time_3   smallint,
    lap_time_3     smallint,
    haron_time_2   smallint,
    lap_time_2     smallint,
    lap_time_1     smallint,

    PRIMARY KEY (tresen_kubun, chokyo_date, chokyo_time, ketto_num)
);

COMMENT ON COLUMN hc.record_spec  IS 'RecordSpec — always "HC"';
COMMENT ON COLUMN hc.data_kubun   IS 'DataKubun';
COMMENT ON COLUMN hc.make_date    IS 'MakeDate';
COMMENT ON COLUMN hc.tresen_kubun IS 'TresenKubun';
COMMENT ON COLUMN hc.chokyo_date  IS 'ChokyoDate';
COMMENT ON COLUMN hc.chokyo_time  IS 'ChokyoTime';
COMMENT ON COLUMN hc.ketto_num    IS 'KettoNum';
COMMENT ON COLUMN hc.haron_time_4 IS 'HaronTime4';
COMMENT ON COLUMN hc.lap_time_4   IS 'LapTime4';
COMMENT ON COLUMN hc.haron_time_3 IS 'HaronTime3';
COMMENT ON COLUMN hc.lap_time_3   IS 'LapTime3';
COMMENT ON COLUMN hc.haron_time_2 IS 'HaronTime2';
COMMENT ON COLUMN hc.lap_time_2   IS 'LapTime2';
COMMENT ON COLUMN hc.lap_time_1   IS 'LapTime1';
