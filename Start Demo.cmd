@echo off
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%~dp0scripts\Start-Demo.ps1"
if errorlevel 1 (
    echo Demo startup failed. Review the message above.
    pause
)
