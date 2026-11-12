using System;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Windows.Forms;

namespace CapstoneProject
{
    public partial class UserFormDirectory : Form
    {
        public UserFormDirectory()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void Center(Form form) // function to call in the btnAdminButton
        {
            form.Location = new Point(
            (Screen.PrimaryScreen.Bounds.Size.Width / 2) - (form.Size.Width / 2),
            (Screen.PrimaryScreen.Bounds.Size.Height / 2) - (form.Size.Height / 2));
        }

        private void txtInstructorName_Click(object sender, EventArgs e)
        {

        }

        private void UserFormDirectory_Load(object sender, EventArgs e) //defaults the stringtype to passwordchar
        {
            txtPassword.UseSystemPasswordChar = true;
        }

        private void chbShowPass_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtUsername.Clear();
            txtPassword.Clear();
            txtFacName.Clear();
            cbDepartment.SelectedIndex = -1;
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this,"Account provisioning is unavailable in this local version.","Account access");
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            LoginForm nf = new LoginForm();
            nf.Show();
            this.Hide();
        }

        private void btnAdminButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this,"Account provisioning must be configured by your administrator. Desktop administrator access has been removed.","Account access",MessageBoxButtons.OK,MessageBoxIcon.Information);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            LoginForm nf = new LoginForm();
            nf.Show();
            this.Hide();
        }

        private void cbDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txtUsername_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
