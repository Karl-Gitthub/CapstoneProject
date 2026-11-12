[CmdletBinding()]
param([string]$Name='presentation',[ValidateSet('Debug','Release')][string]$Configuration='Release')
$ErrorActionPreference='Stop'
. (Join-Path $PSScriptRoot 'Demo-Session.ps1')
$layout=Get-DemoLayout $Name
$management=Open-DemoManagement $layout
try {
    $guard=Open-DemoResetGuard $layout
    try {
        . (Join-Path $PSScriptRoot 'Preflight.ps1')
        Assert-DatabasePrerequisites -DataDirectory $layout.Data
        $runDirectory=Join-Path $demoRepoRoot ('artifacts\runs\'+[Guid]::NewGuid().ToString('N'))
        & (Join-Path $PSScriptRoot 'Build.ps1') -OutputDirectory $runDirectory -Configuration $Configuration
        Reset-DemoData $layout (Join-Path $runDirectory 'CapstoneProject.exe')
    } finally { $guard.Dispose() }
} finally { $management.Dispose() }
