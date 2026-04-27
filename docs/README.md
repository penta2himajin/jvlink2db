# jvlink2db documentation

Design and specification documents for jvlink2db.

| File | Topic |
|---|---|
| [00-overview.md](./00-overview.md) | Goals, scope, and design principles |
| [01-architecture.md](./01-architecture.md) | C# project layout and dependencies |
| [02-jvlink-protocol.md](./02-jvlink-protocol.md) | JV-Link COM protocol summary |
| [03-data-specs.md](./03-data-specs.md) | dataspec / record-type mapping |
| [04-database-schema.md](./04-database-schema.md) | PostgreSQL schema design |
| [05-pipeline-modes.md](./05-pipeline-modes.md) | Acquisition modes |
| [06-error-handling.md](./06-error-handling.md) | Return code handling |
| [07-development.md](./07-development.md) | Build, test, and contribution |
| [08-roadmap.md](./08-roadmap.md) | Implementation phases and milestones |

All specifications here are derived from the official JRA-VAN Data Lab. SDK
Ver 4.9.0.2, in particular:

- JV-Link Interface Specification 4.9.0.1 (Windows)
- JV-Data Specification 4.9.0.1
- Cumulative Data Provision List (`蓄積系提供データ一覧`)
- Reference structure definitions: `JVData_Struct.cs`, `JVData_Structure.h`

The SDK itself is not redistributed in this repository; obtain it from the
JRA-VAN developer portal.
