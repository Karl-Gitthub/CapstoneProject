using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;



namespace CapstoneProject
{
    public partial class Evaluationform : Form
    {
        public Evaluationform()
        {
            InitializeComponent();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //select of BSIT or BSBA textboxes will be hidden to restrict some irrelevant course codes
            if (cbProgram.Text == "BSIT")
            {
                // hide or show BSIT text boxes
            }
            else if (cbProgram.Text == "BSBA")
            {
                // hide or show BSBA text boxes
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ProgramCoordinatorDirectoryForm nf = new ProgramCoordinatorDirectoryForm();
            nf.Show();
            nf.btnBSBA.Enabled = false;
            nf.DGWBSBA.Hide();
            nf.btn_bsbasubenlist.Hide();
            nf.btnBSBAEvaluation.Hide();
            this.Hide();
        }

        private void btnExitRegistrar_Click(object sender, EventArgs e)
        {
            RegistrarDirectoryForm nf = new RegistrarDirectoryForm();
            nf.Show();
            this.Hide();
        }

        private void btnExitBSBAProgramCoordinator_Click(object sender, EventArgs e)
        {
            ProgramCoordinatorDirectoryForm nf = new ProgramCoordinatorDirectoryForm();
            nf.Show();
            nf.btnBSIT.Enabled = false;
            nf.DGVBSIT.Hide();
            nf.btn_bsitsubenlist.Hide();
            nf.btnBSITEvaluation.Hide();
            this.Hide();
        }

        private void Evaluationform_Load(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Label_Vertical(object sender, PaintEventArgs e)
        {
            Font myfont = new Font("Arial", 12);
            Brush myBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
            e.Graphics.TranslateTransform(30, 20);
            e.Graphics.RotateTransform(90);
            e.Graphics.DrawString("_______________________________________________________________________________________________________________", myfont, myBrush, 0, 0);
        }

        private void btnClearText_Click(object sender, EventArgs e)
        {
            //
            txtStudID.Clear();
            txtFName.Clear();
            txtLName.Clear();
            txtMName.Clear();
            cbProgram.SelectedIndex = -1;

            //
            txt1s1sc1.Clear();
            txt1st1sC2.Clear();
            txt1st1sC3.Clear();
            txt1st1sC4.Clear();
            txt1st1sC5.Clear();
            txt1st1sC6.Clear();
            txt1st1sC7.Clear();
            txt1st1sC8.Clear();
            txt1st1sSD1.Clear();
            txt1st1sSD2.Clear();
            txt1st1sSD3.Clear();
            txt1st1sSD4.Clear();
            txt1st1sSD5.Clear();
            txt1st1sSD6.Clear();
            txt1st1sSD7.Clear();
            txt1st1sSD8.Clear();
            //
            txt1st2ndC1.Clear();
            txt1st2ndC2.Clear();
            txt1st2ndC3.Clear();
            txt1st2ndC4.Clear();
            txt1st2ndC5.Clear();
            txt1st2ndC6.Clear();
            txt1st2ndC7.Clear();
            txt1st2ndC8.Clear();
            txt1st2ndSD1.Clear();
            txt1st2ndSD2.Clear();
            txt1st2ndSD3.Clear();
            txt1st2ndSD4.Clear();
            txt1st2ndSD5.Clear();
            txt1st2ndSD6.Clear();
            txt1st2ndSD7.Clear();
            txt1st2ndSD8.Clear();


        }


        private void btnPrint_Click(object sender, EventArgs e)
        {
            PrintDialog p1 = new PrintDialog();
            PrintDocument pd = new PrintDocument();

            pd.DocumentName = "Print Document";
            p1.Document = pd;
            p1.AllowSelection = true;
            p1.AllowSomePages = true;

            if (p1.ShowDialog() == DialogResult.OK)
            {
                pd.Print();
            }
        }

        private void printDocument1_PrintPage(object sender, PrintPageEventArgs e)
        {

        }
    }
}
