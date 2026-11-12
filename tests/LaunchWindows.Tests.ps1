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
function Assert-VisibleWindow($process,[string]$title) {
    try {
        $timer=[Diagnostics.Stopwatch]::StartNew()
        do {
            $process.Refresh()
            if ($process.HasExited) { throw 'The launched application exited before showing a window.' }
            if ($process.MainWindowHandle -ne [IntPtr]::Zero -and [CapstoneWindowProbe]::IsWindowVisible($process.MainWindowHandle)) { break }
            Start-Sleep -Milliseconds 50
        } while ($timer.Elapsed.TotalSeconds -lt 10)
        if ($process.MainWindowHandle -eq [IntPtr]::Zero -or ![CapstoneWindowProbe]::IsWindowVisible($process.MainWindowHandle) -or $process.MainWindowTitle -notlike $title) { throw 'The expected application window was not visible.' }
    } finally {
        # Only close the process returned by this test's own launch command.
        $process.Refresh()
        if (!$process.HasExited) { [void]$process.CloseMainWindow(); if (!$process.WaitForExit(5000)) { $process.Kill(); $process.WaitForExit() } }
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
    Assert-VisibleWindow $processes[0] 'Sign in*'
    Write-Output 'PASS local launcher opens a visible sign-in window'
    $output=@(& (Join-Path $repoRoot 'scripts\Provision-Admin.ps1') -DataDirectory $data -PassThru)
    $processes=@($output | Where-Object { $_ -is [Diagnostics.Process] })
    if ($processes.Count -ne 1) { throw 'The administrator launcher did not return its application process.' }
    Assert-VisibleWindow $processes[0] 'Provision first administrator*'
    Write-Output 'PASS administrator launcher opens a visible setup window'
    Write-Output '2 visible-window checks passed. Only test-launched windows were closed.'
} finally {
    $env:CAPSTONE_DATA_DIR=$previousData
    $env:CAPSTONE_DEMO_ROOT=$previousDemo
}
