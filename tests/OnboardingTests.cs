using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using CapstoneProject;
using CapstoneProject.Infrastructure;
using CapstoneProject.Presentation;
using CapstoneProject.Services;

internal static class OnboardingTests
{
    private const string Password = "OnboardingOnly2026!";
    private static void Require(bool condition, string message) { if (!condition) throw new Exception(message); }
    public static void Integration(Action<string, Action> check, string repo)
    {
        string previousData = Environment.GetEnvironmentVariable("CAPSTONE_DATA_DIR"), previousDemo = Environment.GetEnvironmentVariable("CAPSTONE_DEMO_ROOT");
        string directory = Path.Combine(repo, "artifacts", "onboarding", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        foreach (var file in Directory.GetFiles(Database.DirectoryPath, "*.accdb")) File.Copy(file, Path.Combine(directory, Path.GetFileName(file)));
        try
        {
            Environment.SetEnvironmentVariable("CAPSTONE_DATA_DIR", directory);
            Environment.SetEnvironmentVariable("CAPSTONE_DEMO_ROOT", null);
            using (var db = Database.Open(SchemaMigrator.Accounts)) Database.Execute(db, null, "DELETE FROM tbl_UserLogin");
            check("canceling first-run setup leaves no account and no login", () => Run(false, false));
            check("first-run administrator creation continues to login", () =>
            {
                Run(true, false);
                var account = new AuthenticationService().SignIn("onboarding.admin", Password);
                Require(account != null && account.Role == "ADMINISTRATOR", "Chosen administrator credentials did not authenticate");
            });
            check("existing installations go directly to login without another administrator", () => Run(false, true));
        }
        finally
        {
            Environment.SetEnvironmentVariable("CAPSTONE_DATA_DIR", previousData);
            Environment.SetEnvironmentVariable("CAPSTONE_DEMO_ROOT", previousDemo);
            AppDomain.CurrentDomain.SetData("DataDirectory", previousData);
        }
    }
    private static void Run(bool complete, bool existing)
    {
        bool sawSetup = false, sawLogin = false, submitted = false;
        Exception failure = null;
        var elapsed = System.Diagnostics.Stopwatch.StartNew();
        using (var timer = new Timer { Interval = 50 })
        {
            timer.Tick += (s, e) =>
            {
                try
                {
                    var login = Application.OpenForms.OfType<LoginForm>().FirstOrDefault();
                    if (login != null) { sawLogin = true; login.Close(); timer.Stop(); return; }
                    var setup = Application.OpenForms.OfType<FormDialog>().FirstOrDefault(form => form.Text.StartsWith("Provision first administrator"));
                    if (setup != null)
                    {
                        setup.ShowInTaskbar = false;
                        setup.Location = new System.Drawing.Point(-20000, -20000);
                        sawSetup = true;
                        if (!complete) { setup.Close(); timer.Stop(); return; }
                        if (!submitted)
                        {
                            setup.Controls.Find("username", true).Single().Text = "onboarding.admin";
                            setup.Controls.Find("name", true).Single().Text = "Synthetic setup administrator";
                            setup.Controls.Find("password", true).Single().Text = Password;
                            setup.Controls.Find("confirm", true).Single().Text = Password;
                            submitted = true;
                            FindButton(setup, "Create administrator").PerformClick();
                        }
                    }
                    if (elapsed.Elapsed.TotalSeconds > 20) throw new Exception("First-run transition timed out");
                }
                catch (Exception ex) { failure = ex; timer.Stop(); Application.ExitThread(); }
            };
            timer.Start();
            var run = typeof(LoginForm).Assembly.GetType("CapstoneProject.Program").GetMethod("Run", BindingFlags.Static | BindingFlags.NonPublic);
            run.Invoke(null, new object[] { new string[0] });
            timer.Stop();
        }
        if (failure != null) throw failure;
        Require(sawSetup == !existing, "Incorrect first-run routing");
        Require(sawLogin == (existing || complete), "Incorrect login transition after setup");
        Require(AccountAdministration.HasAdministrator() == (existing || complete), "Unexpected administrator state after setup");
        Require(Database.Query(SchemaMigrator.Accounts, "SELECT ID FROM tbl_UserLogin").Rows.Count == (existing || complete ? 1 : 0), "Setup created an unexpected account");
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
