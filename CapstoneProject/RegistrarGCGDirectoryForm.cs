using System;
using System.Windows.Forms;

namespace CapstoneProject
{
    public partial class RegistrarGCGDirectoryForm : Form
    {
        public RegistrarGCGDirectoryForm()
        {
            InitializeComponent();
        }

        private void btnStudPending_Click(object sender, EventArgs e)
        {
            RegistrarDirectoryForm nf = new RegistrarDirectoryForm();
            nf.Show();
            this.Hide();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            Evaluationform nf = new Evaluationform();
            nf.Show();
            nf.btnExitBSITProgramCoordinator.Hide();
            nf.btnExitBSBAProgramCoordinator.Hide();
            this.Hide();
        }

        private void btnUpdateGrades_Click(object sender, EventArgs e)
        {
            RegistrarGradeFormDirectory nf = new RegistrarGradeFormDirectory();
            nf.Show();
            this.Hide();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            RegFormDirectoryRegistrar nf = new RegFormDirectoryRegistrar();
            nf.Show();
            //nf.btnSubmit.Enabled = false;//
            this.Hide();
        }

        private void btnAddStudent_Click(object sender, EventArgs e)
        {
            StudentInformationSheet nf = new StudentInformationSheet();
            nf.Show();
            this.Hide();
        }

        private void btnGenTOR_Click(object sender, EventArgs e)
        {
            RegistrarTORDirectoryForm nf = new RegistrarTORDirectoryForm();
            nf.Show();
            this.Hide();
        }

        private void btnGenCGrades_Click(object sender, EventArgs e)
        {
            RegistrarGCGDirectoryForm nf = new RegistrarGCGDirectoryForm();
            nf.Show();
            this.Hide();
        }

        private void btnGenAList_Click(object sender, EventArgs e)
        {
            AlphaListDirectoryForm nf = new AlphaListDirectoryForm();
            nf.Show();
            nf.btnBSBACancel.Hide();
            nf.btnBSITCancel.Hide();
            this.Hide();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {

        }

        private void button10_Click(object sender, EventArgs e)
        {

        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Now Exiting", "Application Exit", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoginForm nf = new LoginForm();
            nf.Show();
            this.Hide();
        }
    }
}
