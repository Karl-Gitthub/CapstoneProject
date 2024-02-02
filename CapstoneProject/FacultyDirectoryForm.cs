using System;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;

namespace CapstoneProject
{
    public partial class FacultyDirectoryForm : Form
    {
        OleDbConnection connection = new OleDbConnection(@"Provider = Microsoft.ACE.OLEDB.12.0; Data Source = C:\Users\Karl\OneDrive\Documents\CapstoneProject\New folder\dbUserManagement.accdb");
        OleDbConnection conn = new OleDbConnection(@"Provider = Microsoft.ACE.OLEDB.12.0; Data Source = C:\Users\Karl\OneDrive\Documents\CapstoneProject\New folder\db_GradingManagement.accdb");
        OleDbCommand comm;
        OleDbDataReader reader;
        public static FacultyDirectoryForm instance;
        public Label lbl1;
        public Label lbl2;

        public FacultyDirectoryForm()
        {
            InitializeComponent();
            instance = this;
            lbl1 = lblFacUser;
            lbl2 = lblFacName;
        }

        public void FacultyDirectoryForm_Load(object sender, EventArgs e)
        {
            ccgpanel.Visible = false;
            lbl1 = lblFacUser;
            string FacUser = lbl1.ToString();
            conn.Open();
            comm = new OleDbCommand("Select fac_fname, fac_mname, fac_lname From tbl_FacultyDetail Where fac_id = '" + lbl1.Text + "'", conn);
            comm.Parameters.AddWithValue("fac_id", lbl1.Text);
            reader = comm.ExecuteReader();
            while (reader.Read())
            {

                lblFacName.Text = reader.GetValue(0).ToString();
                lbmname.Text = reader.GetValue(1).ToString();
                lblLName.Text = reader.GetValue(2).ToString();
            }
            conn.Close();
        }

        private void txtfacid_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Now Exiting", "Application Exit", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoginForm nf = new LoginForm();
            nf.Show();
            this.Hide();
        }

        private void btnCopyOfGrades_Click(object sender, EventArgs e)
        {
            BackColor.ToArgb();
            ccgpanel.Visible = true;
            // FacultyGradeFormDirectory nf = new FacultyGradeFormDirectory();
            // nf.Show();
            // this.Hide();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            conn.Open();
            comm = new OleDbCommand("Select course_code, school_year From tbl_Course Where course_code = '" + txtCourseCode.Text + "' And course_code = '" + txtCourseCode.Text + "' ", conn);
            if (txtCourseCode.Text == txtCourseCode.Text && txtSY.Text == txtSY.Text)
            {
                comm = new OleDbCommand("Select tbl_StudentDetail.stud_id, tbl_StudentDetail.stud_fname, tbl_StudentDetail.stud_mname, tbl_StudentDetail.stud_lname, tbl_Course.prelim, tbl_Course.midterm, tbl_Course.final, tbl_Course.average, tbl_Course.convert_average From tbl_Course Inner Join tbl_StudentDetail On  Left (tbl_Course.stud_id, 255) =  left (tbl_StudentDetail.stud_id, 255) Where course_code = '" + txtCourseCode.Text + "' And school_year = '" + txtSY.Text + "' And fac_id = '" + lbl1.Text + "'", conn);

                OleDbDataAdapter sda = new OleDbDataAdapter(comm);
                DataTable dt = new DataTable();

                sda.Fill(dt);
                dataGridView1.DataSource = dt;

                DataGridViewRow selectedRow = new DataGridViewRow();
                //dataGridView1.Rows.Insert(0, selectedRow);  
                txtStudPrelimGrade.Enabled = true;
                txtStudMidtermGrade.Enabled = true;
                txtStudFinalsGrade.Enabled = true;
            }
            else
            {
                txtStudAG.Enabled = false;
                txtStudCG.Enabled = false;
                txtStudPrelimGrade.Enabled = false;
                txtStudMidtermGrade.Enabled = false;
                txtStudFinalsGrade.Enabled = false;
            }
            conn.Close();
        }

        public void lblFacUser_Click(object sender, EventArgs e)
        {

            lblFacUser.Text = lbl1.Text;

        }

        public void lblFacName_Click(object sender, EventArgs e)
        {

        }

        private void btncalcu_Click(object sender, EventArgs e)
        {

        }

        private void btnInputGrade_Click(object sender, EventArgs e)
        {
            ccgpanel.Visible = false;
        }

        private void btnSearchFac_Click(object sender, EventArgs e)
        {

        }

        private void button11_Click(object sender, EventArgs e)
        {

        }

        private void btnAddGrade_Click(object sender, EventArgs e)
        {

        }
    }
}
