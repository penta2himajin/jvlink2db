-- jv.bt: BT (系統情報 — bloodline). JV-Data §JV_BT_KEITO, 6889 bytes.

CREATE TABLE IF NOT EXISTS bt (
    record_spec   text NOT NULL,
    data_kubun    text NOT NULL,
    make_date     date,

    hansyoku_num  text NOT NULL,
    keito_id      text,
    keito_name    text,
    keito_ex      text,

    PRIMARY KEY (hansyoku_num)
);

COMMENT ON COLUMN bt.record_spec  IS 'RecordSpec — always "BT"';
COMMENT ON COLUMN bt.data_kubun   IS 'DataKubun';
COMMENT ON COLUMN bt.make_date    IS 'MakeDate';
COMMENT ON COLUMN bt.hansyoku_num IS 'HansyokuNum';
COMMENT ON COLUMN bt.keito_id     IS 'KeitoId';
COMMENT ON COLUMN bt.keito_name   IS 'KeitoName';
COMMENT ON COLUMN bt.keito_ex     IS 'KeitoEx (6800 bytes)';
