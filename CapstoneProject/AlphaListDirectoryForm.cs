using System;
using System.Windows.Forms;

namespace CapstoneProject
{
    public partial class AlphaListDirectoryForm : Form
    {
        public AlphaListDirectoryForm()
        {
            InitializeComponent();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtSY.Clear();
            txtSem.Clear();

        }

        private void btnRegistrarCancel_Click(object sender, EventArgs e)
        {
            RegistrarDirectoryForm nf = new RegistrarDirectoryForm();
            nf.Show();
            this.Hide();
        }

        private void btnBSITCancel_Click(object sender, EventArgs e)
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

        private void btnBSBACancel_Click(object sender, EventArgs e)
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

        private void DGWBSBA_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
