using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;
using CapstoneProject.Infrastructure;
using CapstoneProject.Presentation;
using CapstoneProject.Services;

internal static class AcceptanceRehearsal
{
    private const string Password = "DisposableRehearsal2026!";
    private const string Year = "2026-2027", Term = "First semester";
    private static readonly List<object> Results = new List<object>();
    private static void Require(bool condition, string message) { if (!condition) throw new Exception(message); }
    private static void Reject<T>(Action action) where T : Exception
    {
        try { action(); } catch (T) { return; }
        throw new Exception("Expected " + typeof(T).Name);
    }
    private static void Check(string id, string scenario, Action action)
    {
        try { action(); Results.Add(new { id, scenario, status = "Passed" }); Console.WriteLine("PASS " + id + " " + scenario); }
        catch (Exception error) { Results.Add(new { id, scenario, status = "Failed", error = error.Message }); throw; }
    }
    private static SchoolService Staff(AccountAdministration accounts, UserSession admin, string username, string role, string faculty = "")
    {
        accounts.Request(username, "Fictional rehearsal staff", username + "@example.invalid", "Rehearsal", "Disposable acceptance rehearsal", Password);
        var request = accounts.Requests(admin).AsEnumerable().Single(row => Convert.ToString(row["username_text"]) == username);
        accounts.Review(admin, Convert.ToInt32(request["request_id"]), true, role, faculty, "Synthetic rehearsal identity verified");
        var session = new AuthenticationService().SignIn(username, Password);
        Require(session != null && session.Role == role, "Staff sign-in failed");
        return new SchoolService(session);
    }
    [STAThread]
    private static int Main(string[] args)
    {
        string evidence = null;
        bool success = false;
        try
        {
            if (args.Length != 2) throw new ArgumentException("Supply a fresh schema-only fixture directory and evidence directory.");
            string data = Path.GetFullPath(args[0]); evidence = Path.GetFullPath(args[1]);
            var serializer = new JavaScriptSerializer();
            var manifest = serializer.Deserialize<Dictionary<string, object>>(File.ReadAllText(Path.Combine(data, "synthetic-seed.json")));
            Require(Convert.ToString(manifest["kind"]) == "synthetic-capstone-seed" && Convert.ToBoolean(manifest["schemaOnly"]), "Only a disposable schema-only fixture is allowed");
            // A marker alone is insufficient: refuse any fixture that already contains accounts or students.
            Environment.SetEnvironmentVariable("CAPSTONE_DATA_DIR", data);
            Environment.SetEnvironmentVariable("CAPSTONE_DEMO_ROOT", null);
            Require(Database.Query(SchemaMigrator.Accounts, "SELECT ID FROM tbl_UserLogin").Rows.Count == 0 && Database.Query(SchemaMigrator.Students, "SELECT studid FROM tbl_BSITStud_Detail").Rows.Count == 0, "Fixture is not empty; existing records were preserved");
            SchemaMigrator.Ensure();
            Directory.CreateDirectory(evidence);
            AccountAdministration.Bootstrap("uat.admin", "Fictional administrator", Password);
            var admin = new AuthenticationService().SignIn("uat.admin", Password);
            var accounts = new AccountAdministration();
            var registrar = Staff(accounts, admin, "uat.registrar", "REGISTRAR");
            var it = Staff(accounts, admin, "uat.bsit", "BSIT PROGRAM COORDINATOR");
            var ba = Staff(accounts, admin, "uat.bsba", "BSBA PROGRAM COORDINATOR");
            it.AddFaculty("UAT-FAC-IT", "Gabriel", "Luis", "Navarro");
            ba.AddFaculty("UAT-FAC-BA", "Patricia", "Anne", "Mendoza");
            var itFaculty = Staff(accounts, admin, "uat.faculty.it", "FACULTY", "UAT-FAC-IT");
            var baFaculty = Staff(accounts, admin, "uat.faculty.ba", "FACULTY", "UAT-FAC-BA");
            Check("ACCESS-01", "All six staff roles authenticate after explicit approval", () => Require(accounts.Accounts(admin).Rows.Count == 6, "Expected six approved rehearsal accounts"));
            Check("ACCESS-02", "Administrator cannot open student or grade services", () => {
                Reject<UnauthorizedAccessException>(() => new SchoolService(admin).Students());
                Reject<UnauthorizedAccessException>(() => new SchoolService(admin).Grades());
            });
            Rehearse("BSIT", "Miguel", "Dela Cruz", registrar, it, ba, itFaculty, baFaculty, "UAT-FAC-IT", evidence);
            Rehearse("BSBA", "Alyssa", "Reyes", registrar, ba, it, baFaculty, itFaculty, "UAT-FAC-BA", evidence);
            Check("AUDIT-01", "Both programs have registration enrollment and grade audit events", () => {
                var audit = new SchoolService(admin).Audit();
                foreach (var program in new[] { "BSIT", "BSBA" }) {
                    foreach (var action in new[] { "Student review: Approved", "Enrollment confirmed" })
                        Require(audit.AsEnumerable().Any(row => Convert.ToString(row["action_name"]) == action && Convert.ToString(row["record_key"]).Contains("UAT-" + program)), "Missing " + program + " " + action);
                    var courseIds = registrar.Grades("UAT-" + program + "-001").AsEnumerable().Select(row => Convert.ToString(row["course_id"])).ToArray();
                    Require(audit.AsEnumerable().Any(row => Convert.ToString(row["action_name"]) == "Grades saved" && courseIds.Contains(Convert.ToString(row["record_key"]))), "Missing " + program + " grade audit event");
                }
            });
            success = true;
            Console.WriteLine(Results.Count + " automated rehearsal checks passed. Human UAT remains pending.");
            return 0;
        }
        catch (Exception error) { Console.Error.WriteLine(error); return 1; }
        finally
        {
            if (evidence != null && Directory.Exists(evidence)) File.WriteAllText(Path.Combine(evidence, "acceptance-results.json"), new JavaScriptSerializer().Serialize(new {
                performedUtc = DateTime.UtcNow.ToString("o"), kind = "Automated service rehearsal", humanAcceptance = "Pending", success, checks = Results
            }));
        }
    }
    private static void Review(SchoolService service, string id, bool approve, string note)
    {
        var state = service.StudentState(id).Rows[0];
        service.ReviewStudent(id, Convert.ToInt32(state["record_version"]), approve, note);
    }
    private static void Rehearse(string program, string first, string last, SchoolService registrar, SchoolService coordinator, SchoolService otherCoordinator, SchoolService faculty, SchoolService otherFaculty, string facultyId, string evidence)
    {
        string id = "UAT-" + program + "-001", course1 = "SAMPLE-" + program + "-201", course2 = "SAMPLE-" + program + "-202";
        Func<DataTable, DataRow> firstCourse = table => table.AsEnumerable().Single(row => Convert.ToString(row["course_code"]) == course1);
        Func<DataTable, DataRow> secondCourse = table => table.AsEnumerable().Single(row => Convert.ToString(row["course_code"]) == course2);
        Check(program + "-01", "Registrar creates a student awaiting program review", () => {
            registrar.SaveStudent(new StudentRecord { Id = id, FirstName = first, LastName = last, Program = program, YearLevel = "2", BirthDate = new DateTime(2006, 2, 15), RegistrationDate = DateTime.Today, Email = id.ToLowerInvariant() + "@example.invalid", BirthCertificate = "Submitted", Form137 = "Submitted" }, true);
            Require(Convert.ToString(registrar.StudentState(id).Rows[0]["review_state"]) == "Pending program review", "Wrong initial review state");
        });
        Check(program + "-02", "Program isolation and student editing restrictions hold", () => {
            Reject<UnauthorizedAccessException>(() => otherCoordinator.Student(id));
            Reject<UnauthorizedAccessException>(() => coordinator.SaveStudent(registrar.Student(id), false));
            Reject<UnauthorizedAccessException>(() => faculty.Student(id));
        });
        Check(program + "-03", "Coordinator requests correction and registrar resubmits", () => {
            Review(coordinator, id, false, "Recheck contact address");
            Require(Convert.ToString(registrar.StudentState(id).Rows[0]["review_state"]) == "Correction requested", "Correction was not recorded");
            var record = registrar.Student(id); record.City = "Angeles City"; registrar.SaveStudent(record, false);
            Require(Convert.ToString(registrar.StudentState(id).Rows[0]["review_state"]) == "Pending program review", "Edited record did not return for program review");
        });
        Check(program + "-04", "Coordinator prepares two nonoverlapping classes", () => {
            using (var db = Database.Open(SchemaMigrator.Catalog))
            foreach (var course in new[] { course1, course2 }) {
                string p = program.ToLowerInvariant();
                Database.Execute(db, null, "INSERT INTO tbl_" + program + "CourseDetail(" + p + "_coursecode_id," + p + "_coursecode_desc," + p + "_credunit," + p + "_courseyearlvl) VALUES (?,?,?,?)", course, "Fictional rehearsal course " + course, 3, "2");
            }
            coordinator.AddOffering(program, course1, Year, Term, "UAT-" + program + "-2A", facultyId, "M,W", "09:00", "10:30");
            coordinator.AddOffering(program, course2, Year, Term, "UAT-" + program + "-2A", facultyId, "M,W", "10:30", "12:00");
        });
        var offerings = coordinator.Offerings(program, Year, Term).AsEnumerable().Select(row => Convert.ToInt32(row["offering_id"])).ToArray();
        Check(program + "-05", "Enrollment confirmation cannot skip registration review", () => Reject<ArgumentException>(() => registrar.SaveEnrollment(id, Year, Term, -1, offerings, true)));
        Check(program + "-06", "Program forwarding and registrar approval complete in order", () => {
            Reject<ArgumentException>(() => Review(registrar, id, true, "Premature approval"));
            Review(coordinator, id, true, "Program details verified");
            Require(Convert.ToString(registrar.StudentState(id).Rows[0]["review_state"]) == "Pending registrar review", "Not forwarded");
            Review(registrar, id, true, "Registration verified");
            Require(Convert.ToString(registrar.StudentState(id).Rows[0]["review_state"]) == "Approved", "Registration not approved");
        });
        Check(program + "-07", "Coordinator saves draft but cannot confirm enrollment", () => {
            coordinator.SaveEnrollment(id, Year, Term, -1, offerings, false);
            Reject<UnauthorizedAccessException>(() => coordinator.SaveEnrollment(id, Year, Term, 0, offerings, true));
            Require(faculty.Grades(id).Rows.Count == 0, "Draft created grade rows");
        });
        Check(program + "-08", "Registrar confirms and assigned faculty receives two grade rows", () => {
            registrar.SaveEnrollment(id, Year, Term, 0, offerings, true);
            Require(faculty.Grades(id).Rows.Count == 2 && otherFaculty.Grades(id).Rows.Count == 0, "Class roster scope is incorrect");
            Require(Convert.ToString(registrar.Enrollment(id, Year, Term).Rows[0]["enrollment_state"]) == "Confirmed", "Enrollment not confirmed");
        });
        Check(program + "-09", "Partial grades remain incomplete", () => {
            var table = faculty.Grades(id); firstCourse(table)["prelim"] = "95.5"; faculty.SaveGrades(table);
            Require(firstCourse(faculty.Grades(id))["average"] == DBNull.Value, "An incomplete grade has an average");
        });
        Check(program + "-10", "Out-of-range grades fail without changing saved values", () => {
            foreach (var value in new[] { "-1", "101" }) {
                var table = faculty.Grades(id); firstCourse(table)["prelim"] = value;
                Reject<ArgumentException>(() => faculty.SaveGrades(table));
                Require(Convert.ToString(firstCourse(faculty.Grades(id))["prelim"]) == "95.5", "Invalid save changed a grade");
            }
        });
        Check(program + "-11", "Completed decimal grades and zero grades calculate correctly", () => {
            var table = faculty.Grades(id);
            firstCourse(table)["prelim"] = "95.5"; firstCourse(table)["midterm"] = "96"; firstCourse(table)["final"] = "95";
            secondCourse(table)["prelim"] = "0"; secondCourse(table)["midterm"] = "0"; secondCourse(table)["final"] = "0";
            faculty.SaveGrades(table);
            var saved = faculty.Grades(id);
            Require(Convert.ToDecimal(firstCourse(saved)["average"]) == 95.5m && Convert.ToString(firstCourse(saved)["convert_average"]) == "1.25", "Decimal grade calculation mismatch");
            Require(Convert.ToDecimal(secondCourse(saved)["average"]) == 0m && Convert.ToString(secondCourse(saved)["convert_average"]) == "5.00", "Zero grade calculation mismatch");
            Reject<UnauthorizedAccessException>(() => registrar.SaveGrades(table));
        });
        Check(program + "-12", "Evaluation and exported report contain saved results", () => {
            var evaluation = registrar.Evaluation(id);
            Require(evaluation.AsEnumerable().Any(row => Convert.ToString(row["completion"]) == "Passed") && evaluation.AsEnumerable().Any(row => Convert.ToString(row["completion"]) == "Not passed"), "Evaluation does not match saved grades");
            var grades = registrar.Grades(id, Year, Term);
            var report = grades.DefaultView.ToTable(false, "stud_id", "course_code", "school_year", "sem", "average", "convert_average");
            string csv = ReportViewer.Csv(report);
            File.WriteAllText(Path.Combine(evidence, program + "-internal-grades.csv"), csv);
            Require(grades.Rows.Count == 2 && csv.Contains(id) && csv.Contains("95.5") && csv.Contains("1.25") && csv.Contains("5.00"), "Export does not contain expected saved records");
        });
    }
}
