using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using CapstoneProject.Infrastructure;

namespace CapstoneProject.Services
{
    public sealed class StudentStore
    {
        private static readonly Dictionary<string, string> Detail = Map("Id:studid,FirstName:studfname,MiddleName:studmname,LastName:studlname,Suffix:studsuffix,RegistrationDate:studdatereg,BirthDate:studbdate,Nationality:studnationality,Gender:studgender,Program:studprogram,YearLevel:yearlevel");
        private static readonly Dictionary<string, string> Address = Map("Id:studid,StreetNumber:studaddnum,Street:studaddstreet,Barangay:studaddbrgy,City:studaddcity,Province:studaddprovince,Zip:studaddzip");
        private static readonly Dictionary<string, string> Contact = Map("Id:studid,Phone:studcontactnum,Email:studcontactadd");
        private static readonly Dictionary<string, string> Education = Map("Id:studid,PrimarySchool:studprevprim_schoolname,PrimaryYear:studprev_sy,SecondarySchool:studprevsec_schoolname,SecondaryYear:studprevsec_sy");
        private static readonly Dictionary<string, string> Credentials = Map("Id:studid,Thesis:studthesis,GoodMoral:studcredgm,HonorableDismissal:studcredhd,Diploma:studhsdiploma,BirthCertificate:studbirthcert,CopyOfGrades:studcopyofgrades,Transcript:studcredtor,Form137:studcredf137,Form138:studcredf138,Ojt:studcertojt");
        private static Dictionary<string, string> Map(string value)
        {
            return value.Split(',').Select(x => x.Split(':')).ToDictionary(x => x[0], x => x[1]);
        }
        private static readonly string[] Tables = { "tbl_BSITStud_Detail", "tbl_BSITStud_Address", "tbl_BSITStud_ContactDetail", "tbl_BSITStud_SchoolBackground", "tbl_BSITStud_Credentials" };
        private static readonly Dictionary<string, string>[] Maps = { Detail, Address, Contact, Education, Credentials };
        public StudentRecord Get(string id)
        {
            var student = new StudentRecord();
            using (var db = Database.Open(SchemaMigrator.Students))
            using (var transaction = db.BeginTransaction())
            {
                for (int i = 0; i < Tables.Length; i++)
                {
                    var rows = Database.Query(db, transaction, "SELECT * FROM [" + Tables[i] + "] WHERE studid=?", id).Rows;
                    if (rows.Count > 1)
                        throw new InvalidOperationException("This student has duplicate related records and needs a data review.");
                    if (rows.Count == 0)
                    {
                        if (i == 0)
                            throw new ArgumentException("Student record was not found.");
                        continue;
                    }
                    foreach (var pair in Maps[i])
                    {
                        var field = typeof(StudentRecord).GetField(pair.Key);
                        object value = rows[0][pair.Value];
                        field.SetValue(student, field.FieldType == typeof(DateTime) ? (object)(value == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(value)) : Convert.ToString(value));
                    }
                }
                var state = Database.Query(db, transaction, "SELECT record_version FROM app_student_state WHERE student_id=?", id);
                student.Version = state.Rows.Count == 0 ? 0 : Convert.ToInt32(state.Rows[0][0]);
                transaction.Commit();
            }
            return student;
        }
        public DataTable List(string search, string program)
        {
            var table = Database.Query(SchemaMigrator.Students, "SELECT d.studid,d.studfname,d.studmname,d.studlname,d.studprogram,d.yearlevel FROM tbl_BSITStud_Detail d WHERE (InStr(1,d.studid,?)>0 OR InStr(1,d.studfname,?)>0 OR InStr(1,d.studlname,?)>0) AND (?='' OR d.studprogram=?) ORDER BY d.studlname,d.studfname", search, search, search, program, program);
            table.Columns.Add("review_state");
            var states = Database.Query(SchemaMigrator.Students, "SELECT student_id,review_state FROM app_student_state").AsEnumerable().ToDictionary(r => Convert.ToString(r[0]), r => Convert.ToString(r[1]), StringComparer.OrdinalIgnoreCase);
            foreach (DataRow row in table.Rows)
            {
                string state;
                row["review_state"] = states.TryGetValue(Convert.ToString(row["studid"]), out state) ? state : "Legacy record";
            }
            return table;
        }
        public void Update(StudentRecord student, string actor)
        {
            student.Validate();
            using (var db = Database.Open(SchemaMigrator.Students))
            using (var transaction = db.BeginTransaction())
            {
                if (Database.Query(db, transaction, "SELECT studid FROM tbl_BSITStud_Detail WHERE studid=?", student.Id).Rows.Count != 1)
                    throw new ArgumentException("Student record was not found.");
                var state = Database.Query(db, transaction, "SELECT record_version FROM app_student_state WHERE student_id=?", student.Id);
                if (state.Rows.Count == 0)
                {
                    if (student.Version != 0)
                        throw new DBConcurrencyException("This student record changed. Reload before saving.");
                    Database.Execute(db, transaction, "INSERT INTO app_student_state(student_id,record_version,review_state,review_note,updated_utc) VALUES (?,1,?,?,?)", student.Id, "Pending program review", "", DateTime.UtcNow);
                }
                else
                {
                    using (var command = Database.Command(db, transaction, "UPDATE app_student_state SET record_version=record_version+1,review_state=?,updated_utc=? WHERE student_id=? AND record_version=?", "Pending program review", DateTime.UtcNow, student.Id, student.Version))
                        if (command.ExecuteNonQuery() != 1)
                            throw new DBConcurrencyException("This student record changed. Reload before saving.");
                }
                for (int i = 0; i < Tables.Length; i++)
                {
                    var entries = Maps[i].Where(p => p.Key != "Id").ToList();
                    var values = entries.Select(p => typeof(StudentRecord).GetField(p.Key).GetValue(student)).ToList();
                    if (i == 0)
                    {
                        entries.Add(new KeyValuePair<string, string>("Age", "studage"));
                        values.Add(student.Age);
                    }
                    int count = Database.Query(db, transaction, "SELECT studid FROM [" + Tables[i] + "] WHERE studid=?", student.Id).Rows.Count;
                    if (count > 1)
                        throw new InvalidOperationException("Duplicate related records need review before editing.");
                    if (count == 0)
                    {
                        values.Insert(0, student.Id);
                        Database.Execute(db, transaction, "INSERT INTO [" + Tables[i] + "] (studid," + string.Join(",", entries.Select(p => "[" + p.Value + "]")) + ") VALUES (" + string.Join(",", values.Select(v => "?")) + ")", values.ToArray());
                    }
                    else
                    {
                        values.Add(student.Id);
                        Database.Execute(db, transaction, "UPDATE [" + Tables[i] + "] SET " + string.Join(",", entries.Select(p => "[" + p.Value + "]=?")) + " WHERE studid=?", values.ToArray());
                    }
                }
                SchemaMigrator.Audit(db, transaction, actor, "Student updated", student.Id);
                transaction.Commit();
                student.Version++;
            }
        }
        public DataTable ReviewState(string id)
        {
            return Database.Query(SchemaMigrator.Students, "SELECT * FROM app_student_state WHERE student_id=?", id);
        }
        public void Review(string id, int version, string state, string note, string actor)
        {
            using (var db = Database.Open(SchemaMigrator.Students))
            using (var transaction = db.BeginTransaction())
            using (var command = Database.Command(db, transaction, "UPDATE app_student_state SET review_state=?,review_note=?,record_version=record_version+1,updated_utc=? WHERE student_id=? AND record_version=?", state, note, DateTime.UtcNow, id, version))
            {
                if (command.ExecuteNonQuery() != 1)
                    throw new DBConcurrencyException("The review changed. Reload the student before continuing.");
                SchemaMigrator.Audit(db, transaction, actor, "Student review: " + state, id);
                transaction.Commit();
            }
        }
    }
}
