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

## Operations

### Podman-hosted upstream PG — autostart on Windows

When jvlink2db's target PostgreSQL runs in a Podman container
(`jvlink2db-pg`, `postgres:16-alpine`, volume `jvlink2db-pg-data`),
nothing brings the container back after a host reboot — scheduled
`jvlink2db weekly`/`normal` runs then fail at connect, and any
external reader on `localhost:5432` (e.g. a Cloudflare-Hyperdrive
proxy) silently routes to whatever else happens to listen on the
port.

Register a logon-triggered Task Scheduler entry that boots the
machine and the container:

```powershell
.\scripts\install-podman-autostart.ps1
```

The installer is idempotent (re-runs replace the existing entry),
needs no admin elevation, and lands the task at
`\jvlink2db\podman-autostart` so it colocates with the cron tasks
that `jvlink2db schedule install` creates. Underlying worker:
[`scripts/podman-autostart.ps1`](../scripts/podman-autostart.ps1).

Verify without rebooting:

```powershell
Start-ScheduledTask -TaskName 'podman-autostart' -TaskPath '\jvlink2db\'
Get-ScheduledTaskInfo -TaskName 'podman-autostart' -TaskPath '\jvlink2db\' |
    Format-List LastRunTime, LastTaskResult
# LastTaskResult = 0 → OK
```

Uninstall:

```powershell
Unregister-ScheduledTask -TaskName 'podman-autostart' -TaskPath '\jvlink2db\' -Confirm:$false
```

Caveat: "At logon" fires when the user logs on, so unattended
reboots without a logon do not bring the stack up. WSL2 is per-user
and there is no clean Windows-service equivalent; if you need full
unattended uptime, consider auto-logon plus this task, or move the
container to a different host.

## Contributions

Issues and pull requests are welcome. Before opening a non-trivial PR,
please open an issue first so the design can be discussed; the project
deliberately keeps a small surface area and a small dependency set.

All code is under MIT. Contributors implicitly agree that their
contributions are also under MIT.
