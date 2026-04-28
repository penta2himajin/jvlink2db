-- jv.cs: CS (コース情報 — course information). JV-Data §JV_CS_COURSE, 6829 bytes.

CREATE TABLE IF NOT EXISTS cs (
    record_spec   text NOT NULL,
    data_kubun    text NOT NULL,
    make_date     date,

    jyo_cd        text NOT NULL,
    kyori         text NOT NULL,
    track_cd      text NOT NULL,
    kaishu_date   date NOT NULL,
    course_ex     text,

    PRIMARY KEY (jyo_cd, kyori, track_cd, kaishu_date)
);

COMMENT ON COLUMN cs.record_spec IS 'RecordSpec — always "CS"';
COMMENT ON COLUMN cs.data_kubun  IS 'DataKubun';
COMMENT ON COLUMN cs.make_date   IS 'MakeDate';
COMMENT ON COLUMN cs.jyo_cd      IS 'JyoCD';
COMMENT ON COLUMN cs.kyori       IS 'Kyori';
COMMENT ON COLUMN cs.track_cd    IS 'TrackCD';
COMMENT ON COLUMN cs.kaishu_date IS 'KaishuDate (YYYYMMDD)';
COMMENT ON COLUMN cs.course_ex   IS 'CourseEx (6800 bytes)';
