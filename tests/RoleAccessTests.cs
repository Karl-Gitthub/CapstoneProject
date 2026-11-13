using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CapstoneProject.Presentation;
using CapstoneProject.Services;

internal static class RoleAccessTests
{
    private static readonly string[][] Routes = {
        new[] { "Staff accounts", "Access requests", "Password assistance", "Audit history" },
        new[] { "Workspace", "Students", "Registration approvals", "Enrollment", "Evaluation", "Reports" },
        new[] { "My workspace", "Pending students", "Students", "Enrollment drafts", "Faculty loading", "Evaluation" },
        new[] { "My classes", "Grade entry", "Computed grades" }
    };
    private static void Require(bool condition, string message) { if (!condition) throw new Exception(message); }
    private static void Denied(Action action)
    {
        try { action(); } catch (UnauthorizedAccessException) { return; }
        throw new Exception("The operation should have been denied.");
    }
    private static void Wait(Func<bool> ready)
    {
        var timer = System.Diagnostics.Stopwatch.StartNew();
        while (timer.Elapsed.TotalSeconds < 15) { Application.DoEvents(); if (ready()) return; System.Threading.Thread.Sleep(20); }
        throw new Exception("Role workspace did not finish loading.");
    }
    private static bool Ready(SchoolWorkspace form, string route)
    {
        var state = form.Controls.Find("workspaceFeedback", true).OfType<Label>().Single();
        if (state.Text.Length > 0 && state.ForeColor == Color.FromArgb(155, 48, 53)) throw new Exception(route + ": " + state.Text);
        if (state.Text.StartsWith("Loading")) return false;
        return form.CurrentPage == route && (state.Text.Length > 0 ||
            (new[] { "Enrollment", "Enrollment drafts", "Evaluation", "Reports" }.Contains(route) && Controls(form).OfType<DataGridView>().Any()));
    }
    public static void Integration(Action<string, Action> check, UserSession admin, UserSession registrar, UserSession coordinator, UserSession faculty)
    {
        var management = new SchoolService(admin);
        var records = new SchoolService(registrar);
        var program = new SchoolService(coordinator);
        var student = records.Student("WORK-001");
        check("administrator cannot read or edit student records through services", () => {
            Denied(() => management.Students()); Denied(() => management.Student(student.Id));
            Denied(() => management.SaveStudent(student, false)); Denied(() => management.StudentState(student.Id));
            Denied(() => management.ReviewStudent(student.Id, student.Version, true, "Forbidden"));
        });
        check("administrator cannot access academic enrollment grades evaluation or catalogs", () => {
            Denied(() => management.Catalog("BSIT")); Denied(() => management.Faculty()); Denied(() => management.Offerings());
            Denied(() => management.Enrollment(student.Id, "2026-2027", "First semester"));
            Denied(() => management.SaveEnrollment(student.Id, "2026-2027", "First semester", -1, new[] { 1 }, true));
            Denied(() => management.Grades()); Denied(() => management.Evaluation(student.Id));
        });
        foreach (var user in new[] { admin, registrar, coordinator })
        {
            var role = user;
            check(role.Role + " cannot save grades", () => Denied(() => new SchoolService(role).SaveGrades(null)));
        }
        foreach (var user in new[] { admin, registrar, faculty })
        {
            var role = user;
            check(role.Role + " cannot create or edit teaching assignments", () => {
                var service = new SchoolService(role);
                Denied(() => service.AddOffering("BSIT", "IT201", "2026-2027", "First semester", "2A", "FAC-WORK", "M", "09:00", "10:00"));
                Denied(() => service.AddOffering("BSIT", "IT201", "2026-2027", "First semester", "2A", "FAC-WORK", "M", "09:00", "10:00", 1, 0));
                Denied(() => service.AddFaculty("DENIED", "Forbidden", "", "Profile"));
                Denied(() => service.UpdateFaculty(null, "Forbidden", "", "Profile"));
            });
        }
        check("coordinator cannot modify student details or confirm enrollment", () => {
            Denied(() => program.SaveStudent(student, false));
            Denied(() => program.SaveEnrollment(student.Id, "2026-2027", "First semester", -1, new[] { 1 }, true));
        });
        // Keep a UI message loop alive while individual workspaces close so awaited
        // reads always resume on the UI thread, as they do in the application.
        Exception failure = null;
        using (var host = new Form { StartPosition = FormStartPosition.Manual, Location = new Point(-20000, -20000), ShowInTaskbar = false })
        {
            host.Shown += (sender, args) => host.BeginInvoke((Action)(() => {
                try { Interface(check, new[] { admin, registrar, coordinator, faculty }); }
                catch (Exception error) { failure = error; }
                finally { host.Close(); }
            }));
            Application.Run(host);
        }
        if (failure != null) throw new Exception("Role workspace verification failed", failure);
    }
    private static void Interface(Action<string, Action> check, UserSession[] users)
    {
        for (int index = 0; index < users.Length; index++)
        {
            var user = users[index]; var allowed = Routes[index];
            using (var form = new SchoolWorkspace(user) { StartPosition = FormStartPosition.Manual, Location = new Point(-20000, -20000), ShowInTaskbar = false })
            {
                form.Show(); Wait(() => Ready(form, allowed[0]));
                var navigation = (FlowLayoutPanel)FindButton(form, allowed[0]).Parent;
                check(user.Role + " sidebar contains exactly its approved pages", () => Require(
                    navigation.Controls.OfType<Button>().Select(button => button.Text).SequenceEqual(allowed), "Unexpected navigation access"));
                check(user.Role + " rejects other roles' pages", () => {
                    foreach (var route in Routes.SelectMany(routes => routes).Concat(new[] { "Grades" }).Distinct().Except(allowed))
                        Denied(() => form.OpenPage(route));
                });
                foreach (var route in allowed)
                {
                    var page = route;
                    check(user.Role + " loads " + page, () => {
                        var button = FindButton(form, page);
                        navigation.ScrollControlIntoView(button); button.PerformClick(); Wait(() => Ready(form, page));
                        if (page == "Enrollment drafts") Require(FindButton(form, "Confirm enrollment") == null, "Coordinator can confirm enrollment");
                        if (page == "Computed grades") Require(FindButton(form, "Save grades") == null && form.Controls.Find("Class grades", true).OfType<DataGridView>().Single().ReadOnly, "Computed grades can be edited");
                    });
                }
                check(user.Role + " final navigation option fits at minimum size", () => {
                    form.Size = form.MinimumSize; var last = FindButton(form, allowed.Last());
                    navigation.ScrollControlIntoView(last); Application.DoEvents();
                    Require(navigation.ClientRectangle.Contains(last.Bounds), "Navigation is clipped");
                });
                if (user.Role == "REGISTRAR" || AccessPolicy.Coordinator(user))
                check(user.Role + " student profile has the expected editing access", () => {
                    form.OpenPage("Students"); Wait(() => Ready(form, "Students"));
                    var grid = form.Controls.Find("Students", true).OfType<DataGridView>().Single();
                    grid.CurrentCell = grid.Rows.Cast<DataGridViewRow>().Single(row => Convert.ToString(row.Cells["studid"].Value) == "WORK-001").Cells["studid"];
                    FindButton(form, "Open student record").PerformClick();
                    Wait(() => Controls(form).OfType<Label>().Any(label => label.Text == "WORK-001") && Ready(form, "Students"));
                    Require((FindButton(form, "Edit student") != null) == AccessPolicy.Registrar(user), "Student editor access is incorrect");
                    Require(!FindButton(form, "Approve registration", "Approve and forward").Visible, "Ordinary profile exposes review actions");
                });
                form.Close(); Application.DoEvents();
            }
        }
    }
    private static System.Collections.Generic.IEnumerable<Control> Controls(Control parent)
    {
        foreach (Control child in parent.Controls) { yield return child; foreach (var nested in Controls(child)) yield return nested; }
    }
    private static Button FindButton(Control parent, params string[] text) { return Controls(parent).OfType<Button>().FirstOrDefault(button => text.Contains(button.Text)); }
}
