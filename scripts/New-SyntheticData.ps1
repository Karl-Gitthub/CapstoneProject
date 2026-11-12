[CmdletBinding()]
param([string]$OutputDirectory, [switch]$SchemaOnly, [string]$ApplicationPath)
$ErrorActionPreference='Stop'
$repoRoot=Split-Path $PSScriptRoot -Parent
if (!$OutputDirectory) { $OutputDirectory=Join-Path $repoRoot ('artifacts\synthetic-seed\'+[Guid]::NewGuid().ToString('N')) }
$OutputDirectory=[IO.Path]::GetFullPath($OutputDirectory)
if (Test-Path -LiteralPath $OutputDirectory) { throw 'Choose a new output directory. This generator never overwrites existing data.' }
if (!$SchemaOnly) {
    if (!$ApplicationPath) { $ApplicationPath=Join-Path $repoRoot 'CapstoneProject\bin\x64\Debug\CapstoneProject.exe' }
    $ApplicationPath=[IO.Path]::GetFullPath($ApplicationPath)
    if (!(Test-Path -LiteralPath $ApplicationPath -PathType Leaf)) { throw 'Build the application first, or use -SchemaOnly.' }
}

. (Join-Path $PSScriptRoot 'Preflight.ps1')
Assert-DatabasePrerequisites -DataDirectory $OutputDirectory

# Fresh ACE databases, created from explicit schema; no old database is read or copied.
# Short-text identifiers preserve the active services' contract without copying
# legacy Memo fields, deleted pages, stored queries, attachments, or account records.
$schemas=[ordered]@{
    'dbStudentManagement.accdb'=@(
        'CREATE TABLE tbl_BSITStud_Detail (ID COUNTER,studid TEXT(255) NOT NULL PRIMARY KEY,studfname TEXT(255),studmname TEXT(255),studlname TEXT(255),studsuffix TEXT(255),studprogram TEXT(255),yearlevel TEXT(255),semester TEXT(255),studbdate DATETIME,studdatereg DATETIME,studage LONG,studgender TEXT(255),studnationality TEXT(255),pfpStudent MEMO)',
        'CREATE TABLE tbl_BSITStud_Address (ID COUNTER PRIMARY KEY,studid TEXT(255),studaddnum TEXT(255),studaddstreet TEXT(255),studaddbrgy TEXT(255),studaddcity TEXT(255),studaddprovince TEXT(255),studaddzip TEXT(255),CONSTRAINT FK_address_student FOREIGN KEY (studid) REFERENCES tbl_BSITStud_Detail(studid))',
        'CREATE TABLE tbl_BSITStud_ContactDetail (ID COUNTER PRIMARY KEY,studid TEXT(255),studcontactnum TEXT(255),studcontactadd TEXT(255),CONSTRAINT FK_contact_student FOREIGN KEY (studid) REFERENCES tbl_BSITStud_Detail(studid))',
        'CREATE TABLE tbl_BSITStud_SchoolBackground (ID COUNTER PRIMARY KEY,studid TEXT(255),studprevprim_schoolname TEXT(255),studprev_sy TEXT(255),studprevsec_schoolname TEXT(255),studprevsec_sy TEXT(255),CONSTRAINT FK_school_student FOREIGN KEY (studid) REFERENCES tbl_BSITStud_Detail(studid))',
        'CREATE TABLE tbl_BSITStud_Credentials (ID COUNTER PRIMARY KEY,studid TEXT(255),studbirthcert TEXT(255),studcertojt TEXT(255),studcopyofgrades TEXT(255),studcredf137 TEXT(255),studcredf138 TEXT(255),studcredgm TEXT(255),studcredhd TEXT(255),studcredtor TEXT(255),studhsdiploma TEXT(255),studthesis TEXT(255),CONSTRAINT FK_credentials_student FOREIGN KEY (studid) REFERENCES tbl_BSITStud_Detail(studid))'
    )
    'dbUserManagement.accdb'=@(
        'CREATE TABLE tbl_UserLogin (ID COUNTER PRIMARY KEY,login_fullname TEXT(255),login_password TEXT(255),login_user TEXT(255),login_usertype TEXT(255))'
    )
    'db_GradingManagement.accdb'=@(
        'CREATE TABLE tbl_FacultyDetail (ID COUNTER,fac_id TEXT(255) NOT NULL PRIMARY KEY,fac_fname TEXT(255),fac_mname TEXT(255),fac_lname TEXT(255))',
        'CREATE TABLE tbl_StudentDetail (ID COUNTER,stud_id TEXT(255) NOT NULL PRIMARY KEY,stud_fname TEXT(255),stud_mname TEXT(255),stud_lname TEXT(255),stud_program TEXT(255),stud_suffix TEXT(255))',
        'CREATE TABLE tbl_Course (course_id COUNTER PRIMARY KEY,stud_id TEXT(255),fac_id TEXT(255),course_code TEXT(255),school_year TEXT(255),sem TEXT(255),yearlevel TEXT(255),prelim TEXT(255),midterm TEXT(255),final TEXT(255),average DOUBLE,convert_average TEXT(255),CONSTRAINT FK_grade_student FOREIGN KEY (stud_id) REFERENCES tbl_StudentDetail(stud_id),CONSTRAINT FK_grade_faculty FOREIGN KEY (fac_id) REFERENCES tbl_FacultyDetail(fac_id))'
    )
    'dbCourseManagement.accdb'=@(
        'CREATE TABLE tbl_BSITCourseDetail (ID COUNTER PRIMARY KEY,bsit_coursecode_id TEXT(255),bsit_coursecode_desc MEMO,bsit_credunit LONG,bsit_lecunit LONG,bsit_labunit LONG,bsit_courseyearlvl TEXT(255))',
        'CREATE TABLE tbl_BSBACourseDetail (ID COUNTER PRIMARY KEY,bsba_coursecode_id TEXT(255),bsba_coursecode_desc MEMO,bsba_credunit LONG,bsba_lecunit LONG,bsba_labunit LONG,bsba_courseyearlvl TEXT(255))'
    )
    'db_Temp_RegForm.accdb'=@(
        'CREATE TABLE tbl_Temp_StudInfo (ID COUNTER,temp_referencenum TEXT(255) NOT NULL PRIMARY KEY,temp_stud TEXT(255),temp_stud_cdate DATETIME,temp_stud_fname TEXT(255),temp_stud_mname TEXT(255),temp_stud_lname TEXT(255),temp_stud_suffix TEXT(255),temp_stud_progcode TEXT(255),temp_stud_sem TEXT(255),temp_stud_sy TEXT(255),temp_stud_yearlevel TEXT(255))',
        'CREATE TABLE tbl_Temp_CourseCode (rec_id COUNTER PRIMARY KEY,temp_referencenum TEXT(255),temp_coursecode_1 TEXT(255),temp_coursecode_2 TEXT(255),temp_coursecode_3 TEXT(255),temp_coursecode_4 TEXT(255),temp_coursecode_5 TEXT(255),temp_coursecode_6 TEXT(255),temp_coursecode_7 TEXT(255),temp_coursecode_8 TEXT(255))',
        'CREATE TABLE tbl_Temp_CourseDescription (temp_coursdesc_id COUNTER PRIMARY KEY,temp_coursedesc_1 TEXT(255),temp_coursedesc_2 TEXT(255),temp_coursedesc_3 TEXT(255),temp_coursedesc_4 TEXT(255),temp_coursedesc_5 TEXT(255),temp_coursedesc_6 TEXT(255),temp_coursedesc_7 TEXT(255),temp_coursedesc_8 TEXT(255))'
    )
}
function Open-SeedDatabase([string]$filename) {
    $builder=New-Object System.Data.OleDb.OleDbConnectionStringBuilder
    $builder.Provider='Microsoft.ACE.OLEDB.12.0'
    $builder['Data Source']=[string](Join-Path $OutputDirectory $filename)
    $connection=New-Object System.Data.OleDb.OleDbConnection($builder.ConnectionString)
    $connection.Open()
    return $connection
}
function Invoke-SeedSql($connection,[string]$sql,[object[]]$values=@()) {
    $command=$connection.CreateCommand()
    try {
        $command.CommandText=$sql
        foreach ($value in $values) {
            $type=if($value -is [DateTime]){[System.Data.OleDb.OleDbType]::Date}else{[System.Data.OleDb.OleDbType]::VarWChar}
            $parameter=$command.Parameters.Add('p'+$command.Parameters.Count,$type)
            $parameter.Value=$value
        }
        [void]$command.ExecuteNonQuery()
    } finally { $command.Dispose() }
}
New-Item -ItemType Directory -Path $OutputDirectory | Out-Null
# A completion manifest is written only after every database and optional migration succeeds.
foreach ($file in $schemas.Keys) {
    $catalog=New-Object -ComObject ADOX.Catalog
    try {
        $builder=New-Object System.Data.OleDb.OleDbConnectionStringBuilder
        $builder.Provider='Microsoft.ACE.OLEDB.12.0'
        $builder['Data Source']=[string](Join-Path $OutputDirectory $file)
        [void]$catalog.Create($builder.ConnectionString)
        $catalog.ActiveConnection.Close()
    } finally { [void][Runtime.InteropServices.Marshal]::FinalReleaseComObject($catalog) }
    $connection=Open-SeedDatabase $file
    try { foreach($sql in $schemas[$file]) { Invoke-SeedSql $connection $sql } } finally { $connection.Dispose() }
}
if (!$SchemaOnly) {
    $connection=Open-SeedDatabase 'dbStudentManagement.accdb'
    try {
        foreach($program in @('BSIT','BSBA')) {
            $id='SAMPLE-'+$program+'-001'
            Invoke-SeedSql $connection 'INSERT INTO tbl_BSITStud_Detail(studid,studfname,studlname,studprogram,yearlevel,studbdate,studdatereg) VALUES (?,?,?,?,?,?,?)' @($id,'Sample',('Student '+$program),$program,'2',[DateTime]::new(2006,1,15),[DateTime]::new(2026,6,1))
            Invoke-SeedSql $connection 'INSERT INTO tbl_BSITStud_Address(studid,studaddcity) VALUES (?,?)' @($id,'Sample City')
            Invoke-SeedSql $connection 'INSERT INTO tbl_BSITStud_ContactDetail(studid,studcontactadd) VALUES (?,?)' @($id,('sample.'+$program.ToLower()+'@example.invalid'))
            Invoke-SeedSql $connection 'INSERT INTO tbl_BSITStud_SchoolBackground(studid,studprevsec_schoolname) VALUES (?,?)' @($id,'Sample Secondary School')
            Invoke-SeedSql $connection 'INSERT INTO tbl_BSITStud_Credentials(studid,studbirthcert,studcredf137) VALUES (?,?,?)' @($id,'Submitted','Not Submitted')
        }
    } finally { $connection.Dispose() }
    $connection=Open-SeedDatabase 'db_GradingManagement.accdb'
    try { foreach($program in @('BSIT','BSBA')) { Invoke-SeedSql $connection 'INSERT INTO tbl_FacultyDetail(fac_id,fac_fname,fac_lname) VALUES (?,?,?)' @(('SAMPLE-FAC-'+$program),'Sample',('Faculty '+$program)) } } finally { $connection.Dispose() }
    $connection=Open-SeedDatabase 'dbCourseManagement.accdb'
    try {
        foreach($program in @('BSIT','BSBA')) {
            $prefix=$program.ToLower()
            foreach($number in @(201,202)) {
                Invoke-SeedSql $connection "INSERT INTO tbl_$($program)CourseDetail($($prefix)_coursecode_id,$($prefix)_coursecode_desc,$($prefix)_credunit,$($prefix)_lecunit,$($prefix)_labunit,$($prefix)_courseyearlvl) VALUES (?,?,'3','3','0','2')" @(('SAMPLE-'+$program+'-'+$number),('Sample course '+$number+' (demonstration only)'))
            }
        }
    } finally { $connection.Dispose() }
    $previousDataDirectory=$env:CAPSTONE_DATA_DIR
    try {
        $env:CAPSTONE_DATA_DIR=$OutputDirectory
        $migration=Start-Process -FilePath $ApplicationPath -ArgumentList '--migrate-only' -WindowStyle Hidden -PassThru -Wait
        if ($migration.ExitCode -ne 0) { throw 'Synthetic schema migration failed; this directory is incomplete.' }
    } finally { $env:CAPSTONE_DATA_DIR=$previousDataDirectory }
    $connection=Open-SeedDatabase 'dbStudentManagement.accdb'
    try { foreach($program in @('BSIT','BSBA')) { Invoke-SeedSql $connection "INSERT INTO app_student_state(student_id,record_version,review_state,review_note,updated_utc) VALUES (?,0,'Pending program review','Synthetic demonstration record',?)" @(('SAMPLE-'+$program+'-001'),[DateTime]::UtcNow) } } finally { $connection.Dispose() }
}
[ordered]@{ kind='synthetic-capstone-seed'; version=1; schemaOnly=[bool]$SchemaOnly; createdUtc=[DateTime]::UtcNow.ToString('o'); accounts=0; students=$(if($SchemaOnly){0}else{2}); databases=@($schemas.Keys); note='Fresh generated data. No default password. Not an approved school curriculum.' } | ConvertTo-Json | Set-Content -LiteralPath (Join-Path $OutputDirectory 'synthetic-seed.json') -Encoding UTF8
Write-Output "Fresh synthetic databases prepared: $OutputDirectory"
Write-Output 'No existing databases or credentials were changed. Provision the first administrator separately for a demonstration.'
