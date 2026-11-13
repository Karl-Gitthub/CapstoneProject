[CmdletBinding()]
param([string]$Name='presentation')
$ErrorActionPreference='Stop'
. (Join-Path $PSScriptRoot 'Demo-Session.ps1')
. (Join-Path $PSScriptRoot 'Demo-Profiles.ps1')
. (Join-Path $PSScriptRoot 'Preflight.ps1')
$layout=Get-DemoLayout $Name
$management=Open-DemoManagement $layout
try {
    $guard=Open-DemoResetGuard $layout
    try {
        Assert-DemoData $layout
        Assert-DatabasePrerequisites -DataDirectory $layout.Data
        $backup=Join-Path $layout.Backups ('profiles-'+[DateTime]::UtcNow.ToString('yyyyMMdd-HHmmss')+'-'+[Guid]::NewGuid().ToString('N').Substring(0,8))
        Assert-DemoPath $backup $layout.Root
        New-Item -ItemType Directory -Path $backup -Force | Out-Null
        $files=@('dbStudentManagement.accdb','dbUserManagement.accdb','db_GradingManagement.accdb','dbCourseManagement.accdb','db_Temp_RegForm.accdb','synthetic-seed.json')
        foreach($file in $files){Assert-DemoPath (Join-Path $layout.Data $file) $layout.Root;[IO.File]::Copy((Join-Path $layout.Data $file),(Join-Path $backup $file),$false)}
        $accounts=(Get-FileHash -LiteralPath (Join-Path $layout.Data 'dbUserManagement.accdb')).Hash
        try {
            Set-FictionalProfiles -Directory $layout.Data
            if($accounts -ne (Get-FileHash -LiteralPath (Join-Path $layout.Data 'dbUserManagement.accdb')).Hash){throw 'The account database changed unexpectedly.'}
            $manifest=Get-Content -LiteralPath (Join-Path $layout.Data 'synthetic-seed.json') -Raw | ConvertFrom-Json
            $manifest | Add-Member -NotePropertyName profilesVersion -NotePropertyValue 2 -Force
            $manifest.note='Fictional demonstration profiles with masked phone numbers and example.invalid emails. Not actual school records.'
            $manifest | ConvertTo-Json | Set-Content -LiteralPath (Join-Path $layout.Data 'synthetic-seed.json') -Encoding UTF8
        } catch {
            foreach($file in $files){Assert-DemoPath (Join-Path $layout.Data $file) $layout.Root;[IO.File]::Copy((Join-Path $backup $file),(Join-Path $layout.Data $file),$true)}
            throw
        }
        Write-Output "Fictional sample profiles refreshed: $($layout.Data)"
        Write-Output "Previous data preserved: $backup"
        Write-Output 'Accounts, passwords, enrollment IDs, grades and review decisions were preserved. Only SAMPLE student/faculty/course profiles were refreshed.'
    } finally {$guard.Dispose()}
} finally {$management.Dispose()}
