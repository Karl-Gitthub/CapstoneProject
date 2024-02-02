using System;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace CapstoneProject
{
    public partial class AddFacultyDetailandLoading : Form
    {
        string BSIT, BSBA;

        OleDbConnection conn = new OleDbConnection(@"Provider = Microsoft.ACE.OLEDB.12.0; Data Source = C:\Users\Karl\OneDrive\Documents\CapstoneProject\New folder\db_GradingManagement.accdb");
        OleDbConnection connection = new OleDbConnection(@"Provider = Microsoft.ACE.OLEDB.12.0; Data Source = C:\Users\Karl\OneDrive\Documents\CapstoneProject\New folder\dbCourseManagement.accdb");
        OleDbCommand comm;
        OleDbDataReader reader;
        public AddFacultyDetailandLoading()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            ProgramCoordinatorDirectoryForm nf = new ProgramCoordinatorDirectoryForm();
            nf.Show();
            this.Hide();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            MemoryStream MS = new MemoryStream();
            pictureBox1.BackgroundImage.Save(MS, System.Drawing.Imaging.ImageFormat.Jpeg);
            byte[] arrImage = MS.GetBuffer();

            conn.Open();
            comm = new OleDbCommand("Insert Into tbl_FacultyDetail(fac_id, fac_fname, fac_mname, fac_lname) values(@fac_id, @fac_fname, @fac_mname, @fac_lname)", conn);
            comm.Parameters.AddWithValue("@fac_id", txt1username.Text);
            comm.Parameters.AddWithValue("@fac_fname", txt2firstname.Text);
            comm.Parameters.AddWithValue("@fac_mname", txt3middlename.Text);
            comm.Parameters.AddWithValue("@fac_lname", txt4lastname.Text);
            comm.Parameters.AddWithValue("@fac_pfp", arrImage);
            comm.ExecuteNonQuery();
            comm.Parameters.Clear();
            conn.Close();

            txt1username.Clear();
            txt2firstname.Clear();
            txt3middlename.Clear();
            txt4lastname.Clear();
            //arrImage = null;
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            //1
            conn.Open();
            comm = new OleDbCommand("Insert Into tbl_FacultyAttachment (fac_id, coursecode, school_year, semester, program) values(@fac_id, coursecode, school_year, semester, program)", conn);
            comm.Parameters.AddWithValue("fac_id", txtFacUsername1.Text);
            comm.Parameters.AddWithValue("coursecode", txtCC1.Text);
            comm.Parameters.AddWithValue("school_year", txtschoolyear1.Text);
            comm.Parameters.AddWithValue("semester", txtSemester1.Text);
            comm.Parameters.AddWithValue("program", txtProgram.Text);
            comm.ExecuteNonQuery();
            comm.Parameters.Clear();
            conn.Close();
            //2
            conn.Open();
            comm = new OleDbCommand("Insert Into tbl_FacultyAttachment (fac_id, coursecode, school_year, semester, program) values(@fac_id, coursecode, school_year, semester, program)", conn);
            comm.Parameters.AddWithValue("fac_id", txtFacUsername2.Text);
            comm.Parameters.AddWithValue("coursecode", txtCC2.Text);
            comm.Parameters.AddWithValue("school_year", txtschoolyear1.Text);
            comm.Parameters.AddWithValue("semester", txtSemester1.Text);
            comm.Parameters.AddWithValue("program", txtProgram.Text);
            comm.ExecuteNonQuery();
            comm.Parameters.Clear();
            conn.Close();
            //3
            conn.Open();
            comm = new OleDbCommand("Insert Into tbl_FacultyAttachment (fac_id, coursecode, school_year, semester, program) values(@fac_id, coursecode, school_year, semester, program)", conn);
            comm.Parameters.AddWithValue("fac_id", txtFacUsername3.Text);
            comm.Parameters.AddWithValue("coursecode", txtCC3.Text);
            comm.Parameters.AddWithValue("school_year", txtschoolyear1.Text);
            comm.Parameters.AddWithValue("semester", txtSemester1.Text);
            comm.Parameters.AddWithValue("program", txtProgram.Text);
            comm.ExecuteNonQuery();
            comm.Parameters.Clear();
            conn.Close();
            //4
            conn.Open();
            comm = new OleDbCommand("Insert Into tbl_FacultyAttachment (fac_id, coursecode, school_year, semester, program) values(@fac_id, coursecode, school_year, semester, program)", conn);
            comm.Parameters.AddWithValue("fac_id", txtFacUsername4.Text);
            comm.Parameters.AddWithValue("coursecode", txtCC4.Text);
            comm.Parameters.AddWithValue("school_year", txtschoolyear1.Text);
            comm.Parameters.AddWithValue("semester", txtSemester1.Text);
            comm.Parameters.AddWithValue("program", txtProgram.Text);
            comm.ExecuteNonQuery();
            comm.Parameters.Clear();
            conn.Close();
            //5
            conn.Open();
            comm = new OleDbCommand("Insert Into tbl_FacultyAttachment (fac_id, coursecode, school_year, semester, program) values(@fac_id, coursecode, school_year, semester, program)", conn);
            comm.Parameters.AddWithValue("fac_id", txtFacUsername5.Text);
            comm.Parameters.AddWithValue("coursecode", txtCC5.Text);
            comm.Parameters.AddWithValue("school_year", txtschoolyear1.Text);
            comm.Parameters.AddWithValue("semester", txtSemester1.Text);
            comm.Parameters.AddWithValue("program", txtProgram.Text);
            comm.ExecuteNonQuery();
            comm.Parameters.Clear();
            conn.Close();
            //6
            conn.Open();
            comm = new OleDbCommand("Insert Into tbl_FacultyAttachment (fac_id, coursecode, school_year, semester, program) values(@fac_id, coursecode, school_year, semester, program)", conn);
            comm.Parameters.AddWithValue("fac_id", txtFacUsername6.Text);
            comm.Parameters.AddWithValue("coursecode", txtCC6.Text);
            comm.Parameters.AddWithValue("school_year", txtschoolyear1.Text);
            comm.Parameters.AddWithValue("semester", txtSemester1.Text);
            comm.Parameters.AddWithValue("program", txtProgram.Text);
            comm.ExecuteNonQuery();
            comm.Parameters.Clear();
            conn.Close();
            //7
            conn.Open();
            comm = new OleDbCommand("Insert Into tbl_FacultyAttachment (fac_id, coursecode, school_year, semester, program) values(@fac_id, coursecode, school_year, semester, program)", conn);
            comm.Parameters.AddWithValue("fac_id", txtFacUsername7.Text);
            comm.Parameters.AddWithValue("coursecode", txtCC7.Text);
            comm.Parameters.AddWithValue("school_year", txtschoolyear1.Text);
            comm.Parameters.AddWithValue("semester", txtSemester1.Text);
            comm.Parameters.AddWithValue("program", txtProgram.Text);
            comm.ExecuteNonQuery();
            comm.Parameters.Clear();
            conn.Close();
            //8
            conn.Open();
            comm = new OleDbCommand("Insert Into tbl_FacultyAttachment (fac_id, coursecode, school_year, semester, program) values(@fac_id, coursecode, school_year, semester, program)", conn);
            comm.Parameters.AddWithValue("fac_id", txtFacUsername8.Text);
            comm.Parameters.AddWithValue("coursecode", txtCC8.Text);
            comm.Parameters.AddWithValue("school_year", txtschoolyear1.Text);
            comm.Parameters.AddWithValue("semester", txtSemester1.Text);
            comm.Parameters.AddWithValue("program", txtProgram.Text);
            comm.ExecuteNonQuery();
            comm.Parameters.Clear();
            conn.Close();

            txtProgram.Clear();
            txtSemester1.Clear();
            txtschoolyear1.Clear();
            txtCC1.Clear();
            txtCC2.Clear();
            txtCC3.Clear();
            txtCC4.Clear();
            txtCC5.Clear();
            txtCC6.Clear();
            txtCC7.Clear();
            txtCC8.Clear();
            txtCD1.Clear();
            txtCD2.Clear();
            txtCD3.Clear();
            txtCD4.Clear();
            txtCD5.Clear();
            txtCD6.Clear();
            txtCD7.Clear();
            txtCD8.Clear();
            txtFacUsername1.Clear();
            txtFacUsername2.Clear();
            txtFacUsername3.Clear();
            txtFacUsername4.Clear();
            txtFacUsername5.Clear();
            txtFacUsername6.Clear();
            txtFacUsername7.Clear();
            txtFacUsername8.Clear();
            lblfname.Text = "";
            lblfname2.Text = "";
            lblfname3.Text = "";
            lblfname4.Text = "";
            lblfname5.Text = "";
            lblfname6.Text = "";
            lblfname7.Text = "";
            lblfname8.Text = "";
            lblmname.Text = "";
            lblmname2.Text = "";
            lblmname3.Text = "";
            lblmname4.Text = "";
            lblmname5.Text = "";
            lblmname6.Text = "";
            lblmname7.Text = "";
            lblmname8.Text = "";
            lblname.Text = "";
            lblname2.Text = "";
            lblname3.Text = "";
            lblname4.Text = "";
            lblname5.Text = "";
            lblname6.Text = "";
            lblname7.Text = "";
            lblname8.Text = "";
        }

        private void txtCC1_TextChanged(object sender, EventArgs e)
        {
            string PROGRAM;

            PROGRAM = txtProgram.Text;

            switch (PROGRAM)
            {
                case "BSIT":
                    //1
                    if (txtCC1.Text != "")
                    {
                        connection.Open();
                        comm = new OleDbCommand("Select bsit_coursecode_desc From tbl_BSITCourseDetail Where bsit_coursecode_id = '" + txtCC1.Text + "'", connection);
                        comm.Parameters.AddWithValue("bsit_coursecode_id", txtCC1.Text.ToString());
                        reader = comm.ExecuteReader();
                        while (reader.Read())
                        {
                            txtCD1.Text = reader.GetValue(0).ToString();
                        }
                        connection.Close();
                    }
                    else
                    {
                        txtCC1.Clear();
                        txtCD1.Clear();
                    }

                    break;

                case "BSBA":

                    if (txtCC1.Text != "")
                    {
                        connection.Open();
                        comm = new OleDbCommand("Select bsba_coursecode_desc From tbl_BSBACourseDetail Where bsba_coursecode_id = '" + txtCC1.Text + "'", connection);
                        comm.Parameters.AddWithValue("bsba_coursecode_id", txtCC1.Text.ToString());
                        reader = comm.ExecuteReader();
                        while (reader.Read())
                        {
                            txtCD1.Text = reader.GetValue(0).ToString();
                        }
                        connection.Close();

                    }
                    else
                    {
                        txtCC1.Clear();
                        txtCD1.Clear();
                    }

                    break;
            }

        }

        private void txtCC2_TextChanged(object sender, EventArgs e)
        {
            string PROGRAM;

            PROGRAM = txtProgram.Text;

            switch (PROGRAM)
            {
                case "BSIT":
                    //2
                    if (txtCC2.Text != "")
                    {
                        connection.Open();
                        comm = new OleDbCommand("Select bsit_coursecode_desc From tbl_BSITCourseDetail Where bsit_coursecode_id = '" + txtCC2.Text + "'", connection);
                        comm.Parameters.AddWithValue("bsit_coursecode_id", txtCC2.Text.ToString());
                        reader = comm.ExecuteReader();
                        while (reader.Read())
                        {
                            txtCD2.Text = reader.GetValue(0).ToString();
                        }
                        connection.Close();
                    }
                    else
                    {
                        txtCC2.Clear();
                        txtCD2.Clear();
                    }

                    break;

                case "BSBA":

                    if (txtCC2.Text != "")
                    {
                        connection.Open();
                        comm = new OleDbCommand("Select bsba_coursecode_desc From tbl_BSBACourseDetail Where bsba_coursecode_id = '" + txtCC2.Text + "'", connection);
                        comm.Parameters.AddWithValue("bsba_coursecode_id", txtCC2.Text.ToString());
                        reader = comm.ExecuteReader();
                        while (reader.Read())
                        {
                            txtCD2.Text = reader.GetValue(0).ToString();
                        }
                        connection.Close();

                    }
                    else
                    {
                        txtCC2.Clear();
                        txtCD2.Clear();
                    }

                    break;
            }
        }

        private void txtCC3_TextChanged(object sender, EventArgs e)
        {
            string PROGRAM;

            PROGRAM = txtProgram.Text;

            switch (PROGRAM)
            {
                case "BSIT":
                    //3
                    if (txtCC3.Text != "")
                    {
                        connection.Open();
                        comm = new OleDbCommand("Select bsit_coursecode_desc From tbl_BSITCourseDetail Where bsit_coursecode_id = '" + txtCC3.Text + "'", connection);
                        comm.Parameters.AddWithValue("bsit_coursecode_id", txtCC3.Text.ToString());
                        reader = comm.ExecuteReader();
                        while (reader.Read())
                        {
                            txtCD3.Text = reader.GetValue(0).ToString();
                        }
                        connection.Close();
                    }
                    else
                    {
                        txtCC3.Clear();
                        txtCD3.Clear();
                    }

                    break;

                case "BSBA":

                    if (txtCC1.Text != "")
                    {
                        connection.Open();
                        comm = new OleDbCommand("Select bsba_coursecode_desc From tbl_BSBACourseDetail Where bsba_coursecode_id = '" + txtCC3.Text + "'", connection);
                        comm.Parameters.AddWithValue("bsba_coursecode_id", txtCC3.Text.ToString());
                        reader = comm.ExecuteReader();
                        while (reader.Read())
                        {
                            txtCD3.Text = reader.GetValue(0).ToString();
                        }
                        connection.Close();

                    }
                    else
                    {
                        txtCC3.Clear();
                        txtCD3.Clear();
                    }

                    break;
            }
        }

        private void txtCC4_TextChanged(object sender, EventArgs e)
        {
            string PROGRAM;

            PROGRAM = txtProgram.Text;

            switch (PROGRAM)
            {
                case "BSIT":
                    //4
                    if (txtCC4.Text != "")
                    {
                        connection.Open();
                        comm = new OleDbCommand("Select bsit_coursecode_desc From tbl_BSITCourseDetail Where bsit_coursecode_id = '" + txtCC4.Text + "'", connection);
                        comm.Parameters.AddWithValue("bsit_coursecode_id", txtCC4.Text.ToString());
                        reader = comm.ExecuteReader();
                        while (reader.Read())
                        {
                            txtCD4.Text = reader.GetValue(0).ToString();
                        }
                        connection.Close();
                    }
                    else
                    {
                        txtCC4.Clear();
                        txtCD4.Clear();
                    }

                    break;

                case "BSBA":

                    if (txtCC4.Text != "")
                    {
                        connection.Open();
                        comm = new OleDbCommand("Select bsba_coursecode_desc From tbl_BSBACourseDetail Where bsba_coursecode_id = '" + txtCC4.Text + "'", connection);
                        comm.Parameters.AddWithValue("bsba_coursecode_id", txtCC4.Text.ToString());
                        reader = comm.ExecuteReader();
                        while (reader.Read())
                        {
                            txtCD4.Text = reader.GetValue(0).ToString();
                        }
                        connection.Close();

                    }
                    else
                    {
                        txtCC4.Clear();
                        txtCD4.Clear();
                    }

                    break;
            }
        }

        private void txtCC5_TextChanged(object sender, EventArgs e)
        {
            string PROGRAM;

            PROGRAM = txtProgram.Text;

            switch (PROGRAM)
            {
                case "BSIT":
                    //5
                    if (txtCC5.Text != "")
                    {
                        connection.Open();
                        comm = new OleDbCommand("Select bsit_coursecode_desc From tbl_BSITCourseDetail Where bsit_coursecode_id = '" + txtCC5.Text + "'", connection);
                        comm.Parameters.AddWithValue("bsit_coursecode_id", txtCC5.Text.ToString());
                        reader = comm.ExecuteReader();
                        while (reader.Read())
                        {
                            txtCD5.Text = reader.GetValue(0).ToString();
                        }
                        connection.Close();
                    }
                    else
                    {
                        txtCC5.Clear();
                        txtCD5.Clear();
                    }

                    break;

                case "BSBA":

                    if (txtCC5.Text != "")
                    {
                        connection.Open();
                        comm = new OleDbCommand("Select bsba_coursecode_desc From tbl_BSBACourseDetail Where bsba_coursecode_id = '" + txtCC5.Text + "'", connection);
                        comm.Parameters.AddWithValue("bsba_coursecode_id", txtCC5.Text.ToString());
                        reader = comm.ExecuteReader();
                        while (reader.Read())
                        {
                            txtCD5.Text = reader.GetValue(0).ToString();
                        }
                        connection.Close();

                    }
                    else
                    {
                        txtCC5.Clear();
                        txtCD5.Clear();
                    }

                    break;
            }
        }

        private void txtCC6_TextChanged(object sender, EventArgs e)
        {
            string PROGRAM;

            PROGRAM = txtProgram.Text;

            switch (PROGRAM)
            {
                case "BSIT":
                    //6
                    if (txtCC6.Text != "")
                    {
                        connection.Open();
                        comm = new OleDbCommand("Select bsit_coursecode_desc From tbl_BSITCourseDetail Where bsit_coursecode_id = '" + txtCC6.Text + "'", connection);
                        comm.Parameters.AddWithValue("bsit_coursecode_id", txtCC6.Text.ToString());
                        reader = comm.ExecuteReader();
                        while (reader.Read())
                        {
                            txtCD6.Text = reader.GetValue(0).ToString();
                        }
                        connection.Close();
                    }
                    else
                    {
                        txtCC6.Clear();
                        txtCD6.Clear();
                    }

                    break;

                case "BSBA":

                    if (txtCC6.Text != "")
                    {
                        connection.Open();
                        comm = new OleDbCommand("Select bsba_coursecode_desc From tbl_BSBACourseDetail Where bsba_coursecode_id = '" + txtCC6.Text + "'", connection);
                        comm.Parameters.AddWithValue("bsba_coursecode_id", txtCC6.Text.ToString());
                        reader = comm.ExecuteReader();
                        while (reader.Read())
                        {
                            txtCD6.Text = reader.GetValue(0).ToString();
                        }
                        connection.Close();

                    }
                    else
                    {
                        txtCC6.Clear();
                        txtCD6.Clear();
                    }

                    break;
            }
        }

        private void txtCC7_TextChanged(object sender, EventArgs e)
        {
            string PROGRAM;

            PROGRAM = txtProgram.Text;

            switch (PROGRAM)
            {
                case "BSIT":
                    //7
                    if (txtCC1.Text != "")
                    {
                        connection.Open();
                        comm = new OleDbCommand("Select bsit_coursecode_desc From tbl_BSITCourseDetail Where bsit_coursecode_id = '" + txtCC7.Text + "'", connection);
                        comm.Parameters.AddWithValue("bsit_coursecode_id", txtCC7.Text.ToString());
                        reader = comm.ExecuteReader();
                        while (reader.Read())
                        {
                            txtCD7.Text = reader.GetValue(0).ToString();
                        }
                        connection.Close();
                    }
                    else
                    {
                        txtCC7.Clear();
                        txtCD7.Clear();
                    }

                    break;

                case "BSBA":

                    if (txtCC7.Text != "")
                    {
                        connection.Open();
                        comm = new OleDbCommand("Select bsba_coursecode_desc From tbl_BSBACourseDetail Where bsba_coursecode_id = '" + txtCC7.Text + "'", connection);
                        comm.Parameters.AddWithValue("bsba_coursecode_id", txtCC7.Text.ToString());
                        reader = comm.ExecuteReader();
                        while (reader.Read())
                        {
                            txtCD7.Text = reader.GetValue(0).ToString();
                        }
                        connection.Close();

                    }
                    else
                    {
                        txtCC7.Clear();
                        txtCD7.Clear();
                    }

                    break;
            }
        }

        private void txtCC8_TextChanged(object sender, EventArgs e)
        {
            string PROGRAM;

            PROGRAM = txtProgram.Text;

            switch (PROGRAM)
            {
                case "BSIT":
                    //8
                    if (txtCC1.Text != "")
                    {
                        connection.Open();
                        comm = new OleDbCommand("Select bsit_coursecode_desc From tbl_BSITCourseDetail Where bsit_coursecode_id = '" + txtCC8.Text + "'", connection);
                        comm.Parameters.AddWithValue("bsit_coursecode_id", txtCC8.Text.ToString());
                        reader = comm.ExecuteReader();
                        while (reader.Read())
                        {
                            txtCD8.Text = reader.GetValue(0).ToString();
                        }
                        connection.Close();
                    }
                    else
                    {
                        txtCC8.Clear();
                        txtCD8.Clear();
                    }

                    break;

                case "BSBA":

                    if (txtCC8.Text != "")
                    {
                        connection.Open();
                        comm = new OleDbCommand("Select bsba_coursecode_desc From tbl_BSBACourseDetail Where bsba_coursecode_id = '" + txtCC8.Text + "'", connection);
                        comm.Parameters.AddWithValue("bsba_coursecode_id", txtCC8.Text.ToString());
                        reader = comm.ExecuteReader();
                        while (reader.Read())
                        {
                            txtCD8.Text = reader.GetValue(0).ToString();
                        }
                        connection.Close();

                    }
                    else
                    {
                        txtCC8.Clear();
                        txtCD8.Clear();
                    }

                    break;
            }
        }

        private void txtFacUsername1_TextChanged(object sender, EventArgs e)
        {
            if (txtCC1.Text != "" && txtFacUsername1.Text != "")
            {
                conn.Open();
                comm = new OleDbCommand("Select fac_fname, fac_mname, fac_lname From tbl_FacultyDetail Where fac_id = '" + txtFacUsername1.Text + "'", conn);
                comm.Parameters.AddWithValue("fac_id", txtFacUsername1.Text);
                reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    lblfname.Text = reader.GetValue(0).ToString();
                    lblmname.Text = reader.GetValue(1).ToString();
                    lblname.Text = reader.GetValue(2).ToString();
                }
                conn.Close();
            }
            else
            {
                txtFacUsername1.Clear();
                lblfname.Text = "";
                lblmname.Text = "";
                lblname.Text = "";
            }

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void txtFacUsername2_TextChanged(object sender, EventArgs e)
        {
            if (txtCC2.Text != "" && txtFacUsername2.Text != "")
            {
                conn.Open();
                comm = new OleDbCommand("Select fac_fname, fac_mname, fac_lname From tbl_FacultyDetail Where fac_id = '" + txtFacUsername2.Text + "'", conn);
                comm.Parameters.AddWithValue("fac_id", txtFacUsername2.Text);
                reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    lblfname2.Text = reader.GetValue(0).ToString();
                    lblmname2.Text = reader.GetValue(1).ToString();
                    lblname2.Text = reader.GetValue(2).ToString();
                }
                conn.Close();
            }
            else
            {
                txtFacUsername2.Clear();
                lblfname2.Text = "";
                lblmname2.Text = "";
                lblname2.Text = "";
            }
        }

        private void txtFacUsername3_TextChanged(object sender, EventArgs e)
        {
            if (txtCC3.Text != "" && txtFacUsername3.Text != "")
            {
                conn.Open();
                comm = new OleDbCommand("Select fac_fname, fac_mname, fac_lname From tbl_FacultyDetail Where fac_id = '" + txtFacUsername3.Text + "'", conn);
                comm.Parameters.AddWithValue("fac_id", txtFacUsername3.Text);
                reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    lblfname3.Text = reader.GetValue(0).ToString();
                    lblmname3.Text = reader.GetValue(1).ToString();
                    lblname3.Text = reader.GetValue(2).ToString();
                }
                conn.Close();
            }
            else
            {
                txtFacUsername3.Clear();
                lblfname3.Text = "";
                lblmname3.Text = "";
                lblname3.Text = "";
            }
        }

        private void txtFacUsername4_TextChanged(object sender, EventArgs e)
        {
            if (txtCC4.Text != "" && txtFacUsername4.Text != "")
            {
                conn.Open();
                comm = new OleDbCommand("Select fac_fname, fac_mname, fac_lname From tbl_FacultyDetail Where fac_id = '" + txtFacUsername4.Text + "'", conn);
                comm.Parameters.AddWithValue("fac_id", txtFacUsername4.Text);
                reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    lblfname4.Text = reader.GetValue(0).ToString();
                    lblmname4.Text = reader.GetValue(1).ToString();
                    lblname4.Text = reader.GetValue(2).ToString();
                }
                conn.Close();
            }
            else
            {
                txtFacUsername4.Clear();
                lblfname4.Text = "";
                lblmname4.Text = "";
                lblname4.Text = "";
            }
        }

        private void txtFacUsername5_TextChanged(object sender, EventArgs e)
        {
            if (txtCC5.Text != "" && txtFacUsername5.Text != "")
            {
                conn.Open();
                comm = new OleDbCommand("Select fac_fname, fac_mname, fac_lname From tbl_FacultyDetail Where fac_id = '" + txtFacUsername5.Text + "'", conn);
                comm.Parameters.AddWithValue("fac_id", txtFacUsername5.Text);
                reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    lblfname5.Text = reader.GetValue(0).ToString();
                    lblmname5.Text = reader.GetValue(1).ToString();
                    lblname5.Text = reader.GetValue(2).ToString();
                }
                conn.Close();
            }
            else
            {
                txtFacUsername5.Clear();
                lblfname5.Text = "";
                lblmname5.Text = "";
                lblname5.Text = "";
            }
        }

        private void txtFacUsername6_TextChanged(object sender, EventArgs e)
        {
            if (txtCC6.Text != "" && txtFacUsername6.Text != "")
            {
                conn.Open();
                comm = new OleDbCommand("Select fac_fname, fac_mname, fac_lname From tbl_FacultyDetail Where fac_id = '" + txtFacUsername6.Text + "'", conn);
                comm.Parameters.AddWithValue("fac_id", txtFacUsername6.Text);
                reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    lblfname6.Text = reader.GetValue(0).ToString();
                    lblmname6.Text = reader.GetValue(1).ToString();
                    lblname6.Text = reader.GetValue(2).ToString();
                }
                conn.Close();
            }
            else
            {
                txtFacUsername6.Clear();
                lblfname6.Text = "";
                lblmname6.Text = "";
                lblname6.Text = "";
            }
        }

        private void txtFacUsername7_TextChanged(object sender, EventArgs e)
        {
            if (txtCC7.Text != "" && txtFacUsername7.Text != "")
            {
                conn.Open();
                comm = new OleDbCommand("Select fac_fname, fac_mname, fac_lname From tbl_FacultyDetail Where fac_id = '" + txtFacUsername7.Text + "'", conn);
                comm.Parameters.AddWithValue("fac_id", txtFacUsername7.Text);
                reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    lblfname7.Text = reader.GetValue(0).ToString();
                    lblmname7.Text = reader.GetValue(1).ToString();
                    lblname7.Text = reader.GetValue(2).ToString();
                }
                conn.Close();
            }
            else
            {
                txtFacUsername7.Clear();
                lblfname7.Text = "";
                lblmname7.Text = "";
                lblname7.Text = "";
            }
        }

        private void txtFacUsername8_TextChanged(object sender, EventArgs e)
        {
            if (txtCC8.Text != "" && txtFacUsername8.Text != "")
            {
                conn.Open();
                comm = new OleDbCommand("Select fac_fname, fac_mname, fac_lname From tbl_FacultyDetail Where fac_id = '" + txtFacUsername8.Text + "'", conn);
                comm.Parameters.AddWithValue("fac_id", txtFacUsername8.Text);
                reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    lblfname8.Text = reader.GetValue(0).ToString();
                    lblmname8.Text = reader.GetValue(1).ToString();
                    lblname8.Text = reader.GetValue(2).ToString();
                }
                conn.Close();
            }
            else
            {
                txtFacUsername8.Clear();
                lblfname8.Text = "";
                lblmname8.Text = "";
                lblname8.Text = "";
            }
        }

        private void btnSearchFac_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.Filter = "Image Files (*.png)|*.png|(*.jpg)|*.jpg";
                openFileDialog1.ShowDialog();
                pictureBox1.BackgroundImage = Image.FromFile(openFileDialog1.FileName);
            }
            catch (Exception msg)
            {
                MessageBox.Show(msg.Message, "Error I Think", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void cb_Program_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
