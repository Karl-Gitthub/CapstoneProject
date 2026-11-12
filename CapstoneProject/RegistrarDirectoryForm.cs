using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;
using CapstoneProject.Infrastructure;
using CapstoneProject.Services;
namespace CapstoneProject
{
    public partial class RegistrarDirectoryForm : Form
    {
        private int searchVersion;
        private readonly bool preview;
        public RegistrarDirectoryForm() : this(false) {}
        public RegistrarDirectoryForm(bool preview) { this.preview=preview; InitializeComponent(); Shown += async (s,e) => await SearchAsync(); }
        private async Task SearchAsync()
        {
            int version = ++searchVersion;
            string query = txtSearch.Text; string program = programFilter.SelectedIndex > 0 ? programFilter.Text : "";
            status.Text = "Loading student records…"; searchButton.Enabled = false;
            try
            {
                var table = preview ? PreviewRecords(query,program) : await Task.Run(() => new StudentRepository().Search(query,program));
                if (IsDisposed || version != searchVersion) return;
                dataGridView1.DataSource = table;
                status.Text = table.Rows.Count == 200 ? "Showing the first 200 matches. Refine your search." : table.Rows.Count + " student records";
                SelectStudent();
            }
            catch (Exception ex) { if (!IsDisposed && version == searchVersion) { dataGridView1.DataSource = null; detail.Text = "No record selected"; status.Text = Database.FailureMessage; } System.Diagnostics.Trace.TraceError("Student search: {0}",ex.GetType().Name); }
            finally { if (!IsDisposed && version == searchVersion) searchButton.Enabled = true; }
        }
        private void SelectStudent()
        {
            var view = dataGridView1.CurrentRow == null ? null : dataGridView1.CurrentRow.DataBoundItem as DataRowView;
            detail.Text = view == null ? "No record selected" : Convert.ToString(view["studfname"]) + " " + Convert.ToString(view["studlname"]) + Environment.NewLine + Environment.NewLine + "Student number   " + view["studid"] + Environment.NewLine + "Program              " + view["studprogram"] + Environment.NewLine + "Year level            " + view["yearlevel"];
        }
        private async void AddStudent(object sender, EventArgs e)
        {
            if(preview) return;
            using (var form = new StudentInformationSheet())
                if (form.ShowDialog(this) == DialogResult.OK) await SearchAsync();
        }
        private void OpenLegacy(Form form)
        {
            if(preview) { form.Dispose(); return; }
            form.FormBorderStyle = FormBorderStyle.Sizable;
            using (form) form.ShowDialog(this);
        }

        private static DataTable PreviewRecords(string query,string program)
        {
            var table=new DataTable(); foreach(var name in new[]{"studid","studfname","studmname","studlname","studprogram","yearlevel"})table.Columns.Add(name);
            string[][] rows={new[]{"DEMO-001","Ana","","Santos","BSIT","2"},new[]{"DEMO-002","Miguel","","Reyes","BSIT","3"},new[]{"DEMO-003","Bea","","Cruz","BSBA","1"},new[]{"DEMO-004","Paolo","","Garcia","BSIT","4"},new[]{"DEMO-005","Isabel","","Ramos","BSBA","2"}};
            foreach(var row in rows) if((program==""||row[4]==program)&&string.Join(" ",row).IndexOf(query,StringComparison.OrdinalIgnoreCase)>=0)table.Rows.Add(row);
            return table;
        }
    }
}
