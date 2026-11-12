using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using CapstoneProject.Services;

namespace CapstoneProject.Presentation
{
    public sealed class StudentEditor : Form
    {
        private readonly SchoolService service;
        private readonly StudentRecord original;
        private readonly Dictionary<string, Control> fields = new Dictionary<string, Control>();
        private readonly Label message;
        private readonly ErrorProvider errors;
        private bool dirty, busy;
        public string StudentId
        {
            get; private set;
        }
        public static readonly Dictionary<string, string> CredentialFields = new Dictionary<string, string> { { "BirthCertificate", "Birth certificate" }, { "GoodMoral", "Good moral certificate" }, { "HonorableDismissal", "Honorable dismissal" }, { "Diploma", "High school diploma" }, { "CopyOfGrades", "Copy of grades" }, { "Transcript", "Transcript of records" }, { "Form137", "Form 137" }, { "Form138", "Form 138" }, { "Thesis", "Thesis" }, { "Ojt", "OJT certificate" } };
        public StudentEditor(SchoolService service, StudentRecord record)
        {
            this.service = service;
            original = record;
            Text = (record == null ? "Add student" : "Edit student") + " · Student Records";
            Font = new Font("Segoe UI", 10);
            BackColor = Color.White;
            AutoScaleDimensions = new SizeF(96, 96);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(960, 740);
            MinimumSize = new Size(780, 620);
            StartPosition = FormStartPosition.CenterParent;
            errors = new ErrorProvider { ContainerControl = this, BlinkStyle = ErrorBlinkStyle.NeverBlink };
            var shell = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 4, Padding = new Padding(28) };
            shell.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            shell.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            shell.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            shell.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            shell.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            var heading = Theme.Label(record == null ? "Add student" : record.FirstName + " " + record.LastName, 22, true);
            heading.ForeColor = Theme.Navy;
            shell.Controls.Add(heading, 0, 0);
            var tabs = new TabControl { Dock = DockStyle.Fill, Margin = new Padding(0, 20, 0, 20) };
            shell.Controls.Add(tabs, 0, 1);
            AddTab(tabs, "Identity", new[] { "Id|Student number *", "Program|Program *", "FirstName|First name *", "MiddleName|Middle name", "LastName|Last name *", "Suffix|Suffix", "YearLevel|Year level", "BirthDate|Birth date *", "RegistrationDate|Registration date *", "Gender|Gender", "Nationality|Nationality" });
            AddTab(tabs, "Contact & address", new[] { "Phone|Contact number", "Email|Email address", "StreetNumber|House / unit number", "Street|Street", "Barangay|Barangay", "City|City / municipality", "Province|Province / region", "Zip|Postal code" });
            AddTab(tabs, "Prior education", new[] { "PrimarySchool|Primary school", "PrimaryYear|Year completed", "SecondarySchool|Secondary school", "SecondaryYear|Year completed" });
            AddTab(tabs, "Credentials", CredentialFields.Select(p => p.Key + "|" + p.Value).ToArray());
            message = Theme.Label("Required fields are marked *. Your entries are retained if saving fails.", 9);
            message.MaximumSize = new Size(850, 0);
            shell.Controls.Add(message, 0, 2);
            var buttons = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, FlowDirection = FlowDirection.RightToLeft, Margin = new Padding(0, 12, 0, 0) };
            var save = Theme.Button("Save student", true);
            var cancel = Theme.Button("Cancel");
            cancel.Click += (s, e) => Close();
            save.Click += async (s, e) => await Commit(save, tabs);
            buttons.Controls.Add(save);
            buttons.Controls.Add(cancel);
            shell.Controls.Add(buttons, 0, 3);
            Controls.Add(shell);
            AcceptButton = save;
            CancelButton = cancel;
            if (record != null)
                fields["Id"].Enabled = false;
        }
        private void AddTab(TabControl tabs, string name, string[] specs)
        {
            var tab = new TabPage(name) { BackColor = Color.White, Padding = new Padding(20), AutoScroll = true };
            var table = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 2, Padding = new Padding(0, 0, 24, 0) };
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            for (int i = 0; i < specs.Length; i++)
            {
                var spec = specs[i].Split('|');
                string key = spec[0];
                Control field;
                var model = typeof(StudentRecord).GetField(key);
                if (model.FieldType == typeof(DateTime))
                {
                    var date = new DateTimePicker { Format = DateTimePickerFormat.Short, ShowCheckBox = true };
                    var value = original == null ? DateTime.Today : (DateTime)model.GetValue(original);
                    date.Value = value.Year >= 1900 ? value : DateTime.Today;
                    date.Checked = original == null ? key == "RegistrationDate" : value.Year >= 1900;
                    date.ValueChanged += (s, e) => dirty = true;
                    field = date;
                }
                else if (key == "Program" || key == "Gender" || CredentialFields.ContainsKey(key))
                {
                    var choice = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
                    choice.Items.Add("");
                    choice.Items.AddRange(key == "Program" ? new object[] { "BSIT", "BSBA" } : key == "Gender" ? new object[] { "Female", "Male", "Prefer not to say" } : new object[] { "Submitted", "Not Submitted", "Not Applicable" });
                    string value = original == null ? "" : Convert.ToString(model.GetValue(original));
                    if (!choice.Items.Contains(value))
                        choice.Items.Add(value);
                    choice.SelectedItem = value;
                    choice.SelectedIndexChanged += (s, e) => dirty = true;
                    field = choice;
                }
                else
                {
                    var input = new TextBox { MaxLength = 255, Text = original == null ? "" : Convert.ToString(model.GetValue(original)) };
                    input.TextChanged += (s, e) => dirty = true;
                    field = input;
                }
                field.Name = key;
                field.AccessibleName = spec[1];
                field.Dock = DockStyle.Top;
                field.Margin = new Padding(0, 0, 12, 20);
                field.TabIndex = i;
                var stack = Theme.Stack();
                stack.Dock = DockStyle.Top;
                stack.Margin = new Padding(0, 0, 16, 0);
                stack.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
                Theme.Field(stack, spec[1], field);
                table.Controls.Add(stack, i % 2, i / 2);
                fields[key] = field;
            }
            tab.Controls.Add(table);
            tabs.TabPages.Add(tab);
        }
        private async Task Commit(Button save, TabControl tabs)
        {
            if (busy)
                return;
            errors.Clear();
            foreach (var key in new[] { "Id", "FirstName", "LastName", "Program" })
            if (string.IsNullOrWhiteSpace(fields[key].Text))
            {
                errors.SetError(fields[key], "Required");
                message.Text = "Complete the required identity fields.";
                tabs.SelectedIndex = 0;
                fields[key].Focus();
                return;
            }
            var record = new StudentRecord { Version = original == null ? 0 : original.Version };
            foreach (var pair in fields)
            {
                var field = typeof(StudentRecord).GetField(pair.Key);
                var date = pair.Value as DateTimePicker;
                field.SetValue(record, date == null ? (object)pair.Value.Text.Trim() : date.Checked ? date.Value.Date : DateTime.MinValue);
            }
            try
            {
                record.Validate();
                busy = true;
                save.Enabled = false;
                tabs.Enabled = false;
                message.ForeColor = Theme.Muted;
                message.Text = "Saving student record…";
                await Task.Run(() => service.SaveStudent(record, original == null));
                StudentId = record.Id;
                dirty = false;
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex) { message.ForeColor = Color.FromArgb(155, 48, 53); message.Text = ex is ArgumentException || ex is System.Data.DBConcurrencyException || ex is UnauthorizedAccessException ? ex.Message : Infrastructure.Database.FailureMessage; }
            finally { busy = false; if (!IsDisposed) { save.Enabled = true; tabs.Enabled = true; } }
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (busy && DialogResult != DialogResult.OK)
            {
                e.Cancel = true;
                return;
            }
            if (dirty && DialogResult != DialogResult.OK && MessageBox.Show(this, "Discard your unsaved student changes?", "Unsaved changes", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
            {
                e.Cancel = true;
                return;
            }
            base.OnFormClosing(e);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                errors.Dispose();
            base.Dispose(disposing);
        }
    }
}
