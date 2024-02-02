using System;
using System.Windows.Forms;

namespace CapstoneProject
{
    public partial class FacultyGradeFormDirectory : Form
    {
        public FacultyGradeFormDirectory()
        {
            InitializeComponent();
        }

        private void DGVRegGrading_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void FacultyGradeFormDirectory_Load(object sender, EventArgs e)
        {

        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Now Exiting", "Application Exit", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoginForm nf = new LoginForm();
            nf.Show();
            this.Hide();
        }

        private void txtCourseCode_TextChanged(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {

        }

        private void txtSY_TextChanged(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtProgram.Clear();
            txtSY.Clear();
            txtSem.Clear();
        }

        private void btnInputGrade_Click(object sender, EventArgs e)
        {
            FacultyDirectoryForm nf = new FacultyDirectoryForm();
            //LoginForm of = new LoginForm();
            //FacultyDirectoryForm.instance.lbl1.Text = of.txtUsername.Text;
            nf.Show();
            this.Hide();
        }

        private void btnCopyOfGrades_Click(object sender, EventArgs e)
        {

        }

        private void btnSearchFac_Click(object sender, EventArgs e)
        {

        }
    }
}
