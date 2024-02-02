using System;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Windows.Forms;

namespace CapstoneProject
{
    public partial class UserFormDirectory : Form
    {
        OleDbConnection connection = new OleDbConnection(@"Provider = Microsoft.ACE.OLEDB.12.0; Data Source = C:\Users\Karl\OneDrive\Documents\CapstoneProject\DB ACCESS FILES\dbUserManagement.accdb");
        OleDbCommand comm;
        OleDbDataReader reader;

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
            //open connection to submit to the db_usermanagement + show a messagebox to present that the account is created
            connection.Open();
            comm = new OleDbCommand("Select * From tbl_UserLogin Where login_user ='" + txtUsername.Text + "' And login_password = '" + txtPassword.Text + "' And login_usertype = '" + cbDepartment.SelectedItem + "' And login_fullname = '" + txtFacName.Text + "'", connection);
            OleDbDataAdapter SDA = new OleDbDataAdapter(comm);
            DataTable DT = new DataTable();

            SDA.Fill(DT);
            if (DT.Rows.Count > 0)
            {
                MessageBox.Show("Welcome Back User"); // Call or Retrieve the username // pless code this ahaha

                if (cbDepartment.SelectedIndex == 0) // To Registrar Form
                {
                    RegistrarDirectoryForm nf = new RegistrarDirectoryForm();
                    nf.Show();

                    this.Hide();
                }
                else if (cbDepartment.SelectedIndex == 1) // To Program Coordinator Form Disables BSBA PENDING BUTTON
                {
                    ProgramCoordinatorDirectoryForm nf = new ProgramCoordinatorDirectoryForm();
                    nf.Show();
                    nf.btnBSBA.Enabled = false;
                    nf.DGWBSBA.Hide();
                    nf.btn_bsbasubenlist.Hide();
                    nf.btnBSBAEvaluation.Hide();
                    this.Hide();
                }
                else if (cbDepartment.SelectedIndex == 2) // To Program Coordinator Form Disables BSIT PENDING BUTTON
                {
                    ProgramCoordinatorDirectoryForm nf = new ProgramCoordinatorDirectoryForm();
                    nf.Show();
                    nf.btnBSIT.Enabled = false;
                    nf.DGVBSIT.Hide();
                    nf.btn_bsitsubenlist.Hide();
                    nf.btnBSITEvaluation.Hide();
                    this.Hide();
                }
                else if (cbDepartment.SelectedIndex >= 3) // To Faculty Form
                {
                    FacultyDirectoryForm nf = new FacultyDirectoryForm();
                    nf.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Error I Think");
                }
            }
            connection.Close();

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            LoginForm nf = new LoginForm();
            nf.Show();
            this.Hide();
        }

        private void btnAdminButton_Click(object sender, EventArgs e)
        {

            if (txtAdmin.Text == "ADMIN" && txtAdminPass.Text == "ADMIN123")
            {
                // Changes the size and positioning the form to centerscreen
                Size = new Size(703, 954);
                Center(this);

                //Enables Registration Form
                txtUsername.Enabled = true;
                txtPassword.Enabled = true;
                txtFacName.Enabled = true;
                cbDepartment.Enabled = true;
                btnClear.Enabled = true;
                btnSubmit.Enabled = true;

                //Disables Admin Form
                txtAdmin.Enabled = false;
                txtAdminPass.Enabled = false;
                btnAdminButton.Enabled = false;
                btnCancel1.Enabled = false;


            }
            else
            {
                MessageBox.Show("Error, Try Again");
            }
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
