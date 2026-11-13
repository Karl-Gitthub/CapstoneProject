using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using CapstoneProject.Services;

namespace CapstoneProject.Presentation
{
    public partial class SchoolWorkspace
    {
        private async Task Dashboard()
        {
            if (user.Role == "FACULTY")
            {
                await ClassesPage();
                return;
            }
            var page = Page("Start with student registrations that need a decision.", Auto(), Auto(), Fill(), Auto());
            var counts = Theme.Label("Loading work queue…", 15, true);
            counts.ForeColor = Theme.Navy;
            page.Controls.Add(counts, 0, 0);
            var description = Theme.Label("Select a student to review the record and required credentials.");
            description.Margin = new Padding(0, 12, 0, 18);
            page.Controls.Add(description, 0, 1);
            var grid = Theme.Grid("Registration work queue");
            page.Controls.Add(grid, 0, 2);
            var actions = Bar();
            var review = Theme.Button("Review selected record", true);
            review.Click += async (s, e) => { var row = Selected(grid); if (row == null) { NeedSelection(); return; } await StudentProfile(Value(row, "studid"), true); };
            actions.Controls.Add(review);
            var find = Theme.Button("Find a student");
            find.Click += (s, e) => Navigate("Students");
            actions.Controls.Add(find);
            page.Controls.Add(actions, 0, 3);
            await LoadData(() => service.ReviewQueue(), data => { counts.Text = data.Rows.Count + " registrations in your review queue"; Bind(grid, data, "studid", "studfname", "studlname", "studprogram", "yearlevel", "review_state"); if (data.Rows.Count == 0) description.Text = "No registrations are awaiting your review."; });
        }
        private async Task ReviewQueuePage()
        {
            await Dashboard();
        }
        private async Task StudentList()
        {
            var page = Page("Search student records and open the selected student's profile.", Auto(), Fill(), Auto());
            var toolbar = Bar();
            var query = Input("Student number or name", 310);
            var program = Choice("Program", "All programs", "BSIT", "BSBA");
            if (AccessPolicy.Coordinator(user))
            {
                program.SelectedItem = AccessPolicy.Program(user);
                program.Enabled = false;
            }
            toolbar.Controls.Add(Labeled("Student number or name", query));
            toolbar.Controls.Add(Labeled("Program", program));
            var search = Theme.Button("Search", true);
            search.Margin = new Padding(0, 25, 8, 0);
            toolbar.Controls.Add(search);
            if (AccessPolicy.Registrar(user))
            {
                var add = Theme.Button("Add student");
                add.Margin = new Padding(0, 25, 0, 0);
                add.Click += async (s, e) => { using (var dialog = new StudentEditor(service, null)) if (dialog.ShowDialog(this) == DialogResult.OK) await StudentProfile(dialog.StudentId); };
                toolbar.Controls.Add(add);
            }
            page.Controls.Add(toolbar, 0, 0);
            var grid = Theme.Grid("Students");
            page.Controls.Add(grid, 0, 1);
            var footer = Bar();
            var open = Theme.Button("Open student record", true);
            footer.Controls.Add(open);
            var count = Theme.Label("");
            count.Margin = new Padding(12, 9, 0, 0);
            footer.Controls.Add(count);
            page.Controls.Add(footer, 0, 2);
            Func<Task> refresh = () => { string text = query.Text.Trim(), scope = program.SelectedIndex > 0 ? program.Text : ""; return LoadData(() => service.Students(text, scope), data => { Bind(grid, data, "studid", "studfname", "studlname", "studprogram", "yearlevel", "review_state"); count.Text = data.Rows.Count == 0 ? "No matching students. Try another search." : data.Rows.Count + " student records"; }); };
            search.Click += async (s, e) => await refresh();
            query.KeyDown += async (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; await refresh(); } };
            open.Click += async (s, e) => { var row = Selected(grid); if (row == null) { NeedSelection(); return; } await StudentProfile(Value(row, "studid")); };
            grid.CellDoubleClick += async (s, e) => { if (e.RowIndex >= 0) { var row = Selected(grid); if (row != null) await StudentProfile(Value(row, "studid")); } };
            await refresh();
        }
        private void ClearBody()
        {
            generation++;
            foreach (Control control in body.Controls.Cast<Control>().ToArray())
            {
                body.Controls.Remove(control);
                control.Dispose();
            }
        }
        private async Task StudentProfile(string id, bool reviewing = false)
        {
            ClearBody();
            selectedStudent = id;
            title.Text = reviewing ? "Registration review" : "Student record";
            var page = Page("Loading student…", Auto(), Fill(), Auto());
            var actions = Bar();
            var back = Theme.Button("Back to students");
            back.Click += (s, e) => Navigate("Students");
            actions.Controls.Add(back);
            if (AccessPolicy.Registrar(user))
            {
                var edit = Theme.Button("Edit student", true);
                edit.Click += async (s, e) => { await LoadData(() => service.Student(id), record => { using (var dialog = new StudentEditor(service, record)) if (dialog.ShowDialog(this) == DialogResult.OK) { _ = StudentProfile(id); } }); };
                actions.Controls.Add(edit);
            }
            var enroll = Theme.Button("Open enrollment");
            enroll.Click += (s, e) => Navigate(AccessPolicy.Coordinator(user) ? "Enrollment drafts" : "Enrollment");
            actions.Controls.Add(enroll);
            page.Controls.Add(actions, 0, 0);
            var tabs = new TabControl { Dock = DockStyle.Fill };
            page.Controls.Add(tabs, 0, 1);
            var overview = new TabPage("Overview") { BackColor = Color.White, Padding = new Padding(20), AutoScroll = true };
            var credentials = new TabPage("Credentials") { BackColor = Color.White, Padding = new Padding(16) };
            var grades = new TabPage("Grades & enrollment") { BackColor = Color.White, Padding = new Padding(16) };
            tabs.TabPages.AddRange(new[] { overview, credentials, grades });
            var info = Theme.Stack();
            info.Dock = DockStyle.Top;
            overview.Controls.Add(info);
            var documents = Theme.Grid("Student credential status");
            credentials.Controls.Add(documents);
            var academic = Theme.Grid("Student grades");
            grades.Controls.Add(academic);
            var reviewPanel = new FlowLayoutPanel { AutoSize = true, Dock = DockStyle.Top, WrapContents = true, Margin = new Padding(0, 20, 0, 0) };
            // Ordinary student profiles are for viewing. Decisions start in the review queue.
            reviewPanel.Visible = reviewing;
            var note = Input("Review note", 440);
            note.MaxLength = 1000;
            var stateLabel = Theme.Label("");
            stateLabel.Margin = new Padding(0, 12, 20, 0);
            reviewPanel.Controls.Add(stateLabel);
            reviewPanel.Controls.Add(Labeled("Review note / correction needed", note));
            var approve = Theme.Button(AccessPolicy.Registrar(user) ? "Approve registration" : "Approve and forward", true);
            approve.Margin = new Padding(0, 25, 8, 0);
            var correction = Theme.Button("Request correction");
            correction.Margin = new Padding(0, 25, 8, 0);
            reviewPanel.Controls.Add(approve);
            reviewPanel.Controls.Add(correction);
            page.Controls.Add(reviewPanel, 0, 2);
            int version = -1;
            await LoadData(() => new { Student = service.Student(id), State = service.StudentState(id), Grades = service.Grades(id) }, data =>
            {
                var record = data.Student;
                title.Text = record.FirstName + " " + record.LastName;
                subtitle.Text = record.Id + " · " + record.Program + " · Year level " + record.YearLevel;
                foreach (var pair in new[] { new[] { "Student number", record.Id }, new[] { "Full name", record.FirstName + " " + record.MiddleName + " " + record.LastName }, new[] { "Program", record.Program }, new[] { "Year level", record.YearLevel }, new[] { "Email", record.Email }, new[] { "Phone", record.Phone }, new[] { "Address", string.Join(", ", new[] { record.StreetNumber, record.Street, record.Barangay, record.City, record.Province, record.Zip }.Where(x => !string.IsNullOrWhiteSpace(x))) }, new[] { "Prior education", record.PrimarySchool + " / " + record.SecondarySchool } })
                {
                    info.Controls.Add(Theme.Label(pair[0], 9, true));
                    var value = Theme.Label(string.IsNullOrWhiteSpace(pair[1]) ? "Not recorded" : pair[1]);
                    value.Margin = new Padding(0, 0, 0, 20);
                    value.MaximumSize = new Size(780, 0);
                    info.Controls.Add(value);
                }
                var documentTable = new DataTable();
                documentTable.Columns.Add("Document");
                documentTable.Columns.Add("Status");
                foreach (var pair in StudentEditor.CredentialFields)
                {
                    var value = Convert.ToString(typeof(StudentRecord).GetField(pair.Key).GetValue(record));
                    documentTable.Rows.Add(pair.Value, string.IsNullOrWhiteSpace(value) ? "Not reviewed" : value);
                }
                Bind(documents, documentTable);
                Bind(academic, data.Grades, "course_code", "school_year", "sem", "prelim", "midterm", "final", "average", "convert_average");
                if (data.State.Rows.Count == 0)
                {
                    stateLabel.Text = "Legacy record — save details to begin review";
                    approve.Enabled = false;
                    correction.Enabled = false;
                }
                else
                {
                    var state = data.State.Rows[0];
                    stateLabel.Text = Value(state, "review_state");
                    note.Text = Value(state, "review_note");
                    version = Convert.ToInt32(state["record_version"]);
                    approve.Enabled = AccessPolicy.Registrar(user) ? stateLabel.Text == "Pending registrar review" : stateLabel.Text == "Pending program review" || stateLabel.Text == "Correction requested";
                }
            });
            approve.Click += async (s, e) => { string text = note.Text; await Save(() => service.ReviewStudent(id, version, true, text), () => StudentProfile(id, true)); };
            correction.Click += async (s, e) => { string text = note.Text; await Save(() => service.ReviewStudent(id, version, false, text), () => StudentProfile(id, true)); };
        }
        private async Task EvaluationPage()
        {
            var page = Page("Review catalog requirements alongside every recorded course attempt.", Auto(), Fill(), Auto());
            var bar = Bar();
            var id = Input("Student number", 240);
            id.Text = selectedStudent;
            var load = Theme.Button("Load evaluation", true);
            load.Margin = new Padding(0, 25, 0, 0);
            bar.Controls.Add(Labeled("Student number", id));
            bar.Controls.Add(load);
            page.Controls.Add(bar, 0, 0);
            var grid = Theme.Grid("Curriculum evaluation");
            page.Controls.Add(grid, 0, 1);
            var export = Theme.Button("Print / export evaluation");
            page.Controls.Add(export, 0, 2);
            load.Click += async (s, e) => { string student = id.Text.Trim(); await LoadData(() => service.Evaluation(student), data => { selectedStudent = student; Bind(grid, data, "course_code", "course_description", "units", "year_level", "recorded_grades", "completion"); }); };
            export.Click += (s, e) => { if (grid.DataSource is DataTable table) OpenReport("Curriculum evaluation", table, "Student " + id.Text + " · Internal evaluation"); else NeedSelection(); };
            if (id.Text != "")
                load.PerformClick();
            await Task.CompletedTask;
        }
    }
}
