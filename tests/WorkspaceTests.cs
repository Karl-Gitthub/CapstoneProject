using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using CapstoneProject.Infrastructure;
using CapstoneProject.Services;
using CapstoneProject.Presentation;

internal static class WorkspaceTests
{
    private const string Password = "FixtureStaff2026!";
    private static void Equal<T>(T expected, T actual)
    {
        if (!Equals(expected, actual))
            throw new Exception("Expected " + expected + ", got " + actual);
    }
    private static void Throws<T>(Action action) where T : Exception
    {
        try
        {
            action();
        }
        catch (T) { return; }
        throw new Exception("Expected " + typeof(T).Name);
    }
    public static void Units(Action<string, Action> check)
    {
        ApprovedBatchTests.Units(check);
        DemoSessionTests.Units(check);
        check("schedule overlap detects shared days", () => Equal(true, SchoolService.ScheduleOverlap("M,W", 540, 630, "W,F", 600, 690)));
        check("back-to-back classes do not overlap", () => Equal(false, SchoolService.ScheduleOverlap("M,W", 540, 630, "M", 630, 720)));
        check("different class days do not overlap", () => Equal(false, SchoolService.ScheduleOverlap("M", 540, 630, "Tu", 540, 630)));
        check("invalid schedule day rejected", () => Throws<ArgumentException>(() => SchoolService.Days("Monday")));
        check("invalid schedule time rejected", () => Throws<ArgumentException>(() => SchoolService.Minute("25:30")));
        check("nonconsecutive academic year rejected", () => Throws<ArgumentException>(() => SchoolService.Term("2026-2028", "First semester")));
        check("short new passwords rejected", () => Throws<ArgumentException>(() => AccountAdministration.ValidatePassword("abc123")));
        check("CSV formula input is neutralized", () => Equal("\"'=1+1\"", ReportViewer.CsvCell("=1+1")));
        check("CSV quoted text is escaped", () => Equal("\"O\"\"Neil\"", ReportViewer.CsvCell("O\"Neil")));
        check("report pagination always makes progress", () => Equal(1, ReportViewer.RowsPerPage(100)));
    }
    private static UserSession Account(string username, string role, string faculty = "")
    {
        using (var db = Database.Open(SchemaMigrator.Accounts))
            Database.Execute(db, null, "INSERT INTO tbl_UserLogin(login_user,login_password,login_usertype,login_fullname,app_disabled,app_must_change,app_faculty_id,app_version) VALUES (?,?,?,?,?,?,?,0)", username, PasswordHash.Create(Password), role, username, false, false, faculty);
        return new AuthenticationService().SignIn(username, Password);
    }
    public static void Integration(Action<string, Action> check, string repo)
    {
        check("additive schema migration is repeatable", () => { SchemaMigrator.Ensure(); SchemaMigrator.Ensure(); });
        var registrar = Account("fixture.registrar", "REGISTRAR");
        var coordinator = Account("fixture.bsit", "BSIT PROGRAM COORDINATOR");
        var otherCoordinator = Account("fixture.bsba", "BSBA PROGRAM COORDINATOR");
        AccountAdministration.Bootstrap("fixture.admin", "Fixture administrator", Password);
        var admin = new AuthenticationService().SignIn("fixture.admin", Password);
        var school = new SchoolService(registrar);
        var program = new SchoolService(coordinator);
        var otherProgram = new SchoolService(otherCoordinator);
        var student = new StudentRecord { Id = "WORK-001", FirstName = "Ana", MiddleName = "L", LastName = "Santos", Program = "BSIT", YearLevel = "2", BirthDate = new DateTime(2006, 2, 15), RegistrationDate = DateTime.Today, Email = "ana@example.invalid", BirthCertificate = "Submitted", Form137 = "Not Submitted" };
        check("registrar creates a reviewable student record", () => { school.SaveStudent(student, true); Equal("Pending program review", Convert.ToString(school.StudentState(student.Id).Rows[0]["review_state"])); });
        var stale = school.Student(student.Id);
        var updated = school.Student(student.Id);
        check("student edits round trip related contact records", () => { updated.Email = "updated@example.invalid"; school.SaveStudent(updated, false); Equal("updated@example.invalid", school.Student(student.Id).Email); });
        check("stale student edits are rejected", () => { stale.Phone = "123"; Throws<DBConcurrencyException>(() => school.SaveStudent(stale, false)); });
        check("coordinator cannot read another program", () => Throws<UnauthorizedAccessException>(() => otherProgram.Student(student.Id)));
        check("coordinator cannot create student records", () => Throws<UnauthorizedAccessException>(() => program.SaveStudent(student, true)));
        check("registrar cannot skip program review", () => Throws<ArgumentException>(() => school.ReviewStudent(student.Id, updated.Version, true, "")));
        check("coordinator forwards registration to registrar", () => { var state = program.StudentState(student.Id).Rows[0]; program.ReviewStudent(student.Id, Convert.ToInt32(state["record_version"]), true, "Checked"); Equal("Pending registrar review", Convert.ToString(school.StudentState(student.Id).Rows[0]["review_state"])); });
        check("registrar completes registration approval", () => { var state = school.StudentState(student.Id).Rows[0]; school.ReviewStudent(student.Id, Convert.ToInt32(state["record_version"]), true, "Verified"); Equal("Approved", Convert.ToString(school.StudentState(student.Id).Rows[0]["review_state"])); });
        using (var db = Database.Open(SchemaMigrator.Catalog))
        {
            Database.Execute(db, null, "DELETE FROM tbl_BSITCourseDetail");
            Database.Execute(db, null, "DELETE FROM tbl_BSBACourseDetail");
            Database.Execute(db, null, "INSERT INTO tbl_BSITCourseDetail(bsit_coursecode_id,bsit_coursecode_desc,bsit_credunit,bsit_courseyearlvl) VALUES (?,?,?,?)", "IT201", "Information management", 3, "2");
            Database.Execute(db, null, "INSERT INTO tbl_BSITCourseDetail(bsit_coursecode_id,bsit_coursecode_desc,bsit_credunit,bsit_courseyearlvl) VALUES (?,?,?,?)", "IT202", "Application development", 3, "2");
        }
        school.AddFaculty("FAC-WORK", "Jordan", "", "Lee");
        school.AddFaculty("FAC-OTHER", "Alex", "", "Rivera");
        var faculty = Account("fixture.faculty", "FACULTY", "FAC-WORK");
        var otherFaculty = Account("fixture.other", "FACULTY", "FAC-OTHER");
        var instructor = new SchoolService(faculty);
        check("coordinator creates a program-scoped teaching assignment", () => program.AddOffering("BSIT", "IT201", "2026-2027", "First semester", "2A", "FAC-WORK", "M,W", "09:00", "10:30"));
        check("faculty schedule conflicts are rejected", () => Throws<ArgumentException>(() => program.AddOffering("BSIT", "IT202", "2026-2027", "First semester", "2B", "FAC-WORK", "W", "09:30", "11:00")));
        check("cross-program teaching assignments are denied", () => Throws<UnauthorizedAccessException>(() => otherProgram.AddOffering("BSIT", "IT202", "2026-2027", "First semester", "2A", "FAC-WORK", "F", "09:00", "10:30")));
        school.AddOffering("BSIT", "IT202", "2026-2027", "First semester", "2A", "FAC-WORK", "M,W", "10:30", "12:00");
        school.AddOffering("BSIT", "IT202", "2026-2027", "First semester", "2B", "FAC-OTHER", "M,W", "09:30", "11:00");
        var offerings = school.Offerings("BSIT", "2026-2027", "First semester");
        int first = offerings.AsEnumerable().Where(r => Convert.ToString(r["course_code"]) == "IT201").Select(r => Convert.ToInt32(r["offering_id"])).Single();
        int second = offerings.AsEnumerable().Where(r => Convert.ToString(r["course_code"]) == "IT202" && Convert.ToString(r["section_name"]) == "2A").Select(r => Convert.ToInt32(r["offering_id"])).Single();
        int conflict = offerings.AsEnumerable().Where(r => Convert.ToString(r["section_name"]) == "2B").Select(r => Convert.ToInt32(r["offering_id"])).Single();
        check("student enrollment schedule conflicts are rejected", () => Throws<ArgumentException>(() => school.SaveEnrollment(student.Id, "2026-2027", "First semester", -1, new[] { first, conflict }, false)));
        check("coordinator can save a subject draft", () => program.SaveEnrollment(student.Id, "2026-2027", "First semester", -1, new[] { first, second }, false));
        check("coordinator cannot confirm enrollment", () => Throws<UnauthorizedAccessException>(() => program.SaveEnrollment(student.Id, "2026-2027", "First semester", 0, new[] { first, second }, true)));
        check("stale enrollment drafts are rejected", () => Throws<DBConcurrencyException>(() => school.SaveEnrollment(student.Id, "2026-2027", "First semester", -1, new[] { first }, false)));
        check("confirmation creates both enrollment and class records", () => { school.SaveEnrollment(student.Id, "2026-2027", "First semester", 0, new[] { first, second }, true); Equal(2, instructor.Grades(student.Id).Rows.Count); Equal("Confirmed", Convert.ToString(school.Enrollment(student.Id, "2026-2027", "First semester").Rows[0]["enrollment_state"])); });
        check("faculty cannot view another instructor's enrollments", () => Equal(0, new SchoolService(otherFaculty).Grades(student.Id).Rows.Count));
        check("faculty cannot open full student personal records", () => Throws<UnauthorizedAccessException>(() => instructor.Student(student.Id)));
        var period = instructor.Grades(student.Id);
        period.Rows[0]["prelim"] = "95.5";
        check("partial period grades persist as incomplete", () => { instructor.SaveGrades(period); var row = instructor.Grades(student.Id).AsEnumerable().Single(r => Convert.ToInt32(r["course_id"]) == Convert.ToInt32(period.Rows[0]["course_id"])); Equal(DBNull.Value, row["average"]); Equal("95.5", Convert.ToString(row["prelim"])); });
        var batch = instructor.Grades(student.Id);
        batch.Rows[0]["midterm"] = "96";
        batch.Rows[0]["final"] = "95";
        batch.Rows[1]["prelim"] = "80";
        batch.Rows[1]["midterm"] = "81";
        batch.Rows[1]["final"] = "82";
        check("class grade batch commits calculated decimals", () => { instructor.SaveGrades(batch); Equal(2, instructor.Grades(student.Id).AsEnumerable().Count(r => r["average"] != DBNull.Value)); });
        var staleBatch = instructor.Grades(student.Id);
        foreach (DataRow row in staleBatch.Rows)
            row["final"] = "90";
        var concurrent = instructor.Grades(student.Id);
        concurrent.Rows[1]["final"] = "70";
        instructor.SaveGrades(concurrent);
        check("a stale row rolls back the entire grade batch", () => { Throws<DBConcurrencyException>(() => instructor.SaveGrades(staleBatch)); Equal("95", Convert.ToString(instructor.Grades(student.Id).Rows[0]["final"])); });
        check("faculty grade saves cannot target another instructor", () => { var table = school.Grades(); var forbidden = table.AsEnumerable().First(r => Convert.ToString(r["fac_id"]) == "FAC-SYN"); forbidden["final"] = "90"; Throws<UnauthorizedAccessException>(() => instructor.SaveGrades(table)); });
        check("evaluation includes saved curriculum attempts", () => Equal(2, school.Evaluation(student.Id).Rows.Count));
        var accounts = new AccountAdministration();
        check("staff access requests do not create active accounts", () => { accounts.Request("fixture.request", "Requested Staff", "request@example.invalid", "BSIT", "Teaching", Password); Equal<UserSession>(null, new AuthenticationService().SignIn("fixture.request", Password)); });
        check("non-admin approval is denied", () => Throws<UnauthorizedAccessException>(() => accounts.Review(registrar, 1, true, "FACULTY", "FAC-OTHER", "")));
        int request = Convert.ToInt32(accounts.Requests(admin).Rows[0]["request_id"]);
        check("administrator approves requested role and faculty mapping", () => { accounts.Review(admin, request, true, "FACULTY", "FAC-OTHER", "Identity verified"); Equal("FAC-OTHER", new AuthenticationService().SignIn("fixture.request", Password).FacultyId); });
        check("the same access request cannot be approved twice", () => Throws<DBConcurrencyException>(() => accounts.Review(admin, request, true, "FACULTY", "FAC-OTHER", "")));
        var oldSession = new AuthenticationService().SignIn("fixture.request", Password);
        accounts.RequestRecovery("fixture.request", "request@example.invalid");
        int recovery = Convert.ToInt32(accounts.RecoveryRequests(admin).Rows[0]["request_id"]);
        check("password assistance requires a verification note", () => Throws<ArgumentException>(() => accounts.ResolveRecovery(admin, recovery, true, "TemporaryStaff2026!", "")));
        accounts.ResolveRecovery(admin, recovery, true, "TemporaryStaff2026!", "Verified by staff office");
        var temporary = new AuthenticationService().SignIn("fixture.request", "TemporaryStaff2026!");
        check("password reset invalidates existing sessions", () => Throws<UnauthorizedAccessException>(() => AccessPolicy.Current(oldSession)));
        check("temporary passwords require a change before record access", () => { Equal(true, temporary.MustChangePassword); Throws<UnauthorizedAccessException>(() => new SchoolService(temporary).Grades()); });
        check("verified password change releases the forced-change gate", () => { accounts.ChangePassword(temporary, "TemporaryStaff2026!", "ReplacementStaff2026!"); Equal(false, temporary.MustChangePassword); AccessPolicy.Current(temporary); });
        var account = accounts.Accounts(admin).AsEnumerable().Single(r => Convert.ToString(r["login_user"]) == "fixture.other");
        accounts.SetAccess(admin, Convert.ToInt32(account["ID"]), Convert.ToInt32(account["app_version"]), "FACULTY", "FAC-OTHER", true);
        check("disabled accounts cannot authenticate", () => Equal<UserSession>(null, new AuthenticationService().SignIn("fixture.other", Password)));
        check("disabled accounts lose existing record access", () => Throws<UnauthorizedAccessException>(() => new SchoolService(otherFaculty).Grades()));
        check("administrators cannot disable themselves", () => Throws<ArgumentException>(() => accounts.SetAccess(admin, admin.AccountId, admin.Version, "ADMINISTRATOR", "", true)));
        check("workflow changes produce audit records", () => { if (new SchoolService(admin).Audit().Rows.Count < 12) throw new Exception("Missing audit records"); });
        check("first-administrator setup cannot run twice", () => Throws<InvalidOperationException>(() => AccountAdministration.Bootstrap("another.admin", "Other", Password)));
        var facultySnapshot = school.Faculty().AsEnumerable().Single(r => Convert.ToString(r["fac_id"]) == "FAC-WORK");
        check("faculty profile edits preserve identifiers", () => { school.UpdateFaculty(facultySnapshot, "Jordan", "A", "Lee"); Equal("A", Convert.ToString(school.Faculty().AsEnumerable().Single(r => Convert.ToString(r["fac_id"]) == "FAC-WORK")["fac_mname"])); });
        check("stale faculty profile edits are rejected", () => Throws<DBConcurrencyException>(() => school.UpdateFaculty(facultySnapshot, "Jordan", "B", "Lee")));
        school.AddFaculty("FAC-TRANSFER", "Taylor", "", "Lim");
        var transferred = Account("fixture.transfer", "FACULTY", "FAC-TRANSFER");
        var beforeTransfer = instructor.Grades(student.Id);
        var assignment = school.Offerings("BSIT").AsEnumerable().Single(r => Convert.ToInt32(r["offering_id"]) == first);
        check("assignment edits reject enrolled-student schedule conflicts", () => Throws<ArgumentException>(() => school.AddOffering("BSIT", "IT201", "2026-2027", "First semester", "2C", "FAC-TRANSFER", "M,W", "11:00", "12:30", first, 0)));
        check("assignment transfer changes linked grade access atomically", () => { school.AddOffering("BSIT", "IT201", "2026-2027", "First semester", "2A", "FAC-TRANSFER", "M,W", "09:00", "10:30", first, 0); Equal(1, new SchoolService(transferred).Grades(student.Id).Rows.Count); Equal(1, instructor.Grades(student.Id).Rows.Count); });
        check("stale teaching assignment updates are rejected", () => Throws<DBConcurrencyException>(() => school.AddOffering("BSIT", "IT201", "2026-2027", "First semester", "2A", "FAC-TRANSFER", "M,W", "09:00", "10:30", first, 0)));
        check("old instructor cannot save a transferred enrollment", () => { var row = beforeTransfer.AsEnumerable().Single(r => Convert.ToString(r["course_code"]) == "IT201"); row["final"] = "99"; Throws<UnauthorizedAccessException>(() => instructor.SaveGrades(beforeTransfer)); });
        var rollbackStudent = new StudentRecord { Id = "WORK-ROLLBACK", FirstName = "Rollback", LastName = "Student", Program = "BSIT", YearLevel = "2", BirthDate = new DateTime(2006, 1, 1), RegistrationDate = DateTime.Today };
        school.SaveStudent(rollbackStudent, true);
        var stateRow = program.StudentState(rollbackStudent.Id).Rows[0];
        program.ReviewStudent(rollbackStudent.Id, Convert.ToInt32(stateRow["record_version"]), true, "");
        stateRow = school.StudentState(rollbackStudent.Id).Rows[0];
        school.ReviewStudent(rollbackStudent.Id, Convert.ToInt32(stateRow["record_version"]), true, "");
        using (var db = Database.Open(SchemaMigrator.Academic))
        {
            Database.Execute(db, null, "INSERT INTO tbl_StudentDetail(stud_id,stud_fname,stud_lname,stud_program) VALUES (?,?,?,?)", rollbackStudent.Id, "Rollback", "Student", "BSIT");
            Database.Execute(db, null, "INSERT INTO tbl_Course(stud_id,course_code,school_year,sem,fac_id) VALUES (?,?,?,?,?)", rollbackStudent.Id, "IT202", "2026-2027", "First semester", "FAC-WORK");
        }
        check("late enrollment failure rolls back header and earlier course writes", () => { Throws<ArgumentException>(() => school.SaveEnrollment(rollbackStudent.Id, "2026-2027", "First semester", -1, new[] { first, second }, true)); Equal(0, school.Enrollment(rollbackStudent.Id, "2026-2027", "First semester").Rows.Count); Equal(0, school.Grades(rollbackStudent.Id).AsEnumerable().Count(r => Convert.ToString(r["course_code"]) == "IT201")); });
        check("student name edits appear in grade and report records",()=>{var identity=school.Student(student.Id);identity.LastName="Santos-Updated";school.SaveStudent(identity,false);Equal("Santos-Updated",Convert.ToString(school.Grades(student.Id).Rows[0]["stud_lname"]));});
        ApprovedBatchTests.Integration(check, school, program, otherProgram, instructor, student.Id);
        GradeContextTests.Integration(check, instructor);
        DemoSessionTests.Interface(check, registrar, repo);
        OnboardingTests.Integration(check, repo);
        Application.EnableVisualStyles();
        AdministratorNavigationTests.Integration(check, admin, registrar, coordinator, faculty);
        foreach (var tuple in new[] { Tuple.Create(registrar, "Workspace"), Tuple.Create(registrar, "Students"), Tuple.Create(registrar, "Enrollment"), Tuple.Create(registrar, "Grades"), Tuple.Create(registrar, "Faculty loading"), Tuple.Create(registrar, "Evaluation"), Tuple.Create(registrar, "Reports"), Tuple.Create(coordinator, "Pending students"), Tuple.Create(faculty, "My classes"), Tuple.Create(faculty, "Grade entry") })
        {
            var value = tuple;
            check("runtime page renders: " + tuple.Item1.Role + " / " + tuple.Item2, () => CaptureWorkspace(value.Item1, value.Item2, repo));
        }
        check("new student editor renders", () => CaptureDialog(new StudentEditor(school, null), "student-editor", repo));
        check("staff request screen renders", () => CaptureDialog(new AccessRequestDialog(), "request-staff-access", repo));
        check("password change screen renders", () => CaptureDialog(new PasswordDialog(registrar), "change-password", repo));
        var currentAssignment = school.Offerings("BSIT").AsEnumerable().Single(r => Convert.ToInt32(r["offering_id"]) == first);
        check("assignment editor preserves selected course and instructor", () => CaptureDialog(FormDialog.Offering(school, "BSIT", school.Catalog("BSIT"), school.Faculty(), "2026-2027", currentAssignment), "edit-assignment", repo, form => { Equal("IT201", Convert.ToString(((ComboBox)form.Controls.Find("course", true).Single()).SelectedValue)); Equal("FAC-TRANSFER", Convert.ToString(((ComboBox)form.Controls.Find("faculty", true).Single()).SelectedValue)); }));
        check("saved record report viewer renders", () => CaptureDialog(new ReportViewer("Copy of grades", school.Grades(student.Id).DefaultView.ToTable(false, "stud_id", "course_code", "school_year", "sem", "average", "convert_average"), "WORK-001 · Internal copy"), "report-viewer", repo));
        check("workspace fits its minimum window size", () => CaptureWorkspace(registrar, "Students", repo, true));
    }
    private static void CaptureDialog(Form form, string name, string repo, Action<Form> inspect = null)
    {
        using (form)
        {
            form.StartPosition = FormStartPosition.Manual;
            form.Location = new Point(-20000, -20000);
            form.ShowInTaskbar = false;
            form.Show();
            Application.DoEvents();
            inspect?.Invoke(form);
            string directory = Path.Combine(repo, "docs", "screenshots", "runtime");
            Directory.CreateDirectory(directory);
            using (var image = new Bitmap(form.Width, form.Height))
            {
                form.DrawToBitmap(image, new Rectangle(Point.Empty, image.Size));
                image.Save(Path.Combine(directory, name + ".png"), System.Drawing.Imaging.ImageFormat.Png);
            }
            form.Hide();
        }
    }
    private static void CaptureWorkspace(UserSession user, string route, string repo, bool minimum = false)
    {
        using (var form = new SchoolWorkspace(user))
        {
            form.StartPosition = FormStartPosition.Manual;
            form.Location = new Point(-20000, -20000);
            form.ShowInTaskbar = false;
            if (minimum)
                form.Size = form.MinimumSize;
            form.Show();
            form.OpenPage(route);
            bool loaded = false;
            var timer = System.Diagnostics.Stopwatch.StartNew();
            while (timer.ElapsedMilliseconds < 10000)
            {
                Application.DoEvents();
                var state = form.Controls.Find("workspaceFeedback", true).OfType<Label>().FirstOrDefault();
                if (state != null && !state.Text.StartsWith("Loading"))
                {
                    if (state.ForeColor == Color.FromArgb(155, 48, 53))
                        throw new Exception("Page failed: " + state.Text);
                    loaded = true;
                    break;
                }
                System.Threading.Thread.Sleep(20);
            }
            if (!loaded || form.CurrentPage != route)
                throw new Exception("The requested workspace page did not finish loading: " + route);
            Application.DoEvents();
            string folder = Path.Combine(repo, "docs", "screenshots", "runtime");
            Directory.CreateDirectory(folder);
            using (var image = new Bitmap(form.Width, form.Height))
            {
                form.DrawToBitmap(image, new Rectangle(Point.Empty, image.Size));
                image.Save(Path.Combine(folder, user.Role.Replace(' ', '-') + "-" + route.Replace(' ', '-') + (minimum ? "-minimum" : "") + ".png"), System.Drawing.Imaging.ImageFormat.Png);
            }
            form.Hide();
        }
    }
}
