using System;
using System.Data.OleDb;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace CapstoneProject
{
    public partial class RegFormDirectoryRegistrar : Form
    {
        OleDbConnection connection = new OleDbConnection(@"Provider = Microsoft.ACE.OLEDB.12.0; Data Source = C:\Users\Karl\OneDrive\Documents\CapstoneProject\New folder\dbCourseManagement.accdb");
        OleDbConnection conn = new OleDbConnection(@"Provider = Microsoft.ACE.OLEDB.12.0; Data Source = C:\Users\Karl\OneDrive\Documents\CapstoneProject\New folder\dbStudentManagement.accdb");
        OleDbConnection con = new OleDbConnection(@"Provider = Microsoft.ACE.OLEDB.12.0; Data Source = C:\Users\Karl\OneDrive\Documents\CapstoneProject\New folder\db_Temp_RegForm.accdb");
        OleDbConnection gr = new OleDbConnection(@"Provider = Microsoft.ACE.OLEDB.12.0; Data Source = C:\Users\Karl\OneDrive\Documents\CapstoneProject\New folder\db_GradingManagement.accdb");
        OleDbCommand comm;
        OleDbDataReader reader;
        public RegFormDirectoryRegistrar()
        {
            InitializeComponent();
        }

        private void Submit_Click(object sender, EventArgs e)
        {
            RegistrarDirectoryForm nf = new RegistrarDirectoryForm();
            nf.Show();
            this.Hide();

            // Db code here
            try
            {
                gr.Open();
                if (txtsubcode1.Text == txtsubcode1.Text && txtStudID.Text == txtStudID.Text)
                {
                    comm = new OleDbCommand("Insert into tbl_Course(course_code, yearlevel, school_year, sem, stud_id) values(@course_code, @yearlevel, @school_year, @sem, @stud_id)", gr);
                    comm.Parameters.AddWithValue("course_code", txtsubcode1.Text);
                    comm.Parameters.AddWithValue("yearlevel", txtYL.Text);
                    comm.Parameters.AddWithValue("school_year", txtSY.Text);
                    comm.Parameters.AddWithValue("sem", txtSemester.Text);
                    comm.Parameters.AddWithValue("stud_id", txtStudID.Text);
                    comm.ExecuteNonQuery();
                }
                else if (txtsubcode1.Text != txtsubcode1.Text)
                {
                    MessageBox.Show("Subject Code is duplicated", "Change the subject code!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Information Updated.", "Authorization", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                gr.Close();

                gr.Open();
                if (txtsubcode2.Text == txtsubcode2.Text && txtStudID.Text == txtStudID.Text)
                {
                    comm = new OleDbCommand("Insert into tbl_Course(course_code, yearlevel, school_year, sem, stud_id) values(@course_code, @yearlevel, @school_year, @sem, @stud_id)", gr);
                    comm.Parameters.AddWithValue("course_code", txtsubcode2.Text);
                    comm.Parameters.AddWithValue("yearlevel", txtYL.Text);
                    comm.Parameters.AddWithValue("school_year", txtSY.Text);
                    comm.Parameters.AddWithValue("sem", txtSemester.Text);
                    comm.Parameters.AddWithValue("stud_id", txtStudID.Text);
                    comm.ExecuteNonQuery();
                }
                else if (txtsubcode2.Text != txtsubcode2.Text && txtStudID.Text == txtStudID.Text)
                {
                    MessageBox.Show("Subject Code is duplicated", "Change the subject code!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Information Updated.", "Authorization", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                gr.Close();

                gr.Open();
                if (txtsubcode3.Text == txtsubcode3.Text && txtStudID.Text == txtStudID.Text)
                {
                    comm = new OleDbCommand("Insert into tbl_Course(course_code, yearlevel, school_year, sem, stud_id) values(@course_code, @yearlevel, @school_year, @sem, @stud_id)", gr);
                    comm.Parameters.AddWithValue("course_code", txtsubcode3.Text);
                    comm.Parameters.AddWithValue("yearlevel", txtYL.Text);
                    comm.Parameters.AddWithValue("school_year", txtSY.Text);
                    comm.Parameters.AddWithValue("sem", txtSemester.Text);
                    comm.Parameters.AddWithValue("stud_id", txtStudID.Text);
                    comm.ExecuteNonQuery();
                }
                else if (txtsubcode3.Text != txtsubcode3.Text && txtStudID.Text == txtStudID.Text)
                {
                    MessageBox.Show("Subject Code is duplicated", "Change the subject code!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Information Updated.", "Authorization", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                gr.Close();

                gr.Open();
                if (txtsubcode4.Text == txtsubcode4.Text && txtStudID.Text == txtStudID.Text)
                {
                    comm = new OleDbCommand("Insert into tbl_Course(course_code, yearlevel, school_year, sem, stud_id) values(@course_code, @yearlevel, @school_year, @sem, @stud_id)", gr);
                    comm.Parameters.AddWithValue("course_code", txtsubcode4.Text);
                    comm.Parameters.AddWithValue("yearlevel", txtYL.Text);
                    comm.Parameters.AddWithValue("school_year", txtSY.Text);
                    comm.Parameters.AddWithValue("sem", txtSemester.Text);
                    comm.Parameters.AddWithValue("stud_id", txtStudID.Text);
                    comm.ExecuteNonQuery();
                }
                else if (txtsubcode4.Text != txtsubcode4.Text && txtStudID.Text == txtStudID.Text)
                {
                    MessageBox.Show("Subject Code is duplicated", "Change the subject code!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Information Updated.", "Authorization", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                gr.Close();

                gr.Open();
                if (txtsubcode5.Text == txtsubcode5.Text && txtStudID.Text == txtStudID.Text)
                {
                    comm = new OleDbCommand("Insert into tbl_Course(course_code, yearlevel, school_year, sem, stud_id) values(@course_code, @yearlevel, @school_year, @sem, @stud_id)", gr);
                    comm.Parameters.AddWithValue("course_code", txtsubcode5.Text);
                    comm.Parameters.AddWithValue("yearlevel", txtYL.Text);
                    comm.Parameters.AddWithValue("school_year", txtSY.Text);
                    comm.Parameters.AddWithValue("sem", txtSemester.Text);
                    comm.Parameters.AddWithValue("stud_id", txtStudID.Text);
                    comm.ExecuteNonQuery();
                }
                else if (txtsubcode5.Text != txtsubcode5.Text && txtStudID.Text == txtStudID.Text)
                {
                    MessageBox.Show("Subject Code is duplicated", "Change the subject code!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Information Updated.", "Authorization", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                gr.Close();

                gr.Open();
                if (txtsubcode6.Text == txtsubcode6.Text && txtStudID.Text == txtStudID.Text)
                {
                    comm = new OleDbCommand("Insert into tbl_Course(course_code, yearlevel, school_year, sem, stud_id) values(@course_code, @yearlevel, @school_year, @sem, @stud_id)", gr);
                    comm.Parameters.AddWithValue("course_code", txtsubcode6.Text);
                    comm.Parameters.AddWithValue("yearlevel", txtYL.Text);
                    comm.Parameters.AddWithValue("school_year", txtSY.Text);
                    comm.Parameters.AddWithValue("sem", txtSemester.Text);
                    comm.Parameters.AddWithValue("stud_id", txtStudID.Text);
                    comm.ExecuteNonQuery();
                }
                else if (txtsubcode6.Text != txtsubcode6.Text && txtStudID.Text == txtStudID.Text)
                {
                    MessageBox.Show("Subject Code is duplicated", "Change the subject code!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Information Updated.", "Authorization", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                gr.Close();

                gr.Open();
                if (txtsubcode7.Text == txtsubcode7.Text && txtStudID.Text == txtStudID.Text)
                {
                    comm = new OleDbCommand("Insert into tbl_Course(course_code, yearlevel, school_year, sem, stud_id) values(@course_code, @yearlevel, @school_year, @sem, @stud_id)", gr);
                    comm.Parameters.AddWithValue("course_code", txtsubcode7.Text);
                    comm.Parameters.AddWithValue("yearlevel", txtYL.Text);
                    comm.Parameters.AddWithValue("school_year", txtSY.Text);
                    comm.Parameters.AddWithValue("sem", txtSemester.Text);
                    comm.Parameters.AddWithValue("stud_id", txtStudID.Text);
                    comm.ExecuteNonQuery();
                }
                else if (txtsubcode7.Text != txtsubcode7.Text && txtStudID.Text == txtStudID.Text)
                {
                    MessageBox.Show("Subject Code is duplicated", "Change the subject code!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Information Updated.", "Authorization", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                gr.Close();

                gr.Open();
                if (txtsubcode8.Text == txtsubcode8.Text && txtStudID.Text == txtStudID.Text)
                {
                    comm = new OleDbCommand("Insert into tbl_Course(course_code, yearlevel, school_year, sem, stud_id) values(@course_code, @yearlevel, @school_year, @sem, @stud_id)", gr);
                    comm.Parameters.AddWithValue("course_code", txtsubcode8.Text);
                    comm.Parameters.AddWithValue("yearlevel", txtYL.Text);
                    comm.Parameters.AddWithValue("school_year", txtSY.Text);
                    comm.Parameters.AddWithValue("sem", txtSemester.Text);
                    comm.Parameters.AddWithValue("stud_id", txtStudID.Text);
                    comm.ExecuteNonQuery();
                }
                else if (txtsubcode8.Text != txtsubcode8.Text && txtStudID.Text == txtStudID.Text)
                {
                    MessageBox.Show("Subject Code is duplicated", "Change the subject code!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Information Updated.", "Authorization", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error I Think", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                gr.Close();
            }

        }



        private void txtRerenceNum_TextChanged(object sender, EventArgs e)
        {
            con.Open();
            if (txtRerenceNum.Text != "")
            {
                comm = new OleDbCommand("Select temp_stud, temp_stud_fname, temp_stud_mname, temp_stud_lname, temp_stud_suffix, temp_stud_progcode, temp_stud_sy ,temp_stud_yearlevel ,temp_stud_sem ,temp_stud_cdate From tbl_Temp_StudInfo WHERE temp_referencenum = '" + txtRerenceNum.Text + "'", con);
                comm.Parameters.AddWithValue("temp_referencenum", txtRerenceNum.Text.ToString());
                reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    txtStudID.Text = reader.GetValue(0).ToString();
                    txtFName.Text = reader.GetValue(1).ToString();
                    txtMName.Text = reader.GetValue(2).ToString();
                    txtLName.Text = reader.GetValue(3).ToString();
                    txtSuffix.Text = reader.GetValue(4).ToString();
                    txtCourse.Text = reader.GetValue(5).ToString();
                    txtSY.Text = reader.GetValue(6).ToString();
                    txtYL.Text = reader.GetValue(7).ToString();
                    txtSemester.Text = reader.GetValue(8).ToString();
                    txtDate.Text = reader.GetValue(9).ToString();

                }
            }
            else
            {
                txtStudID.Clear();
                txtFName.Clear();
                txtMName.Clear();
                txtLName.Clear();
                txtSuffix.Clear();
                txtCourse.Clear();
                txtYL.Clear();
                txtSemester.Clear();
                txtSY.Clear();
                txtDate.Clear();
            }

            con.Close();

            con.Open();
            if (txtRerenceNum.Text != "")
            {
                comm = new OleDbCommand("Select temp_coursecode_1, temp_coursecode_2, temp_coursecode_3, temp_coursecode_4, temp_coursecode_5, temp_coursecode_6, temp_coursecode_7, temp_coursecode_8 From tbl_Temp_CourseCode Where temp_referencenum = '" + txtRerenceNum.Text + "'", con);
                comm.Parameters.AddWithValue("temp_referencenum", txtRerenceNum.Text.ToString());
                comm.ExecuteNonQuery();
                comm.Parameters.Clear();
                reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    txtsubcode1.Text = reader.GetValue(0).ToString();
                    txtsubcode2.Text = reader.GetValue(1).ToString();
                    txtsubcode3.Text = reader.GetValue(2).ToString();
                    txtsubcode4.Text = reader.GetValue(3).ToString();
                    txtsubcode5.Text = reader.GetValue(4).ToString();
                    txtsubcode6.Text = reader.GetValue(5).ToString();
                    txtsubcode7.Text = reader.GetValue(6).ToString();
                    txtsubcode8.Text = reader.GetValue(7).ToString();
                }
            }
            else
            {
                txtsubcode1.Clear();
                txtsubcode2.Clear();
                txtsubcode3.Clear();
                txtsubcode4.Clear();
                txtsubcode5.Clear();
                txtsubcode6.Clear();
                txtsubcode7.Clear();
                txtsubcode8.Clear();
            }
            con.Close();

            connection.Open();
            if (txtRerenceNum.Text != "")
            {
                comm = new OleDbCommand("Select bsit_coursecode_desc,bsit_credunit,bsit_labunit,bsit_lecunit From tbl_BSITCourseDetail WHERE bsit_coursecode_id = '" + txtsubcode1.Text + "'", connection);
                comm.Parameters.AddWithValue("bsit_coursecode_id", txtsubcode1.Text.ToString());
                comm.Parameters.AddWithValue("bsit_coursecode_id", txtsubcode2.Text.ToString());
                comm.Parameters.AddWithValue("bsit_coursecode_id", txtsubcode3.Text.ToString());
                comm.Parameters.AddWithValue("bsit_coursecode_id", txtsubcode4.Text.ToString());
                comm.Parameters.AddWithValue("bsit_coursecode_id", txtsubcode5.Text.ToString());
                comm.Parameters.AddWithValue("bsit_coursecode_id", txtsubcode6.Text.ToString());
                comm.Parameters.AddWithValue("bsit_coursecode_id", txtsubcode7.Text.ToString());
                comm.Parameters.AddWithValue("bsit_coursecode_id", txtsubcode8.Text.ToString());
                reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    txtdesc1.Text = reader.GetValue(0).ToString();
                    txtcred1.Text = reader.GetValue(1).ToString();
                    txtlec1.Text = reader.GetValue(2).ToString();
                    txtlab1.Text = reader.GetValue(3).ToString();
                }
            }
            else
            {
                txtdesc1.Clear();
                txtcred1.Clear();
                txtlec1.Clear();
                txtlab1.Clear();
            }
            connection.Close();

            connection.Open();
            if (txtRerenceNum.Text != "")
            {
                comm = new OleDbCommand("Select bsit_coursecode_desc,bsit_credunit,bsit_labunit,bsit_lecunit From tbl_BSITCourseDetail WHERE bsit_coursecode_id = '" + txtsubcode2.Text + "'", connection);
                comm.Parameters.AddWithValue("bsit_coursecode_id", txtsubcode2.Text.ToString());
                reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    txtdesc2.Text = reader.GetValue(0).ToString();
                    txtcred2.Text = reader.GetValue(1).ToString();
                    txtlec2.Text = reader.GetValue(2).ToString();
                    txtlab2.Text = reader.GetValue(3).ToString();
                }
            }
            else
            {
                txtdesc2.Clear();
                txtcred2.Clear();
                txtlec2.Clear();
                txtlab2.Clear();
            }
            connection.Close();

            connection.Open();
            if (txtRerenceNum.Text != "")
            {
                comm = new OleDbCommand("Select bsit_coursecode_desc,bsit_credunit,bsit_labunit,bsit_lecunit From tbl_BSITCourseDetail WHERE bsit_coursecode_id = '" + txtsubcode3.Text + "'", connection);
                comm.Parameters.AddWithValue("bsit_coursecode_id", txtsubcode3.Text.ToString());
                reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    txtdesc3.Text = reader.GetValue(0).ToString();
                    txtcred3.Text = reader.GetValue(1).ToString();
                    txtlec3.Text = reader.GetValue(2).ToString();
                    txtlab3.Text = reader.GetValue(3).ToString();
                }
            }
            else
            {
                txtdesc3.Clear();
                txtcred3.Clear();
                txtlec3.Clear();
                txtlab3.Clear();
            }
            connection.Close();

            connection.Open();
            if (txtRerenceNum.Text != "")
            {
                comm = new OleDbCommand("Select bsit_coursecode_desc,bsit_credunit,bsit_labunit,bsit_lecunit From tbl_BSITCourseDetail WHERE bsit_coursecode_id = '" + txtsubcode4.Text + "'", connection);
                comm.Parameters.AddWithValue("bsit_coursecode_id", txtsubcode4.Text.ToString());
                reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    txtdesc4.Text = reader.GetValue(0).ToString();
                    txtcred4.Text = reader.GetValue(1).ToString();
                    txtlec4.Text = reader.GetValue(2).ToString();
                    txtlab4.Text = reader.GetValue(3).ToString();
                }
            }
            else
            {
                txtdesc4.Clear();
                txtcred4.Clear();
                txtlec4.Clear();
                txtlab4.Clear();
            }
            connection.Close();

            connection.Open();
            if (txtRerenceNum.Text != "")
            {
                comm = new OleDbCommand("Select bsit_coursecode_desc,bsit_credunit,bsit_labunit,bsit_lecunit From tbl_BSITCourseDetail WHERE bsit_coursecode_id = '" + txtsubcode5.Text + "'", connection);
                comm.Parameters.AddWithValue("bsit_coursecode_id", txtsubcode5.Text.ToString());
                reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    txtdesc5.Text = reader.GetValue(0).ToString();
                    txtcred5.Text = reader.GetValue(1).ToString();
                    txtlec5.Text = reader.GetValue(2).ToString();
                    txtlab5.Text = reader.GetValue(3).ToString();
                }
            }
            else
            {
                txtdesc5.Clear();
                txtcred5.Clear();
                txtlec5.Clear();
                txtlab5.Clear();
            }
            connection.Close();

            connection.Open();
            if (txtRerenceNum.Text != "")
            {
                comm = new OleDbCommand("Select bsit_coursecode_desc,bsit_credunit,bsit_labunit,bsit_lecunit From tbl_BSITCourseDetail WHERE bsit_coursecode_id = '" + txtsubcode6.Text + "'", connection);
                comm.Parameters.AddWithValue("bsit_coursecode_id", txtsubcode6.Text.ToString());
                reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    txtdesc6.Text = reader.GetValue(0).ToString();
                    txtcred6.Text = reader.GetValue(1).ToString();
                    txtlec6.Text = reader.GetValue(2).ToString();
                    txtlab6.Text = reader.GetValue(3).ToString();
                }
            }
            else
            {
                txtdesc6.Clear();
                txtcred6.Clear();
                txtlec6.Clear();
                txtlab6.Clear();
            }
            connection.Close();

            connection.Open();
            if (txtRerenceNum.Text != "")
            {
                comm = new OleDbCommand("Select bsit_coursecode_desc,bsit_credunit,bsit_labunit,bsit_lecunit From tbl_BSITCourseDetail WHERE bsit_coursecode_id = '" + txtsubcode7.Text + "'", connection);
                comm.Parameters.AddWithValue("bsit_coursecode_id", txtsubcode7.Text.ToString());
                reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    txtdesc7.Text = reader.GetValue(0).ToString();
                    txtcred7.Text = reader.GetValue(1).ToString();
                    txtlec7.Text = reader.GetValue(2).ToString();
                    txtlab7.Text = reader.GetValue(3).ToString();
                }
            }
            else
            {
                txtdesc7.Clear();
                txtcred7.Clear();
                txtlec7.Clear();
                txtlab7.Clear();
            }
            connection.Close();

            connection.Open();
            if (txtRerenceNum.Text != "")
            {
                comm = new OleDbCommand("Select bsit_coursecode_desc,bsit_credunit,bsit_labunit,bsit_lecunit From tbl_BSITCourseDetail WHERE bsit_coursecode_id = '" + txtsubcode8.Text + "'", connection);
                comm.Parameters.AddWithValue("bsit_coursecode_id", txtsubcode8.Text.ToString());
                reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    txtdesc8.Text = reader.GetValue(0).ToString();
                    txtcred8.Text = reader.GetValue(1).ToString();
                    txtlec8.Text = reader.GetValue(2).ToString();
                    txtlab8.Text = reader.GetValue(3).ToString();
                }
            }
            else
            {
                txtdesc8.Clear();
                txtcred8.Clear();
                txtlec8.Clear();
                txtlab8.Clear();
            }
            connection.Close();
        }

        private void btnCancelRegistrar_Click(object sender, EventArgs e)
        {
            RegistrarDirectoryForm nf = new RegistrarDirectoryForm();
            nf.Show();
            this.Hide();
        }

        private void Print_Click(object sender, EventArgs e)
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

        private void RegFormDirectoryRegistrar_Load(object sender, EventArgs e)
        {

        }

        private void txtsubcode1_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtLName.Clear();
            txtFName.Clear();
            txtMName.Clear();
            txtSuffix.Clear();
            txtStudID.Clear();
            txtCourse.Clear();
            txtSY.Clear();
            txtYL.Clear();
            txtSemester.Clear();
            txtRerenceNum.Clear();
            txtDate.Clear();
        }
    }
}
