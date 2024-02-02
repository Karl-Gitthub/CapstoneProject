
namespace CapstoneProject
{
    partial class RegistrarGCGDirectoryForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnGenCompGrades = new System.Windows.Forms.Button();
            this.btnGenAList = new System.Windows.Forms.Button();
            this.btnGenCGrades = new System.Windows.Forms.Button();
            this.btnGenTOR = new System.Windows.Forms.Button();
            this.btnAddStudent = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.btnLogout = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnUpdateGrades = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.btnStudPending = new System.Windows.Forms.Button();
            this.DGVTOR = new System.Windows.Forms.DataGridView();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.button10 = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Print = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DGVTOR)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.MidnightBlue;
            this.panel1.Controls.Add(this.btnGenCompGrades);
            this.panel1.Controls.Add(this.btnGenAList);
            this.panel1.Controls.Add(this.btnGenCGrades);
            this.panel1.Controls.Add(this.btnGenTOR);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Controls.Add(this.btnAddStudent);
            this.panel1.Controls.Add(this.button6);
            this.panel1.Controls.Add(this.btnLogout);
            this.panel1.Controls.Add(this.btnAdd);
            this.panel1.Controls.Add(this.btnUpdateGrades);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.btnStudPending);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(156, 701);
            this.panel1.TabIndex = 5;
            // 
            // btnGenCompGrades
            // 
            this.btnGenCompGrades.FlatAppearance.BorderSize = 0;
            this.btnGenCompGrades.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGenCompGrades.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGenCompGrades.ForeColor = System.Drawing.Color.Gold;
            this.btnGenCompGrades.Location = new System.Drawing.Point(24, 518);
            this.btnGenCompGrades.Name = "btnGenCompGrades";
            this.btnGenCompGrades.Size = new System.Drawing.Size(132, 54);
            this.btnGenCompGrades.TabIndex = 19;
            this.btnGenCompGrades.Text = "GENERATE COMPUTED GRADES";
            this.btnGenCompGrades.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnGenCompGrades.UseVisualStyleBackColor = true;
            // 
            // btnGenAList
            // 
            this.btnGenAList.FlatAppearance.BorderSize = 0;
            this.btnGenAList.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGenAList.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGenAList.ForeColor = System.Drawing.Color.Gold;
            this.btnGenAList.Location = new System.Drawing.Point(24, 470);
            this.btnGenAList.Name = "btnGenAList";
            this.btnGenAList.Size = new System.Drawing.Size(132, 42);
            this.btnGenAList.TabIndex = 18;
            this.btnGenAList.Text = "GENERATE ALPHALIST";
            this.btnGenAList.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnGenAList.UseVisualStyleBackColor = true;
            this.btnGenAList.Click += new System.EventHandler(this.btnGenAList_Click);
            // 
            // btnGenCGrades
            // 
            this.btnGenCGrades.BackColor = System.Drawing.Color.Gold;
            this.btnGenCGrades.FlatAppearance.BorderSize = 0;
            this.btnGenCGrades.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGenCGrades.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGenCGrades.ForeColor = System.Drawing.Color.MidnightBlue;
            this.btnGenCGrades.Location = new System.Drawing.Point(24, 420);
            this.btnGenCGrades.Name = "btnGenCGrades";
            this.btnGenCGrades.Size = new System.Drawing.Size(132, 44);
            this.btnGenCGrades.TabIndex = 17;
            this.btnGenCGrades.Text = "GENERATE COPY OF GRADES";
            this.btnGenCGrades.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnGenCGrades.UseVisualStyleBackColor = false;
            this.btnGenCGrades.Click += new System.EventHandler(this.btnGenCGrades_Click);
            // 
            // btnGenTOR
            // 
            this.btnGenTOR.FlatAppearance.BorderSize = 0;
            this.btnGenTOR.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGenTOR.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGenTOR.ForeColor = System.Drawing.Color.Gold;
            this.btnGenTOR.Location = new System.Drawing.Point(24, 384);
            this.btnGenTOR.Name = "btnGenTOR";
            this.btnGenTOR.Size = new System.Drawing.Size(132, 30);
            this.btnGenTOR.TabIndex = 16;
            this.btnGenTOR.Text = "GENERATE TOR";
            this.btnGenTOR.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnGenTOR.UseVisualStyleBackColor = true;
            this.btnGenTOR.Click += new System.EventHandler(this.btnGenTOR_Click);
            // 
            // btnAddStudent
            // 
            this.btnAddStudent.FlatAppearance.BorderSize = 0;
            this.btnAddStudent.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddStudent.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAddStudent.ForeColor = System.Drawing.Color.Gold;
            this.btnAddStudent.Location = new System.Drawing.Point(24, 314);
            this.btnAddStudent.Name = "btnAddStudent";
            this.btnAddStudent.Size = new System.Drawing.Size(132, 38);
            this.btnAddStudent.TabIndex = 14;
            this.btnAddStudent.Text = "ADD A STUDENT";
            this.btnAddStudent.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAddStudent.UseVisualStyleBackColor = true;
            this.btnAddStudent.Click += new System.EventHandler(this.btnAddStudent_Click);
            // 
            // button6
            // 
            this.button6.FlatAppearance.BorderSize = 0;
            this.button6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button6.ForeColor = System.Drawing.Color.Gold;
            this.button6.Location = new System.Drawing.Point(24, 272);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(132, 38);
            this.button6.TabIndex = 13;
            this.button6.Text = "SUBJECT ENLISTMENT";
            this.button6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // btnLogout
            // 
            this.btnLogout.FlatAppearance.BorderSize = 0;
            this.btnLogout.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLogout.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLogout.ForeColor = System.Drawing.Color.Gold;
            this.btnLogout.Location = new System.Drawing.Point(25, 641);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(132, 48);
            this.btnLogout.TabIndex = 7;
            this.btnLogout.Text = "LOGOUT\r\nUSER";
            this.btnLogout.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnLogout.UseVisualStyleBackColor = true;
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.FlatAppearance.BorderSize = 0;
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAdd.ForeColor = System.Drawing.Color.Gold;
            this.btnAdd.Location = new System.Drawing.Point(28, 192);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(129, 29);
            this.btnAdd.TabIndex = 6;
            this.btnAdd.Text = "EVALUATION";
            this.btnAdd.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnUpdateGrades
            // 
            this.btnUpdateGrades.BackColor = System.Drawing.Color.MidnightBlue;
            this.btnUpdateGrades.FlatAppearance.BorderSize = 0;
            this.btnUpdateGrades.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUpdateGrades.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUpdateGrades.ForeColor = System.Drawing.Color.Gold;
            this.btnUpdateGrades.Location = new System.Drawing.Point(25, 227);
            this.btnUpdateGrades.Name = "btnUpdateGrades";
            this.btnUpdateGrades.Size = new System.Drawing.Size(132, 39);
            this.btnUpdateGrades.TabIndex = 4;
            this.btnUpdateGrades.Text = "ADD GRADES";
            this.btnUpdateGrades.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnUpdateGrades.UseVisualStyleBackColor = false;
            this.btnUpdateGrades.Click += new System.EventHandler(this.btnUpdateGrades_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Gold;
            this.label2.Location = new System.Drawing.Point(16, 348);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(134, 18);
            this.label2.TabIndex = 3;
            this.label2.Text = "______________";
            // 
            // btnStudPending
            // 
            this.btnStudPending.FlatAppearance.BorderSize = 0;
            this.btnStudPending.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStudPending.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStudPending.ForeColor = System.Drawing.Color.Gold;
            this.btnStudPending.Location = new System.Drawing.Point(28, 145);
            this.btnStudPending.Name = "btnStudPending";
            this.btnStudPending.Size = new System.Drawing.Size(129, 41);
            this.btnStudPending.TabIndex = 0;
            this.btnStudPending.Text = "STUDENT PENDING LIST";
            this.btnStudPending.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnStudPending.UseVisualStyleBackColor = true;
            this.btnStudPending.Click += new System.EventHandler(this.btnStudPending_Click);
            // 
            // DGVTOR
            // 
            this.DGVTOR.AllowUserToResizeColumns = false;
            this.DGVTOR.AllowUserToResizeRows = false;
            this.DGVTOR.BackgroundColor = System.Drawing.SystemColors.ButtonHighlight;
            this.DGVTOR.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.DGVTOR.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGVTOR.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.Column4,
            this.Column1,
            this.Column2,
            this.Column3,
            this.Column5,
            this.Column6});
            this.DGVTOR.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.DGVTOR.GridColor = System.Drawing.SystemColors.ControlDarkDark;
            this.DGVTOR.Location = new System.Drawing.Point(156, 44);
            this.DGVTOR.Name = "DGVTOR";
            this.DGVTOR.RightToLeft = System.Windows.Forms.RightToLeft.No;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.DGVTOR.RowHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.DGVTOR.RowHeadersVisible = false;
            this.DGVTOR.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            this.DGVTOR.Size = new System.Drawing.Size(805, 657);
            this.DGVTOR.TabIndex = 146;
            // 
            // txtSearch
            // 
            this.txtSearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSearch.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSearch.Location = new System.Drawing.Point(243, 12);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(396, 21);
            this.txtSearch.TabIndex = 148;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            // 
            // button10
            // 
            this.button10.FlatAppearance.BorderSize = 0;
            this.button10.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button10.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button10.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.button10.Location = new System.Drawing.Point(164, 1);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(92, 41);
            this.button10.TabIndex = 147;
            this.button10.Text = "SEARCH :";
            this.button10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button10.UseVisualStyleBackColor = true;
            this.button10.Click += new System.EventHandler(this.button10_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.pictureBox1.Image = global::CapstoneProject.Properties.Resources.spcclogo;
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(131, 121);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 15;
            this.pictureBox1.TabStop = false;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "id";
            this.dataGridViewTextBoxColumn1.MinimumWidth = 6;
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.Visible = false;
            this.dataGridViewTextBoxColumn1.Width = 125;
            // 
            // Column4
            // 
            this.Column4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Column4.HeaderText = "DEPARTMENT";
            this.Column4.Name = "Column4";
            this.Column4.Width = 107;
            // 
            // Column1
            // 
            this.Column1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Column1.HeaderText = "STUDENT NUMBER";
            this.Column1.Name = "Column1";
            this.Column1.Width = 123;
            // 
            // Column2
            // 
            this.Column2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column2.HeaderText = "STUDENT NAME";
            this.Column2.Name = "Column2";
            // 
            // Column3
            // 
            this.Column3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Column3.HeaderText = "SEMESTER";
            this.Column3.Name = "Column3";
            this.Column3.Width = 91;
            // 
            // Column5
            // 
            this.Column5.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Column5.HeaderText = "COURSE CODE";
            this.Column5.Name = "Column5";
            this.Column5.Width = 101;
            // 
            // Column6
            // 
            this.Column6.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Column6.HeaderText = "GRADE";
            this.Column6.Name = "Column6";
            this.Column6.Width = 70;
            // 
            // Print
            // 
            this.Print.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Print.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Print.Location = new System.Drawing.Point(762, 6);
            this.Print.Name = "Print";
            this.Print.Size = new System.Drawing.Size(187, 32);
            this.Print.TabIndex = 429;
            this.Print.Text = "PRINT COPY OF GRADES";
            this.Print.UseVisualStyleBackColor = true;
            // 
            // RegistrarGCGDirectoryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.ClientSize = new System.Drawing.Size(961, 701);
            this.Controls.Add(this.Print);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.button10);
            this.Controls.Add(this.DGVTOR);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "RegistrarGCGDirectoryForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RegistrarGCGDirectoryForm";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DGVTOR)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        public System.Windows.Forms.Button btnGenCompGrades;
        public System.Windows.Forms.Button btnGenAList;
        public System.Windows.Forms.Button btnGenCGrades;
        public System.Windows.Forms.Button btnGenTOR;
        private System.Windows.Forms.PictureBox pictureBox1;
        public System.Windows.Forms.Button btnAddStudent;
        public System.Windows.Forms.Button button6;
        public System.Windows.Forms.Button btnLogout;
        public System.Windows.Forms.Button btnAdd;
        public System.Windows.Forms.Button btnUpdateGrades;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.Button btnStudPending;
        public System.Windows.Forms.DataGridView DGVTOR;
        private System.Windows.Forms.TextBox txtSearch;
        public System.Windows.Forms.Button button10;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column6;
        private System.Windows.Forms.Button Print;
    }
}