using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using CapstoneProject.Services;

namespace CapstoneProject.Presentation
{
    public class FormDialog : Form
    {
        private readonly Dictionary<string, Control> fields = new Dictionary<string, Control>();
        private readonly TableLayoutPanel inputs;
        private readonly Label message;
        private readonly Button commit;
        private readonly Action<Dictionary<string, string>> work;
        private Control editorSurface;
        private TabControl requestTabs;
        private bool busy, dirty;
        public FormDialog(string title, string description, Action<Dictionary<string, string>> work, string action = "Save")
        {
            this.work = work;
            Text = title + " · Student Records";
            Font = new Font("Segoe UI", 10);
            BackColor = Color.White;
            AutoScaleDimensions = new SizeF(96, 96);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(660, 680);
            MinimumSize = new Size(580, 520);
            StartPosition = FormStartPosition.CenterParent;
            var shell = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 5, Padding = new Padding(28) };
            shell.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            shell.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            shell.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            shell.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            shell.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            shell.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            var heading = Theme.Label(title, 21, true);
            heading.ForeColor = Theme.Navy;
            shell.Controls.Add(heading, 0, 0);
            var hint = Theme.Label(description, 9);
            hint.MaximumSize = new Size(560, 0);
            hint.Margin = new Padding(0, 0, 0, 20);
            shell.Controls.Add(hint, 0, 1);
            var scroll = new Panel { Dock = DockStyle.Fill, AutoScroll = true };
            editorSurface = scroll;
            inputs = Theme.Stack();
            inputs.Padding = new Padding(0, 0, 24, 0);
            inputs.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            scroll.Controls.Add(inputs);
            shell.Controls.Add(scroll, 0, 2);
            message = Theme.Label("", 9);
            message.MaximumSize = new Size(560, 0);
            shell.Controls.Add(message, 0, 3);
            var actions = new FlowLayoutPanel { AutoSize = true, Dock = DockStyle.Top, FlowDirection = FlowDirection.RightToLeft, Margin = new Padding(0, 16, 0, 0) };
            commit = Theme.Button(action, true);
            var cancel = Theme.Button("Cancel");
            cancel.Click += (s, e) => Close();
            commit.Click += async (s, e) => await Commit();
            actions.Controls.Add(commit);
            actions.Controls.Add(cancel);
            shell.Controls.Add(actions, 0, 4);
            Controls.Add(shell);
            AcceptButton = commit;
            CancelButton = cancel;
        }
        public FormDialog Field(string key, string label, string value = "", bool password = false, bool multiline = false)
        {
            var input = new TextBox { Name = key, Text = value, MaxLength = multiline ? 1000 : 255, UseSystemPasswordChar = password, Multiline = multiline, Height = multiline ? 84 : 28, ScrollBars = multiline ? ScrollBars.Vertical : ScrollBars.None };
            fields[key] = input;
            Theme.Field(inputs, label, input);
            input.TextChanged += (s, e) => dirty = true;
            return this;
        }
        public FormDialog Options(string key, string label, IEnumerable<string> options, string value = null)
        {
            var choice = new ComboBox { Name = key, DropDownStyle = ComboBoxStyle.DropDownList };
            choice.Items.AddRange(options.Cast<object>().ToArray());
            if (value != null && choice.Items.Contains(value))
                choice.SelectedItem = value;
            else if (choice.Items.Count > 0)
                choice.SelectedIndex = 0;
            fields[key] = choice;
            Theme.Field(inputs, label, choice);
            choice.SelectedIndexChanged += (s, e) => dirty = true;
            return this;
        }
        public FormDialog Records(string key, string label, DataTable data, string valueColumn, string[] displayColumns, string selected = null)
        {
            var table = data.Copy();
            table.Columns.Add("display_text");
            foreach (DataRow row in table.Rows)
                row["display_text"] = string.Join(" · ", displayColumns.Select(c => Convert.ToString(row[c])));
            var choice = new ComboBox { Name = key, DropDownStyle = ComboBoxStyle.DropDownList, BindingContext = new BindingContext(), DataSource = table, ValueMember = valueColumn, DisplayMember = "display_text" };
            if (selected != null)
                choice.SelectedValue = selected;
            fields[key] = choice;
            Theme.Field(inputs, label, choice);
            choice.SelectedIndexChanged += (s, e) => dirty = true;
            return this;
        }
        public FormDialog Lock(string key)
        {
            fields[key].Enabled = false;
            return this;
        }
        protected void SeparateRequestCredentials()
        {
            var pairs = fields.Select(pair => new { Key = pair.Key, Field = pair.Value, Label = inputs.Controls[inputs.Controls.IndexOf(pair.Value) - 1] }).ToArray();
            editorSurface.Controls.Remove(inputs);
            requestTabs = new TabControl { Dock = DockStyle.Fill };
            var details = new TabPage("Request details") { BackColor = Color.White, Padding = new Padding(12), AutoScroll = true };
            var credentials = new TabPage("Account password") { BackColor = Color.White, Padding = new Padding(12), AutoScroll = true };
            var detailFields = Theme.Stack(); detailFields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            var passwordFields = Theme.Stack(); passwordFields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            var hint = Theme.Label("Choose a password with 12–128 characters, including letters and numbers.", 9); hint.MaximumSize = new Size(500, 0); passwordFields.Controls.Add(hint);
            foreach (var pair in pairs) { var target = pair.Key == "password" || pair.Key == "confirm" ? passwordFields : detailFields; target.Controls.Add(pair.Label); target.Controls.Add(pair.Field); }
            details.Controls.Add(detailFields); credentials.Controls.Add(passwordFields); requestTabs.TabPages.AddRange(new[] { details, credentials }); editorSurface.Controls.Add(requestTabs); inputs.Dispose();
        }
        private async Task Commit()
        {
            if (busy)
                return;
            if (requestTabs != null && fields["password"].Text.Length == 0) { requestTabs.SelectedIndex = 1; message.Text = "Choose and confirm the account password before submitting."; fields["password"].Focus(); return; }
            var values = fields.ToDictionary(p => p.Key, p => p.Value is ComboBox combo && combo.DataSource != null ? Convert.ToString(combo.SelectedValue) : p.Value is TextBox box && box.UseSystemPasswordChar ? box.Text : p.Value.Text.Trim());
            busy = true;
            editorSurface.Enabled = false;
            commit.Enabled = false;
            message.ForeColor = Theme.Muted;
            message.Text = "Saving…";
            try
            {
                await Task.Run(() => work(values));
                dirty = false;
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex) { message.ForeColor = Color.FromArgb(155, 48, 53); message.Text = ex is ArgumentException || ex is UnauthorizedAccessException || ex is DBConcurrencyException || ex is InvalidOperationException ? ex.Message : Infrastructure.Database.FailureMessage; }
            finally { busy = false; if (!IsDisposed) { editorSurface.Enabled = true; commit.Enabled = true; } }
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (busy && DialogResult != DialogResult.OK)
            {
                e.Cancel = true;
                return;
            }
            if (dirty && DialogResult != DialogResult.OK && MessageBox.Show(this, "Discard your unsaved changes?", "Unsaved changes", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
            {
                e.Cancel = true;
                return;
            }
            base.OnFormClosing(e);
        }
        public static FormDialog Faculty(SchoolService service, DataRow original = null)
        {
            var dialog = new FormDialog(original == null ? "Add faculty profile" : "Edit faculty profile", "Use the staff identifier linked to the faculty account.", v => { if (original == null) service.AddFaculty(v["id"], v["first"], v["middle"], v["last"]); else service.UpdateFaculty(original, v["first"], v["middle"], v["last"]); })
                .Field("id", "Faculty identifier *", original == null ? "" : Convert.ToString(original["fac_id"])).Field("first", "First name *", original == null ? "" : Convert.ToString(original["fac_fname"])).Field("middle", "Middle name", original == null ? "" : Convert.ToString(original["fac_mname"])).Field("last", "Last name *", original == null ? "" : Convert.ToString(original["fac_lname"]));
            if (original != null)
                dialog.Lock("id");
            return dialog;
        }
        public static FormDialog Offering(SchoolService service, string program, DataTable courses, DataTable faculty, string year, DataRow original = null)
        {
            var dialog = new FormDialog(original == null ? "Assign course" : "Edit teaching assignment", program + " · Times use a 24-hour clock. Updating an enrolled class changes the linked instructor access.", v => service.AddOffering(program, v["course"], v["year"], v["semester"], v["section"], v["faculty"], v["days"], v["start"], v["end"], original == null ? 0 : Convert.ToInt32(original["offering_id"]), original == null ? 0 : Convert.ToInt32(original["record_version"])))
                .Records("course", "Course *", courses, "course_code", new[] { "course_code", "course_description" }, original == null ? null : Convert.ToString(original["course_code"])).Records("faculty", "Faculty *", faculty, "fac_id", new[] { "fac_id", "fac_fname", "fac_lname" }, original == null ? null : Convert.ToString(original["faculty_id"]))
                .Field("year", "Academic year *", original == null ? year : Convert.ToString(original["school_year"])).Options("semester", "Semester *", new[] { "First semester", "Second semester", "Summer" }, original == null ? null : Convert.ToString(original["semester_name"])).Field("section", "Section *", original == null ? "" : Convert.ToString(original["section_name"]))
                .Field("days", "Days: M,Tu,W,Th,F,Sa,Su *", original == null ? "M,W" : Convert.ToString(original["day_codes"])).Field("start", "Start time *", original == null ? "09:00" : TimeSpan.FromMinutes(Convert.ToInt32(original["start_minute"])).ToString(@"hh\:mm")).Field("end", "End time *", original == null ? "10:30" : TimeSpan.FromMinutes(Convert.ToInt32(original["end_minute"])).ToString(@"hh\:mm"));
            if (original != null)
                dialog.Lock("course").Lock("year").Lock("semester");
            return dialog;
        }
    }
    public sealed class PasswordDialog : FormDialog
    {
        public PasswordDialog(UserSession user) : base("Change password", user.MustChangePassword ? "Change your temporary password before opening school records." : "Use 12–128 characters, including letters and numbers.", v =>
        {
            if (v["new"] != v["confirm"])
                throw new ArgumentException("The new password and confirmation must match.");
            new AccountAdministration().ChangePassword(user, v["current"], v["new"]);
        }, "Change password")
        {
            Field("current", "Current password", password: true);
            Field("new", "New password", password: true);
            Field("confirm", "Confirm new password", password: true);
        }
    }
    public sealed class AccessRequestDialog : FormDialog
    {
        public AccessRequestDialog() : base("Request staff access", "An administrator must approve your request and assign your role. No access is granted immediately.", v =>
        {
            if (v["password"] != v["confirm"])
                throw new ArgumentException("The password and confirmation must match.");
            new AccountAdministration().Request(v["username"], v["name"], v["email"], v["department"], v["reason"], v["password"]);
        }, "Submit access request")
        {
            Field("name", "Full name *");
            Field("email", "Staff email *");
            Field("username", "Username *");
            Options("department", "Department", new[] { "Registrar", "BSIT", "BSBA", "Other" });
            Field("reason", "Reason for access *", multiline: true);
            Field("password", "Choose a password (12–128 characters) *", password: true);
            Field("confirm", "Confirm password *", password: true);
            SeparateRequestCredentials();
        }
    }
}
