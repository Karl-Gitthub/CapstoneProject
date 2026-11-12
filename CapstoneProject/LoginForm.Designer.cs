using System.Drawing;
using System.Windows.Forms;
using CapstoneProject.Presentation;
namespace CapstoneProject
{
    partial class LoginForm
    {
        private System.ComponentModel.IContainer components;
        private TextBox txtUsername, txtPassword;
        private Button btnLogin;
        private Label status;
        protected override void Dispose(bool disposing) { if (disposing && components != null) components.Dispose(); base.Dispose(disposing); }
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            SuspendLayout();
            AutoScaleDimensions = new SizeF(96,96); AutoScaleMode = AutoScaleMode.Dpi;
            Font = new Font("Segoe UI", 10); BackColor = Color.White;
            Text = CapstoneProject.Infrastructure.DemoSession.Enabled ? "Demo · Sign in · Student Records" : "Sign in · Student Records"; StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(960, 680); MinimumSize = new Size(860, 640);
            var shell = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1 };
            shell.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 39)); shell.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 61));
            shell.RowStyles.Add(new RowStyle(SizeType.Percent,100));
            var brand = new Panel { Dock = DockStyle.Fill, BackColor = Theme.Navy, Padding = new Padding(32), Margin = Padding.Empty };
            var brandStack = Theme.Stack();
            var name = Theme.Label("SYSTEMS PLUS\nCOMPUTER COLLEGE", 15, true); name.ForeColor = Color.White; name.Margin = new Padding(0,48,0,40); brandStack.Controls.Add(name);
            var title = Theme.Label("Student records", 22, true); title.ForeColor = Color.White; brandStack.Controls.Add(title);
            var subtitle = Theme.Label("Registration, enrollment\nand academic records.", 11); subtitle.ForeColor = Color.FromArgb(210,220,230); brandStack.Controls.Add(subtitle);
            if (CapstoneProject.Infrastructure.DemoSession.Enabled)
            {
                var demo = Theme.Label("DEMO · SAMPLE DATA", 10, true); demo.Name = "demoIndicator"; demo.ForeColor = Theme.Gold; demo.Margin = new Padding(0,24,0,0); brandStack.Controls.Add(demo);
            }
            brand.Controls.Add(brandStack); brand.Controls.Add(new Panel { Dock=DockStyle.Top,Height=4,BackColor=Theme.Gold });
            var content = new Panel { Dock = DockStyle.Fill, Padding = new Padding(40,48,40,24), Margin = Padding.Empty, AutoScroll = true };
            var fields = Theme.Stack(); fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,100));
            fields.Controls.Add(Theme.Label("Sign in",22,true));
            var help = Theme.Label("Use your staff account to continue."); help.ForeColor = Theme.Muted; help.Margin = new Padding(0,0,0,32); fields.Controls.Add(help);
            txtUsername = new TextBox { Name = "txtUsername", TabIndex = 0, MaxLength = 255 };
            txtPassword = new TextBox { Name = "txtPassword", UseSystemPasswordChar = true, TabIndex = 1, MaxLength = 255 };
            Theme.Field(fields,"&Username",txtUsername); Theme.Field(fields,"&Password",txtPassword);
            var reveal = new CheckBox { Text = "Show password", AutoSize = true, TabIndex = 2, Margin = new Padding(0,0,0,24) };
            reveal.CheckedChanged += (s,e) => txtPassword.UseSystemPasswordChar = !reveal.Checked; fields.Controls.Add(reveal);
            btnLogin = Theme.Button("&Sign in",true); btnLogin.Dock = DockStyle.Top; btnLogin.TabIndex = 3; btnLogin.Click += btnLogin_Click; fields.Controls.Add(btnLogin);
            status = Theme.Label(""); status.Name = "status"; status.MaximumSize = new Size(380,0); status.AutoSize = true; status.ForeColor = Color.FromArgb(145,40,40); status.Margin = new Padding(0,16,0,16); fields.Controls.Add(status);
            var support = Theme.Label("Need access? Contact your registrar.",9); support.ForeColor = Theme.Muted; fields.Controls.Add(support);
            var request=new LinkLabel {Text="Request staff access",AutoSize=true,LinkColor=Theme.Blue,Margin=new Padding(0,12,0,12)};
            request.LinkClicked+=(s,e)=>{using(var dialog=new AccessRequestDialog())if(dialog.ShowDialog(this)==DialogResult.OK)status.Text="If eligible, your request will be reviewed by an administrator. No account access is granted until approval.";}; fields.Controls.Add(request);
            var recovery=new LinkLabel {Text="Forgot your password?",AutoSize=true,LinkColor=Theme.Blue,Margin=new Padding(0,0,0,8)};
            recovery.LinkClicked+=(s,e)=>{using(var dialog=new FormDialog("Request password assistance","An administrator must verify your identity. This local application does not send automated reset emails.",v=>new CapstoneProject.Services.AccountAdministration().RequestRecovery(v["username"],v["email"]),"Submit request")){dialog.Field("username","Username").Field("email","Staff email");if(dialog.ShowDialog(this)==DialogResult.OK)status.Text="If eligible, your request will be reviewed. Contact your school administrator through the usual staff-support channel.";}}; fields.Controls.Add(recovery);
            content.Controls.Add(fields); shell.Controls.Add(brand,0,0); shell.Controls.Add(content,1,0); Controls.Add(shell);
            AcceptButton = btnLogin; ResumeLayout(true);
        }
    }
}
