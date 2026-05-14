<#
.SYNOPSIS
    Start the Podman machine and the jvlink2db-pg container.

.DESCRIPTION
    Idempotent — safe to run repeatedly. If the machine is already
    running, it just verifies and exits. Same for the container.

    Designed to run at user logon via a Task Scheduler entry registered
    by `install-podman-autostart.ps1`. Can also be run by hand to bring
    the stack up after `podman machine stop` or a manual outage.

.PARAMETER Container
    Container name. Defaults to `jvlink2db-pg`.

.PARAMETER MachineName
    Podman machine name. Defaults to `podman-machine-default`.

.EXAMPLE
    .\scripts\podman-autostart.ps1
    # → start podman-machine-default if not running, then start jvlink2db-pg.

.EXAMPLE
    .\scripts\podman-autostart.ps1 -Container my-other-pg
    # → same but for a differently-named container.

.NOTES
    Why this exists: jvlink2db's upstream PostgreSQL runs in a Podman
    container by convention (postgres:16-alpine, volume
    `jvlink2db-pg-data`). After a host reboot nothing brings the
    Podman machine and that container back automatically — the next
    scheduled `jvlink2db weekly` cron task then fails, and any external
    reader on `localhost:5432` (db-tunnel-mcp-proxy / Hyperdrive /
    cloudflared) routes to whatever else happens to listen there. This
    script restores the canonical state; the companion installer
    `install-podman-autostart.ps1` wires it to user logon.
#>

[CmdletBinding()]
param(
    [string]$Container = 'jvlink2db-pg',
    [string]$MachineName = 'podman-machine-default'
)

$ErrorActionPreference = 'Stop'

# Podman invokes `wsl.exe` from PATH on Windows. Task Scheduler logon
# tasks inherit a PATH that does not always include System32.
if ($env:PATH -notmatch 'System32') {
    $env:PATH = "C:\Windows\System32;" + $env:PATH
}

# Use scoop's `current` symlink so version upgrades don't break this.
$podman = 'C:\Users\penta\scoop\apps\podman\current\podman.exe'
if (-not (Test-Path $podman)) {
    throw "podman.exe not found at $podman — adjust the path or reinstall via scoop."
}

# ---------------------------------------------------------------------------
# 1. Machine
# ---------------------------------------------------------------------------
# `podman machine list` decorates the default machine with a trailing
# asterisk ("podman-machine-default*"), which breaks simple substring
# matching. `machine inspect` returns just the state cleanly.
$state = (& $podman machine inspect $MachineName --format '{{.State}}' 2>$null).Trim()
$isRunning = ($LASTEXITCODE -eq 0) -and ($state -eq 'running')

if (-not $isRunning) {
    Write-Host "Starting podman machine '$MachineName'..."
    & $podman machine start $MachineName | Out-Host
    if ($LASTEXITCODE -ne 0) {
        throw "podman machine start exited with $LASTEXITCODE"
    }
} else {
    Write-Host "podman machine '$MachineName' already running."
}

# Wait for daemon to be reachable. `podman machine start` exits once the
# VM is up, but the API socket can take a beat longer.
$deadline = (Get-Date).AddSeconds(60)
$ok = $false
while ((Get-Date) -lt $deadline) {
    & $podman info --format '{{.Host.OS}}' 2>$null | Out-Null
    if ($LASTEXITCODE -eq 0) { $ok = $true; break }
    Start-Sleep -Seconds 2
}
if (-not $ok) {
    throw "Podman daemon did not respond within 60s of machine start."
}

# ---------------------------------------------------------------------------
# 2. Container
# ---------------------------------------------------------------------------
$containerState = & $podman ps -a --filter "name=^$Container$" --format '{{.Status}}' 2>$null
if (-not $containerState) {
    throw "Container '$Container' does not exist. Create it (`podman run -d --name $Container -p 5432:5432 -v ${Container}-data:/var/lib/postgresql/data postgres:16-alpine`) or pass -Container <name>."
}

if ($containerState -match '^Up ') {
    Write-Host "Container '$Container' already running."
} else {
    Write-Host "Starting container '$Container'..."
    & $podman start $Container | Out-Host
    if ($LASTEXITCODE -ne 0) {
        throw "podman start $Container exited with $LASTEXITCODE"
    }
}

Write-Host "OK: $MachineName + $Container are up."
