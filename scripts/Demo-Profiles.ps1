# Fictional fixture data only. Identifiers and table/column mappings are fixed here.
function Invoke-ProfileSql($connection,$transaction,[string]$sql,[object[]]$values=@(),[switch]$Scalar) {
    $command=$connection.CreateCommand(); $command.Transaction=$transaction
    try {
        $command.CommandText=$sql
        foreach($value in $values) {
            $type=if($value -is [DateTime]){[Data.OleDb.OleDbType]::Date}elseif($value -is [int]){[Data.OleDb.OleDbType]::Integer}else{[Data.OleDb.OleDbType]::VarWChar}
            $parameter=$command.Parameters.Add('p'+$command.Parameters.Count,$type); $parameter.Value=$value
        }
        if($Scalar){return $command.ExecuteScalar()}else{[void]$command.ExecuteNonQuery()}
    } finally {$command.Dispose()}
}
function Write-ProfileRow($connection,$transaction,[string]$table,[string]$key,$values,[switch]$ExistingOnly) {
    $count=[int](Invoke-ProfileSql $connection $transaction "SELECT COUNT(*) FROM [$table] WHERE [$key]=?" @($values[$key]) -Scalar)
    if($count -gt 1){throw 'Duplicate sample identifiers were found. No rows in this database were updated.'}
    if($count -eq 0 -and $ExistingOnly){return}
    $columns=@($values.Keys)
    if($count -eq 0){
        $names=($columns | ForEach-Object {'['+$_+']'}) -join ','
        $parameters=($columns | ForEach-Object {'?'}) -join ','
        Invoke-ProfileSql $connection $transaction "INSERT INTO [$table] ($names) VALUES ($parameters)" @($columns | ForEach-Object {$values[$_]})
    }else{
        $columns=@($columns | Where-Object {$_ -ne $key})
        $assignments=($columns | ForEach-Object {'['+$_+']=?'}) -join ','
        $parameters=@($columns | ForEach-Object {$values[$_]})+@($values[$key])
        Invoke-ProfileSql $connection $transaction "UPDATE [$table] SET $assignments WHERE [$key]=?" $parameters
    }
}
function Set-FictionalProfiles([string]$Directory) {
    $profiles=Get-Content -LiteralPath (Join-Path $PSScriptRoot 'demo-profiles.json') -Raw | ConvertFrom-Json
    if($profiles.fictional -ne $true -or @($profiles.students | Where-Object {$_.Id -notlike 'SAMPLE-*'}).Count -ne 0 -or @($profiles.faculty | Where-Object {$_.Id -notlike 'SAMPLE-FAC-*'}).Count -ne 0 -or @($profiles.courses | Where-Object {$_.Program -notin @('BSIT','BSBA') -or $_.Code -notlike ('SAMPLE-'+$_.Program+'-*')}).Count -ne 0){throw 'Only fictional SAMPLE identifiers are permitted in demo profiles.'}
    $studentMaps=[ordered]@{
        tbl_BSITStud_Detail='Id:studid,FirstName:studfname,MiddleName:studmname,LastName:studlname,Suffix:studsuffix,Program:studprogram,YearLevel:yearlevel,Gender:studgender,Nationality:studnationality,BirthDate:studbdate,RegistrationDate:studdatereg'
        tbl_BSITStud_Address='Id:studid,StreetNumber:studaddnum,Street:studaddstreet,Barangay:studaddbrgy,City:studaddcity,Province:studaddprovince,Zip:studaddzip'
        tbl_BSITStud_ContactDetail='Id:studid,Phone:studcontactnum,Email:studcontactadd'
        tbl_BSITStud_SchoolBackground='Id:studid,PrimarySchool:studprevprim_schoolname,PrimaryYear:studprev_sy,SecondarySchool:studprevsec_schoolname,SecondaryYear:studprevsec_sy'
        tbl_BSITStud_Credentials='Id:studid,BirthCertificate:studbirthcert,GoodMoral:studcredgm,HonorableDismissal:studcredhd,Diploma:studhsdiploma,CopyOfGrades:studcopyofgrades,Transcript:studcredtor,Form137:studcredf137,Form138:studcredf138,Thesis:studthesis,Ojt:studcertojt'
    }
    foreach($file in @('dbStudentManagement.accdb','db_GradingManagement.accdb','dbCourseManagement.accdb')) {
        $builder=New-Object Data.OleDb.OleDbConnectionStringBuilder; $builder.Provider='Microsoft.ACE.OLEDB.12.0'; $builder['Data Source']=[string](Join-Path $Directory $file)
        $connection=New-Object Data.OleDb.OleDbConnection($builder.ConnectionString)
        try {
            $connection.Open(); $transaction=$connection.BeginTransaction()
            try {
                if($file -eq 'dbStudentManagement.accdb') {
                    foreach($student in $profiles.students) {
                        foreach($table in $studentMaps.Keys) {
                            $values=[ordered]@{}
                            foreach($mapping in $studentMaps[$table].Split(',')) {
                                $pair=$mapping.Split(':'); $value=$student.($pair[0])
                                if($pair[0] -in @('BirthDate','RegistrationDate')){$value=[DateTime]::ParseExact($value,'yyyy-MM-dd',[Globalization.CultureInfo]::InvariantCulture)}
                                $values[$pair[1]]=$value
                            }
                            Write-ProfileRow $connection $transaction $table 'studid' $values
                        }
                        if(@($connection.GetSchema('Tables') | Where-Object TABLE_NAME -eq 'app_student_state').Count -gt 0) {
                            Invoke-ProfileSql $connection $transaction 'UPDATE app_student_state SET record_version=record_version+1,updated_utc=? WHERE student_id=?' @([DateTime]::UtcNow,$student.Id)
                            Invoke-ProfileSql $connection $transaction "INSERT INTO app_audit(actor_name,action_name,record_key,occurred_utc) VALUES ('Demo fixture refresh','Fictional sample profile refreshed',?,?)" @($student.Id,[DateTime]::UtcNow)
                        }
                    }
                } elseif($file -eq 'db_GradingManagement.accdb') {
                    foreach($faculty in $profiles.faculty){Write-ProfileRow $connection $transaction 'tbl_FacultyDetail' 'fac_id' ([ordered]@{fac_id=$faculty.Id;fac_fname=$faculty.FirstName;fac_mname=$faculty.MiddleName;fac_lname=$faculty.LastName})}
                    foreach($student in $profiles.students){Write-ProfileRow $connection $transaction 'tbl_StudentDetail' 'stud_id' ([ordered]@{stud_id=$student.Id;stud_fname=$student.FirstName;stud_mname=$student.MiddleName;stud_lname=$student.LastName;stud_program=$student.Program;stud_suffix=$student.Suffix}) -ExistingOnly}
                } else {
                    foreach($course in $profiles.courses){$prefix=$course.Program.ToLowerInvariant();$values=[ordered]@{};$values[$prefix+'_coursecode_id']=$course.Code;$values[$prefix+'_coursecode_desc']=$course.Description;$values[$prefix+'_credunit']=[int]$course.Units;$values[$prefix+'_lecunit']=[int]$course.Lecture;$values[$prefix+'_labunit']=[int]$course.Lab;$values[$prefix+'_courseyearlvl']=$course.YearLevel;Write-ProfileRow $connection $transaction ('tbl_'+$course.Program+'CourseDetail') ($prefix+'_coursecode_id') $values}
                }
                $transaction.Commit()
            } catch { $transaction.Rollback();throw } finally { $transaction.Dispose() }
        } finally {$connection.Dispose()}
    }
}
