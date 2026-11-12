using System;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;

namespace CapstoneProject
{
    public partial class RegistrarGradeFormDirectory : Form
    {
        int index;
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
            try {
                var table = new Services.GradeRepository().Search(txtCourseCode.Text.Trim(),txtSY.Text.Trim(),txtFacUser.Text.Trim());
                dataGridView1.DataSource=table;
                foreach(var name in new[]{"course_id","sem","school_year"}) if(dataGridView1.Columns.Contains(name)) dataGridView1.Columns[name].Visible=false;
            } catch(Exception ex) { System.Diagnostics.Trace.TraceError("Grade search: {0}",ex.GetType().Name); MessageBox.Show(this,Infrastructure.Database.FailureMessage,"Grades"); }
        }

        private void btnAddGrade_Click(object sender, EventArgs e)
        {
            SaveSelectedGrade();
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
            SaveSelectedGrade();
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
            try {
                using(var db=Infrastructure.Database.Open("db_GradingManagement.accdb"))
                using(var query=Infrastructure.Database.Command(db,null,"SELECT fac_fname,fac_mname,fac_lname FROM tbl_FacultyDetail WHERE fac_id=?",txtFacUser.Text))
                using(var result=query.ExecuteReader()) { if(result.Read()) { lblfname.Text=Convert.ToString(result[0]); lblmname.Text=Convert.ToString(result[1]); lblname.Text=Convert.ToString(result[2]); } }
            } catch(Exception ex) { System.Diagnostics.Trace.TraceError("Faculty lookup: {0}",ex.GetType().Name); }
        }

        private void RegistrarGradeFormDirectory_Load(object sender, EventArgs e)
        {
            txtStudAG.ReadOnly=true; txtStudCG.ReadOnly=true;
            this.tbl_FacultyDetailTableAdapter.Connection.ConnectionString=Infrastructure.Database.ConnectionString("db_GradingManagement.accdb");
            try { this.tbl_FacultyDetailTableAdapter.Fill(this.db_GradingManagementDataSet1.tbl_FacultyDetail); }
            catch(Exception ex) { System.Diagnostics.Trace.TraceError("Faculty list: {0}",ex.GetType().Name); MessageBox.Show(this,Infrastructure.Database.FailureMessage,"Grades"); }
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
            decimal prelim,midterm,final;
            if (!TryGrades(out prelim,out midterm,out final)) return;
            decimal average=Services.GradeCalculator.Average(prelim,midterm,final);
            txtStudAG.Text=average.ToString("0.00");
            txtStudCG.Text=Services.GradeCalculator.ConvertAverage(average).ToString("0.00");
        }

        private void txtStudCG_TextChanged(object sender, EventArgs e)
        {

        }

        private bool TryGrades(out decimal prelim,out decimal midterm,out decimal final)
        {
            prelim=midterm=final=0;
            if(!decimal.TryParse(txtStudPrelimGrade.Text,out prelim)||!decimal.TryParse(txtStudMidtermGrade.Text,out midterm)||!decimal.TryParse(txtStudFinalsGrade.Text,out final)) { MessageBox.Show(this,"Enter all three numeric grades. Missing grades are not zero.","Check grades"); return false; }
            try { Services.GradeCalculator.Average(prelim,midterm,final); return true; }
            catch(ArgumentOutOfRangeException) { MessageBox.Show(this,"Grades must be from 0 to 100.","Check grades"); return false; }
        }
        private void SaveSelectedGrade()
        {
            var view=dataGridView1.CurrentRow==null?null:dataGridView1.CurrentRow.DataBoundItem as DataRowView;
            if(view==null) { MessageBox.Show(this,"Search and select an enrollment before saving.","Grades"); return; }
            decimal prelim,midterm,final; if(!TryGrades(out prelim,out midterm,out final)) return;
            try {
                new Services.GradeRepository().Save(view.Row,prelim,midterm,final);
                btnSearch_Click(this,EventArgs.Empty);
                btncalcu_Click(this,EventArgs.Empty);
                MessageBox.Show(this,"Grades saved.","Grades");
            } catch(DBConcurrencyException ex) { MessageBox.Show(this,ex.Message,"Record changed"); }
            catch(Exception ex) { System.Diagnostics.Trace.TraceError("Grade save: {0}",ex.GetType().Name); MessageBox.Show(this,Infrastructure.Database.FailureMessage,"Grades not saved"); }
        }
    }
}
