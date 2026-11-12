using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using CapstoneProject.Infrastructure;
using CapstoneProject.Services;

namespace CapstoneProject
{
    public partial class LoginForm : Form
    {
        private DateTime retryAfter;
        private int failures;
        public LoginForm()
        {
            InitializeComponent();
        }
        private async void btnLogin_Click(object sender, EventArgs e)
        {
            if (DateTime.UtcNow < retryAfter)
            {
                status.Text = "Please wait a minute before trying again.";
                return;
            }
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrEmpty(txtPassword.Text))
            {
                status.Text = "Enter your username and password.";
                return;
            }
            string username = txtUsername.Text, password = txtPassword.Text;
            btnLogin.Enabled = false;
            txtUsername.Enabled = false;
            txtPassword.Enabled = false;
            status.Text = "Signing in…";
            try
            {
                var user = await Task.Run(() => new AuthenticationService().SignIn(username, password));
                if (IsDisposed)
                    return;
                if (user == null)
                {
                    if (++failures >= 5)
                    {
                        retryAfter = DateTime.UtcNow.AddMinutes(1);
                        failures = 0;
                    }
                    status.Text = "Sign-in failed. Check your username and password.";
                    return;
                }
                failures = 0;
                if (user.MustChangePassword)
                {
                    using (var passwordDialog = new Presentation.PasswordDialog(user))
                        if (passwordDialog.ShowDialog(this) != DialogResult.OK)
                        {
                            status.Text = "Change your temporary password to continue.";
                            return;
                        }
                }
                Form next = new Presentation.SchoolWorkspace(user);
                txtPassword.Clear();
                Hide();
                using (next)
                    next.ShowDialog();
                if (!IsDisposed)
                {
                    Show();
                    status.Text = "Signed out.";
                    txtPassword.Focus();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("Sign-in failed: {0}", ex.GetType().Name);
                if (!IsDisposed)
                    status.Text = Database.FailureMessage;
            }
            finally { if (!IsDisposed) { btnLogin.Enabled = true; txtUsername.Enabled = true; txtPassword.Enabled = true; } }
        }
    }
}
