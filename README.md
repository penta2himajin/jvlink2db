# jvlink2db

PostgreSQL importer for JRA-VAN Data Lab. JV-Link records.

Written in C# (.NET 8). Reads racing data from the JV-Link COM component
and writes it into a PostgreSQL database using the bulk `COPY` protocol.

## Status

Early design phase. No usable implementation yet. See [`docs/`](./docs/)
for the specification this project is being built against.

## What it does

- Calls JV-Link via COM (`JVInit` / `JVOpen` / `JVStatus` / `JVRead` / `JVClose`)
  following the official protocol described in the JV-Link Interface
  Specification 4.9.0.1.
- Decodes JV-Data records into typed structures using the field layouts
  published in the JV-Data Specification 4.9.0.1.
- Loads the decoded records into PostgreSQL with `COPY ... FROM STDIN BINARY`
  through Npgsql.
- Supports the four JV-Link acquisition modes: setup / range / normal / weekly
  (see [`docs/05-pipeline-modes.md`](./docs/05-pipeline-modes.md)).

## What it does not do

- No GUI. Command-line only.
- No NAR (chihou) racing support. The UmaConn data source is out of scope.
- No analytics, no machine-learning helpers, no visualisation.
- No support for databases other than PostgreSQL.

## Requirements

- Windows 10 or 11 (64-bit). JV-Link is a 32-bit COM component and only
  runs on Windows.
- An active JRA-VAN Data Lab. subscription and a valid service key
  configured in JV-Link.
- JV-Link 4.9.0 or later, installed via the JRA-VAN installer.
- .NET 8 SDK.
- PostgreSQL 14 or later.

## License

MIT. See [LICENSE](./LICENSE).

JRA-VAN, JV-Link, and JV-Data are products and trademarks of JRA SYSTEM
SERVICE Co., Ltd. This project is not affiliated with or endorsed by
JRA-VAN or JRA SYSTEM SERVICE Co., Ltd.
