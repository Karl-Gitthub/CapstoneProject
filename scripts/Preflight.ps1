# Read-only dependency checks and short-lived write probes. No database contents change.
function Assert-LaunchHost {
    if ([Environment]::OSVersion.Platform -ne [PlatformID]::Win32NT) { throw 'This application requires Windows.' }
    if (![Environment]::Is64BitProcess) { throw 'Open 64-bit Windows PowerShell and try again. The application and ACE provider must both be x64.' }
    if ($PSVersionTable.PSVersion -lt [version]'5.1') { throw 'Use Windows PowerShell 5.1 or later. The CMD launchers select Windows PowerShell automatically.' }
}
function Assert-BuildPrerequisites {
    Assert-LaunchHost
    $targetPack=Join-Path ${env:ProgramFiles(x86)} 'Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.2\mscorlib.dll'
    if (!(Test-Path -LiteralPath $targetPack -PathType Leaf)) { throw 'Install the .NET Framework 4.7.2 targeting pack through Visual Studio Installer (.NET desktop development), then try again.' }
}
function Get-LaunchProviders {
    $reader=[System.Data.OleDb.OleDbEnumerator]::GetRootEnumerator()
    try { while($reader.Read()) { [string]$reader['SOURCES_NAME'] } } finally { $reader.Dispose() }
}
function Assert-LaunchFolder([string]$Path,[switch]$DatabaseFiles) {
    $absolute=[IO.Path]::GetFullPath($Path)
    $suffix=if($DatabaseFiles){'dbStudentManagement.accdb.before-workspaces-'+('0'*32)+'.bak'}else{'CapstoneProject.exe.config'}
    if ((Join-Path $absolute $suffix).Length -ge 260) { throw 'The selected path is too long for the desktop database tools. Move the project or choose a shorter data/output directory before continuing.' }
    $ancestor=$absolute
    while(!(Test-Path -LiteralPath $ancestor -PathType Container)) {
        if (Test-Path -LiteralPath $ancestor) { throw 'The selected data/output path includes a file where a folder is required.' }
        $ancestor=Split-Path $ancestor -Parent
        if (!$ancestor) { throw 'Choose an existing drive and a valid data/output folder.' }
    }
    $probe=Join-Path $ancestor ('.capstone-write-'+[Guid]::NewGuid().ToString('N')+'.tmp')
    try {
        $stream=[IO.FileStream]::new($probe,[IO.FileMode]::CreateNew,[IO.FileAccess]::ReadWrite,[IO.FileShare]::None,4096,[IO.FileOptions]::DeleteOnClose)
        $stream.Dispose()
    } catch { throw 'The selected data/output location is not writable. Choose a folder you can modify; do not install working data in a protected system folder.' }
    if ($DatabaseFiles -and (Test-Path -LiteralPath $absolute -PathType Container)) {
        foreach($file in Get-ChildItem -LiteralPath $absolute -Filter '*.accdb' -File) {
            if ($file.Attributes -band [IO.FileAttributes]::ReadOnly) { throw 'A selected database is read-only. Use a writable working copy before starting the application.' }
            try { $stream=[IO.File]::Open($file.FullName,[IO.FileMode]::Open,[IO.FileAccess]::ReadWrite,[IO.FileShare]::ReadWrite); $stream.Dispose() }
            catch { throw 'A selected database cannot be opened for updates. Check its permissions and close applications holding an exclusive lock.' }
        }
    }
}
function Assert-DatabasePrerequisites([string]$DataDirectory) {
    Assert-LaunchHost
    try { $providers=@(Get-LaunchProviders) } catch { throw 'The ACE database provider could not be checked. Use 64-bit Windows PowerShell with a compatible 64-bit ACE OleDb provider installed.' }
    if ($providers -notcontains 'Microsoft.ACE.OLEDB.12.0') { throw 'The required 64-bit Microsoft ACE OleDb provider is missing. Install/register a compatible x64 ACE provider before starting. Installing Visual Studio alone does not supply it.' }
    Assert-LaunchFolder -Path $DataDirectory -DatabaseFiles
}
