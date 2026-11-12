[CmdletBinding()]
param([Parameter(Mandatory=$true)][string]$ApplicationPath)
$ErrorActionPreference='Stop'
. (Join-Path (Split-Path $PSScriptRoot -Parent) 'scripts\Demo-Session.ps1')
$ApplicationPath=[IO.Path]::GetFullPath($ApplicationPath)
$name='test-'+[Guid]::NewGuid().ToString('N')
$layout=Get-DemoLayout $name
$passed=0
function Assert-DemoTest([bool]$condition,[string]$description) {
    if (!$condition) { throw "FAIL $description" }
    $script:passed++; Write-Output "PASS $description"
}
function Reject-DemoAction([scriptblock]$action,[string]$message) {
    try { & $action | Out-Null } catch { if ($_.Exception.Message -like $message) { return }; throw }
    throw 'Expected demo operation to be rejected.'
}
function Demo-Count([string]$directory,[string]$table) {
    $builder=New-Object System.Data.OleDb.OleDbConnectionStringBuilder
    $builder.Provider='Microsoft.ACE.OLEDB.12.0'
    $builder['Data Source']=[string](Join-Path $directory 'db_GradingManagement.accdb')
    $connection=New-Object System.Data.OleDb.OleDbConnection($builder.ConnectionString)
    try { $connection.Open(); $command=$connection.CreateCommand(); try { $command.CommandText='SELECT COUNT(*) FROM ['+$table+']'; return [int]$command.ExecuteScalar() } finally { $command.Dispose() } } finally { $connection.Dispose() }
}
$previousData=$env:CAPSTONE_DATA_DIR
$previousDemo=$env:CAPSTONE_DEMO_ROOT
try {
    Reject-DemoAction { Get-DemoLayout '..\working-data' } 'Use a demo name*'
    Assert-DemoTest $true 'demo names reject path traversal'
    Reject-DemoAction { Assert-DemoPath (Join-Path $demoRepoRoot 'CapstoneProject\bin\x64\Debug\Data') $layout.Root } 'Demo path is outside*'
    Assert-DemoTest $true 'demo move boundaries reject the working-data directory'
    $unknown=Get-DemoLayout ('unknown-'+[Guid]::NewGuid().ToString('N'))
    New-Item -ItemType Directory -Path $unknown.Root -Force | Out-Null
    Set-Content -LiteralPath (Join-Path $unknown.Root 'keep.txt') -Value 'Preserve unrelated files'
    Reject-DemoAction { Open-DemoManagement $unknown -Create } 'This is not a recognized demo session*'
    Assert-DemoTest (@(Get-ChildItem -LiteralPath $unknown.Root -Force).Count -eq 1) 'unmarked demo directories are refused without replacing files'
    $missing=Get-DemoLayout ('missing-'+[Guid]::NewGuid().ToString('N'))
    Reject-DemoAction { Open-DemoManagement $missing } 'This demo does not exist*'
    Assert-DemoTest (!(Test-Path -LiteralPath $missing.Root)) 'reset of a missing session does not create data'

    $management=Open-DemoManagement $layout -Create
    try {
        $guard=Open-DemoResetGuard $layout
        try {
            $env:CAPSTONE_DEMO_ROOT='preserve-parent-demo-environment'
            Initialize-DemoData $layout $ApplicationPath | Out-Null
            Assert-DemoTest ((Get-DemoAdministratorCount $layout) -eq 0 -and (Demo-Count $layout.Data 'tbl_FacultyDetail') -eq 2) 'dedicated demo contains samples and no default account'
            Assert-DemoTest ($env:CAPSTONE_DEMO_ROOT -eq 'preserve-parent-demo-environment') 'demo generation restores its caller environment'
            $env:CAPSTONE_DEMO_ROOT=$null
            $env:CAPSTONE_DATA_DIR=$layout.Data
            [void][Reflection.Assembly]::LoadFrom($ApplicationPath)
            [CapstoneProject.Services.AccountAdministration]::Bootstrap('demo.test.admin','Disposable demo administrator','TestDemoOnly2026!')
            Initialize-DemoData $layout $ApplicationPath | Out-Null
            Assert-DemoTest ((Get-DemoAdministratorCount $layout) -eq 1) 'reopening a prepared demo preserves its chosen administrator'
            $before=(Get-FileHash -LiteralPath (Join-Path $layout.Data 'dbUserManagement.accdb')).Hash
            Reject-DemoAction { Reset-DemoData $layout (Join-Path $layout.Root 'missing.exe') } 'Build the application first*'
            Assert-DemoTest ($before -eq (Get-FileHash -LiteralPath (Join-Path $layout.Data 'dbUserManagement.accdb')).Hash) 'failed replacement generation leaves the existing demo untouched'
        } finally { $guard.Dispose() }
    } finally { $management.Dispose() }

    # Exercise the reset entry point itself while a client-style lifetime lease is held.
    $client=[IO.File]::Open($layout.RuntimeLock,[IO.FileMode]::Open,[IO.FileAccess]::ReadWrite,[IO.FileShare]::ReadWrite)
    try {
        Reject-DemoAction { & (Join-Path $demoRepoRoot 'scripts\Reset-Demo.ps1') -Name $name } 'Close all demo application*'
        Assert-DemoTest ((Get-DemoAdministratorCount $layout) -eq 1) 'reset command refuses an open demo without changing accounts'
    } finally { $client.Dispose() }

    $management=Open-DemoManagement $layout
    try {
        $guard=Open-DemoResetGuard $layout
        try {
            Reset-DemoData $layout $ApplicationPath | Out-Null
            $backups=@(Get-ChildItem -LiteralPath $layout.Backups -Directory)
            Assert-DemoTest ($backups.Count -eq 1 -and (Get-DemoAdministratorCount $layout) -eq 0 -and (Demo-Count $layout.Data 'tbl_FacultyDetail') -eq 2) 'reset restores sample data and removes only demo accounts from the active set'
            $archive=[pscustomobject]@{ Data=$backups[0].FullName }
            Assert-DemoTest ((Get-DemoAdministratorCount $archive) -eq 1) 'reset archive preserves the previous administrator database'
            Reset-DemoData $layout $ApplicationPath | Out-Null
            Assert-DemoTest (@(Get-ChildItem -LiteralPath $layout.Backups -Directory).Count -eq 2 -and (Get-DemoAdministratorCount $layout) -eq 0) 'repeated resets keep separate archives'
        } finally { $guard.Dispose() }
    } finally { $management.Dispose() }
    Write-Output "$passed demo script checks passed. Only uniquely named disposable demo sessions were used."
} finally {
    $env:CAPSTONE_DATA_DIR=$previousData
    $env:CAPSTONE_DEMO_ROOT=$previousDemo
}
