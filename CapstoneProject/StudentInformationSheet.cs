using System;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace CapstoneProject
{
    public partial class StudentInformationSheet : Form
    {
        public StudentInformationSheet()
        {
            InitializeComponent();
            ApplyLayout();
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
            txtAge.Text = Services.StudentRecord.AgeOn(dtBDate.Value.Date, dtRegDate.Value.Date).ToString();
        }

        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        // Redirects to RegistrarForm
        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (Modal) { DialogResult=DialogResult.Cancel; Close(); } else { new RegistrarDirectoryForm().Show(); Close(); }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {

        }

        private void label18_Click(object sender, EventArgs e)
        {

        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            btnSubmit.Enabled = false;
            try
            {
                var student = new Services.StudentRecord {
                    Id=txtStudentId.Text.Trim(), FirstName=txtFName.Text.Trim(), MiddleName=txtMName.Text.Trim(), LastName=txtLName.Text.Trim(), Suffix=txtSuffix.Text.Trim(),
                    Program=cbProgram.Text, Gender=cbGender.Text, Nationality=txtNationality.Text.Trim(), YearLevel=txtYearLevel.Text.Trim(), BirthDate=dtBDate.Value.Date, RegistrationDate=dtRegDate.Value.Date,
                    StreetNumber=txtStreetNo.Text.Trim(), Street=txtStreetName.Text.Trim(), Barangay=txtBarangay.Text.Trim(), City=txtCity.Text.Trim(), Province=txtProv.Text.Trim(), Zip=txtZip.Text.Trim(), Phone=txtContactNo.Text.Trim(), Email=txtEmailAddress.Text.Trim(),
                    PrimarySchool=txtPrimarySchool.Text.Trim(), PrimaryYear=txtprimyr.Text.Trim(), SecondarySchool=txtSecondarySchool.Text.Trim(), SecondaryYear=txtsecyr.Text.Trim(),
                    Thesis=cbThesis.Text, GoodMoral=cbGMC.Text, HonorableDismissal=cbHD.Text, Diploma=cb_hsd.Text, BirthCertificate=cbBC.Text, CopyOfGrades=cbCopyOfGrades.Text, Transcript=cbTOR.Text, Form137=cbF137.Text, Form138=cbF138.Text, Ojt=cbOJT.Text
                };
                new Services.StudentRepository().Create(student);
                MessageBox.Show(this,"Student record saved.","Student records",MessageBoxButtons.OK,MessageBoxIcon.Information);
                if (Modal) { DialogResult=DialogResult.OK; Close(); }
            }
            catch (ArgumentException ex) { MessageBox.Show(this,ex.Message,"Check student details",MessageBoxButtons.OK,MessageBoxIcon.Warning); }
            catch (Exception ex) { System.Diagnostics.Trace.TraceError("Student creation: {0}", ex.GetType().Name); MessageBox.Show(this,Infrastructure.Database.FailureMessage,"Record not saved",MessageBoxButtons.OK,MessageBoxIcon.Error); }
            finally { if(!IsDisposed) btnSubmit.Enabled=true; }
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
            MessageBox.Show(this,"Photo storage is not available for this database schema.","Student records");
        }
    }
}
