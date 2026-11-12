[CmdletBinding()]
param([string]$Name='presentation')
$ErrorActionPreference='Stop'
. (Join-Path $PSScriptRoot 'Demo-Session.ps1')
$layout=Get-DemoLayout $Name
$management=Open-DemoManagement $layout
try {
    $guard=Open-DemoResetGuard $layout
    try {
        $runDirectory=Join-Path $demoRepoRoot ('artifacts\runs\'+[Guid]::NewGuid().ToString('N'))
        & (Join-Path $PSScriptRoot 'Build.ps1') -OutputDirectory $runDirectory
        Reset-DemoData $layout (Join-Path $runDirectory 'CapstoneProject.exe')
    } finally { $guard.Dispose() }
} finally { $management.Dispose() }
