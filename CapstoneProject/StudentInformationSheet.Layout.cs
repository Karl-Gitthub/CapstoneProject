using System.Drawing;
using System.Windows.Forms;
using CapstoneProject.Presentation;

namespace CapstoneProject
{
    public partial class StudentInformationSheet
    {
        // Reuse the existing field controls and their public contracts with legacy forms.
        private void ApplyLayout()
        {
            SuspendLayout();
            Controls.Clear();
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoScaleDimensions = new SizeF(96,96);
            Font = new Font("Segoe UI",10);
            FormBorderStyle = FormBorderStyle.Sizable;
            Text = "Add student · Student Records";
            ClientSize = new Size(800,700); MinimumSize = new Size(740,580);
            BackColor = Color.White;
            var shell = new TableLayoutPanel { Dock=DockStyle.Fill, Padding=new Padding(24), ColumnCount=1, RowCount=3 };
            shell.RowStyles.Add(new RowStyle(SizeType.AutoSize)); shell.RowStyles.Add(new RowStyle(SizeType.Percent,100)); shell.RowStyles.Add(new RowStyle(SizeType.AutoSize)); shell.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,100));
            shell.Controls.Add(Theme.Label("Add student",22,true),0,0);
            var tabs = new TabControl { Dock=DockStyle.Fill, Margin=new Padding(0,16,0,20) };
            AddSection(tabs,"Identity",new string[]{"Student number *","First name *","Middle name","Last name *","Suffix","Program *","Year level","Birth date","Registration date","Gender","Nationality"},new Control[]{txtStudentId,txtFName,txtMName,txtLName,txtSuffix,cbProgram,txtYearLevel,dtBDate,dtRegDate,cbGender,txtNationality});
            AddSection(tabs,"Contact & address",new string[]{"Phone","Email","Street number","Street","Barangay","City","Province","Postal code"},new Control[]{txtContactNo,txtEmailAddress,txtStreetNo,txtStreetName,txtBarangay,txtCity,txtProv,txtZip});
            AddSection(tabs,"Prior education",new string[]{"Primary school","School year","Secondary school","School year"},new Control[]{txtPrimarySchool,txtprimyr,txtSecondarySchool,txtsecyr});
            AddSection(tabs,"Credentials",new string[]{"Thesis","Good moral certificate","Honorable dismissal","High school diploma","Birth certificate","Copy of grades","Transcript","Form 137","Form 138","OJT certificate"},new Control[]{cbThesis,cbGMC,cbHD,cb_hsd,cbBC,cbCopyOfGrades,cbTOR,cbF137,cbF138,cbOJT});
            shell.Controls.Add(tabs,0,1);
            var actions=new FlowLayoutPanel { Dock=DockStyle.Fill, AutoSize=true, FlowDirection=FlowDirection.RightToLeft, WrapContents=false };
            btnSubmit.Text="&Save student"; btnSubmit.AutoSize=true; btnSubmit.MinimumSize=new Size(140,38); btnSubmit.BackColor=Theme.Navy; btnSubmit.ForeColor=Color.White; btnSubmit.FlatStyle=FlatStyle.Flat; btnSubmit.Margin=new Padding(12,0,0,0);
            btnCancel.Text="Cancel"; btnCancel.AutoSize=true; btnCancel.MinimumSize=new Size(100,38); btnCancel.ForeColor=Theme.Ink; btnCancel.BackColor=Color.White; btnCancel.FlatStyle=FlatStyle.Flat;
            actions.Controls.Add(btnSubmit); actions.Controls.Add(btnCancel); shell.Controls.Add(actions,0,2); Controls.Add(shell);
            cbProgram.DropDownStyle=ComboBoxStyle.DropDownList; cbGender.DropDownStyle=ComboBoxStyle.DropDownList;
            foreach(var combo in new[]{cbThesis,cbGMC,cbHD,cb_hsd,cbBC,cbCopyOfGrades,cbTOR,cbF137,cbF138,cbOJT}) combo.DropDownStyle=ComboBoxStyle.DropDownList;
            AcceptButton=btnSubmit; CancelButton=btnCancel;
            ResumeLayout(true);
        }
        private static void AddSection(TabControl tabs,string title,string[] labels,Control[] fields)
        {
            var page=new TabPage(title) { Padding=new Padding(16), AutoScroll=true, BackColor=Color.White };
            var table=new TableLayoutPanel { Dock=DockStyle.Top, AutoSize=true, ColumnCount=2, Padding=new Padding(0,0,16,0) };
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute,190)); table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,100));
            for(int i=0;i<fields.Length;i++)
            {
                var label=Theme.Label(labels[i]); label.Margin=new Padding(0,6,16,12); table.Controls.Add(label,0,i);
                var field=fields[i]; field.Font=new Font("Segoe UI",10); field.ForeColor=Theme.Ink; field.Enabled=true; field.Dock=DockStyle.Top; field.Margin=new Padding(0,0,0,12); field.TabIndex=i; field.AccessibleName=labels[i]; field.MinimumSize=Size.Empty;
                table.Controls.Add(field,1,i);
            }
            page.Controls.Add(table); tabs.TabPages.Add(page);
        }
    }
}
