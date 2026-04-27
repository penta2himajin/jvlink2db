# Development

## Tooling

- .NET 8 SDK.
- Visual Studio 2022, JetBrains Rider, or VS Code with the C# Dev Kit.
  Any of the three is fine.
- PostgreSQL 14+ available locally for integration tests. Docker is
  the path of least resistance; the test suite uses Testcontainers so
  a Docker daemon is enough.
- JV-Link 4.9.0 or later, installed and configured with a valid
  service key. Required only for end-to-end tests; unit and parser
  tests run on any platform.

## Building

```
dotnet restore
dotnet build -c Release -r win-x86 --self-contained false
```

The runtime identifier (`win-x86`) is the important part: it forces a
32-bit process so that the JV-Link COM component can be loaded
directly. A 64-bit build will fail to instantiate JV-Link at runtime.

## Running

```
dotnet run --project src/Jvlink2Db.Cli -- <args>
```

or against a published artefact:

```
dotnet publish src/Jvlink2Db.Cli -c Release -r win-x86 --self-contained true
./publish/jvlink2db.exe <args>
```

Configuration is read from `appsettings.json` next to the executable,
from `appsettings.Local.json` (gitignored), and from environment
variables prefixed `JVLINK2DB_`. Command-line flags override all of
the above.

## Tests

Three tiers, each in its own project:

- **Unit tests** (`*.Tests`). Pure logic. No external dependencies.
  Run on any platform: `dotnet test`.
- **Parser fixtures.** Decoders are exercised against captured raw
  `.jvd` byte sequences stored under `tests/fixtures/`. Fixtures are
  hand-authored or extracted from real downloads with all sensitive
  fields (service identifiers, etc.) redacted. Cross-checked against
  the SDK's `JVDataCheckTool` where possible.
- **Integration tests.** Spin up a PostgreSQL container with
  Testcontainers, run the importer against canned fixture data, and
  assert the resulting row counts and a sample of values per table.
  These run on any platform with Docker.

A fourth tier — **end-to-end tests against a live JV-Link** — is run
manually on a Windows host. They are not part of CI.

## Continuous integration

GitHub Actions, Windows runner, on every push and pull request:

1. Restore and build, `win-x86`.
2. Run unit tests and parser-fixture tests.
3. Run integration tests (Docker available on the GitHub-hosted Windows
  runner via Linux containers? — falls back to a Linux job for the
  PostgreSQL portion if needed).
4. Build a self-contained `win-x86` artefact and upload it.

Releases are tagged `vMAJOR.MINOR.PATCH` and trigger a separate
workflow that attaches the artefact to a GitHub Release.

## Contributions

Issues and pull requests are welcome. Before opening a non-trivial PR,
please open an issue first so the design can be discussed; the project
deliberately keeps a small surface area and a small dependency set.

All code is under MIT. Contributors implicitly agree that their
contributions are also under MIT.
