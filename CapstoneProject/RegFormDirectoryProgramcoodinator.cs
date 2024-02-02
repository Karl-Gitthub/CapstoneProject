using System;
using System.Data.OleDb;
using System.Windows.Forms;

namespace CapstoneProject
{
    public partial class RegFormDirectoryProgramcoodinator : Form
    {
        OleDbConnection connection = new OleDbConnection(@"Provider = Microsoft.ACE.OLEDB.12.0; Data Source = C:\Users\Karl\OneDrive\Documents\CapstoneProject\New folder\dbCourseManagement.accdb");
        OleDbConnection conn = new OleDbConnection(@"Provider = Microsoft.ACE.OLEDB.12.0; Data Source = C:\Users\Karl\OneDrive\Documents\CapstoneProject\New folder\dbStudentManagement.accdb");
        OleDbConnection con = new OleDbConnection(@"Provider = Microsoft.ACE.OLEDB.12.0; Data Source = C:\Users\Karl\OneDrive\Documents\CapstoneProject\New folder\db_Temp_RegForm.accdb");
        OleDbCommand comm;
        OleDbDataReader reader;
        public RegFormDirectoryProgramcoodinator()
        {
            InitializeComponent();
        }

        private void txtsubcode1_TextChanged(object sender, EventArgs e)
        {
            connection.Open();
            if (txtsubcode1.Text != "")
            {
                comm = new OleDbCommand("Select bsit_coursecode_desc,bsit_credunit,bsit_labunit,bsit_lecunit From tbl_BSITCourseDetail WHERE bsit_coursecode_id = '" + txtsubcode1.Text + "'", connection);
                comm.Parameters.AddWithValue("bsit_coursecode_id", txtsubcode1.Text.ToString());
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
        }

        private void txtsubcode2_TextChanged(object sender, EventArgs e)
        {
            connection.Open();
            if (txtsubcode2.Text != "")
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
        }

        private void txtsubcode3_TextChanged(object sender, EventArgs e)
        {
            connection.Open();
            if (txtsubcode3.Text != "")
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
        }

        private void txtsubcode4_TextChanged(object sender, EventArgs e)
        {
            connection.Open();
            if (txtsubcode4.Text != "")
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
        }

        private void txtsubcode5_TextChanged(object sender, EventArgs e)
        {
            connection.Open();
            if (txtsubcode5.Text != "")
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
        }

        private void txtsubcode6_TextChanged(object sender, EventArgs e)
        {
            connection.Open();
            if (txtsubcode6.Text != "")
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
        }

        private void txtsubcode7_TextChanged(object sender, EventArgs e)
        {
            connection.Open();
            if (txtsubcode7.Text != "")
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
        }

        private void txtsubcode8_TextChanged(object sender, EventArgs e)
        {
            connection.Open();
            if (txtsubcode8.Text != "")
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

        private void txtStudID_TextChanged(object sender, EventArgs e)
        {
            conn.Open();
            if (txtStudID.Text != "")
            {
                comm = new OleDbCommand("Select studfname, studmname, studlname, studsuffix, studprogram, yearlevel, semester From tbl_BSITStud_Detail WHERE bstudid = '" + txtStudID.Text + "'", conn);
                comm.Parameters.AddWithValue("studid", txtStudID.Text.ToString());
                reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    txtFName.Text = reader.GetValue(0).ToString();
                    txtMName.Text = reader.GetValue(1).ToString();
                    txtLName.Text = reader.GetValue(2).ToString();
                    txtSuffix.Text = reader.GetValue(3).ToString();
                    txtCourse.Text = reader.GetValue(4).ToString();
                    txtYL.Text = reader.GetValue(5).ToString();
                    txtSemester.Text = reader.GetValue(6).ToString();
                }
            }
            else
            {
                txtFName.Clear();
                txtMName.Clear();
                txtLName.Clear();
                txtSuffix.Clear();
                txtCourse.Clear();
                txtYL.Clear();
                txtSemester.Clear();
                txtSY.Clear();
            }
            conn.Close();

        }

        private void Submit_Click(object sender, EventArgs e)
        {
            con.Open();
            comm = new OleDbCommand("Insert into tbl_Temp_StudInfo(temp_referencenum, temp_stud, temp_stud_fname, temp_stud_mname, temp_stud_lname, temp_stud_suffix, temp_stud_progcode, temp_stud_sy, temp_stud_yearlevel, temp_stud_sem, temp_stud_cdate) values(@temp_referencenum, @temp_stud, @temp_stud_fname, @temp_stud_mname, @temp_stud_lname, @temp_stud_suffix, @temp_stud_progcode, @temp_stud_sy, @temp_stud_yearlevel, @temp_stud_sem, @temp_stud_cdate)", con);
            comm.Parameters.AddWithValue("@temp_referencenum", txtRerenceNum.Text);
            comm.Parameters.AddWithValue("@temp_stud", txtStudID.Text);
            comm.Parameters.AddWithValue("@temp_stud_fname", txtFName.Text);
            comm.Parameters.AddWithValue("@temp_stud_mname", txtMName.Text);
            comm.Parameters.AddWithValue("@temp_stud_lname", txtLName.Text);
            comm.Parameters.AddWithValue("@temp_stud_suffix", txtSuffix.Text);
            comm.Parameters.AddWithValue("@temp_stud_progcode", txtCourse.Text);
            comm.Parameters.AddWithValue("@temp_stud_sy", txtSY.Text);
            comm.Parameters.AddWithValue("@temp_stud_yearlevel", txtYL.Text);
            comm.Parameters.AddWithValue("@temp_stud_sem", txtSemester.Text);
            comm.Parameters.AddWithValue("@temp_stud_cdate", txtDate.Text);
            comm.ExecuteNonQuery();
            comm.Parameters.Clear();
            con.Close();

            con.Open();
            comm = new OleDbCommand("Insert into tbl_Temp_CourseCode(temp_referencenum, temp_coursecode_1, temp_coursecode_2, temp_coursecode_3, temp_coursecode_4, temp_coursecode_5, temp_coursecode_6, temp_coursecode_7, temp_coursecode_8) values(@temp_referencenum, @temp_coursecode_1, @temp_coursecode_2, @temp_coursecode_3, @temp_coursecode_4, @temp_coursecode_5, @temp_coursecode_6, @temp_coursecode_7, @temp_coursecode_8)", con);
            comm.Parameters.AddWithValue("temp_referencenum", txtRerenceNum.Text);
            comm.Parameters.AddWithValue("temp_coursecode_1", txtsubcode1.Text);
            comm.Parameters.AddWithValue("temp_coursecode_2", txtsubcode2.Text);
            comm.Parameters.AddWithValue("temp_coursecode_3", txtsubcode3.Text);
            comm.Parameters.AddWithValue("temp_coursecode_4", txtsubcode4.Text);
            comm.Parameters.AddWithValue("temp_coursecode_5", txtsubcode5.Text);
            comm.Parameters.AddWithValue("temp_coursecode_6", txtsubcode6.Text);
            comm.Parameters.AddWithValue("temp_coursecode_7", txtsubcode7.Text);
            comm.Parameters.AddWithValue("temp_coursecode_8", txtsubcode8.Text);
            comm.ExecuteNonQuery();
            comm.Parameters.Clear();
            con.Close();
        }

        private void txtRerenceNum_TextChanged(object sender, EventArgs e)
        {

        }

        private void Print_Click(object sender, EventArgs e)
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
