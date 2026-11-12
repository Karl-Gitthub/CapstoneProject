[CmdletBinding()]
param([switch]$Preview)
$ErrorActionPreference='Stop'
& (Join-Path $PSScriptRoot 'Build.ps1')
$repoRoot=Split-Path $PSScriptRoot -Parent
$exe=Join-Path $repoRoot 'CapstoneProject\bin\x64\Debug\CapstoneProject.exe'
if ($Preview) { Start-Process -FilePath $exe -ArgumentList '--preview-students' -WindowStyle Hidden }
else {
    & (Join-Path $PSScriptRoot 'Initialize-LocalData.ps1')
    Start-Process -FilePath $exe -WindowStyle Hidden
}
