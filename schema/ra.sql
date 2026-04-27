-- jv.ra: RA (race detail) records.
-- One row per race, identified by the JV-Data RACE_ID composite.
-- See JV-Data Specification 4.9.0.1 §JV_RA_RACE for the on-wire layout.
-- Column types per docs/04-database-schema.md.
-- Original mixed-case JV-Data field names live in COMMENTs.

CREATE TABLE IF NOT EXISTS ra (
    -- RECORD_ID
    record_spec      text     NOT NULL,
    data_kubun       text     NOT NULL,
    make_date        date,

    -- RACE_ID (composite primary key)
    year             text     NOT NULL,
    month_day        text     NOT NULL,
    jyo_cd           text     NOT NULL,
    kaiji            text     NOT NULL,
    nichiji          text     NOT NULL,
    race_num         text     NOT NULL,

    -- RACE_INFO
    youbi_cd         text,
    toku_num         text,
    hondai           text,
    fukudai          text,
    kakko            text,
    hondai_eng       text,
    fukudai_eng      text,
    kakko_eng        text,
    ryakusyo10       text,
    ryakusyo6        text,
    ryakusyo3        text,
    kubun            text,
    nkai             smallint,

    -- Grade codes
    grade_cd         text,
    grade_cd_before  text,

    -- RACE_JYOKEN
    syubetu_cd       text,
    kigo_cd          text,
    jyuryo_cd        text,
    jyoken_cd        text[],

    jyoken_name              text,
    kyori                    smallint,
    kyori_before             smallint,
    track_cd                 text,
    track_cd_before          text,
    course_kubun_cd          text,
    course_kubun_cd_before   text,

    honsyokin                integer[],
    honsyokin_before         integer[],
    fukasyokin               integer[],
    fukasyokin_before        integer[],

    hasso_time               text,
    hasso_time_before        text,
    toroku_tosu              smallint,
    syusso_tosu              smallint,
    nyusen_tosu              smallint,

    -- TENKO_BABA_INFO
    tenko_cd                 text,
    siba_baba_cd             text,
    dirt_baba_cd             text,

    -- LAP_TIME[25] — raw 3-digit values (1/10 seconds)
    lap_time                 smallint[],

    -- HARON times — raw 3-digit values (1/10 seconds)
    syogai_mile_time         smallint,
    haron_time_s3            smallint,
    haron_time_s4            smallint,
    haron_time_l3            smallint,
    haron_time_l4            smallint,

    -- CORNER_INFO[4]
    corner1_corner           text,
    corner1_syukaisu         text,
    corner1_jyuni            text,
    corner2_corner           text,
    corner2_syukaisu         text,
    corner2_jyuni            text,
    corner3_corner           text,
    corner3_syukaisu         text,
    corner3_jyuni            text,
    corner4_corner           text,
    corner4_syukaisu         text,
    corner4_jyuni            text,

    record_up_kubun          text,

    PRIMARY KEY (year, month_day, jyo_cd, kaiji, nichiji, race_num)
);

