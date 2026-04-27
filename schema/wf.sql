-- jv.wf: WF (WIN5) records.
-- One row per kaisai-date. JV-Data 4.9.0.1 §JV_WF_INFO (7215 bytes).

CREATE TABLE IF NOT EXISTS wf (
    record_spec       text     NOT NULL,
    data_kubun        text     NOT NULL,
    make_date         date,

    kaisai_date       date     NOT NULL,
    reserved1         text,

    race_jyo_cd       text[],     -- 5 races
    race_kaiji        text[],
    race_nichiji      text[],
    race_num          text[],

    reserved2         text,
    hatsubai_hyo      bigint,
    yuko_hyo          bigint[],   -- 5

    henkan_flag       text,
    fuseiritsu_flag   text,
    tekichunashi_flag text,
    co_shoki          bigint,
    co_zan_daka       bigint,

    pay_kumiban       text[],     -- 243 entries
    pay_amount        integer[],
    pay_tekichu_hyo   bigint[],

    PRIMARY KEY (kaisai_date)
);

COMMENT ON COLUMN wf.record_spec       IS 'RecordSpec — always "WF"';
COMMENT ON COLUMN wf.data_kubun        IS 'DataKubun';
COMMENT ON COLUMN wf.make_date         IS 'MakeDate';
COMMENT ON COLUMN wf.kaisai_date       IS 'KaisaiDate';
COMMENT ON COLUMN wf.reserved1         IS 'reserved1';
COMMENT ON COLUMN wf.race_jyo_cd       IS 'WFRaceInfo[5].JyoCD';
COMMENT ON COLUMN wf.race_kaiji        IS 'WFRaceInfo[5].Kaiji';
COMMENT ON COLUMN wf.race_nichiji      IS 'WFRaceInfo[5].Nichiji';
COMMENT ON COLUMN wf.race_num          IS 'WFRaceInfo[5].RaceNum';
COMMENT ON COLUMN wf.reserved2         IS 'reserved2';
COMMENT ON COLUMN wf.hatsubai_hyo      IS 'Hatsubai_Hyo';
COMMENT ON COLUMN wf.yuko_hyo          IS 'WFYukoHyoInfo[5].Yuko_Hyo';
COMMENT ON COLUMN wf.henkan_flag       IS 'HenkanFlag';
COMMENT ON COLUMN wf.fuseiritsu_flag   IS 'FuseiritsuFlag';
COMMENT ON COLUMN wf.tekichunashi_flag IS 'TekichunashiFlag';
COMMENT ON COLUMN wf.co_shoki          IS 'COShoki';
COMMENT ON COLUMN wf.co_zan_daka       IS 'COZanDaka';
COMMENT ON COLUMN wf.pay_kumiban       IS 'WFPayInfo[243].Kumiban';
COMMENT ON COLUMN wf.pay_amount        IS 'WFPayInfo[243].Pay';
COMMENT ON COLUMN wf.pay_tekichu_hyo   IS 'WFPayInfo[243].Tekichu_Hyo';
