
namespace CapstoneProject
{
    partial class FacultyDirectoryForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtfacid = new System.Windows.Forms.TextBox();
            this.txtname = new System.Windows.Forms.TextBox();
            this.txtdept = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(350, 67);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Faculty";
            // 
            // txtfacid
            // 
            this.txtfacid.Location = new System.Drawing.Point(134, 128);
            this.txtfacid.Name = "txtfacid";
            this.txtfacid.Size = new System.Drawing.Size(100, 20);
            this.txtfacid.TabIndex = 1;
            this.txtfacid.TextChanged += new System.EventHandler(this.txtfacid_TextChanged);
            // 
            // txtname
            // 
            this.txtname.Location = new System.Drawing.Point(291, 128);
            this.txtname.Name = "txtname";
            this.txtname.Size = new System.Drawing.Size(100, 20);
            this.txtname.TabIndex = 2;
            // 
            // txtdept
            // 
            this.txtdept.Location = new System.Drawing.Point(435, 128);
            this.txtdept.Name = "txtdept";
            this.txtdept.Size = new System.Drawing.Size(100, 20);
            this.txtdept.TabIndex = 3;
            // 
            // FacultyDirectoryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.txtdept);
            this.Controls.Add(this.txtname);
            this.Controls.Add(this.txtfacid);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FacultyDirectoryForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FacultyDirectoryForm";
            this.Load += new System.EventHandler(this.FacultyDirectoryForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtfacid;
        private System.Windows.Forms.TextBox txtname;
        private System.Windows.Forms.TextBox txtdept;
    }
}