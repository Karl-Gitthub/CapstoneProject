[CmdletBinding()]
param([string]$DataDirectory,[switch]$PassThru,[ValidateSet('Debug','Release')][string]$Configuration='Release')
$ErrorActionPreference='Stop'
$repoRoot=Split-Path $PSScriptRoot -Parent
. (Join-Path $PSScriptRoot 'Preflight.ps1')
if (!$DataDirectory) { $DataDirectory=Join-Path $repoRoot 'CapstoneProject\bin\x64\Debug\Data' }
Assert-DatabasePrerequisites -DataDirectory $DataDirectory
$runDirectory=Join-Path $repoRoot ('artifacts\runs\'+[Guid]::NewGuid().ToString('N'))
& (Join-Path $PSScriptRoot 'Build.ps1') -OutputDirectory $runDirectory -Configuration $Configuration
$applicationPath=Join-Path $runDirectory 'CapstoneProject.exe'
& (Join-Path $PSScriptRoot 'Initialize-LocalData.ps1') -ApplicationPath $applicationPath -DataDirectory $DataDirectory
Start-Process -FilePath $applicationPath -ArgumentList '--provision-admin' -WindowStyle Normal -PassThru:$PassThru
