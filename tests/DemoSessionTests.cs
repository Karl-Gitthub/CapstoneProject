using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using CapstoneProject;
using CapstoneProject.Infrastructure;
using CapstoneProject.Presentation;
using CapstoneProject.Services;

internal static class DemoSessionTests
{
    private static void Equal<T>(T expected, T actual)
    {
        if (!Equals(expected, actual)) throw new Exception("Expected " + expected + ", got " + actual);
    }
    private static void Throws<T>(Action action) where T : Exception
    {
        try { action(); } catch (T) { return; }
        throw new Exception("Expected " + typeof(T).Name);
    }
    private static void Session(Action<string> test)
    {
        string previousRoot = Environment.GetEnvironmentVariable("CAPSTONE_DEMO_ROOT"), previousData = Environment.GetEnvironmentVariable("CAPSTONE_DATA_DIR");
        string root = Path.Combine(Path.GetTempPath(), "CapstoneDemoTests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(Path.Combine(root, "data"));
        File.WriteAllText(Path.Combine(root, ".demo-session"), DemoSession.Marker);
        try
        {
            Environment.SetEnvironmentVariable("CAPSTONE_DEMO_ROOT", root);
            Environment.SetEnvironmentVariable("CAPSTONE_DATA_DIR", Path.Combine(root, "data"));
            test(root);
        }
        finally
        {
            Environment.SetEnvironmentVariable("CAPSTONE_DEMO_ROOT", previousRoot);
            Environment.SetEnvironmentVariable("CAPSTONE_DATA_DIR", previousData);
        }
    }
    public static void Units(Action<string, Action> check)
    {
        check("demo session prevents reset for the entire window lifetime", () => Session(root =>
        {
            string path = Path.Combine(root, ".demo-in-use.lock");
            using (DemoSession.Open())
            using (DemoSession.Open())
                Throws<IOException>(() => { using (File.Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None)) { } });
            using (File.Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None)) { }
        }));
        check("exclusive reset guard prevents a demo from starting", () => Session(root =>
        {
            using (File.Open(Path.Combine(root, ".demo-in-use.lock"), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
                Throws<IOException>(() => { using (DemoSession.Open()) { } });
        }));
        check("demo startup rejects a working-directory mismatch", () => Session(root =>
        {
            Environment.SetEnvironmentVariable("CAPSTONE_DATA_DIR", Path.Combine(root, "different"));
            Throws<InvalidOperationException>(() => { using (DemoSession.Open()) { } });
        }));
        check("demo startup rejects an unrecognized session marker", () => Session(root =>
        {
            File.WriteAllText(Path.Combine(root, ".demo-session"), "unrecognized");
            Throws<InvalidOperationException>(() => { using (DemoSession.Open()) { } });
        }));
    }
    public static void Interface(Action<string, Action> check, UserSession user, string repo)
    {
        check("demo login visibly identifies sample data", () => Session(root =>
        {
            using (var form = new LoginForm())
            {
                Equal(true, form.Text.StartsWith("Demo"));
                Equal(1, form.Controls.Find("demoIndicator", true).Length);
                form.StartPosition = FormStartPosition.Manual;
                form.Location = new Point(-20000, -20000);
                form.ShowInTaskbar = false;
                form.Show(); Application.DoEvents();
                string output = Path.Combine(repo, "artifacts", "demo-renders");
                Directory.CreateDirectory(output);
                using (var bitmap = new Bitmap(form.Width, form.Height))
                {
                    form.DrawToBitmap(bitmap, new Rectangle(Point.Empty, bitmap.Size));
                    bitmap.Save(Path.Combine(output, "demo-login.png"));
                }
                form.Hide();
            }
        }));
        check("demo workspace visibly identifies sample data", () => Session(root =>
        {
            using (var form = new SchoolWorkspace(user))
            {
                Equal(true, form.Text.StartsWith("Demo"));
                var label = form.Controls.Find("demoIndicator", true).OfType<Label>().Single();
                Equal("DEMO · SAMPLE DATA", label.Text);
            }
        }));
    }
}
