[CmdletBinding()]
param([string]$DataDirectory,[string]$ApplicationPath)
$ErrorActionPreference='Stop'
$repoRoot=Split-Path $PSScriptRoot -Parent
if (!$DataDirectory) { $DataDirectory=Join-Path $repoRoot 'CapstoneProject\bin\x64\Debug\Data' }
$DataDirectory=[IO.Path]::GetFullPath($DataDirectory)
$sourceDirectory=[IO.Path]::GetFullPath((Join-Path $repoRoot 'New folder'))
if ($DataDirectory.TrimEnd('\') -eq $sourceDirectory.TrimEnd('\') -or $DataDirectory.StartsWith($sourceDirectory+'\',[StringComparison]::OrdinalIgnoreCase)) { throw 'Choose a working directory outside the source database folder.' }
New-Item -ItemType Directory -Path $DataDirectory -Force | Out-Null
foreach ($filename in @('dbStudentManagement.accdb','dbUserManagement.accdb','db_GradingManagement.accdb','dbCourseManagement.accdb','db_Temp_RegForm.accdb')) {
    $target=Join-Path $DataDirectory $filename
    if (!(Test-Path -LiteralPath $target)) { Copy-Item -LiteralPath (Join-Path $sourceDirectory $filename) -Destination $target }
}
# Run under 64-bit Windows PowerShell 5.1 with a matching ACE provider.
$connection=New-Object System.Data.OleDb.OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=$(Join-Path $DataDirectory 'db_GradingManagement.accdb')")
try {
    $connection.Open()
    $column=$connection.GetSchema('Columns') | Where-Object { $_.TABLE_NAME -eq 'tbl_Course' -and $_.COLUMN_NAME -eq 'average' }
    $needsMigration=$column.DATA_TYPE -ne 5
} finally { $connection.Dispose() }
if ($needsMigration) {
    $gradePath=Join-Path $DataDirectory 'db_GradingManagement.accdb'
    $backupPath=$gradePath+'.before-decimal-'+[Guid]::NewGuid().ToString('N')+'.bak'
    Copy-Item -LiteralPath $gradePath -Destination $backupPath
    $connection=New-Object System.Data.OleDb.OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=$gradePath")
    try { $connection.Open(); $command=$connection.CreateCommand(); $command.CommandText='ALTER TABLE tbl_Course ALTER COLUMN average DOUBLE'; [void]$command.ExecuteNonQuery(); $command.Dispose() } finally { $connection.Dispose() }
}
# The source schema already has a primary key on studid. Preserve its relationships.
$env:CAPSTONE_DATA_DIR=$DataDirectory
if(!$ApplicationPath){$ApplicationPath=Join-Path $repoRoot 'CapstoneProject\bin\x64\Debug\CapstoneProject.exe'}
if(!(Test-Path -LiteralPath $ApplicationPath)){throw 'Build the current application before initializing the workflow schema.'}
$migration=Start-Process -FilePath $ApplicationPath -ArgumentList '--migrate-only' -WindowStyle Hidden -PassThru -Wait
if($migration.ExitCode -ne 0){throw 'Workflow migration failed. Close all database users and rerun setup. Pre-migration backups were preserved.'}
Write-Output "Working databases ready: $DataDirectory"
Write-Output 'Original repository databases were not changed. Existing working copies were preserved.'
