using System.Drawing;
using System.Windows.Forms;
using CapstoneProject.Presentation;
namespace CapstoneProject
{
    partial class RegistrarDirectoryForm
    {
        private System.ComponentModel.IContainer components;
        private TextBox txtSearch;
        private ComboBox programFilter;
        private DataGridView dataGridView1;
        private Label status, detail;
        private Button searchButton;
        protected override void Dispose(bool disposing) { if(disposing && components != null) components.Dispose(); base.Dispose(disposing); }
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container(); SuspendLayout();
            AutoScaleDimensions = new SizeF(96,96); AutoScaleMode = AutoScaleMode.Dpi;
            Font = new Font("Segoe UI",10); BackColor = Theme.Surface;
            Text = preview ? "Design preview · Synthetic student records" : "Students · Student Records"; ClientSize = new Size(1160,720); MinimumSize = new Size(980,640); StartPosition = FormStartPosition.CenterScreen;
            var shell = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, Padding = Padding.Empty, Margin = Padding.Empty };
            shell.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute,208)); shell.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,100)); shell.RowStyles.Add(new RowStyle(SizeType.Percent,100));
            var sidebar = new TableLayoutPanel { Dock = DockStyle.Fill, BackColor = Theme.Navy, Padding = new Padding(24,32,16,24), Margin = Padding.Empty, ColumnCount = 1, RowCount = 8 };
            var brand = Theme.Label("SYSTEMS PLUS\nCOMPUTER COLLEGE",11,true); brand.ForeColor = Color.White; brand.Margin = new Padding(0,0,0,40); sidebar.Controls.Add(brand,0,0);
            var desk = Theme.Label("REGISTRAR",9); desk.ForeColor = Color.FromArgb(188,205,220); sidebar.Controls.Add(desk,0,1);
            string[] names = { "Students", "Enrollment", "Grades", "Reports" };
            for(int i=0;i<names.Length;i++) { int index=i; var button=Theme.Button(names[i]); button.Dock=DockStyle.Top; button.TextAlign=ContentAlignment.MiddleLeft; button.BackColor=i==0?Color.FromArgb(48,75,105):Theme.Navy; button.ForeColor=Color.White; button.FlatAppearance.BorderSize=0; button.Margin=new Padding(0,4,0,4); if(i>0) button.Enabled=!preview; if(i>0) button.Click+=(s,e)=> { if(index==1) OpenLegacy(new RegFormDirectoryRegistrar()); else if(index==2) OpenLegacy(new RegistrarGradeFormDirectory()); else OpenLegacy(new RegistrarGCGDirectoryForm()); }; sidebar.Controls.Add(button,0,i+2); }
            sidebar.RowStyles.Clear(); for(int i=0;i<6;i++)sidebar.RowStyles.Add(new RowStyle(SizeType.AutoSize)); sidebar.RowStyles.Add(new RowStyle(SizeType.Percent,100)); sidebar.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            var logout=Theme.Button("Sign out"); logout.ForeColor=Color.White; logout.BackColor=Theme.Navy; logout.FlatAppearance.BorderColor=Color.FromArgb(90,113,138); logout.Click+=(s,e)=>Close(); sidebar.Controls.Add(logout,0,7);
            var main = new TableLayoutPanel { Dock=DockStyle.Fill, Padding=new Padding(32), ColumnCount=1, RowCount=4, Margin=Padding.Empty };
            main.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,100)); main.RowStyles.Add(new RowStyle(SizeType.AutoSize)); main.RowStyles.Add(new RowStyle(SizeType.AutoSize)); main.RowStyles.Add(new RowStyle(SizeType.Percent,100)); main.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            var header=new TableLayoutPanel { Dock=DockStyle.Top, AutoSize=true, ColumnCount=2, Margin=new Padding(0,0,0,24) }; header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,100)); header.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            var heading=Theme.Stack(); heading.Controls.Add(Theme.Label("Students",24,true)); var subtitle=Theme.Label("Find a student and review their academic record."); subtitle.ForeColor=Theme.Muted; heading.Controls.Add(subtitle); header.Controls.Add(heading,0,0);
            var add=Theme.Button("&Add student",true); add.Enabled=!preview; add.Click+=AddStudent; header.Controls.Add(add,1,0);
            var filters=new TableLayoutPanel { Dock=DockStyle.Top, AutoSize=true, ColumnCount=3, Margin=new Padding(0,0,0,20) }; filters.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,100)); filters.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute,136)); filters.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            txtSearch=new TextBox { Name="txtSearch", Dock=DockStyle.Fill, AccessibleName="Student number or name", Margin=new Padding(0,5,12,5) }; txtSearch.KeyDown+=async(s,e)=> { if(e.KeyCode==Keys.Enter) { e.SuppressKeyPress=true; await SearchAsync(); } };
            programFilter=new ComboBox { Name="programFilter", DropDownStyle=ComboBoxStyle.DropDownList, Dock=DockStyle.Fill, AccessibleName="Program", Margin=new Padding(0,4,12,4) }; programFilter.Items.AddRange(new object[]{"All programs","BSIT","BSBA"}); programFilter.SelectedIndex=0;
            searchButton=Theme.Button("&Search"); searchButton.Click+=async(s,e)=>await SearchAsync(); programFilter.SelectedIndexChanged+=async(s,e)=>await SearchAsync();
            filters.Controls.Add(txtSearch,0,0); filters.Controls.Add(programFilter,1,0); filters.Controls.Add(searchButton,2,0);
            var records=new TableLayoutPanel { Dock=DockStyle.Fill, ColumnCount=1, RowCount=2, Margin=Padding.Empty }; records.RowStyles.Add(new RowStyle(SizeType.Percent,100)); records.RowStyles.Add(new RowStyle(SizeType.Absolute,154)); records.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,100));
            dataGridView1=new DataGridView { Name="studentGrid", AccessibleName="Student records", Dock=DockStyle.Fill, ReadOnly=true, AllowUserToAddRows=false, AllowUserToDeleteRows=false, MultiSelect=false, SelectionMode=DataGridViewSelectionMode.FullRowSelect, RowHeadersVisible=false, AutoGenerateColumns=false, BackgroundColor=Color.White, BorderStyle=BorderStyle.FixedSingle, AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill, EnableHeadersVisualStyles=false, Margin=Padding.Empty };
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor=Color.FromArgb(232,237,242); dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor=Theme.Ink; dataGridView1.ColumnHeadersHeight=40; dataGridView1.RowTemplate.Height=36;
            dataGridView1.DefaultCellStyle.SelectionBackColor=Color.FromArgb(224,234,245); dataGridView1.DefaultCellStyle.SelectionForeColor=Theme.Ink;
            string[] columns={"studid","studfname","studlname","studprogram","yearlevel"}; string[] titles={"Student number","First name","Last name","Program","Year level"};
            for(int i=0;i<columns.Length;i++) dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName=columns[i], HeaderText=titles[i], Name=columns[i] });
            dataGridView1.SelectionChanged+=(s,e)=>SelectStudent();
            detail=Theme.Label("No record selected",10); detail.Dock=DockStyle.Fill; detail.AutoSize=false; detail.BackColor=Color.White; detail.Padding=new Padding(20,16,20,16); detail.Margin=new Padding(0,12,0,0); detail.AccessibleName="Selected student details";
            records.Controls.Add(dataGridView1,0,0); records.Controls.Add(detail,0,1);
            status=Theme.Label("",9); status.Name="status"; status.Dock=DockStyle.Top; status.Margin=new Padding(0,12,0,0); status.ForeColor=Theme.Muted;
            main.Controls.Add(header,0,0); main.Controls.Add(filters,0,1); main.Controls.Add(records,0,2); main.Controls.Add(status,0,3);
            shell.Controls.Add(sidebar,0,0); shell.Controls.Add(main,1,0); Controls.Add(shell); ResumeLayout(true);
        }
    }
}
