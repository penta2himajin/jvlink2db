<#
.SYNOPSIS
    Register the podman-autostart Task Scheduler entry.

.DESCRIPTION
    Creates an at-logon scheduled task that runs `podman-autostart.ps1`
    so the jvlink2db-pg container is up before any other scheduled
    jvlink2db cron task fires.

    Idempotent — re-running replaces the existing entry. No admin
    elevation required (runs as the current user, no stored password).

.PARAMETER TaskName
    Task name within the folder. Defaults to `podman-autostart`.

.PARAMETER TaskFolder
    Task Scheduler folder. Defaults to `\jvlink2db` to colocate with
    jvlink2db's own scheduled tasks installed by `jvlink2db schedule`.

.PARAMETER ScriptPath
    Path to the worker script. Defaults to `podman-autostart.ps1` next
    to this installer.

.EXAMPLE
    .\scripts\install-podman-autostart.ps1
    # → registers \jvlink2db\podman-autostart at logon.

.EXAMPLE
    .\scripts\install-podman-autostart.ps1 -TaskFolder '\'
    # → registers at the root of Task Scheduler.

.NOTES
    To uninstall:
        Unregister-ScheduledTask -TaskName 'podman-autostart' -TaskPath '\jvlink2db\' -Confirm:$false

    To run the task on demand (verify it works):
        Start-ScheduledTask -TaskName 'podman-autostart' -TaskPath '\jvlink2db\'
#>

[CmdletBinding()]
param(
    [string]$TaskName = 'podman-autostart',
    [string]$TaskFolder = '\jvlink2db',
    [string]$ScriptPath
)

$ErrorActionPreference = 'Stop'

if (-not $ScriptPath) {
    $ScriptPath = Join-Path $PSScriptRoot 'podman-autostart.ps1'
}
$ScriptPath = (Resolve-Path -LiteralPath $ScriptPath).Path
if (-not (Test-Path -LiteralPath $ScriptPath)) {
    throw "Worker script not found: $ScriptPath"
}

# Normalise: New-ScheduledTaskPrincipal/Register-ScheduledTask want TaskPath
# to end in a backslash; the canonical full name does not.
$folder = $TaskFolder.TrimEnd('\')
$registerFolder = "$folder\"
$fullName = "$folder\$TaskName"

$action = New-ScheduledTaskAction `
    -Execute 'powershell.exe' `
    -Argument "-NoProfile -WindowStyle Hidden -ExecutionPolicy Bypass -File `"$ScriptPath`""

$trigger = New-ScheduledTaskTrigger -AtLogOn -User $env:USERNAME

$settings = New-ScheduledTaskSettingsSet `
    -StartWhenAvailable `
    -AllowStartIfOnBatteries `
    -DontStopIfGoingOnBatteries `
    -MultipleInstances IgnoreNew `
    -ExecutionTimeLimit (New-TimeSpan -Minutes 5)

$principal = New-ScheduledTaskPrincipal `
    -UserId $env:USERNAME `
    -LogonType Interactive `
    -RunLevel Limited

$definition = New-ScheduledTask `
    -Action $action `
    -Trigger $trigger `
    -Settings $settings `
    -Principal $principal `
    -Description "Start podman-machine-default and the jvlink2db-pg container at user logon. Source: jvlink2db/scripts/podman-autostart.ps1."

Register-ScheduledTask `
    -TaskName $TaskName `
    -TaskPath $registerFolder `
    -InputObject $definition `
    -Force | Out-Null

Write-Host "Registered: $fullName"
Write-Host "  Script : $ScriptPath"
Write-Host "  Trigger: At logon ($env:USERNAME)"
Write-Host "  Run as : $env:USERNAME (interactive, limited privileges)"
Write-Host ""
Write-Host "Verify it works now (idempotent if podman is already up):"
Write-Host "    Start-ScheduledTask -TaskName '$TaskName' -TaskPath '$registerFolder'"
Write-Host ""
Write-Host "Uninstall:"
Write-Host "    Unregister-ScheduledTask -TaskName '$TaskName' -TaskPath '$registerFolder' -Confirm:`$false"
