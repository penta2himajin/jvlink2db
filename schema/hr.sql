-- jv.hr: HR (払戻 — payouts) records.
-- One row per race. JV-Data 4.9.0.1 §JV_HR_PAY (719 bytes).
-- Pay info is stored as parallel arrays per pay type.

CREATE TABLE IF NOT EXISTS hr (
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

    fuseiritu_flag   text[],
    tokubarai_flag   text[],
    henkan_flag      text[],
    henkan_uma       text[],
    henkan_waku      text[],
    henkan_do_waku   text[],

    pay_tansyo_umaban    text[],
    pay_tansyo_pay       integer[],
    pay_tansyo_ninki     smallint[],

    pay_fukusyo_umaban   text[],
    pay_fukusyo_pay      integer[],
    pay_fukusyo_ninki    smallint[],

    pay_wakuren_umaban   text[],
    pay_wakuren_pay      integer[],
    pay_wakuren_ninki    smallint[],

    pay_umaren_kumi      text[],
    pay_umaren_pay       integer[],
    pay_umaren_ninki     smallint[],

    pay_wide_kumi        text[],
    pay_wide_pay         integer[],
    pay_wide_ninki       smallint[],

    pay_reserved1_kumi   text[],
    pay_reserved1_pay    integer[],
    pay_reserved1_ninki  smallint[],

    pay_umatan_kumi      text[],
    pay_umatan_pay       integer[],
    pay_umatan_ninki     smallint[],

    pay_sanrenpuku_kumi  text[],
    pay_sanrenpuku_pay   integer[],
    pay_sanrenpuku_ninki smallint[],

    pay_sanrentan_kumi   text[],
    pay_sanrentan_pay    integer[],
    pay_sanrentan_ninki  smallint[],

    PRIMARY KEY (year, month_day, jyo_cd, kaiji, nichiji, race_num)
);

COMMENT ON COLUMN hr.record_spec      IS 'RecordSpec — always "HR"';
COMMENT ON COLUMN hr.data_kubun       IS 'DataKubun';
COMMENT ON COLUMN hr.make_date        IS 'MakeDate';
COMMENT ON COLUMN hr.year             IS 'Year — RACE_ID';
COMMENT ON COLUMN hr.month_day        IS 'MonthDay — RACE_ID';
COMMENT ON COLUMN hr.jyo_cd           IS 'JyoCD — RACE_ID';
COMMENT ON COLUMN hr.kaiji            IS 'Kaiji — RACE_ID';
COMMENT ON COLUMN hr.nichiji          IS 'Nichiji — RACE_ID';
COMMENT ON COLUMN hr.race_num         IS 'RaceNum — RACE_ID';
COMMENT ON COLUMN hr.toroku_tosu      IS 'TorokuTosu';
COMMENT ON COLUMN hr.syusso_tosu      IS 'SyussoTosu';
COMMENT ON COLUMN hr.fuseiritu_flag   IS 'FuseirituFlag[9]';
COMMENT ON COLUMN hr.tokubarai_flag   IS 'TokubaraiFlag[9]';
COMMENT ON COLUMN hr.henkan_flag      IS 'HenkanFlag[9]';
COMMENT ON COLUMN hr.henkan_uma       IS 'HenkanUma[28]';
COMMENT ON COLUMN hr.henkan_waku      IS 'HenkanWaku[8]';
COMMENT ON COLUMN hr.henkan_do_waku   IS 'HenkanDoWaku[8]';
COMMENT ON COLUMN hr.pay_tansyo_umaban   IS 'PayTansyo[3].Umaban';
COMMENT ON COLUMN hr.pay_tansyo_pay      IS 'PayTansyo[3].Pay';
COMMENT ON COLUMN hr.pay_tansyo_ninki    IS 'PayTansyo[3].Ninki';
COMMENT ON COLUMN hr.pay_fukusyo_umaban  IS 'PayFukusyo[5].Umaban';
COMMENT ON COLUMN hr.pay_fukusyo_pay     IS 'PayFukusyo[5].Pay';
COMMENT ON COLUMN hr.pay_fukusyo_ninki   IS 'PayFukusyo[5].Ninki';
COMMENT ON COLUMN hr.pay_wakuren_umaban  IS 'PayWakuren[3].Umaban';
COMMENT ON COLUMN hr.pay_wakuren_pay     IS 'PayWakuren[3].Pay';
COMMENT ON COLUMN hr.pay_wakuren_ninki   IS 'PayWakuren[3].Ninki';
COMMENT ON COLUMN hr.pay_umaren_kumi     IS 'PayUmaren[3].Kumi';
COMMENT ON COLUMN hr.pay_umaren_pay      IS 'PayUmaren[3].Pay';
COMMENT ON COLUMN hr.pay_umaren_ninki    IS 'PayUmaren[3].Ninki';
COMMENT ON COLUMN hr.pay_wide_kumi       IS 'PayWide[7].Kumi';
COMMENT ON COLUMN hr.pay_wide_pay        IS 'PayWide[7].Pay';
COMMENT ON COLUMN hr.pay_wide_ninki      IS 'PayWide[7].Ninki';
COMMENT ON COLUMN hr.pay_reserved1_kumi  IS 'PayReserved1[3].Kumi';
COMMENT ON COLUMN hr.pay_reserved1_pay   IS 'PayReserved1[3].Pay';
COMMENT ON COLUMN hr.pay_reserved1_ninki IS 'PayReserved1[3].Ninki';
COMMENT ON COLUMN hr.pay_umatan_kumi     IS 'PayUmatan[6].Kumi';
COMMENT ON COLUMN hr.pay_umatan_pay      IS 'PayUmatan[6].Pay';
COMMENT ON COLUMN hr.pay_umatan_ninki    IS 'PayUmatan[6].Ninki';
COMMENT ON COLUMN hr.pay_sanrenpuku_kumi  IS 'PaySanrenpuku[3].Kumi';
COMMENT ON COLUMN hr.pay_sanrenpuku_pay   IS 'PaySanrenpuku[3].Pay';
COMMENT ON COLUMN hr.pay_sanrenpuku_ninki IS 'PaySanrenpuku[3].Ninki';
COMMENT ON COLUMN hr.pay_sanrentan_kumi   IS 'PaySanrentan[6].Kumi';
COMMENT ON COLUMN hr.pay_sanrentan_pay    IS 'PaySanrentan[6].Pay';
COMMENT ON COLUMN hr.pay_sanrentan_ninki  IS 'PaySanrentan[6].Ninki';
