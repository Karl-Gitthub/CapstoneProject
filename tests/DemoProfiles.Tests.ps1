[CmdletBinding()]
param([Parameter(Mandatory=$true)][string]$ApplicationPath)
$ErrorActionPreference='Stop'
$repoRoot=Split-Path $PSScriptRoot -Parent
. (Join-Path $repoRoot 'scripts\Demo-Session.ps1')
$layout=Get-DemoLayout ('profiles-'+[Guid]::NewGuid().ToString('N').Substring(0,8))
$ApplicationPath=[IO.Path]::GetFullPath($ApplicationPath)
$passed=0
function Assert-Profile([bool]$condition,[string]$message){if(!$condition){throw "FAIL $message"};$script:passed++;Write-Output "PASS $message"}
function Profile-Sql([string]$file,[string]$sql){
    $connection=New-Object Data.OleDb.OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=$(Join-Path $layout.Data $file)")
    try{$connection.Open();$command=$connection.CreateCommand();try{$command.CommandText=$sql;[void]$command.ExecuteNonQuery()}finally{$command.Dispose()}}finally{$connection.Dispose()}
}
$oldData=$env:CAPSTONE_DATA_DIR;$oldDemo=$env:CAPSTONE_DEMO_ROOT
try{
    $management=Open-DemoManagement $layout -Create
    try{$guard=Open-DemoResetGuard $layout;try{Initialize-DemoData $layout $ApplicationPath | Out-Null}finally{$guard.Dispose()}}finally{$management.Dispose()}
    $env:CAPSTONE_DATA_DIR=$layout.Data;$env:CAPSTONE_DEMO_ROOT=$null
    [void][Reflection.Assembly]::LoadFrom($ApplicationPath)
    $store=New-Object CapstoneProject.Services.StudentStore
    $students=@($store.Get('SAMPLE-BSIT-001'),$store.Get('SAMPLE-BSBA-001'))
    foreach($student in $students){$student.Validate()}
    Assert-Profile ($students[0].FirstName -eq 'Miguel' -and $students[1].FirstName -eq 'Alyssa') 'fictional student identities load and pass production validation'
    $missing=@(foreach($student in $students){foreach($field in $student.GetType().GetFields()){if($field.FieldType -eq [string] -and $field.Name -ne 'Suffix' -and [string]::IsNullOrWhiteSpace([string]$field.GetValue($student))){$field.Name}}})
    Assert-Profile ($missing.Count -eq 0) 'student identity address education and credential fields are filled'
    Assert-Profile (@($students | Where-Object {$_.Email -notlike '*@example.invalid' -or $_.Phone -notlike '*XX*'}).Count -eq 0) 'contact data uses noncontactable demo email and masked phone numbers'
    $emptyService=[CapstoneProject.Services.SchoolService]::new([CapstoneProject.Services.UserSession]$null)
    foreach($student in $students){
        $editor=New-Object CapstoneProject.Presentation.StudentEditor($emptyService,$student)
        try{
            if($editor.Controls.Find('FirstName',$true)[0].Text -ne $student.FirstName -or $editor.Controls.Find('Street',$true)[0].Text -ne $student.Street -or $editor.Controls.Find('PrimarySchool',$true)[0].Text -ne $student.PrimarySchool -or $editor.Controls.Find('BirthCertificate',$true)[0].Text -ne 'Submitted'){throw 'Student editor did not bind a complete profile'}
        }finally{$editor.Dispose()}
    }
    Assert-Profile $true 'all student-editor tabs bind realistic fictional data'
    Assert-Profile ((Get-DemoAdministratorCount $layout) -eq 0) 'richer profiles do not create accounts or passwords'

    $password='FictionalProfiles2026!'
    [CapstoneProject.Services.AccountAdministration]::Bootstrap('profiles.admin','Disposable fixture administrator',$password)
    $auth=New-Object CapstoneProject.Services.AuthenticationService
    $admin=$auth.SignIn('profiles.admin',$password)
    $accounts=New-Object CapstoneProject.Services.AccountAdministration
    $accounts.Request('profiles.coordinator','Disposable Coordinator','coordinator@example.invalid','BSIT','Fictional fixture test',$password)
    $request=$accounts.Requests($admin).Rows[0]
    $accounts.Review($admin,[int]$request['request_id'],$true,'BSIT PROGRAM COORDINATOR','','Synthetic identity verified')
    $coordinator=New-Object CapstoneProject.Services.SchoolService($auth.SignIn('profiles.coordinator',$password))
    $school=New-Object CapstoneProject.Services.SchoolService($admin)
    $state=$coordinator.StudentState('SAMPLE-BSIT-001').Rows[0]
    $coordinator.ReviewStudent('SAMPLE-BSIT-001',[int]$state['record_version'],$true,'Synthetic review')
    $state=$school.StudentState('SAMPLE-BSIT-001').Rows[0]
    $school.ReviewStudent('SAMPLE-BSIT-001',[int]$state['record_version'],$true,'Synthetic approval')
    $school.AddOffering('BSIT','SAMPLE-BSIT-201','2026-2027','First semester','2A','SAMPLE-FAC-BSIT','M,W','09:00','10:30',0,0)
    $offering=[int]$school.Offerings('BSIT','2026-2027','First semester').Rows[0]['offering_id']
    $school.SaveEnrollment('SAMPLE-BSIT-001','2026-2027','First semester',-1,[int[]]@($offering),$true)
    $grades=$school.Grades('SAMPLE-BSIT-001','','','')
    $grades.Rows[0]['prelim']='85.5';$grades.Rows[0]['midterm']='86';$grades.Rows[0]['final']='88';$school.SaveGrades($grades)
    $original=$school.Grades('SAMPLE-BSIT-001','','','').Rows[0]
    $extra=New-Object CapstoneProject.Services.StudentRecord
    $extra.Id='EXTRA-DEMO-001';$extra.FirstName='Preserve';$extra.LastName='Unrelated';$extra.Program='BSIT';$extra.BirthDate=[DateTime]::new(2006,1,1);$extra.RegistrationDate=[DateTime]::Today
    $school.SaveStudent($extra,$true)
    Profile-Sql 'dbStudentManagement.accdb' "UPDATE tbl_BSITStud_Detail SET studlname='Old fixture label' WHERE studid='SAMPLE-BSIT-001'"
    $accountHash=(Get-FileHash -LiteralPath (Join-Path $layout.Data 'dbUserManagement.accdb')).Hash
    & (Join-Path $repoRoot 'scripts\Refresh-DemoProfiles.ps1') -Name (Split-Path $layout.Root -Leaf) | Out-Null
    Assert-Profile ($accountHash -eq (Get-FileHash -LiteralPath (Join-Path $layout.Data 'dbUserManagement.accdb')).Hash) 'refresh preserves account database bytes and chosen credentials'
    $current=$school.Grades('SAMPLE-BSIT-001','','','').Rows[0]
    Assert-Profile ($current['course_id'] -eq $original['course_id'] -and $current['average'] -eq $original['average'] -and $current['final'] -eq $original['final'] -and $school.Enrollment('SAMPLE-BSIT-001','2026-2027','First semester').Rows[0]['enrollment_state'] -eq 'Confirmed') 'refresh preserves linked enrollment identifiers and saved grades'
    Assert-Profile ($store.Get('SAMPLE-BSIT-001').LastName -eq 'Dela Cruz' -and $school.StudentState('SAMPLE-BSIT-001').Rows[0]['review_state'] -eq 'Approved' -and $store.Get('EXTRA-DEMO-001').LastName -eq 'Unrelated') 'refresh updates only sample profiles and preserves decisions and unrelated students'
    Assert-Profile (@($school.Faculty().Rows | Where-Object {$_.fac_fname -eq 'Gabriel'}).Count -eq 1 -and @($school.Catalog('BSIT').Rows | Where-Object {$_.course_description -like 'Database Management*'}).Count -eq 1) 'faculty and course forms have meaningful demonstration names'
    Profile-Sql 'dbStudentManagement.accdb' "INSERT INTO tbl_BSITStud_ContactDetail(studid,studcontactadd) VALUES ('SAMPLE-BSIT-001','duplicate@example.invalid')"
    $before=(Get-FileHash -LiteralPath (Join-Path $layout.Data 'dbStudentManagement.accdb')).Hash
    $rejected=$false
    try{& (Join-Path $repoRoot 'scripts\Refresh-DemoProfiles.ps1') -Name (Split-Path $layout.Root -Leaf) | Out-Null}catch{if($_.Exception.Message -like 'Duplicate sample identifiers*'){$rejected=$true}else{throw}}
    Assert-Profile ($rejected -and $before -eq (Get-FileHash -LiteralPath (Join-Path $layout.Data 'dbStudentManagement.accdb')).Hash) 'failed refresh restores the previous demo files'
    Write-Output "$passed fictional-profile checks passed. Only disposable demo data was used."
}finally{$env:CAPSTONE_DATA_DIR=$oldData;$env:CAPSTONE_DEMO_ROOT=$oldDemo}
