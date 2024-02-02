using System;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;

namespace CapstoneProject
{
    public partial class RegistrarGradeFormDirectory : Form
    {
        int index;
        OleDbConnection connection = new OleDbConnection(@"Provider = Microsoft.ACE.OLEDB.12.0; Data Source = C:\Users\Karl\OneDrive\Documents\CapstoneProject\New folder\db_GradingManagement.accdb");
        OleDbCommand comm;
        OleDbDataReader reader;
        public RegistrarGradeFormDirectory()
        {
            InitializeComponent();
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

        private void btnAdd_Click(object sender, EventArgs e)
        {
            Evaluationform nf = new Evaluationform();
            nf.Show();
            nf.btnExitBSITProgramCoordinator.Hide();
            nf.btnExitBSBAProgramCoordinator.Hide();
            this.Hide();
        }

        private void btnStudPending_Click(object sender, EventArgs e)
        {
            RegistrarDirectoryForm nf = new RegistrarDirectoryForm();
            nf.Show();
            this.Hide();

        }

        private void btnUpdateGrades_Click(object sender, EventArgs e)
        {

        }

        private void btnGenTOR_Click(object sender, EventArgs e)
        {
            RegistrarTORDirectoryForm nf = new RegistrarTORDirectoryForm();
            nf.Show();
            this.Hide();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Now Exiting", "Application Exit", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoginForm nf = new LoginForm();
            nf.Show();
            this.Hide();
        }

        private void btnGenCGrades_Click(object sender, EventArgs e)
        {
            RegistrarGCGDirectoryForm nf = new RegistrarGCGDirectoryForm();
            nf.Show();
            this.Hide();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            //if course code and school year are correct then these txt boxes will be enabled
            //pls make sure to insert it in a if else clause
            connection.Open();
            comm = new OleDbCommand("Select course_code, school_year From tbl_Course Where course_code = '" + txtCourseCode.Text + "' And course_code = '" + txtCourseCode.Text + "' ", connection);
            if (txtCourseCode.Text == txtCourseCode.Text && txtSY.Text == txtSY.Text)
            {
                comm = new OleDbCommand("Select tbl_StudentDetail.stud_id, tbl_StudentDetail.stud_fname, tbl_StudentDetail.stud_mname, tbl_StudentDetail.stud_lname, tbl_Course.prelim, tbl_Course.midterm, tbl_Course.final, tbl_Course.average, tbl_Course.convert_average From tbl_Course Inner Join tbl_StudentDetail On  Left (tbl_Course.stud_id, 255) =  left (tbl_StudentDetail.stud_id, 255) Where course_code = '" + txtCourseCode.Text + "' And school_year = '" + txtSY.Text + "' And fac_id = '" + txtFacUser.Text + "'", connection);

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
                btnAdd.Enabled = false;
                btnAdd.Enabled = false;
                txtStudPrelimGrade.Enabled = false;
                txtStudMidtermGrade.Enabled = false;
                txtStudFinalsGrade.Enabled = false;
            }
            connection.Close();
        }

        private void btnAddGrade_Click(object sender, EventArgs e)
        {
            double prelim, midterm, final, ave, con_ave;
            txtStudNum.Enabled = false;
            //txtStName.Enabled = false;
            txtStudPrelimGrade.Enabled = true;
            txtStudMidtermGrade.Enabled = true;
            txtStudFinalsGrade.Enabled = true;
            txtStudAG.Enabled = true;
            txtStudCG.Enabled = true;
            //reactivate btn confirm
            btnConfirm.Enabled = true;


            //DataGridViewRow newdata = this.dataGridView1.Rows[index];
            // newdata.Cells[4].Value = txtStudPrelimGrade.Text;


            connection.Open();
            index = dataGridView1.SelectedRows[0].Index;
            if (index >= 0)
            {
                foreach (DataGridViewRow newdata in dataGridView1.SelectedRows)
                {

                    newdata.Cells[4].Value = txtStudPrelimGrade.Text;
                    comm = new OleDbCommand("Update tbl_Course Set prelim = '" + txtStudPrelimGrade.Text + "' Where stud_id = '" + txtStudNum.Text + "' And course_code = '" + txtCourseCode.Text + "'", connection);
                    comm.Parameters.AddWithValue("@prelim", txtStudPrelimGrade.Text);
                    comm.ExecuteNonQuery();
                    comm.Parameters.Clear();
                }

            }

            connection.Close();

            connection.Open();
            index = dataGridView1.SelectedRows[0].Index;
            if (index >= 0)
            {
                foreach (DataGridViewRow newdata in dataGridView1.SelectedRows)
                {

                    newdata.Cells[5].Value = txtStudMidtermGrade.Text;
                    comm = new OleDbCommand("Update tbl_Course Set midterm = '" + txtStudMidtermGrade.Text + "' Where stud_id = '" + txtStudNum.Text + "' And course_code = '" + txtCourseCode.Text + "'", connection);
                    comm.Parameters.AddWithValue("@midterm", txtStudMidtermGrade.Text);
                    comm.ExecuteNonQuery();
                    comm.Parameters.Clear();
                }

            }
            connection.Close();

            connection.Open();
            index = dataGridView1.SelectedRows[0].Index;
            if (index >= 0)
            {
                foreach (DataGridViewRow newdata in dataGridView1.SelectedRows)
                {

                    newdata.Cells[6].Value = txtStudFinalsGrade.Text;
                    comm = new OleDbCommand("Update tbl_Course Set final = '" + txtStudFinalsGrade.Text + "' Where stud_id = '" + txtStudNum.Text + "' And course_code = '" + txtCourseCode.Text + "'", connection);
                    comm.Parameters.AddWithValue("@final", txtStudFinalsGrade.Text);
                    comm.ExecuteNonQuery();
                    comm.Parameters.Clear();
                }

            }
            connection.Close();


            /* connection.Open();

             prelim = int.Parse(txtStudPrelimGrade.Text);
             midterm = int.Parse(txtStudMidtermGrade.Text);
             final = int.Parse(txtStudFinalsGrade.Text);
             //ave = int.Parse(txtStudAG.Text);
             //con_ave = int.Parse(txtStudCG.Text);

             ave = prelim + midterm + final;

             con_ave = ave / 3;

             connection.Close();*/
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            txtStudNum.Enabled = false;
            //txtStudName.Enabled = false;
            txtStudPrelimGrade.Enabled = false;
            txtStudMidtermGrade.Enabled = true;
            txtStudFinalsGrade.Enabled = true;
            txtStudAG.Enabled = true;
            txtStudCG.Enabled = true;
            //reactivate btn confirm
            btnConfirm.Enabled = true;
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            btnConfirm.Enabled = false;
        }

        private void btnGenAList_Click(object sender, EventArgs e)
        {
            AlphaListDirectoryForm nf = new AlphaListDirectoryForm();
            nf.Show();
            nf.btnBSBACancel.Hide();
            nf.btnBSITCancel.Hide();
            this.Hide();
        }

        private void txtFacUser_TextChanged(object sender, EventArgs e)
        {
            connection.Open();
            comm = new OleDbCommand("Select fac_fname, fac_mname, fac_lname From tbl_FacultyDetail Where fac_id = '" + txtFacUser.Text + "'", connection);
            comm.Parameters.AddWithValue("fac_id", txtFacUser.Text);
            reader = comm.ExecuteReader();
            while (reader.Read())
            {
                lblfname.Text = reader.GetValue(0).ToString();
                lblmname.Text = reader.GetValue(1).ToString();
                lblname.Text = reader.GetValue(2).ToString();
            }
            connection.Close();


        }

        private void RegistrarGradeFormDirectory_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'db_GradingManagementDataSet1.tbl_FacultyDetail' table. You can move, or remove it, as needed.
            this.tbl_FacultyDetailTableAdapter.Fill(this.db_GradingManagementDataSet1.tbl_FacultyDetail);
            // TODO: This line of code loads data into the 'db_GradingManagementDataSet.tbl_Course' table. You can move, or remove it, as needed.
            this.tbl_CourseTableAdapter.Fill(this.db_GradingManagementDataSet.tbl_Course);

        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            index = e.RowIndex;
            if (index >= 0)
            {
                DataGridViewRow row = this.dataGridView1.Rows[index];

                txtStudNum.Text = row.Cells[0].Value.ToString();
                txtFName.Text = row.Cells[1].Value.ToString();
                txtMName.Text = row.Cells[2].Value.ToString();
                txtLName.Text = row.Cells[3].Value.ToString();
                txtStudPrelimGrade.Text = row.Cells[4].Value.ToString();
                txtStudMidtermGrade.Text = row.Cells[5].Value.ToString();
                txtStudFinalsGrade.Text = row.Cells[6].Value.ToString();
                txtStudAG.Text = row.Cells[7].Value.ToString();
                txtStudCG.Text = row.Cells[8].Value.ToString();
            }

        }

