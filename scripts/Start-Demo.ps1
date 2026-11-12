[CmdletBinding()]
param([string]$Name='presentation',[switch]$NoLaunch)
$ErrorActionPreference='Stop'
. (Join-Path $PSScriptRoot 'Demo-Session.ps1')
$layout=Get-DemoLayout $Name
$runDirectory=Join-Path $demoRepoRoot ('artifacts\runs\'+[Guid]::NewGuid().ToString('N'))
& (Join-Path $PSScriptRoot 'Build.ps1') -OutputDirectory $runDirectory
$applicationPath=Join-Path $runDirectory 'CapstoneProject.exe'
$management=Open-DemoManagement $layout -Create
$previousData=$env:CAPSTONE_DATA_DIR
$previousDemo=$env:CAPSTONE_DEMO_ROOT
try {
    $guard=Open-DemoResetGuard $layout
    try { Initialize-DemoData $layout $applicationPath } finally { $guard.Dispose() }
    Write-Output "Demo data: $($layout.Data)"
    Write-Output 'Sample data only. Existing working records are separate. Walkthrough: docs/DEMO.md'
    if ($NoLaunch) { return }
    $env:CAPSTONE_DATA_DIR=$layout.Data
    $env:CAPSTONE_DEMO_ROOT=$layout.Root
    if ((Get-DemoAdministratorCount $layout) -eq 0) {
        $setup=Start-Process -FilePath $applicationPath -ArgumentList '--provision-admin' -WindowStyle Normal -Wait -PassThru
        if ($setup.ExitCode -ne 0) { throw 'Demo administrator setup could not complete.' }
        if ((Get-DemoAdministratorCount $layout) -eq 0) { Write-Output 'Administrator setup was canceled. Run this command again when ready.'; return }
    }
    Start-Process -FilePath $applicationPath -WindowStyle Normal
} finally {
    $env:CAPSTONE_DATA_DIR=$previousData
    $env:CAPSTONE_DEMO_ROOT=$previousDemo
    $management.Dispose()
}
