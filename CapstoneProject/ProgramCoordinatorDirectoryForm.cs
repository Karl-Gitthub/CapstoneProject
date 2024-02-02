using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace CapstoneProject
{
    public partial class ProgramCoordinatorDirectoryForm : Form
    {

        SqlConnection connection = new SqlConnection(@"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename=C:\Users\Stephen\source\repos\CapstoneProject\CapstoneProject\dbStudentManagement.mdf;Integrated Security = True");
        public ProgramCoordinatorDirectoryForm()
        {
            InitializeComponent();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }


        //Logout of User
        private void btnLogout_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Now Exiting", "Application Exit", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoginForm nf = new LoginForm();
            nf.Show();
            this.Hide();
        }

        private void BSBAProgramCoordinatorDirectoryForm_Load(object sender, EventArgs e)
        {

        }

        // Button to Open Form RegFormDirectory.cs and hide button that will redirect to other form
        private void button1_Click(object sender, EventArgs e)
        {

            RegFormDirectoryProgramcoodinator nf = new RegFormDirectoryProgramcoodinator();
            nf.Show();
            //nf.btnCancelBSBAProgramCoordinator.Hide();
            this.Hide();
        }

        // IF BSBA PENDING LIST is Clicked then data grid view of BSIT will be hidden.
        private void btnBSBA_Click(object sender, EventArgs e)
        {
            DGVBSIT.Hide();
            DGWBSBA.Show();
        }


        // IF BSIT PENDING LIST is Clicked then data grid view of BSBA will be hidden.
        private void btnBSIT_Click(object sender, EventArgs e)
        {
            DGWBSBA.Hide();
            DGVBSIT.Show();
        }

        // Hides exit button for registrar in order to redirect to Program Coordinator Form
        private void btnAdd_Click(object sender, EventArgs e)
        {
            Evaluationform nf = new Evaluationform();
            nf.btnExitRegistrar.Hide();
            nf.btnExitBSBAProgramCoordinator.Hide();
            nf.Show();
            this.Hide();
        }

        // Hides uneccessary button from misdirecting the user or sending the user to other form
        private void btn_bsbasubenlist_Click(object sender, EventArgs e)
        {
            RegFormDirectoryProgramcoodinator nf = new RegFormDirectoryProgramcoodinator();
            nf.Show();
            nf.btnCancelRegistrar.Hide();
            this.Hide();
        }

        private void btnBSBAEvaluation_Click(object sender, EventArgs e)
        {
            Evaluationform nf = new Evaluationform();
            nf.btnExitRegistrar.Hide();
            nf.btnExitBSITProgramCoordinator.Hide();
            nf.Show();
            this.Hide();
        }

        private void btnBSITGA_Click(object sender, EventArgs e)
        {
            AlphaListDirectoryForm nf = new AlphaListDirectoryForm();
            nf.Show();
            nf.btnBSBACancel.Hide();
            nf.btnRegistrarCancel.Hide();
            this.Hide();
        }

        private void btnBSBAGA_Click(object sender, EventArgs e)
        {
            AlphaListDirectoryForm nf = new AlphaListDirectoryForm();
            nf.Show();
            nf.btnRegistrarCancel.Hide();
            nf.btnBSITCancel.Hide();
            this.Hide();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            AddFacultyDetailandLoading nf = new AddFacultyDetailandLoading();
            nf.Show();
            this.Hide();
        }
    }
}