        private void txtStudAG_TextChanged(object sender, EventArgs e)
        {

        }

        private void btncalcu_Click(object sender, EventArgs e)
        {
            int prelim, midterm, final;

            prelim = int.Parse(txtStudPrelimGrade.Text);
            midterm = int.Parse(txtStudMidtermGrade.Text);
            final = int.Parse(txtStudFinalsGrade.Text);

            int ave;
            double con_ave;



            if (prelim > 0 && midterm > 0 && final > 0)
            {
                ave = (prelim + midterm + final) / 3;
                txtStudAG.Text = ave.ToString();

                //GRADES 96 - 100
                if (ave >= 96 && ave <= 100)
                {
                    con_ave = 1.00;
                    txtStudCG.Text = con_ave.ToString();
                    connection.Open();
                    index = dataGridView1.SelectedRows[0].Index;
                    if (index >= 0)
                    {
                        foreach (DataGridViewRow newdata in dataGridView1.SelectedRows)
                        {

                            newdata.Cells[7].Value = txtStudAG.Text.ToString();
                            newdata.Cells[8].Value = txtStudCG.Text.ToString();

                            comm = new OleDbCommand("Update tbl_Course Set average = '" + txtStudAG.Text + "', convert_average = '" + txtStudCG.Text + "' Where stud_id = '" + txtStudNum.Text + "' And course_code = '" + txtCourseCode.Text + "'", connection);
                            comm.Parameters.AddWithValue("@average", txtStudAG.Text.ToString());
                            comm.Parameters.AddWithValue("@convert_average", txtStudCG.Text.ToString());
                            comm.ExecuteNonQuery();
                            comm.Parameters.Clear();
                        }

                    }

                    connection.Close();
                }
                //GRADES 94 - 95
                else if (ave >= 94 && ave <= 95)
                {
                    con_ave = 1.25;
                    txtStudCG.Text = con_ave.ToString();
                    connection.Open();
                    index = dataGridView1.SelectedRows[0].Index;
                    if (index >= 0)
                    {
                        foreach (DataGridViewRow newdata in dataGridView1.SelectedRows)
                        {

                            newdata.Cells[7].Value = txtStudAG.Text.ToString();
                            newdata.Cells[8].Value = txtStudCG.Text.ToString();

                            comm = new OleDbCommand("Update tbl_Course Set average = '" + txtStudAG.Text + "', convert_average = '" + txtStudCG.Text + "' Where stud_id = '" + txtStudNum.Text + "' And course_code = '" + txtCourseCode.Text + "'", connection);
                            comm.Parameters.AddWithValue("@average", txtStudAG.Text.ToString());
                            comm.Parameters.AddWithValue("@convert_average", txtStudCG.Text.ToString());
                            comm.ExecuteNonQuery();
                            comm.Parameters.Clear();
                        }

                    }

                    connection.Close();
                }
                //GRADES 92 - 23
                else if (ave >= 92 && ave <= 93)
                {
                    con_ave = 1.50;
                    txtStudCG.Text = con_ave.ToString();
                    connection.Open();
                    index = dataGridView1.SelectedRows[0].Index;
                    if (index >= 0)
                    {
                        foreach (DataGridViewRow newdata in dataGridView1.SelectedRows)
                        {

                            newdata.Cells[7].Value = txtStudAG.Text.ToString();
                            newdata.Cells[8].Value = txtStudCG.Text.ToString();

                            comm = new OleDbCommand("Update tbl_Course Set average = '" + txtStudAG.Text + "', convert_average = '" + txtStudCG.Text + "' Where stud_id = '" + txtStudNum.Text + "' And course_code = '" + txtCourseCode.Text + "'", connection);
                            comm.Parameters.AddWithValue("@average", txtStudAG.Text.ToString());
                            comm.Parameters.AddWithValue("@convert_average", txtStudCG.Text.ToString());
                            comm.ExecuteNonQuery();
                            comm.Parameters.Clear();
                        }

                    }

                    connection.Close();
                }
                //GRADES 89 - 91
                else if (ave >= 89 && ave <= 91)
                {
                    con_ave = 1.75;
                    txtStudCG.Text = con_ave.ToString();
                    connection.Open();
                    index = dataGridView1.SelectedRows[0].Index;
                    if (index >= 0)
                    {
                        foreach (DataGridViewRow newdata in dataGridView1.SelectedRows)
                        {

                            newdata.Cells[7].Value = txtStudAG.Text.ToString();
                            newdata.Cells[8].Value = txtStudCG.Text.ToString();

                            comm = new OleDbCommand("Update tbl_Course Set average = '" + txtStudAG.Text + "', convert_average = '" + txtStudCG.Text + "' Where stud_id = '" + txtStudNum.Text + "' And course_code = '" + txtCourseCode.Text + "'", connection);
                            comm.Parameters.AddWithValue("@average", txtStudAG.Text.ToString());
                            comm.Parameters.AddWithValue("@convert_average", txtStudCG.Text.ToString());
                            comm.ExecuteNonQuery();
                            comm.Parameters.Clear();
                        }

                    }

                    connection.Close();
                }
                //GRADES 87 - 88
                else if (ave >= 87 && ave <= 88)
                {
                    con_ave = 2.00;
                    txtStudCG.Text = con_ave.ToString();
                    connection.Open();
                    index = dataGridView1.SelectedRows[0].Index;
                    if (index >= 0)
                    {
                        foreach (DataGridViewRow newdata in dataGridView1.SelectedRows)
                        {

                            newdata.Cells[7].Value = txtStudAG.Text.ToString();
                            newdata.Cells[8].Value = txtStudCG.Text.ToString();

                            comm = new OleDbCommand("Update tbl_Course Set average = '" + txtStudAG.Text + "', convert_average = '" + txtStudCG.Text + "' Where stud_id = '" + txtStudNum.Text + "' And course_code = '" + txtCourseCode.Text + "'", connection);
                            comm.Parameters.AddWithValue("@average", txtStudAG.Text.ToString());
                            comm.Parameters.AddWithValue("@convert_average", txtStudCG.Text.ToString());
                            comm.ExecuteNonQuery();
                            comm.Parameters.Clear();
                        }

                    }

                    connection.Close();
                }
                //GRADES 84 - 86
                else if (ave >= 84 && ave <= 86)
                {
                    con_ave = 2.25;
                    txtStudCG.Text = con_ave.ToString();
                    connection.Open();
                    index = dataGridView1.SelectedRows[0].Index;
                    if (index >= 0)
                    {
                        foreach (DataGridViewRow newdata in dataGridView1.SelectedRows)
                        {

                            newdata.Cells[7].Value = txtStudAG.Text.ToString();
                            newdata.Cells[8].Value = txtStudCG.Text.ToString();

                            comm = new OleDbCommand("Update tbl_Course Set average = '" + txtStudAG.Text + "', convert_average = '" + txtStudCG.Text + "' Where stud_id = '" + txtStudNum.Text + "' And course_code = '" + txtCourseCode.Text + "'", connection);
                            comm.Parameters.AddWithValue("@average", txtStudAG.Text.ToString());
                            comm.Parameters.AddWithValue("@convert_average", txtStudCG.Text.ToString());
                            comm.ExecuteNonQuery();
                            comm.Parameters.Clear();
                        }

                    }

                    connection.Close();
                }
                //GRADES 80 - 83
                else if (ave >= 80 && ave <= 83)
                {
                    con_ave = 2.50;
                    txtStudCG.Text = con_ave.ToString();
                    connection.Open();
                    index = dataGridView1.SelectedRows[0].Index;
                    if (index >= 0)
                    {
                        foreach (DataGridViewRow newdata in dataGridView1.SelectedRows)
                        {

                            newdata.Cells[7].Value = txtStudAG.Text.ToString();
                            newdata.Cells[8].Value = txtStudCG.Text.ToString();

                            comm = new OleDbCommand("Update tbl_Course Set average = '" + txtStudAG.Text + "', convert_average = '" + txtStudCG.Text + "' Where stud_id = '" + txtStudNum.Text + "' And course_code = '" + txtCourseCode.Text + "'", connection);
                            comm.Parameters.AddWithValue("@average", txtStudAG.Text.ToString());
                            comm.Parameters.AddWithValue("@convert_average", txtStudCG.Text.ToString());
                            comm.ExecuteNonQuery();
                            comm.Parameters.Clear();
                        }

                    }

                    connection.Close();
                }
                //GRADES 78 - 79
                else if (ave >= 78 && ave <= 79)
                {
                    con_ave = 2.75;
                    txtStudCG.Text = con_ave.ToString();
                    connection.Open();
                    index = dataGridView1.SelectedRows[0].Index;
                    if (index >= 0)
                    {
                        foreach (DataGridViewRow newdata in dataGridView1.SelectedRows)
                        {

                            newdata.Cells[7].Value = txtStudAG.Text.ToString();
                            newdata.Cells[8].Value = txtStudCG.Text.ToString();

                            comm = new OleDbCommand("Update tbl_Course Set average = '" + txtStudAG.Text + "', convert_average = '" + txtStudCG.Text + "' Where stud_id = '" + txtStudNum.Text + "' And course_code = '" + txtCourseCode.Text + "'", connection);
                            comm.Parameters.AddWithValue("@average", txtStudAG.Text.ToString());
                            comm.Parameters.AddWithValue("@convert_average", txtStudCG.Text.ToString());
                            comm.ExecuteNonQuery();
                            comm.Parameters.Clear();
                        }

                    }

                    connection.Close();
                }
                //GRADES 75 - 77
                else if (ave >= 75 && ave <= 77)
                {
                    con_ave = double.Parse(txtStudCG.Text);
                    con_ave = 3.00;
                    txtStudCG.Text = con_ave.ToString();
                    connection.Open();
                    index = dataGridView1.SelectedRows[0].Index;
                    if (index >= 0)
                    {
                        foreach (DataGridViewRow newdata in dataGridView1.SelectedRows)
                        {

                            newdata.Cells[7].Value = txtStudAG.Text.ToString();
                            newdata.Cells[8].Value = txtStudCG.Text.ToString();

                            comm = new OleDbCommand("Update tbl_Course Set average = '" + txtStudAG.Text + "', convert_average = '" + txtStudCG.Text + "' Where stud_id = '" + txtStudNum.Text + "' And course_code = '" + txtCourseCode.Text + "'", connection);
                            comm.Parameters.AddWithValue("@average", txtStudAG.Text.ToString());
                            comm.Parameters.AddWithValue("@convert_average", txtStudCG.Text.ToString());
                            comm.ExecuteNonQuery();
                            comm.Parameters.Clear();
                        }

                    }
                    connection.Close();
                }
                //GRADES FAILED
                else if (ave > 74)
                {
                    con_ave = 5.00;
                    txtStudCG.Text = con_ave.ToString();
                    connection.Open();
                    index = dataGridView1.SelectedRows[0].Index;
                    if (index >= 0)
                    {
                        foreach (DataGridViewRow newdata in dataGridView1.SelectedRows)
                        {

                            newdata.Cells[7].Value = txtStudAG.Text.ToString();
                            newdata.Cells[8].Value = txtStudCG.Text.ToString();

                            comm = new OleDbCommand("Update tbl_Course Set average = '" + txtStudAG.Text + "', convert_average = '" + txtStudCG.Text + "' Where stud_id = '" + txtStudNum.Text + "' And course_code = '" + txtCourseCode.Text + "'", connection);
                            comm.Parameters.AddWithValue("@average", txtStudAG.Text.ToString());
                            comm.Parameters.AddWithValue("@convert_average", txtStudCG.Text.ToString());
                            comm.ExecuteNonQuery();
                            comm.Parameters.Clear();
                        }

                    }

                    connection.Close();
                }
            }
        }

        private void txtStudCG_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
