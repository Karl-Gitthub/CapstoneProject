using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using CapstoneProject.Infrastructure;

namespace CapstoneProject.Services
{
    public sealed class SchoolService
    {
        private readonly UserSession user;
        private readonly StudentStore students = new StudentStore();
        public SchoolService(UserSession user)
        {
            this.user = user;
        }
        public UserSession User
        {
            get
            {
                return user;
            }
        }
        private void Current()
        {
            AccessPolicy.Current(user);
        }
        public DataTable Students(string search = "", string program = "")
        {
            Current();
            AccessPolicy.Require(AccessPolicy.Registrar(user) || AccessPolicy.Coordinator(user));
            string scope = AccessPolicy.Program(user);
            return students.List(search.Trim(), scope != "" ? scope : program);
        }
        public StudentRecord Student(string id)
        {
            Current();
            var record = students.Get(id);
            AccessPolicy.Student(user, record.Program);
            return record;
        }
        public void SaveStudent(StudentRecord record, bool create)
        {
            Current();
            AccessPolicy.Require(AccessPolicy.Registrar(user));
            if (create)
                new StudentRepository().Create(record, user.Username);
            else
                students.Update(record, user.Username);
        }
        public DataTable StudentState(string id)
        {
            Student(id);
            return students.ReviewState(id);
        }
        public void ReviewStudent(string id, int version, bool approve, string note)
        {
            var record = Student(id);
            AccessPolicy.Require(AccessPolicy.Coordinator(user) || AccessPolicy.Registrar(user));
            var rows = students.ReviewState(id).Rows;
            if (rows.Count != 1)
                throw new ArgumentException("This legacy record has not entered the review workflow. Save its details first.");
            string state = Convert.ToString(rows[0]["review_state"]);
            if (approve && AccessPolicy.Coordinator(user) && state != "Pending program review" && state != "Correction requested")
                throw new ArgumentException("This record is not awaiting a program review.");
            if (approve && AccessPolicy.Registrar(user) && state != "Pending registrar review")
                throw new ArgumentException("A program coordinator must review this registration first.");
            if (!approve && string.IsNullOrWhiteSpace(note))
                throw new ArgumentException("Enter the correction needed before returning this record.");
            if ((note ?? "").Length > 1000)
                throw new ArgumentException("Keep the review note within 1,000 characters.");
            students.Review(id, version, approve ? (AccessPolicy.Registrar(user) ? "Approved" : "Pending registrar review") : "Correction requested", note ?? "", user.Username);
        }
        public DataTable ReviewQueue()
        {
            var table = Students();
            string wanted = AccessPolicy.Coordinator(user) ? "Pending program review" : "Pending registrar review";
            return Filter(table, r => Convert.ToString(r["review_state"]) == wanted || Convert.ToString(r["review_state"]) == "Correction requested");
        }
        public DataTable Catalog(string program)
        {
            Current();
            AccessPolicy.Require(AccessPolicy.Registrar(user) || AccessPolicy.Coordinator(user));
            AccessPolicy.Require(program == "BSIT" || program == "BSBA");
            if (AccessPolicy.Coordinator(user))
                AccessPolicy.Require(AccessPolicy.Program(user) == program);
            string p = program.ToLowerInvariant();
            return Database.Query(SchemaMigrator.Catalog, "SELECT " + p + "_coursecode_id AS course_code," + p + "_coursecode_desc AS course_description," + p + "_credunit AS units," + p + "_courseyearlvl AS year_level FROM tbl_" + program + "CourseDetail ORDER BY " + p + "_coursecode_id");
        }
        public DataTable Faculty()
        {
            Current();
            AccessPolicy.Require(AccessPolicy.Registrar(user) || AccessPolicy.Coordinator(user));
            return Database.Query(SchemaMigrator.Academic, "SELECT fac_id,fac_fname,fac_mname,fac_lname FROM tbl_FacultyDetail ORDER BY fac_lname,fac_fname");
        }
        public void AddFaculty(string id, string first, string middle, string last)
        {
            Current();
            AccessPolicy.Require(AccessPolicy.Coordinator(user));
            if (new[] { id, first, last }.Any(string.IsNullOrWhiteSpace) || new[] { id, first, middle, last }.Any(s => (s ?? "").Length > 255))
                throw new ArgumentException("Enter a faculty identifier, first name and last name (maximum 255 characters each).");
            using (var db = Database.Open(SchemaMigrator.Academic))
            using (var transaction = db.BeginTransaction())
            {
                if (Database.Query(db, transaction, "SELECT ID FROM tbl_FacultyDetail WHERE fac_id=?", id.Trim()).Rows.Count > 0)
                    throw new ArgumentException("That faculty identifier already exists.");
                Database.Execute(db, transaction, "INSERT INTO tbl_FacultyDetail(fac_id,fac_fname,fac_mname,fac_lname) VALUES (?,?,?,?)", id.Trim(), first.Trim(), middle.Trim(), last.Trim());
                SchemaMigrator.Audit(db, transaction, user.Username, "Faculty profile created", id);
                transaction.Commit();
            }
        }
        public void UpdateFaculty(DataRow original, string first, string middle, string last)
        {
            Current();
            AccessPolicy.Require(AccessPolicy.Coordinator(user));
            if (string.IsNullOrWhiteSpace(first) || string.IsNullOrWhiteSpace(last) || new[] { first, middle, last }.Any(s => (s ?? "").Length > 255))
                throw new ArgumentException("Enter a first and last name of up to 255 characters.");
            var values = new List<object> { first.Trim(), middle.Trim(), last.Trim(), original["fac_id"] };
            string sql = "UPDATE tbl_FacultyDetail SET fac_fname=?,fac_mname=?,fac_lname=? WHERE fac_id=?";
            foreach (var field in new[] { "fac_fname", "fac_mname", "fac_lname" })
            {
                sql += " AND ([" + field + "]=? OR ([" + field + "] IS NULL AND ? IS NULL))";
                values.Add(original[field]);
                values.Add(original[field]);
            }
            using (var db = Database.Open(SchemaMigrator.Academic))
            using (var transaction = db.BeginTransaction())
            using (var command = Database.Command(db, transaction, sql, values.ToArray()))
            {
                if (command.ExecuteNonQuery() != 1)
                    throw new DBConcurrencyException("The faculty profile changed. Reload before editing.");
                SchemaMigrator.Audit(db, transaction, user.Username, "Faculty profile updated", Convert.ToString(original["fac_id"]));
                transaction.Commit();
            }
        }
        public DataTable Offerings(string program = "", string year = "", string semester = "")
        {
            Current();
            AccessPolicy.Require(AccessPolicy.Registrar(user) || AccessPolicy.Coordinator(user) || user.Role == "FACULTY");
            string scope = AccessPolicy.Program(user);
            if (scope != "")
                program = scope;
            var values = new List<object>();
            string sql = "SELECT * FROM app_offerings WHERE 1=1";
            if (user.Role == "FACULTY")
            {
                sql += " AND faculty_id=?";
                values.Add(user.FacultyId);
            }
            if (program != "")
            {
                sql += " AND program_code=?";
                values.Add(program);
            }
            if (year != "")
            {
                sql += " AND school_year=?";
                values.Add(year);
            }
            if (semester != "")
            {
                sql += " AND semester_name=?";
                values.Add(semester);
            }
            return Database.Query(SchemaMigrator.Academic, sql + " ORDER BY course_code,section_name", values.ToArray());
        }
        public static void Term(string year, string semester)
        {
            var match = Regex.Match(year ?? "", "^(20[0-9]{2})-(20[0-9]{2})$");
            if (!match.Success || int.Parse(match.Groups[2].Value) != int.Parse(match.Groups[1].Value) + 1)
                throw new ArgumentException("Use an academic year such as 2026-2027.");
            if (!new[] { "First semester", "Second semester", "Summer" }.Contains(semester))
                throw new ArgumentException("Select a semester.");
        }
        public static int Minute(string time)
        {
            TimeSpan value;
            if (!TimeSpan.TryParseExact(time, @"hh\:mm", CultureInfo.InvariantCulture, out value) || value.TotalMinutes >= 1440)
                throw new ArgumentException("Enter times in 24-hour HH:mm format.");
            return (int)value.TotalMinutes;
        }
        public static string Days(string days)
        {
            var items = (days ?? "").Split(',').Select(x => x.Trim()).ToArray();
            if (items.Length == 0 || items.Distinct().Count() != items.Length || items.Any(x => !new[] { "M", "Tu", "W", "Th", "F", "Sa", "Su" }.Contains(x)))
                throw new ArgumentException("Enter day codes separated by commas: M,Tu,W,Th,F,Sa,Su.");
            return string.Join(",", items);
        }
        public void AddOffering(string program, string course, string year, string semester, string section, string faculty, string days, string start, string end, int offeringId = 0, int expectedVersion = 0)
        {
            Current();
            AccessPolicy.Require(AccessPolicy.Coordinator(user));
            AccessPolicy.Student(user, program);
            Term(year, semester);
            days = Days(days);
            int from = Minute(start), to = Minute(end);
            if (from >= to)
                throw new ArgumentException("The end time must follow the start time.");
            if (string.IsNullOrWhiteSpace(section) || section.Length > 80)
                throw new ArgumentException("Enter a section of up to 80 characters.");
            if (Catalog(program).AsEnumerable().Count(r => Convert.ToString(r["course_code"]) == course) != 1)
                throw new ArgumentException("Choose a unique course from the program catalog.");
            if (Faculty().AsEnumerable().Count(r => Convert.ToString(r["fac_id"]) == faculty) != 1)
                throw new ArgumentException("Choose an existing faculty profile.");
            using (var db = Database.Open(SchemaMigrator.Academic))
            using (var transaction = db.BeginTransaction())
            {
                LockAcademicWrites(db, transaction);
                if (offeringId > 0)
                {
                    var current = Database.Query(db, transaction, "SELECT * FROM app_offerings WHERE offering_id=?", offeringId);
                    if (current.Rows.Count != 1 || Convert.ToString(current.Rows[0]["program_code"]) != program || Convert.ToString(current.Rows[0]["course_code"]) != course || Convert.ToString(current.Rows[0]["school_year"]) != year || Convert.ToString(current.Rows[0]["semester_name"]) != semester)
                        throw new ArgumentException("The course, program and term of an existing assignment cannot be changed.");
                    var members = Database.Query(db, transaction, "SELECT c.stud_id FROM tbl_Course c INNER JOIN app_course_metadata m ON c.course_id=m.course_id WHERE m.offering_id=?", offeringId).AsEnumerable().Select(r => Convert.ToString(r["stud_id"])).ToArray();
                    var classes = Database.Query(db, transaction, "SELECT c.stud_id,o.* FROM (tbl_Course c INNER JOIN app_course_metadata m ON c.course_id=m.course_id) INNER JOIN app_offerings o ON m.offering_id=o.offering_id WHERE o.school_year=? AND o.semester_name=?", year, semester);
                    foreach (DataRow row in classes.Rows)
                    if (Convert.ToInt32(row["offering_id"]) != offeringId && members.Contains(Convert.ToString(row["stud_id"])) && ScheduleOverlap(days, from, to, Convert.ToString(row["day_codes"]), Convert.ToInt32(row["start_minute"]), Convert.ToInt32(row["end_minute"])))
                        throw new ArgumentException("This schedule would conflict with another class taken by an enrolled student.");
                }
                var existing = Database.Query(db, transaction, "SELECT * FROM app_offerings WHERE school_year=? AND semester_name=?", year, semester);
                foreach (DataRow row in existing.Rows)
                {
                    if (Convert.ToInt32(row["offering_id"]) == offeringId)
                        continue;
                    bool sameSection = Convert.ToString(row["program_code"]) == program && string.Equals(Convert.ToString(row["section_name"]), section, StringComparison.OrdinalIgnoreCase);
                    if (sameSection && Convert.ToString(row["course_code"]) == course)
                        throw new ArgumentException("That course is already offered to this section for the selected term.");
                    bool sameFaculty = Convert.ToString(row["faculty_id"]) == faculty;
                    if ((sameSection || sameFaculty) && ScheduleOverlap(days, from, to, Convert.ToString(row["day_codes"]), Convert.ToInt32(row["start_minute"]), Convert.ToInt32(row["end_minute"])))
                        throw new ArgumentException("The section or instructor already has a class at that time.");
                }
                if (offeringId == 0)
                    Database.Execute(db, transaction, "INSERT INTO app_offerings(program_code,course_code,school_year,semester_name,section_name,faculty_id,day_codes,start_minute,end_minute,record_version) VALUES (?,?,?,?,?,?,?,?,?,0)", program, course, year, semester, section.Trim(), faculty, days, from, to);
                else
                {
                    using (var command = Database.Command(db, transaction, "UPDATE app_offerings SET section_name=?,faculty_id=?,day_codes=?,start_minute=?,end_minute=?,record_version=record_version+1 WHERE offering_id=? AND record_version=?", section.Trim(), faculty, days, from, to, offeringId, expectedVersion))
                    if (command.ExecuteNonQuery() != 1)
                        throw new DBConcurrencyException("The teaching assignment changed. Reload before editing.");
                    var linked = Database.Query(db, transaction, "SELECT course_id FROM app_course_metadata WHERE offering_id=?", offeringId);
                    foreach (DataRow row in linked.Rows)
                        Database.Execute(db, transaction, "UPDATE tbl_Course SET fac_id=? WHERE course_id=?", faculty, Convert.ToInt32(row["course_id"]));
                }
                SchemaMigrator.Audit(db, transaction, user.Username, offeringId == 0 ? "Teaching assignment created" : "Teaching assignment updated", program + " " + course + " " + year + " " + semester);
                transaction.Commit();
            }
        }
        public static bool ScheduleOverlap(string days, int start, int end, string otherDays, int otherStart, int otherEnd)
        {
            return start < otherEnd && otherStart < end && days.Split(',').Intersect(otherDays.Split(',')).Any();
        }
        private static void LockAcademicWrites(OleDbConnection db, OleDbTransaction transaction)
        {
            // Assignment edits and enrollment confirmation affect the same rows.
            // Always take both existing lock keys in this order, before reading offerings.
            try
            {
                foreach (var key in new[] { "assignments", "enrollments" })
                using (var command = Database.Command(db, transaction, "UPDATE app_write_locks SET revision=revision+1 WHERE lock_key=?", key))
                if (command.ExecuteNonQuery() != 1)
                    throw new DBConcurrencyException("Academic setup changed. Close database users and run local setup before retrying.");
            }
            catch (OleDbException ex) when (ex.Errors.Cast<OleDbError>().Any(error => error.SQLState == "3218"))
            {
                throw new DBConcurrencyException("Another staff member is updating academic records. Reload the class or enrollment and try again. None of this operation was saved.", ex);
            }
        }
        public DataTable Enrollment(string student, string year, string semester)
        {
            Student(student);
            return Database.Query(SchemaMigrator.Academic, "SELECT * FROM app_enrollments WHERE student_id=? AND school_year=? AND semester_name=?", student, year, semester);
        }
        public DataTable EnrollmentLines(int id)
        {
            Current();
            var header = Database.Query(SchemaMigrator.Academic, "SELECT student_id FROM app_enrollments WHERE enrollment_id=?", id);
            if (header.Rows.Count != 1)
                throw new ArgumentException("Enrollment was not found.");
            Student(Convert.ToString(header.Rows[0][0]));
            return Database.Query(SchemaMigrator.Academic, "SELECT o.* FROM app_enrollment_lines l INNER JOIN app_offerings o ON l.offering_id=o.offering_id WHERE l.enrollment_id=? ORDER BY o.course_code", id);
        }
        public void SaveEnrollment(string studentId, string year, string semester, int expectedVersion, IEnumerable<int> offeringIds, bool confirm)
        {
            var student = Student(studentId);
            Term(year, semester);
            AccessPolicy.Require(AccessPolicy.Registrar(user) || AccessPolicy.Coordinator(user));
            if (confirm)
                AccessPolicy.Require(AccessPolicy.Registrar(user));
            var ids = offeringIds.ToArray();
            if (ids.Length == 0 || ids.Length != ids.Distinct().Count())
                throw new ArgumentException("Select at least one class, without duplicate selections.");
            using (var registration = Database.Open(SchemaMigrator.Students))
            using (var registrationLock = registration.BeginTransaction())
            using (var db = Database.Open(SchemaMigrator.Academic))
            using (var transaction = db.BeginTransaction())
            {
                // Hold the registration state stable while committing the academic records.
                Database.Execute(registration, registrationLock, "UPDATE app_student_state SET record_version=record_version WHERE student_id=?", studentId);
                var currentStudent = Database.Query(registration, registrationLock, "SELECT record_version,review_state FROM app_student_state WHERE student_id=?", studentId);
                if (currentStudent.Rows.Count == 1 && Convert.ToInt32(currentStudent.Rows[0]["record_version"]) != student.Version)
                    throw new DBConcurrencyException("The student record changed. Reload before saving enrollment.");
                if (confirm && (currentStudent.Rows.Count != 1 || Convert.ToString(currentStudent.Rows[0]["review_state"]) != "Approved"))
                    throw new ArgumentException("Approve the student registration before confirming enrollment.");
                LockAcademicWrites(db, transaction);
                var available = Database.Query(db, transaction, "SELECT * FROM app_offerings WHERE program_code=? AND school_year=? AND semester_name=?", student.Program, year, semester);
                var selected = ids.Select(offeringId => available.AsEnumerable().SingleOrDefault(r => Convert.ToInt32(r["offering_id"]) == offeringId)).ToArray();
                if (selected.Any(r => r == null))
                    throw new ArgumentException("A class is outside this student's program or term. Reload the offerings.");
                if (selected.Select(r => Convert.ToString(r["course_code"])).Distinct().Count() != selected.Length)
                    throw new ArgumentException("Select only one section of each course.");
                for (int i = 0; i < selected.Length; i++)
                for (int j = i + 1; j < selected.Length; j++)
                if (ScheduleOverlap(Convert.ToString(selected[i]["day_codes"]), Convert.ToInt32(selected[i]["start_minute"]), Convert.ToInt32(selected[i]["end_minute"]), Convert.ToString(selected[j]["day_codes"]), Convert.ToInt32(selected[j]["start_minute"]), Convert.ToInt32(selected[j]["end_minute"])))
                    throw new ArgumentException("The selected classes have a schedule conflict.");
                var header = Database.Query(db, transaction, "SELECT * FROM app_enrollments WHERE student_id=? AND school_year=? AND semester_name=?", studentId, year, semester);
                int id;
                if (header.Rows.Count == 0)
                {
                    if (expectedVersion != -1)
                        throw new DBConcurrencyException("Enrollment changed. Reload before saving.");
                    Database.Execute(db, transaction, "INSERT INTO app_enrollments(student_id,school_year,semester_name,enrollment_state,record_version,updated_utc) VALUES (?,?,?,'Draft',0,?)", studentId, year, semester, DateTime.UtcNow);
                    id = Convert.ToInt32(Database.Query(db, transaction, "SELECT @@IDENTITY").Rows[0][0]);
                }
                else
                {
                    if (Convert.ToString(header.Rows[0]["enrollment_state"]) == "Confirmed")
                        throw new ArgumentException("Confirmed enrollment cannot be replaced through the draft editor.");
                    id = Convert.ToInt32(header.Rows[0]["enrollment_id"]);
                    using (var cmd = Database.Command(db, transaction, "UPDATE app_enrollments SET record_version=record_version+1,updated_utc=? WHERE enrollment_id=? AND record_version=?", DateTime.UtcNow, id, expectedVersion))
                    if (cmd.ExecuteNonQuery() != 1)
                        throw new DBConcurrencyException("The draft changed. Reload before saving.");
                    Database.Execute(db, transaction, "DELETE FROM app_enrollment_lines WHERE enrollment_id=?", id);
                }
                foreach (var offering in selected)
                {
                    int offeringId = Convert.ToInt32(offering["offering_id"]);
                    Database.Execute(db, transaction, "INSERT INTO app_enrollment_lines(enrollment_id,offering_id) VALUES (?,?)", id, offeringId);
                    if (confirm)
                    {
                        string course = Convert.ToString(offering["course_code"]);
                        if (Database.Query(db, transaction, "SELECT course_id FROM tbl_Course WHERE stud_id=? AND course_code=? AND school_year=? AND sem=?", studentId, course, year, semester).Rows.Count > 0)
                            throw new ArgumentException("This student already has that course in the selected term. Existing enrollment was preserved.");
                        if (Database.Query(db, transaction, "SELECT ID FROM tbl_StudentDetail WHERE stud_id=?", studentId).Rows.Count == 0)
                            Database.Execute(db, transaction, "INSERT INTO tbl_StudentDetail(stud_id,stud_fname,stud_mname,stud_lname,stud_program,stud_suffix) VALUES (?,?,?,?,?,?)", studentId, student.FirstName, student.MiddleName, student.LastName, student.Program, student.Suffix);
                        Database.Execute(db, transaction, "INSERT INTO tbl_Course(stud_id,course_code,school_year,sem,fac_id,yearlevel) VALUES (?,?,?,?,?,?)", studentId, course, year, semester, offering["faculty_id"], student.YearLevel);
                        int courseId = Convert.ToInt32(Database.Query(db, transaction, "SELECT @@IDENTITY").Rows[0][0]);
                        Database.Execute(db, transaction, "INSERT INTO app_course_metadata(course_id,offering_id,enrollment_id) VALUES (?,?,?)", courseId, offeringId, id);
                    }
                }
                if (confirm)
                    Database.Execute(db, transaction, "UPDATE app_enrollments SET enrollment_state='Confirmed' WHERE enrollment_id=?", id);
                SchemaMigrator.Audit(db, transaction, user.Username, confirm ? "Enrollment confirmed" : "Enrollment draft saved", studentId + " " + year + " " + semester);
                transaction.Commit();
                registrationLock.Commit();
            }
        }
        public DataTable Grades(string student = "", string year = "", string semester = "", string course = "")
        {
            Current();
            var values = new List<object>();
            string sql = "SELECT c.*,s.stud_fname,s.stud_mname,s.stud_lname,s.stud_program FROM tbl_Course c INNER JOIN tbl_StudentDetail s ON Left(c.stud_id,255)=Left(s.stud_id,255) WHERE c.stud_id=s.stud_id";
            if (user.Role == "FACULTY")
            {
                sql += " AND c.fac_id=?";
                values.Add(user.FacultyId);
            }
            else if (!AccessPolicy.Coordinator(user))
                AccessPolicy.Require(AccessPolicy.Registrar(user));
            foreach (var filter in new[] { new[] { "c.stud_id", student }, new[] { "c.school_year", year }, new[] { "c.sem", semester }, new[] { "c.course_code", course } })
            if (filter[1] != "")
            {
                sql += " AND " + filter[0] + "=?";
                values.Add(filter[1]);
            }
            var result = Database.Query(SchemaMigrator.Academic, sql + " ORDER BY s.stud_lname,s.stud_fname", values.ToArray());
            result.Columns.Add("offering_id", typeof(int));
            result.Columns.Add("section_name");
            var metadata = Database.Query(SchemaMigrator.Academic, "SELECT m.course_id,m.offering_id,o.section_name FROM app_course_metadata m INNER JOIN app_offerings o ON m.offering_id=o.offering_id").AsEnumerable().ToDictionary(r => Convert.ToInt32(r["course_id"]));
            foreach (DataRow row in result.Rows)
            {
                DataRow entry;
                if (metadata.TryGetValue(Convert.ToInt32(row["course_id"]), out entry))
                {
                    row["offering_id"] = entry["offering_id"];
                    row["section_name"] = entry["section_name"];
                }
                else
                {
                    row["offering_id"] = 0;
                    row["section_name"] = "Legacy class";
                }
            }
            // Student identity is authoritative in the student database. The academic
            // database keeps a legacy snapshot, which must not hide later name edits.
            var identities = Database.Query(SchemaMigrator.Students, "SELECT studid,studfname,studmname,studlname,studprogram FROM tbl_BSITStud_Detail")
                .AsEnumerable().ToDictionary(r => Convert.ToString(r["studid"]), StringComparer.OrdinalIgnoreCase);
            foreach (DataRow row in result.Rows)
            {
                DataRow identity;
                if (!identities.TryGetValue(Convert.ToString(row["stud_id"]), out identity)) continue;
                row["stud_fname"] = identity["studfname"]; row["stud_mname"] = identity["studmname"]; row["stud_lname"] = identity["studlname"]; row["stud_program"] = identity["studprogram"];
            }
            // Scope only after reconciliation. A stale academic snapshot must neither
            // hide transferred students nor authorize records missing a primary identity.
            if (AccessPolicy.Coordinator(user)) result = Filter(result, r => identities.ContainsKey(Convert.ToString(r["stud_id"])) && Convert.ToString(r["stud_program"]) == AccessPolicy.Program(user));
            result.AcceptChanges();
            return result;
        }
        public void SaveGrades(DataTable edited)
        {
            Current();
            AccessPolicy.Require(user.Role == "FACULTY" && !string.IsNullOrWhiteSpace(user.FacultyId));
            var authorized = Grades().AsEnumerable().ToDictionary(r => Convert.ToInt32(r["course_id"]));
            var changed = edited.AsEnumerable().Where(r => r.RowState == DataRowState.Modified).ToArray();
            if (changed.Length == 0)
                throw new ArgumentException("There are no changed grades to save.");
            // Batch validation completes before any write. SaveBatch keeps all changed rows atomic.
            foreach (var row in changed)
            {
                int id = Convert.ToInt32(row["course_id"]);
                if (id != Convert.ToInt32(row["course_id", DataRowVersion.Original]) || !authorized.ContainsKey(id))
                    throw new UnauthorizedAccessException("A selected enrollment is not assigned to your account.");
                foreach (var field in new[] { "prelim", "midterm", "final" })
                {
                    if (string.IsNullOrWhiteSpace(Convert.ToString(row[field])))
                        continue;
                    decimal value;
                    if (!GradeCalculator.TryParseStored(Convert.ToString(row[field]), out value))
                        throw new ArgumentException("Enter numeric grades from 0 to 100, or leave an unrecorded grade blank.");
                    GradeCalculator.Validate(value);
                }
            }
            GradeBatch.Save(changed, user.Username, user.FacultyId);
        }
        public DataTable Evaluation(string studentId)
        {
            var student = Student(studentId);
            var catalog = Catalog(student.Program);
            var grades = Grades(studentId);
            catalog.Columns.Add("recorded_grades");
            catalog.Columns.Add("completion");
            foreach (DataRow row in catalog.Rows)
            {
                var attempts = grades.AsEnumerable().Where(g => Convert.ToString(g["course_code"]) == Convert.ToString(row["course_code"])).ToArray();
                row["recorded_grades"] = string.Join("; ", attempts.Select(g => Convert.ToString(g["school_year"]) + " " + Convert.ToString(g["sem"]) + ": " + Convert.ToString(g["convert_average"])));
                row["completion"] = attempts.Length == 0 ? "Not enrolled" : attempts.Length > 1 ? "Multiple attempts — review" : attempts[0]["convert_average"] == DBNull.Value || Convert.ToString(attempts[0]["convert_average"]) == "" ? "Incomplete" : GradePassed(Convert.ToString(attempts[0]["convert_average"])) ? "Passed" : "Not passed";
            }
            return catalog;
        }
        private static bool GradePassed(string value)
        {
            decimal grade;
            return decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out grade) && grade >= 1 && grade <= 3;
        }
        public DataTable Audit()
        {
            Current();
            AccessPolicy.Require(user.Role == "ADMINISTRATOR");
            var result = new DataTable();
            foreach (var name in new[] { "actor_name", "action_name", "record_key", "occurred_utc" })
                result.Columns.Add(name, name == "occurred_utc" ? typeof(DateTime) : typeof(string));
            foreach (var file in new[] { SchemaMigrator.Students, SchemaMigrator.Academic, SchemaMigrator.Accounts })
            foreach (DataRow row in Database.Query(file, "SELECT TOP 200 actor_name,action_name,record_key,occurred_utc FROM app_audit ORDER BY occurred_utc DESC").Rows)
                result.ImportRow(row);
            result.DefaultView.Sort = "occurred_utc DESC";
            return result.DefaultView.ToTable();
        }
        private static DataTable Filter(DataTable source, Func<DataRow, bool> condition)
        {
            var result = source.Clone();
            foreach (DataRow row in source.Rows)
            if (condition(row))
                result.ImportRow(row);
            return result;
        }
    }
}
