using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace CapstoneProject
{
    public partial class LoginForm : Form
    {
        SqlConnection connection = new SqlConnection(@Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename=C:\Users\Admin-PC\source\repos\CapstoneProject\CapstoneProject\dbCapstone.mdf;Integrated Security = True");
        SqlCommand comm;
        SqlDataReader reader;

        public LoginForm()
        {
            InitializeComponent();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {

        }

        private void btnLogin_Click(object sender, EventArgs e) // Inputted Login details redirects appropriate form
        {
            connection.Open();
            comm = new SqlCommand("Select * From tbl_UserManagement Where login_user ='" + txtUsername.Text + "' And login_pass = '" + txtPassword.Text + "' And login_usertype = '" + cbDepartment.SelectedItem + "'", connection);
            SqlDataAdapter SDA = new SqlDataAdapter(comm);
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
                    this.Hide();
                }
                else if (cbDepartment.SelectedIndex == 2) // To Program Coordinator Form Disables BSIT PENDING BUTTON
                {
                    ProgramCoordinatorDirectoryForm nf = new ProgramCoordinatorDirectoryForm();
                    nf.Show();
                    nf.btnBSIT.Enabled = false;
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

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtUsername.Clear();
            txtPassword.Clear();
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
    }
}
