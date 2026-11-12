@echo off
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%~dp0scripts\Start-Local.ps1"
if errorlevel 1 (
    echo Application startup failed. Review the message above.
    pause
)
