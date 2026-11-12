[CmdletBinding()]
param([Parameter(Mandatory=$true)][string]$ApplicationPath,[Parameter(Mandatory=$true)][string]$TestRoot)
$ErrorActionPreference='Stop'
$repoRoot=Split-Path $PSScriptRoot -Parent
$initialize=Join-Path $repoRoot 'scripts\Initialize-LocalData.ps1'
$passed=0
function Assert-Setup([bool]$condition,[string]$name) {
    if (!$condition) { throw "FAIL $name" }
    $script:passed++
    Write-Output "PASS $name"
}
function Count-Rows([string]$directory,[string]$file,[string]$table) {
    $builder=New-Object System.Data.OleDb.OleDbConnectionStringBuilder
    $builder.Provider='Microsoft.ACE.OLEDB.12.0'
    $builder['Data Source']=[string](Join-Path $directory $file)
    $connection=New-Object System.Data.OleDb.OleDbConnection($builder.ConnectionString)
    try {
        $connection.Open()
        $command=$connection.CreateCommand()
        try { $command.CommandText='SELECT COUNT(*) FROM ['+$table+']'; return [int]$command.ExecuteScalar() } finally { $command.Dispose() }
    } finally { $connection.Dispose() }
}
function Expect-Rejection([scriptblock]$action,[string]$message) {
    try { & $action | Out-Null } catch { if ($_.Exception.Message -like $message) { return }; throw }
    throw 'Setup accepted an unsafe destination.'
}
$TestRoot=[IO.Path]::GetFullPath($TestRoot)
if(Test-Path -LiteralPath $TestRoot) { throw 'Use a new test directory.' }
New-Item -ItemType Directory -Path $TestRoot | Out-Null
$previousDataDirectory=$env:CAPSTONE_DATA_DIR
try {
    $fresh=Join-Path $TestRoot 'fresh'
    & $initialize -DataDirectory $fresh -ApplicationPath $ApplicationPath | Out-Null
    Assert-Setup (@(Get-ChildItem -LiteralPath $fresh -Filter '*.accdb').Count -eq 5 -and (Test-Path -LiteralPath (Join-Path $fresh 'synthetic-seed.json'))) 'fresh setup generates all databases without repository binaries'
    Assert-Setup ((Count-Rows $fresh 'dbUserManagement.accdb' 'tbl_UserLogin') -eq 0) 'fresh setup creates no accounts or default credentials'
    Assert-Setup ((Count-Rows $fresh 'dbStudentManagement.accdb' 'tbl_BSITStud_Detail') -eq 2 -and (Count-Rows $fresh 'dbCourseManagement.accdb' 'tbl_BSITCourseDetail') -eq 2) 'fresh setup includes only the expected sample records'

    $connection=New-Object System.Data.OleDb.OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=$(Join-Path $fresh 'db_GradingManagement.accdb')")
    try {
        $connection.Open(); $command=$connection.CreateCommand()
        try { $command.CommandText="INSERT INTO tbl_FacultyDetail(fac_id,fac_fname,fac_lname) VALUES ('SETUP-KEEP','Local','Preserved')"; [void]$command.ExecuteNonQuery() } finally { $command.Dispose() }
    } finally { $connection.Dispose() }
    & $initialize -DataDirectory $fresh -ApplicationPath $ApplicationPath | Out-Null
    Assert-Setup ((Count-Rows $fresh 'db_GradingManagement.accdb' 'tbl_FacultyDetail') -eq 3 -and (Count-Rows $fresh 'dbStudentManagement.accdb' 'tbl_BSITStud_Detail') -eq 2) 'repeat setup preserves existing edits without duplicating sample rows'

    $empty=Join-Path $TestRoot 'empty'
    New-Item -ItemType Directory -Path $empty | Out-Null
    & $initialize -DataDirectory $empty -ApplicationPath $ApplicationPath | Out-Null
    Assert-Setup ((Count-Rows $empty 'dbUserManagement.accdb' 'tbl_UserLogin') -eq 0 -and (Test-Path -LiteralPath (Join-Path $empty 'synthetic-seed.json'))) 'existing empty destination receives a complete synthetic set'

    $partial=Join-Path $TestRoot 'partial'
    New-Item -ItemType Directory -Path $partial | Out-Null
    $partialFile=Join-Path $partial 'dbUserManagement.accdb'
    Copy-Item -LiteralPath (Join-Path $fresh 'dbUserManagement.accdb') -Destination $partialFile
    $before=(Get-FileHash -LiteralPath $partialFile).Hash
    Expect-Rejection { & $initialize -DataDirectory $partial -ApplicationPath $ApplicationPath } 'The working database set is incomplete*'
    Assert-Setup ($before -eq (Get-FileHash -LiteralPath $partialFile).Hash -and @(Get-ChildItem -LiteralPath $partial -Force).Count -eq 1) 'partial working sets are rejected without adding or replacing files'

    $nonempty=Join-Path $TestRoot 'nonempty'
    New-Item -ItemType Directory -Path $nonempty | Out-Null
    Set-Content -LiteralPath (Join-Path $nonempty 'keep.txt') -Value 'Keep this file.'
    Expect-Rejection { & $initialize -DataDirectory $nonempty -ApplicationPath $ApplicationPath } 'Choose an empty directory*'
    Assert-Setup (@(Get-ChildItem -LiteralPath $nonempty -Force).Count -eq 1) 'unrelated files in a destination are preserved'

    Expect-Rejection { & $initialize -DataDirectory (Join-Path $repoRoot 'New folder') -ApplicationPath $ApplicationPath } 'Choose a working directory outside*'
    Assert-Setup $true 'legacy source folder cannot become a working database'
    $invalid=Join-Path $TestRoot 'missing-build-output'
    Expect-Rejection { & $initialize -DataDirectory $invalid -ApplicationPath (Join-Path $TestRoot 'missing.exe') } 'Build the current application*'
    Assert-Setup (!(Test-Path -LiteralPath $invalid)) 'missing executable fails before creating data'
    Write-Output "$passed local setup checks passed. All databases were disposable."
} finally { $env:CAPSTONE_DATA_DIR=$previousDataDirectory }
