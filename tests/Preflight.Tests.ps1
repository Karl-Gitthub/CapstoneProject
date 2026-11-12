[CmdletBinding()]
param([Parameter(Mandatory=$true)][string]$TestRoot)
$ErrorActionPreference='Stop'
. (Join-Path (Split-Path $PSScriptRoot -Parent) 'scripts\Preflight.ps1')
$passed=0
function Assert-PreflightTest([bool]$condition,[string]$name) { if (!$condition) { throw "FAIL $name" }; $script:passed++; Write-Output "PASS $name" }
function Reject-Preflight([scriptblock]$action,[string]$message) {
    try { & $action; } catch { if ($_.Exception.Message -like $message) { return }; throw }
    throw 'Expected preflight rejection.'
}
$TestRoot=[IO.Path]::GetFullPath($TestRoot)
if(Test-Path -LiteralPath $TestRoot) { throw 'Choose a new preflight test directory.' }
New-Item -ItemType Directory -Path $TestRoot | Out-Null
$fresh=Join-Path $TestRoot 'fresh'
Assert-DatabasePrerequisites -DataDirectory $fresh
Assert-PreflightTest (!(Test-Path -LiteralPath $fresh) -and @(Get-ChildItem -LiteralPath $TestRoot -Force).Count -eq 0) 'successful preflight leaves no directories or probe files behind'
$providerCheck=(Get-Item Function:\Get-LaunchProviders).ScriptBlock
try {
    function Get-LaunchProviders { }
    Reject-Preflight { Assert-DatabasePrerequisites -DataDirectory $fresh } 'The required 64-bit Microsoft ACE OleDb provider is missing*'
    Assert-PreflightTest (!(Test-Path -LiteralPath $fresh)) 'missing ACE provider fails before creating a database directory'
    function Get-LaunchProviders { throw 'Provider enumeration failed' }
    Reject-Preflight { Assert-DatabasePrerequisites -DataDirectory $fresh } 'The ACE database provider could not be checked*'
    Assert-PreflightTest $true 'provider discovery failures give an actionable message'
} finally { Set-Item Function:\Get-LaunchProviders -Value $providerCheck }
$long=Join-Path $TestRoot (('x'*80)+'\'+('y'*80)+'\'+('z'*80))
Reject-Preflight { Assert-LaunchFolder -Path $long -DatabaseFiles } 'The selected path is too long*'
Assert-PreflightTest (!(Test-Path -LiteralPath (Join-Path $TestRoot ('x'*80)))) 'overlong database backup paths fail before setup'
$filePath=Join-Path $TestRoot 'a-file.txt'; Set-Content -LiteralPath $filePath -Value 'preserve'
Reject-Preflight { Assert-LaunchFolder -Path (Join-Path $filePath 'data') } 'The selected data/output path includes a file*'
Assert-PreflightTest ((Get-Content -LiteralPath $filePath -Raw).Trim() -eq 'preserve') 'file-versus-folder mistakes preserve the existing file'
$databaseDirectory=Join-Path $TestRoot 'database'
New-Item -ItemType Directory -Path $databaseDirectory | Out-Null
$database=Join-Path $databaseDirectory 'fixture.accdb'; Set-Content -LiteralPath $database -Value 'disposable file-access fixture'
$before=(Get-FileHash -LiteralPath $database).Hash
try {
    (Get-Item -LiteralPath $database).IsReadOnly=$true
    Reject-Preflight { Assert-LaunchFolder -Path $databaseDirectory -DatabaseFiles } 'A selected database is read-only*'
    Assert-PreflightTest ($before -eq (Get-FileHash -LiteralPath $database).Hash) 'read-only database is detected without changing contents'
} finally { (Get-Item -LiteralPath $database).IsReadOnly=$false }
$lock=[IO.File]::Open($database,[IO.FileMode]::Open,[IO.FileAccess]::ReadWrite,[IO.FileShare]::None)
try {
    Reject-Preflight { Assert-LaunchFolder -Path $databaseDirectory -DatabaseFiles } 'A selected database cannot be opened for updates*'
    Assert-PreflightTest $true 'exclusively locked database receives a recovery message'
} finally { $lock.Dispose() }
Assert-PreflightTest ($before -eq (Get-FileHash -LiteralPath $database).Hash) 'preflight preserves existing database bytes'
Write-Output "$passed prerequisite/path checks passed. Only disposable files were used."
