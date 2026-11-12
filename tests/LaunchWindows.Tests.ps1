[CmdletBinding()]
param([Parameter(Mandatory=$true)][string]$TestRoot)
$ErrorActionPreference='Stop'
$repoRoot=Split-Path $PSScriptRoot -Parent
if (!("CapstoneWindowProbe" -as [type])) {
    Add-Type -TypeDefinition @'
using System;
using System.Runtime.InteropServices;
public static class CapstoneWindowProbe {
    [DllImport("user32.dll")] public static extern bool IsWindowVisible(IntPtr window);
}
'@
}
function Assert-VisibleWindow($process,[string]$title,[scriptblock]$Inspection) {
    try {
        $timer=[Diagnostics.Stopwatch]::StartNew()
        do {
            $process.Refresh()
            if ($process.HasExited) { throw 'The launched application exited before showing a window.' }
            if ($process.MainWindowHandle -ne [IntPtr]::Zero -and [CapstoneWindowProbe]::IsWindowVisible($process.MainWindowHandle)) { break }
            Start-Sleep -Milliseconds 50
        } while ($timer.Elapsed.TotalSeconds -lt 10)
        if ($process.MainWindowHandle -eq [IntPtr]::Zero -or ![CapstoneWindowProbe]::IsWindowVisible($process.MainWindowHandle) -or $process.MainWindowTitle -notlike $title) { throw 'The expected application window was not visible.' }
        if ($Inspection) { & $Inspection }
    } finally {
        # Only close the process returned by this test's own launch command.
        $process.Refresh()
        if (!$process.HasExited) { [void]$process.CloseMainWindow(); if (!$process.WaitForExit(5000)) { $process.Kill(); $process.WaitForExit(); throw 'Closing setup did not exit cleanly; an unusable login or another window may have opened.' } }
        $process.Dispose()
    }
}
$data=Join-Path ([IO.Path]::GetFullPath($TestRoot)) 'data'
$previousData=$env:CAPSTONE_DATA_DIR
$previousDemo=$env:CAPSTONE_DEMO_ROOT
try {
    $env:CAPSTONE_DEMO_ROOT=$null
    $output=@(& (Join-Path $repoRoot 'scripts\Start-Local.ps1') -DataDirectory $data -PassThru)
    $processes=@($output | Where-Object { $_ -is [Diagnostics.Process] })
    if ($processes.Count -ne 1) { throw 'The local launcher did not return its application process.' }
    Assert-VisibleWindow $processes[0] 'Provision first administrator*'
    Write-Output 'PASS fresh main launch opens visible administrator setup and cancel exits'
    $output=@(& (Join-Path $repoRoot 'scripts\Provision-Admin.ps1') -DataDirectory $data -PassThru)
    $processes=@($output | Where-Object { $_ -is [Diagnostics.Process] })
    if ($processes.Count -ne 1) { throw 'The administrator launcher did not return its application process.' }
    $applicationPath=$processes[0].Path
    Assert-VisibleWindow $processes[0] 'Provision first administrator*'
    Write-Output 'PASS administrator launcher opens a visible setup window'
    . (Join-Path $repoRoot 'scripts\Demo-Session.ps1')
    $layout=Get-DemoLayout ('window-'+[Guid]::NewGuid().ToString('N').Substring(0,8))
    $management=Open-DemoManagement $layout -Create
    try { $guard=Open-DemoResetGuard $layout; try { Initialize-DemoData $layout $applicationPath | Out-Null } finally { $guard.Dispose() } } finally { $management.Dispose() }
    $env:CAPSTONE_DEMO_ROOT=$null
    $env:CAPSTONE_DATA_DIR=$layout.Data
    $verifyGuard={
        try { $unexpected=Open-DemoResetGuard $layout; $unexpected.Dispose() }
        catch { if ($_.Exception.Message -like 'Close all demo application*') { return }; throw }
        throw 'An alternate entry point did not hold the demo reset guard.'
    }
    $output=@(& (Join-Path $repoRoot 'scripts\Start-Local.ps1') -DataDirectory $layout.Data -PassThru)
    $processes=@($output | Where-Object { $_ -is [Diagnostics.Process] })
    if ($processes.Count -ne 1) { throw 'The alternate demo launch did not return its process.' }
    $applicationPath=$processes[0].Path
    Assert-VisibleWindow $processes[0] 'Demo*' $verifyGuard
    Write-Output 'PASS ordinary launcher detects demo data and prevents reset'
    $process=Start-Process -FilePath $applicationPath -WindowStyle Normal -PassThru
    Assert-VisibleWindow $process 'Demo*' $verifyGuard
    Write-Output 'PASS direct executable detects demo data and prevents reset'
    Write-Output '4 visible-window checks passed. Only test-launched windows were closed.'
} finally {
    $env:CAPSTONE_DATA_DIR=$previousData
    $env:CAPSTONE_DEMO_ROOT=$previousDemo
}
