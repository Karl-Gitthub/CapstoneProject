using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using CapstoneProject.Services;

namespace CapstoneProject.Presentation
{
    public partial class SchoolWorkspace
    {
        private ClassOption selectedClass;
        private sealed class ClassOption
        {
            public string Code, Year, Semester, Section, Faculty;
            public string Key
            {
                get
                {
                    return Code + "|" + Year + "|" + Semester + "|" + Section + "|" + Faculty;
                }
            }
            public override string ToString()
            {
                return Code + " · " + Section + " · " + Year + " · " + Semester;
            }
        }
        private static ClassOption Class(DataRow row, bool offering = false)
        {
            return new ClassOption { Code = Value(row, "course_code"), Year = Value(row, "school_year"), Semester = Value(row, offering ? "semester_name" : "sem"), Section = Value(row, "section_name"), Faculty = Value(row, offering ? "faculty_id" : "fac_id") };
        }
        private static bool Matches(DataRow row, ClassOption selected)
        {
            return Class(row).Key == selected.Key;
        }
        private static List<ClassOption> Options(DataTable grades, DataTable offerings)
        {
            return offerings.AsEnumerable().Select(r => Class(r, true)).Concat(grades.AsEnumerable().Select(r => Class(r))).GroupBy(c => c.Key).Select(g => g.First()).OrderBy(c => c.Code).ThenBy(c => c.Year).ThenBy(c => c.Section).ToList();
        }
        private async Task ClassesPage()
        {
            var page = Page("Open an assigned class to enter or review saved grades.", Fill(), Auto());
            var grid = Theme.Grid("Classes");
            page.Controls.Add(grid, 0, 0);
            var bar = Bar();
            var edit = Theme.Button("Open grade sheet", true);
            var summary = Theme.Button("View computed grades");
            bar.Controls.Add(edit);
            bar.Controls.Add(summary);
            page.Controls.Add(bar, 0, 1);
            var options = new List<ClassOption>();
            await LoadData(() => new { Grades = service.Grades(), Offerings = service.Offerings() }, data =>
            {
                options = Options(data.Grades, data.Offerings);
                var table = new DataTable();
                foreach (var name in new[] { "course_code", "section_name", "school_year", "semester_name", "Students", "Grade status", "class_key" })
                    table.Columns.Add(name);
                foreach (var option in options)
                {
                    var rows = data.Grades.AsEnumerable().Where(r => Matches(r, option)).ToArray();
                    int incomplete = rows.Count(r => string.IsNullOrWhiteSpace(Value(r, "convert_average")));
                    table.Rows.Add(option.Code, option.Section, option.Year, option.Semester, rows.Length, rows.Length == 0 ? "No enrolled students" : incomplete > 0 ? incomplete + " incomplete" : "Complete", option.Key);
                }
                Bind(grid, table, "course_code", "section_name", "school_year", "semester_name", "Students", "Grade status");
                if (table.Rows.Count == 0)
                    feedback.Text = "No classes are assigned to your faculty profile.";
            });
            Action<bool> open = readOnly => { var row = Selected(grid); if (row == null) { NeedSelection(); return; } selectedClass = options.Single(c => c.Key == Value(row, "class_key")); Navigate(readOnly ? "Computed grades" : user.Role == "FACULTY" ? "Grade entry" : "Grades"); };
            edit.Click += (s, e) => open(false);
            summary.Click += (s, e) => open(true);
        }
        protected virtual bool ConfirmGradeDiscard()
        {
            return MessageBox.Show(this, "Discard unsaved grades and open this class?", "Unsaved changes", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes;
        }
        private async Task GradesPage(bool readOnly)
        {
            AccessPolicy.Require(user.Role == "FACULTY");
            var page = Page(readOnly ? "Read-only saved class grades. Make corrections through Grade entry." : "Edit period grades. Blank entries remain incomplete; averages are calculated from all three periods.", Auto(), Auto(), Fill(), Auto());
            var bar = Bar();
            var classes = new ComboBox { Name = "Class selector", AccessibleName = "Class", DropDownStyle = ComboBoxStyle.DropDownList, Width = 650, Margin = new Padding(0, 0, 12, 12) };
            var load = Theme.Button("Open class", true);
            load.Margin = new Padding(0, 25, 0, 0);
            bar.Controls.Add(Labeled("Class and term", classes));
            bar.Controls.Add(load);
            page.Controls.Add(bar, 0, 0);
            var context = Theme.Label("No class loaded.", 10, true);
            context.Name = "loadedGradeClass";
            context.ForeColor = Theme.Navy;
            context.MaximumSize = new Size(800, 0);
            context.Margin = new Padding(0, 0, 0, 12);
            page.Controls.Add(context, 0, 1);
            var grid = Theme.Grid("Class grades", !readOnly);
            page.Controls.Add(grid, 0, 2);
            var actions = Bar();
            var save = Theme.Button("Save grades", true);
            var print = Theme.Button("Print / export saved grades");
            if (!readOnly)
                actions.Controls.Add(save);
            actions.Controls.Add(print);
            page.Controls.Add(actions, 0, 3);
            DataTable source = null, editing = null;
            bool binding = false;
            Func<bool> loadedSelection = () => editing != null && selectedClass != null && classes.SelectedItem is ClassOption choice && choice.Key == selectedClass.Key;
            Action updateContext = () =>
            {
                bool active = loadedSelection();
                grid.Enabled = active;
                save.Enabled = active && !readOnly && editing.Rows.Count > 0;
                print.Enabled = active;
                context.Text = editing == null ? "No class loaded." : "Loaded class: " + selectedClass + (active ? "" : "\nSelection changed. Click Open class to switch, or select the loaded class to keep editing.");
            };
            classes.SelectedIndexChanged += (s, e) => updateContext();
            Action show = () =>
            {
                if (!(classes.SelectedItem is ClassOption option))
                {
                    feedback.Text = "No class is available.";
                    updateContext();
                    return;
                }
                if (dirty && !ConfirmGradeDiscard())
                {
                    classes.SelectedItem = classes.Items.Cast<ClassOption>().FirstOrDefault(item => item.Key == selectedClass.Key);
                    updateContext();
                    return;
                }
                dirty = false;
                selectedClass = option;
                binding = true;
                editing = source.Clone();
                foreach (DataRow row in source.Rows)
                if (Matches(row, option))
                    editing.ImportRow(row);
                editing.AcceptChanges();
                Bind(grid, editing, "stud_id", "stud_fname", "stud_lname", "prelim", "midterm", "final", "average", "convert_average");
                foreach (DataGridViewColumn column in grid.Columns)
                {
                    column.ReadOnly = readOnly || !new[] { "prelim", "midterm", "final" }.Contains(column.Name);
                    column.DefaultCellStyle.NullValue = "—";
                    if (new[] { "prelim", "midterm", "final", "average", "convert_average" }.Contains(column.Name))
                        column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    if (column.Name == "average")
                        column.DefaultCellStyle.Format = "0.00";
                }
                binding = false;
                updateContext();
                feedback.Text = editing.Rows.Count == 0 ? "No students are enrolled in this class." : editing.Rows.Count + " enrolled students · " + option.Year + " · " + option.Semester;
            };
            Func<Task> reload = () => LoadData(() => new { Grades = service.Grades(), Offerings = service.Offerings() }, data => { source = data.Grades; var options = Options(source, data.Offerings); classes.Items.Clear(); classes.Items.AddRange(options.Cast<object>().ToArray()); var previous = selectedClass == null ? null : options.FirstOrDefault(c => c.Key == selectedClass.Key); if (options.Count > 0) classes.SelectedItem = previous ?? options[0]; show(); });
            load.Click += (s, e) => show();
            grid.CellFormatting += (s, e) =>
            {
                if (e.ColumnIndex < 0 || !new[] { "prelim", "midterm", "final" }.Contains(grid.Columns[e.ColumnIndex].Name)) return;
                decimal grade;
                if (GradeCalculator.TryParseStored(Convert.ToString(e.Value), out grade))
                {
                    e.Value = grade.ToString(System.Globalization.CultureInfo.CurrentCulture);
                    e.FormattingApplied = true;
                }
            };
            grid.CellParsing += (s, e) =>
            {
                if (e.ColumnIndex < 0 || !new[] { "prelim", "midterm", "final" }.Contains(grid.Columns[e.ColumnIndex].Name)) return;
                string input = Convert.ToString(e.Value);
                decimal grade;
                if (string.IsNullOrWhiteSpace(input))
                {
                    e.Value = DBNull.Value;
                    e.ParsingApplied = true;
                }
                else if (GradeCalculator.TryParseInput(input, out grade) && grade >= 0 && grade <= 100)
                {
                    e.Value = grade.ToString(System.Globalization.CultureInfo.InvariantCulture);
                    e.ParsingApplied = true;
                }
            };
            grid.CellValidating += (s, e) =>
            {
                if (binding || readOnly || e.RowIndex < 0 || !new[] { "prelim", "midterm", "final" }.Contains(grid.Columns[e.ColumnIndex].Name))
                    return;
                string value = Convert.ToString(e.FormattedValue);
                decimal grade;
                if (!string.IsNullOrWhiteSpace(value) && (!GradeCalculator.TryParseInput(value, out grade) || grade < 0 || grade > 100))
                {
                    e.Cancel = true;
                    grid.Rows[e.RowIndex].ErrorText = "Use a grade from 0 to 100, or leave it blank.";
                }
                else
                    grid.Rows[e.RowIndex].ErrorText = "";
            };
            grid.CellValueChanged += (s, e) =>
            {
                if (binding || readOnly || e.RowIndex < 0 || e.ColumnIndex < 0 || !new[] { "prelim", "midterm", "final" }.Contains(grid.Columns[e.ColumnIndex].Name))
                    return;
                var row = (grid.Rows[e.RowIndex].DataBoundItem as DataRowView)?.Row;
                if (row == null)
                    return;
                decimal a, b, c;
                if (GradeCalculator.TryParseStored(Value(row, "prelim"), out a) && GradeCalculator.TryParseStored(Value(row, "midterm"), out b) && GradeCalculator.TryParseStored(Value(row, "final"), out c) && a >= 0 && a <= 100 && b >= 0 && b <= 100 && c >= 0 && c <= 100)
                {
                    decimal average = GradeCalculator.Average(a, b, c);
                    row["average"] = (double)average;
                    row["convert_average"] = GradeCalculator.ConvertAverage(average).ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
                }
                else
                {
                    row["average"] = DBNull.Value;
                    row["convert_average"] = DBNull.Value;
                }
                Dirty();
            };
            save.Click += async (s, e) => { if (!loadedSelection()) { feedback.Text = "Open the selected class before saving grades."; return; } if (!ValidateChildren()) return; if (!grid.EndEdit()) return; BindingContext[editing].EndCurrentEdit(); var table = editing; await Save(() => service.SaveGrades(table), reload); };
            print.Click += async (s, e) => { if (!loadedSelection()) { feedback.Text = "Open the selected class before printing grades."; return; } var option = selectedClass; await LoadData(() => service.Grades("", option.Year, option.Semester, option.Code), data => { var table = data.Clone(); foreach (DataRow row in data.Rows) if (Matches(row, option)) table.ImportRow(row); OpenReport("Computed class grades", Project(table, "stud_id", "stud_fname", "stud_lname", "prelim", "midterm", "final", "average", "convert_average"), option.ToString()); }); };
            updateContext();
            await reload();
        }
        private async Task EnrollmentPage()
        {
            var page = Page("Save a subject draft, then confirm after the registration has been approved.", Auto(), Auto(), Fill(), Auto());
            var bar = Bar();
            var student = Input("Student number", 200);
            student.Text = selectedStudent;
            var year = Input("Academic year", 150);
            year.Text = AcademicYear;
            var semester = Semester();
            var load = Theme.Button("Load classes", true);
            load.Margin = new Padding(0, 25, 0, 0);
            bar.Controls.Add(Labeled("Student number", student));
            bar.Controls.Add(Labeled("Academic year", year));
            bar.Controls.Add(Labeled("Semester", semester));
            bar.Controls.Add(load);
            page.Controls.Add(bar, 0, 0);
            var identity = Theme.Label("Enter a student number to view available classes.", 12, true);
            identity.ForeColor = Theme.Navy;
            page.Controls.Add(identity, 0, 1);
            var grid = Theme.Grid("Enrollment offerings", true);
            page.Controls.Add(grid, 0, 2);
            var actions = Bar();
            var draft = Theme.Button("Save draft");
            var confirm = Theme.Button("Confirm enrollment", true);
            var total = Theme.Label("");
            total.Margin = new Padding(16, 10, 0, 0);
            actions.Controls.Add(draft);
            if (AccessPolicy.Registrar(user))
                actions.Controls.Add(confirm);
            actions.Controls.Add(total);
            page.Controls.Add(actions, 0, 3);
            int version = -1;
            string loadedStudent = "", loadedYear = "", loadedSemester = "";
            bool binding = false, confirmed = false;
            Action totals = () => { if (grid.DataSource is DataTable table) { decimal units = 0; int count = 0; foreach (DataRow row in table.Rows) if (Convert.ToBoolean(row["Selected"])) { units += Convert.ToDecimal(row["units"]); count++; } total.Text = count + " classes · " + units + " units"; } };
            Func<Task> reload = () =>
            {
                string id = student.Text.Trim(), academicYear = year.Text.Trim(), term = semester.Text;
                return LoadData(() => { var record = service.Student(id); var header = service.Enrollment(id, academicYear, term); int enrollment = header.Rows.Count == 0 ? -1 : Convert.ToInt32(header.Rows[0]["enrollment_id"]); return new { Student = record, Header = header, Offerings = service.Offerings(record.Program, academicYear, term), Catalog = service.Catalog(record.Program), Lines = enrollment < 0 ? null : service.EnrollmentLines(enrollment) }; }, data =>
                {
                    binding = true;
                    loadedStudent = id;
                    loadedYear = academicYear;
                    loadedSemester = term;
                    selectedStudent = id;
                    var table = DecorateOfferings(data.Offerings, data.Catalog);
                    table.Columns.Add("Selected", typeof(bool));
                    var selected = data.Lines == null ? new int[0] : data.Lines.AsEnumerable().Select(r => Convert.ToInt32(r["offering_id"])).ToArray();
                    foreach (DataRow row in table.Rows)
                        row["Selected"] = selected.Contains(Convert.ToInt32(row["offering_id"]));
                    confirmed = data.Header.Rows.Count > 0 && Value(data.Header.Rows[0], "enrollment_state") == "Confirmed";
                    version = data.Header.Rows.Count == 0 ? -1 : Convert.ToInt32(data.Header.Rows[0]["record_version"]);
                    Bind(grid, table, "Selected", "course_code", "course_description", "section_name", "faculty_id", "schedule", "units");
                    grid.Columns["Selected"].DisplayIndex = 0;
                    grid.Columns["Selected"].MinimumWidth = 70;
                    foreach (DataGridViewColumn column in grid.Columns)
                        column.ReadOnly = confirmed || column.Name != "Selected";
                    identity.Text = data.Student.FirstName + " " + data.Student.LastName + " · " + data.Student.Program + " · " + (confirmed ? "Confirmed enrollment" : "Enrollment draft");
                    draft.Enabled = confirm.Enabled = !confirmed;
                    dirty = false;
                    binding = false;
                    totals();
                    if (table.Rows.Count == 0)
                        feedback.Text = "No classes are offered for this program and term. Create teaching assignments first.";
                });
            };
            load.Click += async (s, e) => { if (dirty && MessageBox.Show(this, "Discard your unsaved subject selections?", "Unsaved changes", MessageBoxButtons.YesNo) != DialogResult.Yes) return; await reload(); };
            grid.CurrentCellDirtyStateChanged += (s, e) => { if (grid.IsCurrentCellDirty) grid.CommitEdit(DataGridViewDataErrorContexts.Commit); };
            grid.CellValueChanged += (s, e) => { if (!binding && e.RowIndex >= 0 && e.ColumnIndex >= 0 && grid.Columns[e.ColumnIndex].Name == "Selected") { Dirty(); totals(); } };
            Func<bool, Task> persist = async final => { if (!(grid.DataSource is DataTable table)) { NeedSelection(); return; } if (student.Text.Trim() != loadedStudent || year.Text.Trim() != loadedYear || semester.Text != loadedSemester) { feedback.Text = "The student or term changed. Load classes before saving."; return; } var ids = table.AsEnumerable().Where(r => Convert.ToBoolean(r["Selected"])).Select(r => Convert.ToInt32(r["offering_id"])).ToArray(); int expected = version; await Save(() => service.SaveEnrollment(loadedStudent, loadedYear, loadedSemester, expected, ids, final), reload); };
            draft.Click += async (s, e) => await persist(false);
            confirm.Click += async (s, e) => await persist(true);
            if (student.Text != "")
                await reload();
        }
        private static DataTable DecorateOfferings(DataTable source, DataTable catalog)
        {
            var table = source.Copy();
            table.Columns.Add("course_description");
            table.Columns.Add("units", typeof(int));
            table.Columns.Add("schedule");
            foreach (DataRow row in table.Rows)
            {
                var course = catalog.AsEnumerable().FirstOrDefault(c => Value(c, "course_code") == Value(row, "course_code"));
                row["course_description"] = course == null ? "Catalog entry not found" : course["course_description"];
                row["units"] = course == null ? 0 : course["units"];
                row["schedule"] = Value(row, "day_codes") + " " + Time(Convert.ToInt32(row["start_minute"])) + "–" + Time(Convert.ToInt32(row["end_minute"]));
            }
            return table;
        }
        private static string Time(int minutes)
        {
            return TimeSpan.FromMinutes(minutes).ToString(@"hh\:mm");
        }
        private async Task FacultyPage()
        {
            var page = Page("Maintain faculty profiles and assign program courses, sections and schedules.", Auto(), Fill(), Auto());
            var bar = Bar();
            var program = Choice("Program", "BSIT", "BSBA");
            if (AccessPolicy.Coordinator(user))
            {
                program.SelectedItem = AccessPolicy.Program(user);
                program.Enabled = false;
            }
            bar.Controls.Add(Labeled("Program", program));
            var refresh = Theme.Button("Load assignments");
            refresh.Margin = new Padding(0, 25, 8, 0);
            bar.Controls.Add(refresh);
            page.Controls.Add(bar, 0, 0);
            var tabs = new TabControl { Dock = DockStyle.Fill };
            var assigned = new TabPage("Teaching assignments");
            var profiles = new TabPage("Faculty profiles");
            var catalogPage = new TabPage("Course catalog");
            var offers = Theme.Grid("Teaching assignments");
            var faculty = Theme.Grid("Faculty profiles");
            var catalog = Theme.Grid("Program course catalog");
            assigned.Controls.Add(offers);
            profiles.Controls.Add(faculty);
            catalogPage.Controls.Add(catalog);
            tabs.TabPages.AddRange(new[] { assigned, profiles, catalogPage });
            page.Controls.Add(tabs, 0, 1);
            var actions = Bar();
            var addFaculty = Theme.Button("Add faculty profile");
            var addOffering = Theme.Button("Assign course", true);
            actions.Controls.Add(addOffering);
            actions.Controls.Add(addFaculty);
            page.Controls.Add(actions, 0, 2);
            Func<Task> reload = () => { string scope = program.Text; return LoadData(() => new { Catalog = service.Catalog(scope), Faculty = service.Faculty(), Offers = service.Offerings(scope) }, data => { Bind(offers, DecorateOfferings(data.Offers, data.Catalog), "course_code", "course_description", "section_name", "faculty_id", "school_year", "semester_name", "schedule", "units"); Bind(faculty, data.Faculty); Bind(catalog, data.Catalog); }); };
            refresh.Click += async (s, e) => await reload();
            addFaculty.Click += async (s, e) => { using (var dialog = FormDialog.Faculty(service)) if (dialog.ShowDialog(this) == DialogResult.OK) await reload(); };
            addOffering.Click += async (s, e) => { string scope = program.Text; await LoadData(() => new { Courses = service.Catalog(scope), Faculty = service.Faculty() }, data => { using (var dialog = FormDialog.Offering(service, scope, data.Courses, data.Faculty, AcademicYear)) if (dialog.ShowDialog(this) == DialogResult.OK) { _ = reload(); } }); };
            await reload();
            var editOffering = Theme.Button("Edit assignment");
            actions.Controls.Add(editOffering);
            editOffering.Click += async (s, e) => { var row = Selected(offers); if (row == null) { NeedSelection(); return; } string scope = Value(row, "program_code"); await LoadData(() => new { Courses = service.Catalog(scope), Faculty = service.Faculty() }, data => { using (var dialog = FormDialog.Offering(service, scope, data.Courses, data.Faculty, AcademicYear, row)) if (dialog.ShowDialog(this) == DialogResult.OK) { _ = reload(); } }); };
            if (AccessPolicy.Coordinator(user))
            {
                var editFaculty = Theme.Button("Edit faculty profile");
                actions.Controls.Add(editFaculty);
                editFaculty.Click += async (s, e) => { var row = Selected(faculty); if (row == null) { NeedSelection(); return; } using (var dialog = FormDialog.Faculty(service, row)) if (dialog.ShowDialog(this) == DialogResult.OK) await reload(); };
            }
        }
    }
}
