[CmdletBinding()]
param([ValidateSet('Debug','Release')][string]$Configuration='Debug',[string]$OutputDirectory)
$ErrorActionPreference='Stop'
$repoRoot=Split-Path $PSScriptRoot -Parent
$vswhere=Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
if (!(Test-Path -LiteralPath $vswhere)) { throw 'Install Visual Studio Build Tools with the .NET desktop workload and .NET Framework 4.7.2 targeting pack.' }
$msbuild=& $vswhere -latest -products '*' -find 'MSBuild\**\Bin\MSBuild.exe' | Select-Object -First 1
if (!$msbuild) { throw 'MSBuild was not found. Install the .NET desktop workload.' }
$buildArguments=@('/t:Build',"/p:Configuration=$Configuration",'/p:Platform=x64','/p:UseSharedCompilation=false','/verbosity:minimal','/nologo')
if($OutputDirectory) { $buildArguments+="/p:OutDir=$([IO.Path]::GetFullPath($OutputDirectory).TrimEnd('\'))\" }
& $msbuild (Join-Path $repoRoot 'CapstoneProject.sln') @buildArguments
if ($LASTEXITCODE -ne 0) { throw 'Capstone build failed.' }
