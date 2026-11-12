[CmdletBinding()]
param([string]$DataDirectory,[string]$ApplicationPath)
$ErrorActionPreference='Stop'
$repoRoot=Split-Path $PSScriptRoot -Parent
if (!$DataDirectory) { $DataDirectory=Join-Path $repoRoot 'CapstoneProject\bin\x64\Debug\Data' }
$DataDirectory=[IO.Path]::GetFullPath($DataDirectory)
$DataDirectory=$DataDirectory.TrimEnd('\')
foreach($relative in @('New folder','DB ACCESS FILES')) {
    $legacyDirectory=[IO.Path]::GetFullPath((Join-Path $repoRoot $relative)).TrimEnd('\')
    if ($DataDirectory.Equals($legacyDirectory,[StringComparison]::OrdinalIgnoreCase) -or $DataDirectory.StartsWith($legacyDirectory+'\',[StringComparison]::OrdinalIgnoreCase)) { throw 'Choose a working directory outside the legacy source database folders.' }
}
if ($DataDirectory.Equals($repoRoot.TrimEnd('\'),[StringComparison]::OrdinalIgnoreCase) -or $DataDirectory.Equals((Join-Path $repoRoot 'CapstoneProject'),[StringComparison]::OrdinalIgnoreCase)) { throw 'Choose a separate working data directory.' }
if (!$ApplicationPath) { $ApplicationPath=Join-Path $repoRoot 'CapstoneProject\bin\x64\Debug\CapstoneProject.exe' }
$ApplicationPath=[IO.Path]::GetFullPath($ApplicationPath)
if (!(Test-Path -LiteralPath $ApplicationPath -PathType Leaf)) { throw 'Build the current application before initializing the workflow schema.' }
$filenames=@('dbStudentManagement.accdb','dbUserManagement.accdb','db_GradingManagement.accdb','dbCourseManagement.accdb','db_Temp_RegForm.accdb')
$present=@($filenames | Where-Object { Test-Path -LiteralPath (Join-Path $DataDirectory $_) -PathType Leaf })
if ($present.Count -gt 0 -and $present.Count -lt $filenames.Count) { throw 'The working database set is incomplete. Restore the matching set or choose a new directory. Existing files were preserved.' }
if ($present.Count -eq 0) {
    if (!(Test-Path -LiteralPath $DataDirectory)) {
        & (Join-Path $PSScriptRoot 'New-SyntheticData.ps1') -OutputDirectory $DataDirectory -ApplicationPath $ApplicationPath
    } else {
        if (!(Test-Path -LiteralPath $DataDirectory -PathType Container) -or @(Get-ChildItem -LiteralPath $DataDirectory -Force).Count -ne 0) { throw 'Choose an empty directory or a complete existing database set. Existing files were preserved.' }
        # Stage a fresh set, then copy without overwrite. A partial I/O failure
        # remains detectable as an incomplete set on the next setup attempt.
        $staging=Join-Path $repoRoot ('artifacts\seed-staging\'+[Guid]::NewGuid().ToString('N'))
        & (Join-Path $PSScriptRoot 'New-SyntheticData.ps1') -OutputDirectory $staging -ApplicationPath $ApplicationPath
        foreach($file in @($filenames)+@('synthetic-seed.json')) { [IO.File]::Copy((Join-Path $staging $file),(Join-Path $DataDirectory $file),$false) }
    }
    Write-Output 'New installation: sample records only; no accounts or default password. Run scripts/Provision-Admin.ps1 with the same data directory to create the first administrator.'
} else {
    $previousDataDirectory=$env:CAPSTONE_DATA_DIR
    try {
        $env:CAPSTONE_DATA_DIR=$DataDirectory
        $migration=Start-Process -FilePath $ApplicationPath -ArgumentList '--migrate-only' -WindowStyle Hidden -PassThru -Wait
        if ($migration.ExitCode -ne 0) { throw 'Workflow migration failed. Close database users and rerun setup. Pre-migration backups were preserved.' }
    } finally { $env:CAPSTONE_DATA_DIR=$previousDataDirectory }
    Write-Output 'Existing working databases preserved. No sample records or accounts were added.'
}
$env:CAPSTONE_DATA_DIR=$DataDirectory
Write-Output "Working databases ready: $DataDirectory"
