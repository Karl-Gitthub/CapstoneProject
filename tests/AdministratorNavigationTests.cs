using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CapstoneProject.Presentation;
using CapstoneProject.Services;

internal static class AdministratorNavigationTests
{
    private static void Require(bool condition, string message) { if (!condition) throw new Exception(message); }
    private static void Wait(Func<bool> ready)
    {
        var timer = System.Diagnostics.Stopwatch.StartNew();
        while (timer.Elapsed.TotalSeconds < 15) { Application.DoEvents(); if (ready()) return; System.Threading.Thread.Sleep(20); }
        throw new Exception("Administrator page did not finish loading");
    }
    private static bool Loaded(SchoolWorkspace form)
    {
        var feedback = form.Controls.Find("workspaceFeedback", true).OfType<Label>().FirstOrDefault();
        if (feedback == null || feedback.Text.Length == 0 || feedback.Text.StartsWith("Loading")) return false;
        if (feedback.ForeColor == Color.FromArgb(155, 48, 53)) throw new Exception("Page error: " + feedback.Text);
        return feedback.Text.Length > 0;
    }
    public static void Integration(Action<string, Action> check, UserSession admin, UserSession registrar, UserSession coordinator, UserSession faculty)
    {
        // Keep a real UI message loop alive while individual workspaces close.
        // Otherwise awaited database reads can resume without the UI context.
        Exception failure = null;
        using (var host = new Form { StartPosition = FormStartPosition.Manual, Location = new Point(-20000, -20000), ShowInTaskbar = false })
        {
            host.Shown += (sender, args) => host.BeginInvoke((Action)(() =>
            {
                try { IntegrationCore(check, admin, registrar, coordinator, faculty); }
                catch (Exception error) { failure = error; }
                finally { host.Close(); }
            }));
            Application.Run(host);
        }
        if (failure != null) throw new Exception("Administrator navigation regression failed", failure);
    }
    private static void IntegrationCore(Action<string, Action> check, UserSession admin, UserSession registrar, UserSession coordinator, UserSession faculty)
    {
        var service = new SchoolService(admin);
        if (!service.Students("ADMIN-BSBA").AsEnumerable().Any(row => Convert.ToString(row["studid"]) == "ADMIN-BSBA"))
            service.SaveStudent(new StudentRecord { Id = "ADMIN-BSBA", FirstName = "Synthetic", LastName = "Business Student", Program = "BSBA", YearLevel = "2", BirthDate = new DateTime(2007, 1, 1), RegistrationDate = DateTime.Today }, true);
        check("administrator can list both programs and open a student profile", () =>
        {
            using (var form = new SchoolWorkspace(admin) { StartPosition = FormStartPosition.Manual, Location = new Point(-20000, -20000), ShowInTaskbar = false })
            {
                form.Show(); form.OpenPage("Students");
                DataGridView grid = null;
                Wait(() => { grid = form.Controls.Find("Students", true).OfType<DataGridView>().FirstOrDefault(); return grid != null && grid.DataSource is DataTable && Loaded(form); });
                var table = (DataTable)grid.DataSource;
                Require(table.AsEnumerable().Any(r => Convert.ToString(r["studid"]) == "WORK-001") && table.AsEnumerable().Any(r => Convert.ToString(r["studid"]) == "ADMIN-BSBA"), "Administrator student scope omitted a program");
                grid.CurrentCell = grid.Rows.Cast<DataGridViewRow>().Single(row => Convert.ToString(row.Cells["studid"].Value) == "WORK-001").Cells["studid"];
                FindButton(form, "Open student record").PerformClick();
                Wait(() => FindButton(form, "Edit student") != null && Controls(form).OfType<Label>().Any(label => label.Text == "WORK-001") && Loaded(form));
                Require(FindButton(form, "Edit student").Enabled, "Administrator cannot access the student editor");
                form.Close(); Application.DoEvents();
            }
        });
        check("administrator management navigation remains reachable at minimum size", () =>
        {
            using (var form = new SchoolWorkspace(admin) { StartPosition = FormStartPosition.Manual, Location = new Point(-20000, -20000), ShowInTaskbar = false })
            {
                form.Size = form.MinimumSize; form.Show();
                Wait(() => Loaded(form));
                var audit = FindButton(form, "Audit history");
                var panel = (FlowLayoutPanel)audit.Parent;
                panel.ScrollControlIntoView(audit); Application.DoEvents();
                Require(panel.ClientRectangle.Contains(audit.Bounds), "Last administration option is clipped after scrolling");
                audit.PerformClick(); Wait(() => form.CurrentPage == "Audit history" && Loaded(form));
                form.Close(); Application.DoEvents();
            }
        });
        check("administrator navigation change does not expose management pages to other roles", () =>
        {
            foreach (var user in new[] { registrar, coordinator, faculty })
            using (var form = new SchoolWorkspace(user))
            {
                bool denied = false;
                try { form.OpenPage("Staff accounts"); } catch (UnauthorizedAccessException) { denied = true; }
                Require(denied, "A non-administrator gained account-management navigation");
            }
            using (var form = new SchoolWorkspace(faculty))
            {
                bool denied = false;
                try { form.OpenPage("Students"); } catch (UnauthorizedAccessException) { denied = true; }
                Require(denied, "Faculty gained unrestricted student navigation");
            }
        });
        using (var form = new SchoolWorkspace(admin) { StartPosition = FormStartPosition.Manual, Location = new Point(-20000, -20000), ShowInTaskbar = false })
        {
            form.Show(); Wait(() => Loaded(form));
            foreach (var route in new[] { "Workspace", "Students", "Enrollment", "Grades", "Evaluation", "Faculty loading", "Reports", "Staff accounts", "Access requests", "Password assistance", "Audit history" })
            {
                var page = route;
                check("administrator can open " + page, () =>
                {
                    var button = FindButton(form, page);
                    ((FlowLayoutPanel)button.Parent).ScrollControlIntoView(button);
                    button.PerformClick();
                    bool deferred = page == "Enrollment" || page == "Evaluation" || page == "Reports";
                    Wait(() =>
                    {
                        var state = form.Controls.Find("workspaceFeedback", true).OfType<Label>().Single();
                        if (state.Text.Length > 0 && state.ForeColor == Color.FromArgb(155, 48, 53)) throw new Exception(page + ": " + state.Text);
                        return form.CurrentPage == page && (deferred ? Controls(form).OfType<DataGridView>().Any() : Loaded(form));
                    });
                });
            }
            form.Close(); Application.DoEvents();
        }
    }
    private static System.Collections.Generic.IEnumerable<Control> Controls(Control parent)
    {
        foreach (Control child in parent.Controls) { yield return child; foreach (var nested in Controls(child)) yield return nested; }
    }
    private static Button FindButton(Control parent, string text) { return Controls(parent).OfType<Button>().FirstOrDefault(button => button.Text == text); }
}
