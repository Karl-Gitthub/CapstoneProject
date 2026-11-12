[CmdletBinding()]
param([switch]$UnitOnly)
$ErrorActionPreference='Stop'
$repoRoot=Split-Path $PSScriptRoot -Parent
$verificationDirectory=Join-Path $repoRoot ('artifacts\verification\'+[Guid]::NewGuid().ToString('N'))
& (Join-Path $PSScriptRoot 'Build.ps1') -OutputDirectory $verificationDirectory
$vswhere=Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
$msbuild=& $vswhere -latest -products '*' -find 'MSBuild\**\Bin\MSBuild.exe' | Select-Object -First 1
& $msbuild (Join-Path $repoRoot 'tests\Capstone.RegressionTests.csproj') /t:Rebuild "/p:CapstoneAssembly=$(Join-Path $verificationDirectory 'CapstoneProject.exe')" /p:UseSharedCompilation=false /verbosity:minimal /nologo
if ($LASTEXITCODE -ne 0) { throw 'Test runner build failed.' }
$arguments=@($repoRoot)
if($UnitOnly) { $arguments+='--unit-only' }
& (Join-Path $repoRoot 'tests\bin\Capstone.RegressionTests.exe') @arguments | Tee-Object -FilePath (Join-Path $verificationDirectory 'test-results.txt')
if ($LASTEXITCODE -ne 0) { throw 'Regression tests failed.' }
