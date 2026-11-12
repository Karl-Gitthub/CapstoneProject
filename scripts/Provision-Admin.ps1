[CmdletBinding()]
param([string]$DataDirectory)
$ErrorActionPreference='Stop'
$repoRoot=Split-Path $PSScriptRoot -Parent
$runDirectory=Join-Path $repoRoot ('artifacts\runs\'+[Guid]::NewGuid().ToString('N'))
& (Join-Path $PSScriptRoot 'Build.ps1') -OutputDirectory $runDirectory
$applicationPath=Join-Path $runDirectory 'CapstoneProject.exe'
& (Join-Path $PSScriptRoot 'Initialize-LocalData.ps1') -ApplicationPath $applicationPath -DataDirectory $DataDirectory
Start-Process -FilePath $applicationPath -ArgumentList '--provision-admin' -WindowStyle Hidden
