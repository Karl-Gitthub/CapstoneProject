using System;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace CapstoneProject
{
    public partial class StudentInformationSheet : Form
    {
        //OleDbConnection conn = new OleDbConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Admin-PC\source\repos\CapstoneProject\CapstoneProject\db_GradingManagement.mdf;Integrated Security=True");
        OleDbConnection connection = new OleDbConnection(@"Provider = Microsoft.ACE.OLEDB.12.0; Data Source = C:\Users\Karl\OneDrive\Documents\Capstone\CapstoneProject\New folder\dbStudentManagement.accdb");
        OleDbCommand comm;
        OleDbDataReader reader;
        public StudentInformationSheet()
        {
            InitializeComponent();
        }



        private void label28_Click(object sender, EventArgs e)
        {

        }

        private void label52_Click(object sender, EventArgs e)
        {

        }

        private void RegistrationDirectoryForm_Load(object sender, EventArgs e)
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

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void label19_Click(object sender, EventArgs e)
        {

        }

        private void label87_Click(object sender, EventArgs e)
        {

        }

        private void label83_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label35_Click(object sender, EventArgs e)
        {

        }

        private void label37_Click(object sender, EventArgs e)
        {

        }

        private void label70_Click(object sender, EventArgs e)
        {

        }

        private void label24_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label52_Click_1(object sender, EventArgs e)
        {

        }

        private void label31_Click(object sender, EventArgs e)
        {

        }

        private void label20_Click(object sender, EventArgs e)
        {

        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {

        }

        private void label41_Click(object sender, EventArgs e)
        {

        }

        private void label40_Click(object sender, EventArgs e)
        {

        }

        private void textBox21_TextChanged(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dtBDate_ValueChanged(object sender, EventArgs e)
        {
            DateTime time_start = Convert.ToDateTime(dtBDate.Value);
            DateTime time_end = DateTime.Today;
            TimeSpan span = time_end.Subtract(time_start);
            var daysTotal = span.TotalDays;
            var yearsTotal = Math.Truncate(daysTotal / 365);
            txtAge.Text = Convert.ToString(yearsTotal);
        }

        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        // Redirects to RegistrarForm
        private void btnCancel_Click(object sender, EventArgs e)
        {
            RegistrarDirectoryForm nf = new RegistrarDirectoryForm();
            nf.Show();
            this.Hide();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {

        }

        private void label18_Click(object sender, EventArgs e)
        {

        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show("Student Information Submitted");
                //IMAGE //This uses Using.System.IO since it fetches a photo "Jpeg" and turns it into byte
                MemoryStream MS = new MemoryStream();
                pfpStudent.BackgroundImage.Save(MS, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] arrImage = MS.GetBuffer();

                //STUDENT DETAILS
                connection.Open();
                comm = new OleDbCommand("Insert into tbl_BSITStud_Detail(studid, studfname, studmname, studlname, studsuffix, studdatereg, studbdate, studage, studnationality, studgender, studprogram) values(@studid, @studfname, @studmname, @studlname, @studsuffix, @studdatereg, @studbdate, @studage, @studnationality, @studgender, @studprogram)", connection);
                comm.Parameters.AddWithValue("@studid", txtStudentId.Text);
                comm.Parameters.AddWithValue("@studfname", txtFName.Text);
                comm.Parameters.AddWithValue("@studmname", txtMName.Text);
                comm.Parameters.AddWithValue("@studlname", txtLName.Text);
                comm.Parameters.AddWithValue("@studsuffix", txtSuffix.Text);
                comm.Parameters.AddWithValue("@studdatereg", dtRegDate.Value.Date);
                comm.Parameters.AddWithValue("@studbdate", dtBDate.Value.Date);
                comm.Parameters.AddWithValue("@studage", int.Parse(txtAge.Text));
                comm.Parameters.AddWithValue("@studnationality", txtNationality.Text);
                comm.Parameters.AddWithValue("@studgender", cbGender.SelectedItem);
                comm.Parameters.AddWithValue("@studprogram", cbProgram.SelectedItem);

                comm = new OleDbCommand();
                comm.Parameters.AddWithValue("@pfpStudent", arrImage);
                comm.ExecuteNonQuery();
                comm.Parameters.Clear();
                connection.Close();

                //STUDENT ADDRESS
                connection.Open();
                comm = new OleDbCommand("Insert into tbl_BSITStud_Address(studid, studaddnum, studaddstreet, studaddbrgy, studaddcity, studaddprovince, studaddzip) values(@bsit_studid, @studaddnum, @studaddstreet, @studaddbrgy, @studaddcity,@studaddprovince, @studaddzip)", connection);
                comm.Parameters.AddWithValue("@studid", txtStudentId.Text);
                comm.Parameters.AddWithValue("@studaddnum", txtStreetNo.Text);
                comm.Parameters.AddWithValue("@studaddstreet", txtStreetName.Text);
                comm.Parameters.AddWithValue("@studaddbrgy", txtBarangay.Text);
                comm.Parameters.AddWithValue("@studaddcity", txtCity.Text);
                comm.Parameters.AddWithValue("@studaddprovince", txtProv.Text);
                comm.Parameters.AddWithValue("@studaddzip", txtZip.Text);
                comm.ExecuteNonQuery();
                comm.Parameters.Clear();
                connection.Close();

                //STUDENT CONTACT DETAIL
                connection.Open();
                comm = new OleDbCommand("Insert into tbl_BSITStud_ContactDetail(studid, studcontactnum, studcontactadd) values(@studid, @studcontactnum, @studcontactadd)", connection);
                comm.Parameters.AddWithValue("@studid", txtStudentId.Text);
                comm.Parameters.AddWithValue("@studcontactnum", txtContactNo.Text);
                comm.Parameters.AddWithValue("@studcontactadd", txtEmailAddress.Text);
                comm.ExecuteNonQuery();
                comm.Parameters.Clear();
                connection.Close();

                //STUDENT SCHOOLBACKGROUND 
                connection.Open();
                comm = new OleDbCommand("Insert into tbl_BSITStud_SchoolBackground(studid, studprevprim_schoolname, studprev_sy, studprevsec_schoolname, studprevsec_sy) values(@studid,  @studprevprim_schoolname, @studprev_sy, @studprevsec_schoolname, @studprevsec_sy)", connection);
                comm.Parameters.AddWithValue("@studid", txtStudentId.Text);
                comm.Parameters.AddWithValue("@studprevprim_schoolname", txtPrimarySchool.Text);
                comm.Parameters.AddWithValue("@studprev_sy", txtprimyr.Text);
                comm.Parameters.AddWithValue("@studprevsec_schoolname", txtSecondarySchool.Text);
                comm.Parameters.AddWithValue("@studprevsec_sy", txtsecyr.Text);
                comm.ExecuteNonQuery();
                comm.Parameters.Clear();
                connection.Close();

                //STUDENT CREDENTIALS
                /*connection.Open();
                comm = new OleDbCommand("Insert into tbl_BSITStud_Credentials(bsit_studid, bsit_studcredcert, bsitstudcredgm, bsit_studcredhs, bsit_studcreddiploma, bsit_studcredtor, bsit_studcredf137, bsit_studcredf138) values(@bsit_studid, @bsit_studcredcert, @bsitstudcredgm, @bsit_studcredhs, @bsit_studcreddiploma, @bsit_studcredtor, @bsit_studcredf137, @bsit_studcredf138)", connection);
                comm.Parameters.AddWithValue("@bsit_studid", txtStudentId.Text);*/
                connection.Open();
                comm = new OleDbCommand("Insert into tbl_BSITStud_Credentials(studid, studthesis, studcredgm, studcredhs, studcreddiploma, birthcert, copyofgrades, studcredtor, studcredf137, studcredf138) values(@studid, @studcredcert, @studcredgm, @studcredhs, @studcreddiploma, @birthcert, @copyofgrades, @studcredtor, @studcredf137, @studcredf138)", connection);
                comm.Parameters.AddWithValue("@studid", txtStudentId.Text);
                comm.Parameters.AddWithValue("@studthesis", cbThesis.SelectedItem);
                comm.Parameters.AddWithValue("@studcredgm", cbGMC.SelectedItem);
                comm.Parameters.AddWithValue("@studcredhd", cbHD.SelectedItem);
                comm.Parameters.AddWithValue("@studcreddiploma", cb_hsd.SelectedItem);
                comm.Parameters.AddWithValue("@birthcert", cbBC.SelectedItem);
                comm.Parameters.AddWithValue("@copyofgrades", cbCopyOfGrades.SelectedItem);
                comm.Parameters.AddWithValue("@studcredtor", cbTOR.SelectedItem);
                comm.Parameters.AddWithValue("@studcredf137", cbF137.SelectedItem);
                comm.Parameters.AddWithValue("@studcredf138", cbF138.SelectedItem);
                comm.Parameters.AddWithValue("@studcertojt", cbOJT.SelectedItem);
                comm.ExecuteNonQuery();
                comm.Parameters.Clear();
                connection.Close();

                //STUDENT COURSE REMARKS
                /*connection.Open();
                comm = new OleDbCommand("Insert into tbl_BSITStud_SchoolBackground(bsit_studid, ge1, ge2, ge3, fil1, pe1, it101, prog1, nstp1, ge4, ge5, ge6, fil2, prog2, dsa, pe2, nstp2) values(@bsit_studid, @ge1, @ge2, @ge3, @fil1, @pe1, @it101, @prog1, @nstp1, @ge4, @ge5, @ge6, @fil2, @prog2, @dsa, @pe2, @nstp2)", connection);
                comm.Parameters.AddWithValue("@bsit_studid", txtStudentId.Text);
                comm.Parameters.AddWithValue("@ge1", cb1sem1.SelectedItem);
                comm.Parameters.AddWithValue("@ge2", cb2sem1.SelectedItem);
                comm.Parameters.AddWithValue("@ge3", cb3sem1.SelectedItem);
                comm.Parameters.AddWithValue("@fil1", cb4sem1.SelectedItem);
                comm.Parameters.AddWithValue("@pe1", cb5sem1.SelectedItem);
                comm.Parameters.AddWithValue("@it101", cb6sem1.SelectedItem);
                comm.Parameters.AddWithValue("@prog1", cb7sem1.SelectedItem);
                comm.Parameters.AddWithValue("@nstp1", cb8sem1.SelectedItem);
                comm.Parameters.AddWithValue("@ge4", cb1sem2.SelectedItem);
                comm.Parameters.AddWithValue("@ge5", cb2sem2.SelectedItem);
                comm.Parameters.AddWithValue("@ge6", cb3sem2.SelectedItem);
                comm.Parameters.AddWithValue("@fil2", cb4sem2.SelectedItem);
                comm.Parameters.AddWithValue("@prog2", cb5sem2.SelectedItem);
                comm.Parameters.AddWithValue("@dsa", cb6sem2.SelectedItem);
                comm.Parameters.AddWithValue("@pe2", cb7sem2.SelectedItem);
                comm.Parameters.AddWithValue("@nstp2", cb8sem2.SelectedItem);
                comm.ExecuteNonQuery();
                comm.Parameters.Clear();
                connection.Close();*/






            }
            catch (Exception)
            {
                Console.WriteLine("Something went wrong.");
            }
        }

        private void cbGender_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void dtRegDate_ValueChanged(object sender, EventArgs e)
        {

        }

        private void label36_Click(object sender, EventArgs e)
        {

        }

        private void label36_Click_1(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            //Uses open file dialog to get a photo file from a certain folder with selected file types
            try
            {
                openFileDialog1.Filter = "Image Files (*.png)|*.png|(*.jpg)|*.jpg";
                openFileDialog1.ShowDialog();
                pfpStudent.BackgroundImage = Image.FromFile(openFileDialog1.FileName);
            }
            catch (Exception msg)
            {
                MessageBox.Show(msg.Message, "Error I Think", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
