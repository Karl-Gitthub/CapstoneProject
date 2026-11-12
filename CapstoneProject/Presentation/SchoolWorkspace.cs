using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using CapstoneProject.Services;
using CapstoneProject.Infrastructure;

namespace CapstoneProject.Presentation
{
    public partial class SchoolWorkspace : Form
    {
        private readonly SchoolService service;
        private readonly UserSession user;
        private readonly TableLayoutPanel body;
        private readonly FlowLayoutPanel navigation;
        private readonly Label title, subtitle, feedback;
        private readonly Dictionary<string, Button> links = new Dictionary<string, Button>();
        private string route = "";
        private int generation;
        private int readSerial;
        private bool dirty, busy;
        private string selectedStudent = "";
        public SchoolWorkspace(UserSession user)
        {
            this.user = user;
            service = new SchoolService(user);
            Text = "Student Records · " + DisplayRole;
            Font = new Font("Segoe UI", 10);
            BackColor = Theme.Surface;
            AutoScaleDimensions = new SizeF(96, 96);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(1320, 840);
            MinimumSize = new Size(1080, 720);
            StartPosition = FormStartPosition.CenterScreen;
            var shell = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, Margin = Padding.Empty };
            shell.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 220));
            shell.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            shell.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            var sidebar = new TableLayoutPanel { Dock = DockStyle.Fill, BackColor = Theme.Navy, Padding = new Padding(16, 20, 16, 16), Margin = Padding.Empty, ColumnCount = 1, RowCount = 6 };
            sidebar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            sidebar.RowStyles.Add(new RowStyle(SizeType.Absolute, 6));
            sidebar.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            sidebar.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            sidebar.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            sidebar.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            sidebar.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            sidebar.Controls.Add(new Panel { BackColor = Theme.Gold, Dock = DockStyle.Top, Height = 4 }, 0, 0);
            var brand = Theme.Label("SYSTEMS PLUS\nCOMPUTER COLLEGE", 11, true);
            brand.ForeColor = Color.White;
            brand.Margin = new Padding(8, 22, 0, 24);
            sidebar.Controls.Add(brand, 0, 1);
            var role = Theme.Label(DisplayRole.ToUpperInvariant(), 8);
            role.ForeColor = Color.FromArgb(199, 213, 229);
            role.Margin = new Padding(8, 0, 0, 18);
            sidebar.Controls.Add(role, 0, 2);
            navigation = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false, AutoScroll = true, Margin = Padding.Empty };
            sidebar.Controls.Add(navigation, 0, 3);
            var account = Theme.Label(user.Username, 9, true);
            account.ForeColor = Color.White;
            account.Margin = new Padding(8, 18, 0, 16);
            sidebar.Controls.Add(account, 0, 4);
            var bottom = new FlowLayoutPanel { AutoSize = true, Dock = DockStyle.Top, FlowDirection = FlowDirection.TopDown };
            var change = NavigationButton("Change password");
            change.Click += (s, e) => { using (var dialog = new PasswordDialog(user)) dialog.ShowDialog(this); };
            bottom.Controls.Add(change);
            var logout = NavigationButton("Sign out");
            logout.Click += (s, e) => Close();
            bottom.Controls.Add(logout);
            sidebar.Controls.Add(bottom, 0, 5);
            var main = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3, Padding = new Padding(28), Margin = Padding.Empty };
            main.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            main.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            main.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            main.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            var header = new TableLayoutPanel { AutoSize = true, Dock = DockStyle.Top, ColumnCount = 2, Margin = new Padding(0, 0, 0, 24) };
            header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            header.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            var headings = Theme.Stack();
            title = Theme.Label("Workspace", 23, true);
            title.ForeColor = Theme.Navy;
            subtitle = Theme.Label("", 10);
            subtitle.ForeColor = Theme.Muted;
            subtitle.MaximumSize = new Size(840, 0);
            headings.Controls.Add(title);
            headings.Controls.Add(subtitle);
            var refresh = Theme.Button("Reload");
            refresh.Click += (s, e) => Navigate(route);
            header.Controls.Add(headings, 0, 0);
            header.Controls.Add(refresh, 1, 0);
            main.Controls.Add(header, 0, 0);
            body = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 1, Margin = Padding.Empty };
            body.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            body.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            main.Controls.Add(body, 0, 1);
            feedback = Theme.Label("", 9);
            feedback.Name = "workspaceFeedback";
            feedback.AutoSize = true;
            feedback.Dock = DockStyle.Top;
            feedback.Margin = new Padding(0, 16, 0, 0);
            main.Controls.Add(feedback, 0, 2);
            shell.Controls.Add(sidebar, 0, 0);
            shell.Controls.Add(main, 1, 0);
            Controls.Add(shell);
            if (user.Role == "ADMINISTRATOR")
            foreach (var name in new[] { "Staff accounts", "Access requests", "Password assistance", "Audit history" })
                AddRoute(name);
            else if (user.Role == "FACULTY")
            foreach (var name in new[] { "My workspace", "My classes", "Grade entry", "Computed grades" })
                AddRoute(name);
            else if (AccessPolicy.Coordinator(user))
            foreach (var name in new[] { "My workspace", "Pending students", "Students", "Enrollment", "Evaluation", "Faculty loading", "Reports" })
                AddRoute(name);
            else
            foreach (var name in new[] { "Workspace", "Students", "Enrollment", "Grades", "Evaluation", "Faculty loading", "Reports" })
                AddRoute(name);
            Shown += (s, e) => { if (string.IsNullOrEmpty(route)) Navigate(links.Keys.First()); };
        }
        private string DisplayRole
        {
            get
            {
                return user.Role == "FACULTY" ? "Faculty" : AccessPolicy.Coordinator(user) ? AccessPolicy.Program(user) + " coordinator" : user.Role == "ADMINISTRATOR" ? "Administrator" : "Registrar";
            }
        }
        private static Button NavigationButton(string name)
        {
            var button = Theme.Button(name);
            button.AutoSize = false;
            button.Size = new Size(184, 42);
            button.TextAlign = ContentAlignment.MiddleLeft;
            button.BackColor = Theme.Navy;
            button.ForeColor = Color.White;
            button.FlatAppearance.BorderSize = 0;
            button.Margin = new Padding(0, 0, 0, 5);
            button.FlatAppearance.MouseOverBackColor = Color.FromArgb(35, 70, 112);
            button.Paint += (s, e) => { if (button.BackColor != Theme.Navy) using (var brush = new SolidBrush(Theme.Gold)) e.Graphics.FillRectangle(brush, 0, 8, 3, button.Height - 16); };
            return button;
        }
        private void AddRoute(string name)
        {
            var button = NavigationButton(name);
            button.Click += (s, e) => Navigate(name);
            navigation.Controls.Add(button);
            links.Add(name, button);
        }
        public void OpenPage(string name)
        {
            if (!links.ContainsKey(name))
                throw new UnauthorizedAccessException("This page is not available to this account.");
            Navigate(name);
        }
        public string CurrentPage
        {
            get
            {
                return route;
            }
        }
        private async void Navigate(string name)
        {
            if (busy)
                return;
            if (dirty && MessageBox.Show(this, "Discard your unsaved changes?", "Unsaved changes", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;
            dirty = false;
            generation++;
            route = name;
            feedback.Text = "";
            title.Text = name;
            foreach (var link in links)
            {
                link.Value.BackColor = link.Key == name ? Color.FromArgb(37, 74, 120) : Theme.Navy;
                link.Value.ForeColor = link.Key == name ? Theme.Gold : Color.White;
            }
            foreach (Control control in body.Controls.Cast<Control>().ToArray())
            {
                body.Controls.Remove(control);
                control.Dispose();
            }
            try
            {
                if (name == "Students")
                    await StudentList();
                else if (name == "Pending students")
                    await ReviewQueuePage();
                else if (name == "Enrollment")
                    await EnrollmentPage();
                else if (name == "Faculty loading")
                    await FacultyPage();
                else if (name == "Grade entry" || name == "Grades")
                    await GradesPage(false);
                else if (name == "Computed grades")
                    await GradesPage(true);
                else if (name == "My classes")
                    await ClassesPage();
                else if (name == "Evaluation")
                    await EvaluationPage();
                else if (name == "Reports")
                    await ReportsPage();
                else if (name == "Staff accounts")
                    await AccountsPage();
                else if (name == "Access requests")
                    await RequestsPage();
                else if (name == "Password assistance")
                    await RecoveryPage();
                else if (name == "Audit history")
                    await AuditPage();
                else
                    await Dashboard();
            }
            catch (Exception ex) { Error(ex); }
        }
        private async Task LoadData<T>(Func<T> query, Action<T> bind)
        {
            int expected = generation, read = ++readSerial;
            feedback.Text = "Loading records…";
            try
            {
                var data = await Task.Run(query);
                if (IsDisposed || expected != generation || read != readSerial)
                    return;
                feedback.Text = "";
                feedback.ForeColor = Theme.Muted;
                bind(data);
                if (!IsDisposed && expected == generation && string.IsNullOrEmpty(feedback.Text))
                    feedback.Text = "Records loaded.";
            }
            catch (Exception ex) { if (!IsDisposed && expected == generation && read == readSerial) Error(ex); }
        }
        private async Task Save(Action action, Func<Task> after = null)
        {
            if (busy)
                return;
            busy = true;
            navigation.Enabled = false;
            body.Enabled = false;
            feedback.ForeColor = Theme.Muted;
            feedback.Text = "Saving…";
            try
            {
                await Task.Run(action);
                if (IsDisposed)
                    return;
                dirty = false;
                feedback.Text = "Saved successfully.";
                if (after != null)
                    await after();
            }
            catch (Exception ex) { if (!IsDisposed) Error(ex); }
            finally { if (!IsDisposed) { busy = false; navigation.Enabled = true; body.Enabled = true; } }
        }
        private void Error(Exception ex)
        {
            System.Diagnostics.Trace.TraceError("Workspace operation: {0}", ex.GetType().Name);
            feedback.ForeColor = Color.FromArgb(155, 48, 53);
            feedback.Text = ex is ArgumentException || ex is UnauthorizedAccessException || ex is DBConcurrencyException || ex is InvalidOperationException ? ex.Message : Database.FailureMessage;
        }
        private void Dirty()
        {
            dirty = true;
            feedback.ForeColor = Theme.Navy;
            feedback.Text = "Unsaved changes.";
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (busy)
            {
                e.Cancel = true;
                feedback.Text = "Wait for the current operation to finish.";
                return;
            }
            if (dirty && MessageBox.Show(this, "Discard your unsaved changes and sign out?", "Unsaved changes", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
            {
                e.Cancel = true;
                return;
            }
            generation++;
            base.OnFormClosing(e);
        }
        private TableLayoutPanel Page(string description, params RowStyle[] rows)
        {
            subtitle.Text = description;
            var page = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = rows.Length, Margin = Padding.Empty };
            page.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            foreach (var row in rows)
                page.RowStyles.Add(row);
            body.Controls.Add(page, 0, 0);
            return page;
        }
        private static RowStyle Auto()
        {
            return new RowStyle(SizeType.AutoSize);
        }
        private static RowStyle Fill()
        {
            return new RowStyle(SizeType.Percent, 100);
        }
        private static FlowLayoutPanel Bar()
        {
            return new FlowLayoutPanel { AutoSize = true, Dock = DockStyle.Top, WrapContents = true, Margin = new Padding(0, 0, 0, 16) };
        }
        private static TextBox Input(string name, int width = 240)
        {
            return new TextBox { Name = name, AccessibleName = name, Width = width, MaxLength = 255, Margin = new Padding(0, 0, 12, 12) };
        }
        private static ComboBox Choice(string name, params string[] choices)
        {
            var combo = new ComboBox { Name = name, AccessibleName = name, Width = 200, DropDownStyle = ComboBoxStyle.DropDownList, Margin = new Padding(0, 0, 12, 12) };
            combo.Items.AddRange(choices);
            if (combo.Items.Count > 0)
                combo.SelectedIndex = 0;
            return combo;
        }
        private static Control Labeled(string name, Control field)
        {
            var wrap = Theme.Stack();
            wrap.Dock = DockStyle.None;
            wrap.MinimumSize = new Size(field.Width + 12, 0);
            wrap.Margin = new Padding(0, 0, 16, 0);
            Theme.Field(wrap, name, field);
            return wrap;
        }
        private static DataRow Selected(DataGridView grid)
        {
            return grid.CurrentRow == null ? null : (grid.CurrentRow.DataBoundItem as DataRowView)?.Row;
        }
        private static void Bind(DataGridView grid, DataTable data, params string[] visible)
        {
            grid.DataSource = data;
            foreach (DataGridViewColumn column in grid.Columns)
            {
                column.Visible = visible.Length == 0 || visible.Contains(column.Name);
                column.HeaderText = Header(column.Name);
                column.MinimumWidth = 90;
            }
            for (int i = 0; i < visible.Length; i++)
            if (grid.Columns.Contains(visible[i]))
                grid.Columns[visible[i]].DisplayIndex = i;
            foreach (DataGridViewColumn column in grid.Columns)
            {
                if (column.Name == "review_state" || column.Name == "login_usertype")
                {
                    column.MinimumWidth = 170;
                    column.FillWeight = 150;
                    column.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                    grid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                }
                else if (new[] { "prelim", "midterm", "final", "average", "convert_average", "units", "yearlevel" }.Contains(column.Name))
                {
                    column.MinimumWidth = 80;
                    column.FillWeight = 80;
                }
                else if (column.Name == "course_description" || column.Name == "recorded_grades")
                {
                    column.MinimumWidth = 220;
                    column.FillWeight = 220;
                }
                else if (column.Name == "app_disabled")
                {
                    column.MinimumWidth = 70;
                    column.FillWeight = 60;
                }
            }
        }
        private static string Header(string name)
        {
            var labels = new Dictionary<string, string> { { "studid", "Student number" }, { "stud_id", "Student number" }, { "studfname", "First name" }, { "stud_fname", "First name" }, { "studlname", "Last name" }, { "stud_lname", "Last name" }, { "studmname", "Middle name" }, { "stud_mname", "Middle name" }, { "studprogram", "Program" }, { "stud_program", "Program" }, { "yearlevel", "Year level" }, { "review_state", "Review status" }, { "course_code", "Course code" }, { "course_description", "Course description" }, { "convert_average", "Equivalent" }, { "fac_id", "Faculty ID" }, { "fac_fname", "First name" }, { "fac_lname", "Last name" }, { "fac_mname", "Middle name" }, { "school_year", "Academic year" }, { "sem", "Semester" }, { "semester_name", "Semester" }, { "program_code", "Program" }, { "section_name", "Section" }, { "faculty_id", "Faculty ID" }, { "recorded_grades", "Recorded attempts" }, { "completion", "Completion" }, { "login_user", "Username" }, { "login_fullname", "Full name" }, { "login_usertype", "Role" }, { "app_faculty_id", "Faculty ID" }, { "app_disabled", "Disabled" } };
            string value;
            return labels.TryGetValue(name, out value) ? value : CultureTitle(name.Replace('_', ' '));
        }
        private static string CultureTitle(string name)
        {
            return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name);
        }
        private static string AcademicYear
        {
            get
            {
                int year = DateTime.Today.Month >= 6 ? DateTime.Today.Year : DateTime.Today.Year - 1;
                return year + "-" + (year + 1);
            }
        }
        private static ComboBox Semester()
        {
            return Choice("Semester", "First semester", "Second semester", "Summer");
        }
        private static string Value(DataRow row, string field)
        {
            return row == null ? "" : Convert.ToString(row[field]);
        }
        private void NeedSelection()
        {
            feedback.ForeColor = Theme.Muted;
            feedback.Text = "Select a record first.";
        }
    }
}
