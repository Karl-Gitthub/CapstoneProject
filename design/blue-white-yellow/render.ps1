[CmdletBinding()]
param([switch]$OverviewOnly,[string]$Only='')
$ErrorActionPreference='Stop'
$designRoot=$PSScriptRoot
$repoRoot=Split-Path (Split-Path $designRoot -Parent) -Parent
$chromePath='C:\Program Files\Google\Chrome\Application\chrome.exe'
if(!(Test-Path -LiteralPath $chromePath)) { throw 'Google Chrome is required to render these SVG mockups.' }
$profilePath=Join-Path $repoRoot 'artifacts\design-browser'
New-Item -ItemType Directory -Path $profilePath -Force | Out-Null
$files=if($OverviewOnly) { @(Get-Item -LiteralPath (Join-Path $designRoot 'overview.svg')) } else { @(Get-ChildItem -LiteralPath (Join-Path $designRoot 'screens') -Filter '*.svg' | Sort-Object Name) }
if($Only) { $wanted=$Only.Split(','); $files=@($files | Where-Object { $wanted -contains $_.BaseName.Substring(0,2) }) }
foreach($screenFile in $files) {
    $imagePath=[IO.Path]::ChangeExtension($screenFile.FullName,'.png')
    $uri=([Uri]$screenFile.FullName).AbsoluteUri
    $size=if($OverviewOnly) {'1800,1960'} else {'1600,1000'}
    $browserArguments=@('--headless=new','--disable-gpu','--no-first-run','--no-default-browser-check','--hide-scrollbars',"--user-data-dir=$profilePath","--window-size=$size",'--force-device-scale-factor=1',"--screenshot=$imagePath",$uri)
    $quotedArguments=$browserArguments | ForEach-Object { '"'+$_+'"' }
    $process=Start-Process -FilePath $chromePath -ArgumentList $quotedArguments -WindowStyle Hidden -PassThru -Wait -RedirectStandardError (Join-Path $profilePath ($screenFile.BaseName+'.log'))
    if($process.ExitCode -ne 0) { throw "Chrome failed to render $($screenFile.Name)." }
    if(!(Test-Path -LiteralPath $imagePath)) { throw "No image was created for $($screenFile.Name)." }
    Write-Output "Rendered $($screenFile.BaseName)"
}