COMMENT ON COLUMN ra.record_spec     IS 'RecordSpec — record header, always "RA"';
COMMENT ON COLUMN ra.data_kubun      IS 'DataKubun';
COMMENT ON COLUMN ra.make_date       IS 'MakeDate (YYYYMMDD on the wire)';
COMMENT ON COLUMN ra.year            IS 'Year — RACE_ID';
COMMENT ON COLUMN ra.month_day       IS 'MonthDay — RACE_ID';
COMMENT ON COLUMN ra.jyo_cd          IS 'JyoCD — RACE_ID';
COMMENT ON COLUMN ra.kaiji           IS 'Kaiji — RACE_ID';
COMMENT ON COLUMN ra.nichiji         IS 'Nichiji — RACE_ID';
COMMENT ON COLUMN ra.race_num        IS 'RaceNum — RACE_ID';
COMMENT ON COLUMN ra.youbi_cd        IS 'YoubiCD';
COMMENT ON COLUMN ra.toku_num        IS 'TokuNum';
COMMENT ON COLUMN ra.hondai          IS 'Hondai';
COMMENT ON COLUMN ra.fukudai         IS 'Fukudai';
COMMENT ON COLUMN ra.kakko           IS 'Kakko';
COMMENT ON COLUMN ra.hondai_eng      IS 'HondaiEng';
COMMENT ON COLUMN ra.fukudai_eng     IS 'FukudaiEng';
COMMENT ON COLUMN ra.kakko_eng       IS 'KakkoEng';
COMMENT ON COLUMN ra.ryakusyo10      IS 'Ryakusyo10';
COMMENT ON COLUMN ra.ryakusyo6       IS 'Ryakusyo6';
COMMENT ON COLUMN ra.ryakusyo3       IS 'Ryakusyo3';
COMMENT ON COLUMN ra.kubun           IS 'Kubun';
COMMENT ON COLUMN ra.nkai            IS 'Nkai';
COMMENT ON COLUMN ra.grade_cd        IS 'GradeCD';
COMMENT ON COLUMN ra.grade_cd_before IS 'GradeCDBefore';
COMMENT ON COLUMN ra.syubetu_cd      IS 'SyubetuCD';
COMMENT ON COLUMN ra.kigo_cd         IS 'KigoCD';
COMMENT ON COLUMN ra.jyuryo_cd       IS 'JyuryoCD';
COMMENT ON COLUMN ra.jyoken_cd       IS 'JyokenCD[5]';
COMMENT ON COLUMN ra.jyoken_name             IS 'JyokenName';
COMMENT ON COLUMN ra.kyori                   IS 'Kyori';
COMMENT ON COLUMN ra.kyori_before            IS 'KyoriBefore';
COMMENT ON COLUMN ra.track_cd                IS 'TrackCD';
COMMENT ON COLUMN ra.track_cd_before         IS 'TrackCDBefore';
COMMENT ON COLUMN ra.course_kubun_cd         IS 'CourseKubunCD';
COMMENT ON COLUMN ra.course_kubun_cd_before  IS 'CourseKubunCDBefore';
COMMENT ON COLUMN ra.honsyokin               IS 'Honsyokin[7]';
COMMENT ON COLUMN ra.honsyokin_before        IS 'HonsyokinBefore[5]';
COMMENT ON COLUMN ra.fukasyokin              IS 'Fukasyokin[5]';
COMMENT ON COLUMN ra.fukasyokin_before       IS 'FukasyokinBefore[3]';
COMMENT ON COLUMN ra.hasso_time              IS 'HassoTime';
COMMENT ON COLUMN ra.hasso_time_before       IS 'HassoTimeBefore';
COMMENT ON COLUMN ra.toroku_tosu             IS 'TorokuTosu';
COMMENT ON COLUMN ra.syusso_tosu             IS 'SyussoTosu';
COMMENT ON COLUMN ra.nyusen_tosu             IS 'NyusenTosu';
COMMENT ON COLUMN ra.tenko_cd                IS 'TenkoCD';
COMMENT ON COLUMN ra.siba_baba_cd            IS 'SibaBabaCD';
COMMENT ON COLUMN ra.dirt_baba_cd            IS 'DirtBabaCD';
COMMENT ON COLUMN ra.lap_time                IS 'LapTime[25] (raw 1/10 second integers)';
COMMENT ON COLUMN ra.syogai_mile_time        IS 'SyogaiMileTime';
COMMENT ON COLUMN ra.haron_time_s3           IS 'HaronTimeS3';
COMMENT ON COLUMN ra.haron_time_s4           IS 'HaronTimeS4';
COMMENT ON COLUMN ra.haron_time_l3           IS 'HaronTimeL3';
COMMENT ON COLUMN ra.haron_time_l4           IS 'HaronTimeL4';
COMMENT ON COLUMN ra.corner1_corner          IS 'CornerInfo[0].Corner';
COMMENT ON COLUMN ra.corner1_syukaisu        IS 'CornerInfo[0].Syukaisu';
COMMENT ON COLUMN ra.corner1_jyuni           IS 'CornerInfo[0].Jyuni';
COMMENT ON COLUMN ra.corner2_corner          IS 'CornerInfo[1].Corner';
COMMENT ON COLUMN ra.corner2_syukaisu        IS 'CornerInfo[1].Syukaisu';
COMMENT ON COLUMN ra.corner2_jyuni           IS 'CornerInfo[1].Jyuni';
COMMENT ON COLUMN ra.corner3_corner          IS 'CornerInfo[2].Corner';
COMMENT ON COLUMN ra.corner3_syukaisu        IS 'CornerInfo[2].Syukaisu';
COMMENT ON COLUMN ra.corner3_jyuni           IS 'CornerInfo[2].Jyuni';
COMMENT ON COLUMN ra.corner4_corner          IS 'CornerInfo[3].Corner';
COMMENT ON COLUMN ra.corner4_syukaisu        IS 'CornerInfo[3].Syukaisu';
COMMENT ON COLUMN ra.corner4_jyuni           IS 'CornerInfo[3].Jyuni';
COMMENT ON COLUMN ra.record_up_kubun         IS 'RecordUpKubun';
