using System.Collections.Generic;
using Jvlink2Db.Db.Postgres.Records;
using Jvlink2Db.Parser.Records;
using Jvlink2Db.Pipeline.Setup;
using Npgsql;

namespace Jvlink2Db.Cli.Commands;

internal static class PostgresSinkFactory
{
    public static IReadOnlyList<IRecordSink> CreateAll(NpgsqlDataSource dataSource, string schema) =>
    [
        new RecordSink<Core.Records.Ra>("RA", RaDecoder.Decode, new PostgresRaWriter(dataSource, schema)),
        new RecordSink<Core.Records.Se>("SE", SeDecoder.Decode, new PostgresSeWriter(dataSource, schema)),
        new RecordSink<Core.Records.Hr>("HR", HrDecoder.Decode, new PostgresHrWriter(dataSource, schema)),
        new RecordSink<Core.Records.H1>("H1", H1Decoder.Decode, new PostgresH1Writer(dataSource, schema)),
        new RecordSink<Core.Records.H6>("H6", H6Decoder.Decode, new PostgresH6Writer(dataSource, schema)),
        new RecordSink<Core.Records.O1>("O1", O1Decoder.Decode, new PostgresO1Writer(dataSource, schema)),
        new RecordSink<Core.Records.O2>("O2", O2Decoder.Decode, new PostgresO2Writer(dataSource, schema)),
        new RecordSink<Core.Records.O3>("O3", O3Decoder.Decode, new PostgresO3Writer(dataSource, schema)),
        new RecordSink<Core.Records.O4>("O4", O4Decoder.Decode, new PostgresO4Writer(dataSource, schema)),
        new RecordSink<Core.Records.O5>("O5", O5Decoder.Decode, new PostgresO5Writer(dataSource, schema)),
        new RecordSink<Core.Records.O6>("O6", O6Decoder.Decode, new PostgresO6Writer(dataSource, schema)),
        new RecordSink<Core.Records.Wf>("WF", WfDecoder.Decode, new PostgresWfWriter(dataSource, schema)),
        new RecordSink<Core.Records.Jg>("JG", JgDecoder.Decode, new PostgresJgWriter(dataSource, schema)),
        new RecordSink<Core.Records.Um>("UM", UmDecoder.Decode, new PostgresUmWriter(dataSource, schema)),
        new RecordSink<Core.Records.Ks>("KS", KsDecoder.Decode, new PostgresKsWriter(dataSource, schema)),
        new RecordSink<Core.Records.Ch>("CH", ChDecoder.Decode, new PostgresChWriter(dataSource, schema)),
        new RecordSink<Core.Records.Br>("BR", BrDecoder.Decode, new PostgresBrWriter(dataSource, schema)),
        new RecordSink<Core.Records.Bn>("BN", BnDecoder.Decode, new PostgresBnWriter(dataSource, schema)),
        new RecordSink<Core.Records.Rc>("RC", RcDecoder.Decode, new PostgresRcWriter(dataSource, schema)),
        new RecordSink<Core.Records.Hn>("HN", HnDecoder.Decode, new PostgresHnWriter(dataSource, schema)),
        new RecordSink<Core.Records.Sk>("SK", SkDecoder.Decode, new PostgresSkWriter(dataSource, schema)),
        new RecordSink<Core.Records.Bt>("BT", BtDecoder.Decode, new PostgresBtWriter(dataSource, schema)),
        new RecordSink<Core.Records.Dm>("DM", DmDecoder.Decode, new PostgresDmWriter(dataSource, schema)),
        new RecordSink<Core.Records.Tm>("TM", TmDecoder.Decode, new PostgresTmWriter(dataSource, schema)),
        new RecordSink<Core.Records.Ck>("CK", CkDecoder.Decode, new PostgresCkWriter(dataSource, schema)),
        new RecordSink<Core.Records.Hc>("HC", HcDecoder.Decode, new PostgresHcWriter(dataSource, schema)),
        new RecordSink<Core.Records.Wc>("WC", WcDecoder.Decode, new PostgresWcWriter(dataSource, schema)),
        new RecordSink<Core.Records.Ys>("YS", YsDecoder.Decode, new PostgresYsWriter(dataSource, schema)),
        new RecordSink<Core.Records.Hs>("HS", HsDecoder.Decode, new PostgresHsWriter(dataSource, schema)),
        new RecordSink<Core.Records.Hy>("HY", HyDecoder.Decode, new PostgresHyWriter(dataSource, schema)),
        new RecordSink<Core.Records.Cs>("CS", CsDecoder.Decode, new PostgresCsWriter(dataSource, schema)),
        new RecordSink<Core.Records.Tk>("TK", TkDecoder.Decode, new PostgresTkWriter(dataSource, schema)),
        new RecordSink<Core.Records.Wh>("WH", WhDecoder.Decode, new PostgresWhWriter(dataSource, schema)),
    ];
}
