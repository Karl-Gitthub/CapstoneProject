
namespace CapstoneProject
{
    partial class AlphaListDirectoryForm
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
            this.DGWBSBA = new System.Windows.Forms.DataGridView();
            this.panel4 = new System.Windows.Forms.Panel();
            this.button8 = new System.Windows.Forms.Button();
            this.btnRegistrarCancel = new System.Windows.Forms.Button();
            this.txtSY = new System.Windows.Forms.TextBox();
            this.txtSem = new System.Windows.Forms.TextBox();
            this.button18 = new System.Windows.Forms.Button();
            this.button16 = new System.Windows.Forms.Button();
            this.btnSY = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnPrint = new System.Windows.Forms.Button();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnBSBACancel = new System.Windows.Forms.Button();
            this.btnBSITCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.DGWBSBA)).BeginInit();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // DGWBSBA
            // 
            this.DGWBSBA.AllowUserToResizeColumns = false;
            this.DGWBSBA.AllowUserToResizeRows = false;
            this.DGWBSBA.BackgroundColor = System.Drawing.SystemColors.ButtonHighlight;
            this.DGWBSBA.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.DGWBSBA.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGWBSBA.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column3,
            this.Column7,
            this.Column8,
            this.Column2,
            this.Column4,
            this.Column5,
            this.Column6});
            this.DGWBSBA.GridColor = System.Drawing.SystemColors.ControlDarkDark;
            this.DGWBSBA.Location = new System.Drawing.Point(3, 0);
            this.DGWBSBA.Name = "DGWBSBA";
            this.DGWBSBA.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.DGWBSBA.RowHeadersVisible = false;
            this.DGWBSBA.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            this.DGWBSBA.Size = new System.Drawing.Size(863, 704);
            this.DGWBSBA.TabIndex = 4;
            this.DGWBSBA.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DGWBSBA_CellContentClick);
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel4.Controls.Add(this.btnBSBACancel);
            this.panel4.Controls.Add(this.btnClear);
            this.panel4.Controls.Add(this.btnBSITCancel);
            this.panel4.Controls.Add(this.btnPrint);
            this.panel4.Controls.Add(this.pictureBox1);
            this.panel4.Controls.Add(this.button8);
            this.panel4.Controls.Add(this.btnRegistrarCancel);
            this.panel4.Controls.Add(this.txtSY);
            this.panel4.Controls.Add(this.txtSem);
            this.panel4.Controls.Add(this.button18);
            this.panel4.Controls.Add(this.button16);
            this.panel4.Controls.Add(this.btnSY);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel4.Location = new System.Drawing.Point(868, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(198, 704);
            this.panel4.TabIndex = 143;
            // 
            // button8
            // 
            this.button8.BackColor = System.Drawing.Color.Transparent;
            this.button8.FlatAppearance.BorderSize = 0;
            this.button8.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button8.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.button8.Location = new System.Drawing.Point(42, 453);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(107, 26);
            this.button8.TabIndex = 164;
            this.button8.Text = "___________________________________________________________________________";
            this.button8.UseVisualStyleBackColor = false;
            // 
            // btnRegistrarCancel
            // 
            this.btnRegistrarCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRegistrarCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRegistrarCancel.Location = new System.Drawing.Point(15, 648);
            this.btnRegistrarCancel.Name = "btnRegistrarCancel";
            this.btnRegistrarCancel.Size = new System.Drawing.Size(167, 44);
            this.btnRegistrarCancel.TabIndex = 142;
            this.btnRegistrarCancel.Text = "GO BACK";
            this.btnRegistrarCancel.UseVisualStyleBackColor = true;
            this.btnRegistrarCancel.Click += new System.EventHandler(this.btnRegistrarCancel_Click);
            // 
            // txtSY
            // 
            this.txtSY.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSY.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtSY.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSY.Location = new System.Drawing.Point(14, 304);
            this.txtSY.Name = "txtSY";
            this.txtSY.Size = new System.Drawing.Size(168, 21);
            this.txtSY.TabIndex = 142;
            // 
            // txtSem
            // 
            this.txtSem.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSem.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtSem.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSem.Location = new System.Drawing.Point(14, 352);
            this.txtSem.Name = "txtSem";
            this.txtSem.Size = new System.Drawing.Size(168, 21);
            this.txtSem.TabIndex = 151;
            // 
            // button18
            // 
            this.button18.BackColor = System.Drawing.Color.Transparent;
            this.button18.FlatAppearance.BorderSize = 0;
            this.button18.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button18.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button18.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.button18.Location = new System.Drawing.Point(15, 370);
            this.button18.Name = "button18";
            this.button18.Size = new System.Drawing.Size(167, 21);
            this.button18.TabIndex = 152;
            this.button18.Text = "SEMESTER";
            this.button18.UseVisualStyleBackColor = false;
            // 
            // button16
            // 
            this.button16.BackColor = System.Drawing.Color.Transparent;
            this.button16.FlatAppearance.BorderSize = 0;
            this.button16.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button16.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button16.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.button16.Location = new System.Drawing.Point(3, 249);
            this.button16.Name = "button16";
            this.button16.Size = new System.Drawing.Size(187, 49);
            this.button16.TabIndex = 150;
            this.button16.Text = "INPUT DATA\r\nNEEDED BELOW :";
            this.button16.UseVisualStyleBackColor = false;
            // 
            // btnSY
            // 
            this.btnSY.BackColor = System.Drawing.Color.Transparent;
            this.btnSY.FlatAppearance.BorderSize = 0;
            this.btnSY.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSY.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSY.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnSY.Location = new System.Drawing.Point(15, 322);
            this.btnSY.Name = "btnSY";
            this.btnSY.Size = new System.Drawing.Size(167, 21);
            this.btnSY.TabIndex = 150;
            this.btnSY.Text = "SCHOOL YEAR";
            this.btnSY.UseVisualStyleBackColor = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.pictureBox1.Image = global::CapstoneProject.Properties.Resources.spcclogo;
            this.pictureBox1.Location = new System.Drawing.Point(29, 11);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(131, 121);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 144;
            this.pictureBox1.TabStop = false;
            // 
            // btnPrint
            // 
            this.btnPrint.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPrint.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPrint.Location = new System.Drawing.Point(14, 548);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(167, 44);
            this.btnPrint.TabIndex = 165;
            this.btnPrint.Text = "PRINT";
            this.btnPrint.UseVisualStyleBackColor = true;
            // 
            // Column1
            // 
            this.Column1.HeaderText = "id";
            this.Column1.MinimumWidth = 6;
            this.Column1.Name = "Column1";
            this.Column1.Visible = false;
            this.Column1.Width = 125;
            // 
            // Column3
            // 
            this.Column3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Column3.HeaderText = "STUDENT PROGRAM";
            this.Column3.MinimumWidth = 6;
            this.Column3.Name = "Column3";
            this.Column3.Width = 130;
            // 
            // Column7
            // 
            this.Column7.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Column7.HeaderText = "SCHOOLYEAR";
            this.Column7.Name = "Column7";
            this.Column7.Width = 105;
            // 
            // Column8
            // 
            this.Column8.HeaderText = "SEMESTER";
            this.Column8.Name = "Column8";
            // 
            // Column2
            // 
            this.Column2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Column2.HeaderText = "STUDENT ID";
            this.Column2.MinimumWidth = 6;
            this.Column2.Name = "Column2";
            this.Column2.Width = 90;
            // 
            // Column4
            // 
            this.Column4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column4.HeaderText = "LASTNAME";
            this.Column4.MinimumWidth = 6;
            this.Column4.Name = "Column4";
            // 
            // Column5
            // 
            this.Column5.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column5.HeaderText = "FIRST NAME";
            this.Column5.MinimumWidth = 6;
            this.Column5.Name = "Column5";
            // 
            // Column6
            // 
            this.Column6.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column6.HeaderText = "MIDDLE NAME";
            this.Column6.MinimumWidth = 6;
            this.Column6.Name = "Column6";
            // 
            // btnClear
            // 
            this.btnClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClear.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClear.Location = new System.Drawing.Point(15, 598);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(167, 44);
            this.btnClear.TabIndex = 166;
            this.btnClear.Text = "CLEAR";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnBSBACancel
            // 
            this.btnBSBACancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBSBACancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBSBACancel.Location = new System.Drawing.Point(15, 648);
            this.btnBSBACancel.Name = "btnBSBACancel";
            this.btnBSBACancel.Size = new System.Drawing.Size(167, 44);
            this.btnBSBACancel.TabIndex = 167;
            this.btnBSBACancel.Text = "GO BACK";
            this.btnBSBACancel.UseVisualStyleBackColor = true;
            this.btnBSBACancel.Click += new System.EventHandler(this.btnBSBACancel_Click);
            // 
            // btnBSITCancel
            // 
            this.btnBSITCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBSITCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBSITCancel.Location = new System.Drawing.Point(15, 648);
            this.btnBSITCancel.Name = "btnBSITCancel";
            this.btnBSITCancel.Size = new System.Drawing.Size(167, 44);
            this.btnBSITCancel.TabIndex = 168;
            this.btnBSITCancel.Text = "GO BACK";
            this.btnBSITCancel.UseVisualStyleBackColor = true;
            this.btnBSITCancel.Click += new System.EventHandler(this.btnBSITCancel_Click);
            // 
            // AlphaListDirectoryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.ClientSize = new System.Drawing.Size(1066, 704);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.DGWBSBA);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "AlphaListDirectoryForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AlphaListDirectoryForm";
            ((System.ComponentModel.ISupportInitialize)(this.DGWBSBA)).EndInit();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.DataGridView DGWBSBA;
        private System.Windows.Forms.Panel panel4;
        public System.Windows.Forms.Button button8;
        public System.Windows.Forms.Button btnRegistrarCancel;
        private System.Windows.Forms.TextBox txtSY;
        private System.Windows.Forms.TextBox txtSem;
        public System.Windows.Forms.Button button18;
        public System.Windows.Forms.Button button16;
        public System.Windows.Forms.Button btnSY;
        public System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column7;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column8;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column6;
        public System.Windows.Forms.Button btnClear;
        public System.Windows.Forms.Button btnBSBACancel;
        public System.Windows.Forms.Button btnBSITCancel;
    }
}