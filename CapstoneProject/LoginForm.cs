using System;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;

namespace CapstoneProject
{
    public partial class LoginForm : Form
    {
        public static LoginForm instance;



        OleDbConnection connection = new OleDbConnection(@"Provider = Microsoft.ACE.OLEDB.12.0; Data Source = C:\Users\Karl\OneDrive\Documents\CapstoneProject\New folder\dbUserManagement.accdb");
        OleDbConnection conn = new OleDbConnection(@"Provider = Microsoft.ACE.OLEDB.12.0; Data Source = C:\Users\Karl\Downloads\caps1.1\CapstoneProject\CapstoneProject\db_GradingManagement.accdb");
        OleDbCommand comm;
        OleDbDataReader reader;

        public LoginForm()
        {
            InitializeComponent();
            instance = this;
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            timer1.Start();
            txtPassword.UseSystemPasswordChar = true;
        }


        // Based on what the user selects in the combobox, it will redirects to the appropriate form but user and pass must be met by the user to be verified
        private void btnLogin_Click(object sender, EventArgs e) // Inputted Login details redirects appropriate form
        {
            connection.Open();
            comm = new OleDbCommand("Select * From tbl_UserLogin Where login_user ='" + txtUsername.Text + "' And login_password = '" + txtPassword.Text + "' And login_usertype = '" + cbDepartment.SelectedItem + "'", connection);
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
                    nf.btnBSBAGA.Hide();
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
                    nf.btnBSITGA.Hide();
                    this.Hide();
                }
                else if (cbDepartment.SelectedIndex >= 3) // To Faculty Form
                {

                    FacultyDirectoryForm nf = new FacultyDirectoryForm();
                    FacultyDirectoryForm.instance.lbl1.Text = txtUsername.Text;

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

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtUsername.Clear();
            txtPassword.Clear();
            cbDepartment.SelectedIndex = -1;
            cbDepartment.Focus();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Now Exiting", "Application Exit", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Application.Exit();
        }

        private void cbDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txtUsername_TextChanged(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (pb1.Visible == true)
            {
                pb1.Visible = false;
                pb2.Visible = true;
            }
            else if (pb2.Visible == true)
            {
                pb2.Visible = false;
                pb3.Visible = true;
            }
            else if (pb3.Visible == true)
            {
                pb3.Visible = false;
                pb1.Visible = true;
            }


        }

        private void button1_Click(object sender, EventArgs e)
        {
            UserFormDirectory nf = new UserFormDirectory();
            nf.Show();
            this.Hide();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void chbShowPass_CheckedChanged(object sender, EventArgs e)
        {
            if (chbShowPass.Checked)
            {
                txtPassword.UseSystemPasswordChar = false;
            }
            else
            {
                txtPassword.UseSystemPasswordChar = true;
            }
        }

        private void label42_Click(object sender, EventArgs e)
        {

        }

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
