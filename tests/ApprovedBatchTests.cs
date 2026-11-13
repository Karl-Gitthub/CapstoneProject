using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using CapstoneProject.Infrastructure;
using CapstoneProject.Presentation;
using CapstoneProject.Services;

internal static class ApprovedBatchTests
{
    private static void Equal<T>(T expected, T actual)
    {
        if (!Equals(expected, actual)) throw new Exception("Expected " + expected + ", got " + actual);
    }
    public static void Units(Action<string, Action> check)
    {
        foreach (var name in new[] { "en-US", "de-DE", "fr-FR", "en-PH" })
        check("grade parsing separates input and storage in " + name, () =>
        {
            var previous = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(name);
                Equal(8.5m, GradeCalculator.ParseStored("8.5"));
                decimal value;
                Equal(true, GradeCalculator.TryParseInput(8.5m.ToString(CultureInfo.CurrentCulture), out value));
                Equal(8.5m, value);
                Equal(false, GradeCalculator.TryParseStored("8,5", out value));
                Equal(false, GradeCalculator.TryParseInput("1" + CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator + "000", out value));
            }
            finally { CultureInfo.CurrentCulture = previous; }
        });
    }
    public static void Integration(Action<string, Action> check, SchoolService school, SchoolService program, SchoolService otherProgram, SchoolService instructor, string studentId)
    {
        foreach (var culture in new[] { "de-DE", "fr-FR", "en-US", "en-PH" })
        check("stored decimal grade survives service save in " + culture, () =>
        {
            var previous = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
                var table = instructor.Grades(studentId);
                int id = Convert.ToInt32(table.Rows[0]["course_id"]);
                table.Rows[0]["prelim"] = "8.5";
                table.Rows[0]["midterm"] = "60";
                table.Rows[0]["final"] = "70";
                instructor.SaveGrades(table);
                var edit = instructor.Grades(studentId);
                edit.AsEnumerable().Single(r => Convert.ToInt32(r["course_id"]) == id)["final"] = "80";
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(culture);
                instructor.SaveGrades(edit);
                var saved = instructor.Grades(studentId).AsEnumerable().Single(r => Convert.ToInt32(r["course_id"]) == id);
                Equal("8.5", Convert.ToString(saved["prelim"]));
                Equal(49.5m, Convert.ToDecimal(saved["average"]));
                Equal("5.00", Convert.ToString(saved["convert_average"]));
                saved["midterm"] = DBNull.Value;
                instructor.SaveGrades(saved.Table);
                Equal(DBNull.Value, instructor.Grades(studentId).AsEnumerable().Single(r => Convert.ToInt32(r["course_id"]) == id)["average"]);
            }
            finally { CultureInfo.CurrentCulture = previous; }
        });
        check("noncanonical decimal grades are rejected without changing storage", () =>
        {
            var table = instructor.Grades(studentId);
            table.Rows[0]["prelim"] = "8,5";
            try { instructor.SaveGrades(table); throw new Exception("Ambiguous grade was accepted"); }
            catch (ArgumentException) { }
            Equal("8.5", Convert.ToString(instructor.Grades(studentId).Rows[0]["prelim"]));
        });
        check("approved program transfer moves grade visibility to the current coordinator", () =>
        {
            int count = school.Grades(studentId).Rows.Count;
            Transfer(school, otherProgram, studentId, "BSBA");
            Equal("BSBA", otherProgram.Student(studentId).Program);
            Equal(count, otherProgram.Grades(studentId).Rows.Count);
            Equal(0, program.Grades(studentId).Rows.Count);
            Equal(count, school.Grades(studentId).Rows.Count);
            Transfer(school, program, studentId, "BSIT");
            Equal(count, program.Grades(studentId).Rows.Count);
            Equal(0, otherProgram.Grades(studentId).Rows.Count);
        });
        check("coordinator denies grades with no authoritative student identity", () =>
        {
            using (var db = Database.Open(SchemaMigrator.Academic))
            {
                Database.Execute(db, null, "INSERT INTO tbl_StudentDetail(stud_id,stud_fname,stud_lname,stud_program) VALUES (?,?,?,?)", "NO-PRIMARY", "Synthetic", "Unlinked", "BSIT");
                Database.Execute(db, null, "INSERT INTO tbl_Course(stud_id,course_code,fac_id) VALUES (?,?,?)", "NO-PRIMARY", "IT201", "FAC-WORK");
            }
            Equal(0, program.Grades("NO-PRIMARY").Rows.Count);
            Equal(1, school.Grades("NO-PRIMARY").Rows.Count);
        });
        check("confirmation rereads faculty after an assignment changes since the draft", () =>
        {
            string id = "CONSISTENCY-DRAFT", year = "2030-2031";
            ApprovedStudent(school, program, id);
            program.AddOffering("BSIT", "IT201", year, "First semester", "2A", "FAC-WORK", "Tu", "08:00", "09:00");
            int offering = Convert.ToInt32(school.Offerings("BSIT", year).Rows[0]["offering_id"]);
            school.SaveEnrollment(id, year, "First semester", -1, new[] { offering }, false);
            program.AddOffering("BSIT", "IT201", year, "First semester", "2A", "FAC-TRANSFER", "Tu", "08:00", "09:00", offering, 0);
            school.SaveEnrollment(id, year, "First semester", 0, new[] { offering }, true);
            Equal("FAC-TRANSFER", Convert.ToString(school.Grades(id).Rows[0]["fac_id"]));
        });
        for (int attempt = 0; attempt < 4; attempt++)
        {
            int index = attempt;
            check("competing enrollment and schedule update stay consistent " + index, () => CompetingWrites(school, program, index));
        }
        check("grade grid localizes input and keeps its bound values invariant", () => LocalizedGrid(instructor));
    }
    private static void Transfer(SchoolService school, SchoolService coordinator, string id, string program)
    {
        var student = school.Student(id);
        student.Program = program;
        school.SaveStudent(student, false);
        Approve(school, coordinator, id);
    }
    private static void ApprovedStudent(SchoolService school, SchoolService program, string id)
    {
        school.SaveStudent(new StudentRecord { Id = id, FirstName = "Synthetic", LastName = "Concurrency", Program = "BSIT", YearLevel = "2", BirthDate = new DateTime(2006, 1, 1), RegistrationDate = DateTime.Today }, true);
        Approve(school, program, id);
    }
    private static void Approve(SchoolService school, SchoolService coordinator, string id)
    {
        var state = coordinator.StudentState(id).Rows[0];
        coordinator.ReviewStudent(id, Convert.ToInt32(state["record_version"]), true, "Synthetic regression review");
        state = school.StudentState(id).Rows[0];
        school.ReviewStudent(id, Convert.ToInt32(state["record_version"]), true, "Synthetic regression approval");
    }
    private static void CompetingWrites(SchoolService school, SchoolService program, int index)
    {
        string id = "CONSISTENCY-RACE-" + index, year = (2032 + index) + "-" + (2033 + index);
        ApprovedStudent(school, program, id);
        program.AddOffering("BSIT", "IT201", year, "First semester", "2A", "FAC-WORK", "Tu", "08:00", "09:00");
        program.AddOffering("BSIT", "IT202", year, "First semester", "2B", "FAC-OTHER", "Tu", "09:00", "10:00");
        var offerings = school.Offerings("BSIT", year);
        int first = Convert.ToInt32(offerings.AsEnumerable().Single(r => Convert.ToString(r["course_code"]) == "IT201")["offering_id"]);
        int second = Convert.ToInt32(offerings.AsEnumerable().Single(r => Convert.ToString(r["course_code"]) == "IT202")["offering_id"]);
        Exception enrollmentError = null, assignmentError = null;
        using (var ready = new CountdownEvent(2))
        using (var start = new ManualResetEventSlim())
        {
            var enrollment = Task.Run(() => { ready.Signal(); start.Wait(); try { new SchoolService(school.User).SaveEnrollment(id, year, "First semester", -1, new[] { first, second }, true); } catch (Exception ex) { enrollmentError = ex; } });
            var assignment = Task.Run(() => { ready.Signal(); start.Wait(); try { new SchoolService(program.User).AddOffering("BSIT", "IT201", year, "First semester", "2A", "FAC-TRANSFER", "Tu", "09:30", "10:30", first, 0); } catch (Exception ex) { assignmentError = ex; } });
            if (!ready.Wait(TimeSpan.FromSeconds(10))) { start.Set(); throw new Exception("Workers did not start"); }
            start.Set();
            if (!Task.WaitAll(new[] { enrollment, assignment }, TimeSpan.FromSeconds(20))) throw new Exception("Competing academic writes did not finish");
        }
        // Unexpected database/permission failures must not count as a successful test.
        foreach (var error in new[] { enrollmentError, assignmentError })
            if (error != null && !(error is ArgumentException) && !(error is DBConcurrencyException)) throw error;
        if (enrollmentError != null && assignmentError != null) throw new Exception("Neither competing operation completed", enrollmentError);
        if (enrollmentError == null && assignmentError == null) throw new Exception("A conflicting schedule and enrollment both committed");
        var grades = school.Grades(id);
        var current = school.Offerings("BSIT", year).AsEnumerable().Single(r => Convert.ToInt32(r["offering_id"]) == first);
        if (enrollmentError == null)
        {
            Equal(2, grades.Rows.Count);
            Equal("Confirmed", Convert.ToString(school.Enrollment(id, year, "First semester").Rows[0]["enrollment_state"]));
            Equal("08:00", TimeSpan.FromMinutes(Convert.ToInt32(current["start_minute"])).ToString(@"hh\:mm"));
            Equal(Convert.ToString(current["faculty_id"]), Convert.ToString(grades.AsEnumerable().Single(r => Convert.ToString(r["course_code"]) == "IT201")["fac_id"]));
        }
        else
        {
            Equal(0, grades.Rows.Count);
            Equal(0, school.Enrollment(id, year, "First semester").Rows.Count);
            Equal(570, Convert.ToInt32(current["start_minute"]));
        }
    }
    private static void LocalizedGrid(SchoolService instructor)
    {
        var previous = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");
            Application.EnableVisualStyles();
            using (var form = new SchoolWorkspace(instructor.User))
            {
                form.StartPosition = FormStartPosition.Manual;
                form.Location = new Point(-20000, -20000);
                form.ShowInTaskbar = false;
                form.Show();
                form.OpenPage("Grade entry");
                var timer = System.Diagnostics.Stopwatch.StartNew();
                DataGridView grid = null;
                while (timer.ElapsedMilliseconds < 10000)
                {
                    Application.DoEvents();
                    grid = form.Controls.Find("Class grades", true).OfType<DataGridView>().FirstOrDefault();
                    if (grid != null && grid.DataSource is DataTable && grid.Rows.Count > 0) break;
                    Thread.Sleep(20);
                }
                if (grid == null || !(grid.DataSource is DataTable) || grid.Rows.Count == 0) throw new Exception("Grade grid did not load");
                var classes = form.Controls.Find("Class selector", true).OfType<ComboBox>().Single();
                classes.SelectedItem = classes.Items.Cast<object>().Single(item => item.ToString().Contains("IT202") && item.ToString().Contains("2A") && item.ToString().Contains("2026-2027"));
                FindButton(form, "Open class").PerformClick();
                Application.DoEvents();
                var table = (DataTable)grid.DataSource;
                Equal("8,5", Convert.ToString(grid.Rows[0].Cells["prelim"].FormattedValue));
                grid.CurrentCell = grid.Rows[0].Cells["prelim"];
                grid.Focus();
                if (!grid.BeginEdit(true)) throw new Exception("Grade cell did not enter edit mode");
                ((TextBox)grid.EditingControl).Text = "9,5";
                if (!grid.EndEdit()) throw new Exception("Localized grade did not validate");
                form.BindingContext[table].EndCurrentEdit();
                Equal("9.5", Convert.ToString(table.Rows[0]["prelim"]));
                Equal("9,5", Convert.ToString(grid.Rows[0].Cells["prelim"].FormattedValue));
                instructor.SaveGrades(table);
                int id = Convert.ToInt32(table.Rows[0]["course_id"]);
                Equal("9.5", Convert.ToString(instructor.Grades().AsEnumerable().Single(r => Convert.ToInt32(r["course_id"]) == id)["prelim"]));
                form.Hide();
            }
        }
        finally { CultureInfo.CurrentCulture = previous; }
    }
    private static Button FindButton(Control parent, string text)
    {
        foreach (Control child in parent.Controls)
        {
            if (child is Button button && button.Text == text) return button;
            var found = FindButton(child, text);
            if (found != null) return found;
        }
        return null;
    }
}
