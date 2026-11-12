using System;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;

namespace CapstoneProject.Infrastructure
{
    public static class SchemaMigrator
    {
        public const string Students = "dbStudentManagement.accdb", Academic = "db_GradingManagement.accdb", Accounts = "dbUserManagement.accdb", Catalog = "dbCourseManagement.accdb";
        private static bool HasTable(OleDbConnection db, string name)
        {
            return db.GetSchema("Tables").AsEnumerable().Any(r => Convert.ToString(r["TABLE_NAME"]) == name);
        }
        private static void Table(OleDbConnection db, string name, string columns)
        {
            if (!HasTable(db, name))
                Database.Execute(db, null, "CREATE TABLE [" + name + "] (" + columns + ")");
        }
        private static void Column(OleDbConnection db, string table, string name, string type)
        {
            if (!db.GetSchema("Columns").AsEnumerable().Any(r => Convert.ToString(r["TABLE_NAME"]) == table && Convert.ToString(r["COLUMN_NAME"]) == name))
                Database.Execute(db, null, "ALTER TABLE [" + table + "] ADD COLUMN [" + name + "] " + type);
        }
        public static void Ensure()
        {
            string directory = Path.GetFullPath(Database.DirectoryPath);
            if (new[] { "New folder", "DB ACCESS FILES" }.Contains(new DirectoryInfo(directory).Name) || File.Exists(Path.Combine(directory, "CapstoneProject.sln")) || File.Exists(Path.Combine(directory, "CapstoneProject.csproj")))
                throw new InvalidOperationException("Select a working database directory, not the source database directory.");
            foreach (var file in new[] { Students, Academic, Accounts })
            {
                bool prepared;
                using (var db = Database.Open(file))
                    prepared = HasTable(db, "app_schema") && Database.Query(db, null, "SELECT schema_version FROM app_schema WHERE schema_version=3").Rows.Count > 0;
                if (prepared)
                    continue;
                File.Copy(Path.Combine(directory, file), Path.Combine(directory, file + ".before-workspaces-" + Guid.NewGuid().ToString("N") + ".bak"));
                using (var db = Database.Open(file))
                {
                    Table(db, "app_schema", "schema_version LONG NOT NULL PRIMARY KEY");
                    Table(db, "app_audit", "audit_id COUNTER PRIMARY KEY,actor_name TEXT(255),action_name TEXT(100),record_key TEXT(255),occurred_utc DATETIME");
                    if (file == Students)
                    {
                        Table(db, "app_student_state", "student_id TEXT(255) NOT NULL PRIMARY KEY,record_version LONG,review_state TEXT(60),review_note MEMO,updated_utc DATETIME");
                    }
                    if (file == Academic)
                    {
                        var average = db.GetSchema("Columns").AsEnumerable().Single(r => Convert.ToString(r["TABLE_NAME"]) == "tbl_Course" && Convert.ToString(r["COLUMN_NAME"]) == "average");
                        if (Convert.ToInt32(average["DATA_TYPE"]) != 5)
                            Database.Execute(db, null, "ALTER TABLE tbl_Course ALTER COLUMN average DOUBLE");
                        Table(db, "app_offerings", "offering_id COUNTER PRIMARY KEY,program_code TEXT(10),course_code TEXT(255),school_year TEXT(30),semester_name TEXT(30),section_name TEXT(80),faculty_id TEXT(255),day_codes TEXT(60),start_minute LONG,end_minute LONG,record_version LONG");
                        Table(db, "app_write_locks", "lock_key TEXT(64) NOT NULL PRIMARY KEY,revision LONG");
                        foreach (var key in new[] { "assignments", "enrollments" })
                        if (Database.Query(db, null, "SELECT lock_key FROM app_write_locks WHERE lock_key=?", key).Rows.Count == 0)
                            Database.Execute(db, null, "INSERT INTO app_write_locks(lock_key,revision) VALUES (?,0)", key);
                        Table(db, "app_enrollments", "enrollment_id COUNTER PRIMARY KEY,student_id TEXT(255),school_year TEXT(30),semester_name TEXT(30),enrollment_state TEXT(30),record_version LONG,updated_utc DATETIME");
                        Table(db, "app_enrollment_lines", "line_id COUNTER PRIMARY KEY,enrollment_id LONG,offering_id LONG");
                        Table(db, "app_course_metadata", "course_id LONG PRIMARY KEY,offering_id LONG,enrollment_id LONG");
                        if (!db.GetSchema("Indexes").AsEnumerable().Any(r => Convert.ToString(r["INDEX_NAME"]) == "UX_app_enrollment"))
                            Database.Execute(db, null, "CREATE UNIQUE INDEX UX_app_enrollment ON app_enrollments(student_id,school_year,semester_name)");
                    }
                    if (file == Accounts)
                    {
                        Column(db, "tbl_UserLogin", "app_disabled", "YESNO");
                        Column(db, "tbl_UserLogin", "app_must_change", "YESNO");
                        Column(db, "tbl_UserLogin", "app_faculty_id", "TEXT(255)");
                        Column(db, "tbl_UserLogin", "app_version", "LONG");
                        Database.Execute(db, null, "UPDATE tbl_UserLogin SET app_version=0 WHERE app_version IS NULL");
                        Table(db, "app_access_requests", "request_id COUNTER PRIMARY KEY,username_text TEXT(255),full_name TEXT(255),email_text TEXT(255),department_text TEXT(80),reason_text MEMO,password_hash TEXT(255),request_state TEXT(30),review_note MEMO,created_utc DATETIME");
                        Table(db, "app_username_keys", "username_text TEXT(255) NOT NULL PRIMARY KEY");
                        Table(db, "app_security_lock", "lock_key TEXT(40) NOT NULL PRIMARY KEY,revision LONG");
                        if (Database.Query(db, null, "SELECT lock_key FROM app_security_lock WHERE lock_key='admin'").Rows.Count == 0)
                            Database.Execute(db, null, "INSERT INTO app_security_lock(lock_key,revision) VALUES ('admin',0)");
                        Table(db, "app_recovery_requests", "request_id COUNTER PRIMARY KEY,username_text TEXT(255),email_text TEXT(255),request_state TEXT(30),review_note MEMO,created_utc DATETIME");
                    }
                    Database.Execute(db, null, "INSERT INTO app_schema(schema_version) VALUES (3)");
                }
            }
        }
        public static void Audit(OleDbConnection db, OleDbTransaction transaction, string actor, string action, string key)
        {
            Database.Execute(db, transaction, "INSERT INTO app_audit(actor_name,action_name,record_key,occurred_utc) VALUES (?,?,?,?)", actor, action, key, DateTime.UtcNow);
        }
    }
}
