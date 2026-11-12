# Shared implementation for the demo launch/reset commands.
$demoRepoRoot=[IO.Path]::GetFullPath((Split-Path $PSScriptRoot -Parent))
$demoMarker='CapstoneProject demo session v1'
function Assert-DemoPath([string]$path,[string]$root) {
    $absolute=[IO.Path]::GetFullPath($path).TrimEnd('\')
    $boundary=[IO.Path]::GetFullPath($root).TrimEnd('\')
    if (!$absolute.Equals($boundary,[StringComparison]::OrdinalIgnoreCase) -and !$absolute.StartsWith($boundary+'\',[StringComparison]::OrdinalIgnoreCase)) { throw 'Demo path is outside its session directory.' }
    $cursor=$absolute
    while($cursor -and $cursor.Length -ge $demoRepoRoot.Length) {
        if ((Test-Path -LiteralPath $cursor) -and ((Get-Item -LiteralPath $cursor -Force).Attributes -band [IO.FileAttributes]::ReparsePoint)) { throw 'Demo paths cannot contain symbolic links or junctions.' }
        if ($cursor.Equals($demoRepoRoot,[StringComparison]::OrdinalIgnoreCase)) { break }
        $cursor=Split-Path $cursor -Parent
    }
}
function Get-DemoLayout([string]$name='presentation') {
    if ($name -notmatch '^[A-Za-z0-9][A-Za-z0-9_-]{0,39}$') { throw 'Use a demo name of 1-40 letters, numbers, hyphens or underscores.' }
    $root=[IO.Path]::GetFullPath((Join-Path $demoRepoRoot ('artifacts\demo\'+$name)))
    Assert-DemoPath $root (Join-Path $demoRepoRoot 'artifacts\demo')
    return [pscustomobject]@{ Root=$root; Data=(Join-Path $root 'data'); Marker=(Join-Path $root '.demo-session'); Backups=(Join-Path $root 'backups'); ManagementLock=(Join-Path $root '.demo-management.lock'); RuntimeLock=(Join-Path $root '.demo-in-use.lock') }
}
function Open-DemoManagement($layout,[switch]$Create) {
    Assert-DemoPath $layout.Root $layout.Root
    if (!(Test-Path -LiteralPath $layout.Root)) {
        if (!$Create) { throw 'This demo does not exist. Run scripts/Start-Demo.ps1 first.' }
        New-Item -ItemType Directory -Path $layout.Root -Force | Out-Null
        Set-Content -LiteralPath $layout.Marker -Value $demoMarker -Encoding UTF8
    }
    if (!(Test-Path -LiteralPath $layout.Marker -PathType Leaf) -or (Get-Content -LiteralPath $layout.Marker -Raw).Trim() -ne $demoMarker) { throw 'This is not a recognized demo session. Existing files were preserved.' }
    Assert-DemoPath $layout.ManagementLock $layout.Root
    try { return [IO.File]::Open($layout.ManagementLock,[IO.FileMode]::OpenOrCreate,[IO.FileAccess]::ReadWrite,[IO.FileShare]::None) }
    catch [IO.IOException] { throw 'Another demo setup or reset is running. Wait for it to finish.' }
}
function Open-DemoResetGuard($layout) {
    Assert-DemoPath $layout.RuntimeLock $layout.Root
    try { return [IO.File]::Open($layout.RuntimeLock,[IO.FileMode]::OpenOrCreate,[IO.FileAccess]::ReadWrite,[IO.FileShare]::None) }
    catch [IO.IOException] { throw 'Close all demo application and administrator windows before preparing or resetting this demo.' }
}
function Assert-DemoData($layout) {
    Assert-DemoPath $layout.Data $layout.Root
    $manifestPath=Join-Path $layout.Data 'synthetic-seed.json'
    if (!(Test-Path -LiteralPath $manifestPath -PathType Leaf)) { throw 'The demo data is incomplete or unrecognized. Existing files were preserved.' }
    $manifest=Get-Content -LiteralPath $manifestPath -Raw | ConvertFrom-Json
    if ($manifest.kind -ne 'synthetic-capstone-seed' -or $manifest.schemaOnly -ne $false) { throw 'The demo data is not a complete sample dataset. Existing files were preserved.' }
    foreach($file in @('dbStudentManagement.accdb','dbUserManagement.accdb','db_GradingManagement.accdb','dbCourseManagement.accdb','db_Temp_RegForm.accdb')) {
        $path=Join-Path $layout.Data $file
        Assert-DemoPath $path $layout.Root
        if (!(Test-Path -LiteralPath $path -PathType Leaf)) { throw 'The demo data is incomplete. Existing files were preserved.' }
    }
}
function New-DemoData([string]$path,[string]$applicationPath) {
    $previousDemoRoot=$env:CAPSTONE_DEMO_ROOT
    try {
        # A staged generation is not the running session's data directory.
        $env:CAPSTONE_DEMO_ROOT=$null
        & (Join-Path $PSScriptRoot 'New-SyntheticData.ps1') -OutputDirectory $path -ApplicationPath $applicationPath
    } finally { $env:CAPSTONE_DEMO_ROOT=$previousDemoRoot }
}
function Initialize-DemoData($layout,[string]$applicationPath) {
    Assert-DemoPath $layout.Data $layout.Root
    if (!(Test-Path -LiteralPath $layout.Data)) {
        # Migrate a staged dataset outside the active 'data' path, then activate
        # it under the caller's exclusive guard. Auto-detection also applies to migration.
        $candidate=Join-Path $layout.Root ('.new-'+[Guid]::NewGuid().ToString('N').Substring(0,12))
        Assert-DemoPath $candidate $layout.Root
        New-DemoData $candidate $applicationPath
        Assert-DemoPath $layout.Data $layout.Root
        [IO.Directory]::Move($candidate,$layout.Data)
    }
    Assert-DemoData $layout
}
function Reset-DemoData($layout,[string]$applicationPath) {
    Assert-DemoData $layout
    $stamp=[DateTime]::UtcNow.ToString('yyyyMMdd-HHmmss')+'-'+[Guid]::NewGuid().ToString('N').Substring(0,8)
    # Keep staging short: .NET Framework/ACE and migration backup filenames
    # otherwise exceed Windows path limits in a nested repository checkout.
    $candidate=Join-Path $layout.Root ('.new-'+[Guid]::NewGuid().ToString('N').Substring(0,12))
    $backup=Join-Path $layout.Backups $stamp
    Assert-DemoPath $candidate $layout.Root
    Assert-DemoPath $backup $layout.Root
    # Generation must succeed before the existing demo is moved.
    New-DemoData $candidate $applicationPath
    New-Item -ItemType Directory -Path $layout.Backups -Force | Out-Null
    # Recheck resolved absolute move targets inside this exact session. No deletion.
    Assert-DemoPath $layout.Data $layout.Root
    Assert-DemoPath $candidate $layout.Root
    Assert-DemoPath $backup $layout.Root
    [IO.Directory]::Move($layout.Data,$backup)
    try { [IO.Directory]::Move($candidate,$layout.Data) }
    catch {
        [IO.Directory]::Move($backup,$layout.Data)
        throw
    }
    Write-Output "Previous demo archived: $backup"
    Write-Output 'Fresh samples restored. Demo accounts were reset; choose new administrator credentials on the next launch.'
}
function Get-DemoAdministratorCount($layout) {
    $builder=New-Object System.Data.OleDb.OleDbConnectionStringBuilder
    $builder.Provider='Microsoft.ACE.OLEDB.12.0'
    $builder['Data Source']=[string](Join-Path $layout.Data 'dbUserManagement.accdb')
    $connection=New-Object System.Data.OleDb.OleDbConnection($builder.ConnectionString)
    try {
        $connection.Open(); $command=$connection.CreateCommand()
        try { $command.CommandText="SELECT COUNT(*) FROM tbl_UserLogin WHERE login_usertype='ADMINISTRATOR'"; return [int]$command.ExecuteScalar() } finally { $command.Dispose() }
    } finally { $connection.Dispose() }
}
