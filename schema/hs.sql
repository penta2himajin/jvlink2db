-- jv.hs: HS (競走馬市場取引価格 — yearling-sale price). JV-Data §JV_HS_SALE, 200 bytes.

CREATE TABLE IF NOT EXISTS hs (
    record_spec    text NOT NULL,
    data_kubun     text NOT NULL,
    make_date      date,

    ketto_num      text NOT NULL,
    hansyoku_f_num text,
    hansyoku_m_num text,
    birth_year     text,
    sale_code      text NOT NULL,
    sale_host_name text,
    sale_name      text,
    from_date      date,
    to_date        date,
    barei          text,
    price          bigint,

    PRIMARY KEY (ketto_num, sale_code)
);

COMMENT ON COLUMN hs.record_spec    IS 'RecordSpec — always "HS"';
COMMENT ON COLUMN hs.data_kubun     IS 'DataKubun';
COMMENT ON COLUMN hs.make_date      IS 'MakeDate';
COMMENT ON COLUMN hs.ketto_num      IS 'KettoNum';
COMMENT ON COLUMN hs.hansyoku_f_num IS 'HansyokuFNum';
COMMENT ON COLUMN hs.hansyoku_m_num IS 'HansyokuMNum';
COMMENT ON COLUMN hs.birth_year     IS 'BirthYear';
COMMENT ON COLUMN hs.sale_code      IS 'SaleCode';
COMMENT ON COLUMN hs.sale_host_name IS 'SaleHostName';
COMMENT ON COLUMN hs.sale_name      IS 'SaleName';
COMMENT ON COLUMN hs.from_date      IS 'FromDate';
COMMENT ON COLUMN hs.to_date        IS 'ToDate';
COMMENT ON COLUMN hs.barei          IS 'Barei';
COMMENT ON COLUMN hs.price          IS 'Price';
