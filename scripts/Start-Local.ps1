[CmdletBinding()]
param([switch]$Preview,[switch]$NoLaunch,[string]$DataDirectory)
$ErrorActionPreference='Stop'
$repoRoot=Split-Path $PSScriptRoot -Parent
$runDirectory=Join-Path $repoRoot ('artifacts\runs\'+[Guid]::NewGuid().ToString('N'))
& (Join-Path $PSScriptRoot 'Build.ps1') -OutputDirectory $runDirectory
$exe=Join-Path $runDirectory 'CapstoneProject.exe'
if ($Preview) { if(!$NoLaunch){Start-Process -FilePath $exe -ArgumentList '--preview-students' -WindowStyle Hidden} }
else {
    & (Join-Path $PSScriptRoot 'Initialize-LocalData.ps1') -ApplicationPath $exe -DataDirectory $DataDirectory
    if(!$NoLaunch){Start-Process -FilePath $exe -WindowStyle Hidden}
}
Write-Output "Application build: $exe"
