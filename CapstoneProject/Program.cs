using System;
using System.Windows.Forms;

namespace CapstoneProject
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            IDisposable demo;
            try { demo = Infrastructure.DemoSession.Open(); }
            catch (Exception ex)
            {
                if (Array.IndexOf(args, "--migrate-only") < 0)
                    MessageBox.Show(ex.Message, "Demo setup", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.ExitCode = 1;
                return;
            }
            using (demo) Run(args);
        }
        private static void Run(string[] args)
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", Infrastructure.Database.DirectoryPath);
            if (Array.IndexOf(args, "--preview-students") >= 0)
            {
                Application.Run(new RegistrarDirectoryForm(true));
                return;
            }
            try
            {
                Infrastructure.SchemaMigrator.Ensure();
            }
            catch (Exception ex)
            {
                if (Array.IndexOf(args, "--migrate-only") < 0)
                    MessageBox.Show("Local database setup could not be completed. Run scripts/Initialize-LocalData.ps1 after closing database users.\n\n" + ex.Message, "Student Records", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.ExitCode = 1;
                return;
            }
            if (Array.IndexOf(args, "--migrate-only") >= 0)
                return;
            if (Array.IndexOf(args, "--provision-admin") >= 0)
            {
                using (var dialog = new Presentation.FormDialog(Infrastructure.DemoSession.Enabled ? "Demo · Create administrator" : "Provision first administrator", Infrastructure.DemoSession.Enabled ? "Sample-data demo only. Choose your own administrator credentials; no default password is supplied." : "Local setup only. This works once, before an administrator exists. Keep the administrator password private.", v =>
                {
                    if (v["password"] != v["confirm"])
                        throw new ArgumentException("The passwords must match.");
                    Services.AccountAdministration.Bootstrap(v["username"], v["name"], v["password"]);
                }, "Create administrator"))
                {
                    dialog.Field("username", "Administrator username").Field("name", "Full name").Field("password", "Password (12–128 characters)", password: true).Field("confirm", "Confirm password", password: true);
                    dialog.ShowDialog();
                }
                return;
            }
            Application.Run(new LoginForm());
        }
    }
}
