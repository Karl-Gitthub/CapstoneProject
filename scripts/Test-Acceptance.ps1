[CmdletBinding()]
param()
$ErrorActionPreference='Stop'
$repoRoot=Split-Path $PSScriptRoot -Parent
$run=Join-Path $repoRoot ('artifacts/acceptance/'+[Guid]::NewGuid().ToString('N'))
$build=Join-Path $run 'build'
& (Join-Path $PSScriptRoot 'Build.ps1') -Configuration Release -OutputDirectory $build
$application=Join-Path $build 'CapstoneProject.exe'
$data=Join-Path $run 'data'
& (Join-Path $PSScriptRoot 'New-SyntheticData.ps1') -OutputDirectory $data -SchemaOnly
$vswhere=Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio/Installer/vswhere.exe'
$msbuild=& $vswhere -latest -products '*' -find 'MSBuild\**\Bin\MSBuild.exe' | Select-Object -First 1
$runner=Join-Path $run 'runner'
& $msbuild (Join-Path $repoRoot 'tests/Capstone.AcceptanceRehearsal.csproj') /t:Rebuild "/p:CapstoneAssembly=$application" "/p:OutputPath=$runner/" "/p:IntermediateOutputPath=$run/obj/" /p:UseSharedCompilation=false /verbosity:minimal /nologo
if($LASTEXITCODE -ne 0){throw 'Acceptance rehearsal build failed.'}
$evidence=Join-Path $run 'evidence'
$stdout=Join-Path $run 'rehearsal.log'
$stderr=Join-Path $run 'rehearsal-errors.log'
$process=Start-Process -FilePath (Join-Path $runner 'Capstone.AcceptanceRehearsal.exe') -ArgumentList @(('"'+$data+'"'),('"'+$evidence+'"')) -WindowStyle Hidden -PassThru -RedirectStandardOutput $stdout -RedirectStandardError $stderr
try {
    # Acquire the handle before exit so Windows PowerShell 5.1 retains ExitCode.
    $null=$process.Handle
    if(!$process.WaitForExit(120000)){
        # This object belongs only to the disposable runner launched above.
        $process.Kill(); $process.WaitForExit(); throw "Rehearsal timed out. Inspect $run. Human UAT remains pending."
    }
    $process.WaitForExit()
    Get-Content -LiteralPath $stdout
    if($process.ExitCode -ne 0){Get-Content -LiteralPath $stderr; throw "Rehearsal failed. Inspect $run."}
    Write-Output "Evidence: $evidence"
    Write-Output 'Automated service rehearsal only. Complete docs/UAT-CHECKLIST.csv with actual users before acceptance.'
} finally {$process.Dispose()}
