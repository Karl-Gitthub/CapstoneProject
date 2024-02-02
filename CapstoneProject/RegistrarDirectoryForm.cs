using System;
using System.Windows.Forms;

namespace CapstoneProject
{
    public partial class RegistrarDirectoryForm : Form
    {
        public RegistrarDirectoryForm()
        {
            InitializeComponent();
        }

        private void RegistrarDirectoryForm_Load(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        // Hides Registrar Exit Button in order to redirect to RegistrarDirectoryForm
        private void btnAdd_Click(object sender, EventArgs e)
        {
            Evaluationform nf = new Evaluationform();
            nf.Show();
            nf.btnExitBSITProgramCoordinator.Hide();
            nf.btnExitBSBAProgramCoordinator.Hide();
            this.Hide();
        }

        // exits Application
        private void btnLogout_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Now Exiting", "Application Exit", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoginForm nf = new LoginForm();
            nf.Show();
            this.Hide();
        }

        // Hides button for Program Coordinator in order to redirect to registrar if goes back or cancels process
        private void button6_Click(object sender, EventArgs e)
        {
            RegFormDirectoryRegistrar nf = new RegFormDirectoryRegistrar();
            nf.Show();
            //nf.btnSubmit.Enabled = false;//
            this.Hide();

        }

        // Disables the Update button for Registrar and redirects to StudentinformationSheet Form
        private void button8_Click(object sender, EventArgs e)
        {
            StudentInformationSheet nf = new StudentInformationSheet();
            nf.Show();
            nf.btnUpdate.Enabled = false;
            this.Hide();
        }

        private void btnBSIT_Click(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {
            StudentInformationSheet nf = new StudentInformationSheet();
            nf.Show();
            this.Hide();
        }

        private void btnBSBA_Click(object sender, EventArgs e)
        {
            RegistrarGradeFormDirectory nf = new RegistrarGradeFormDirectory();
            nf.Show();
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RegistrarTORDirectoryForm nf = new RegistrarTORDirectoryForm();
            nf.Show();
            this.Hide();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            RegistrarGCGDirectoryForm nf = new RegistrarGCGDirectoryForm();
            nf.Show();
            this.Hide();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            AlphaListDirectoryForm nf = new AlphaListDirectoryForm();
            nf.Show();
            nf.btnBSBACancel.Hide();
            nf.btnBSITCancel.Hide();
            this.Hide();

        }

        private void button5_Click(object sender, EventArgs e)
        {

        }
    }
}
