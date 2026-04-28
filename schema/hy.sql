-- jv.hy: HY (馬名の意味由来 — bamei origin). JV-Data §JV_HY_BAMEIORIGIN, 123 bytes.

CREATE TABLE IF NOT EXISTS hy (
    record_spec text NOT NULL,
    data_kubun  text NOT NULL,
    make_date   date,

    ketto_num   text NOT NULL,
    bamei       text,
    origin      text,

    PRIMARY KEY (ketto_num)
);

COMMENT ON COLUMN hy.record_spec IS 'RecordSpec — always "HY"';
COMMENT ON COLUMN hy.data_kubun  IS 'DataKubun';
COMMENT ON COLUMN hy.make_date   IS 'MakeDate';
COMMENT ON COLUMN hy.ketto_num   IS 'KettoNum';
COMMENT ON COLUMN hy.bamei       IS 'Bamei';
COMMENT ON COLUMN hy.origin      IS 'Origin';
