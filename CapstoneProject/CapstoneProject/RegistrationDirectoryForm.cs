using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CapstoneProject
{
    public partial class RegistrationDirectoryForm : Form
    {
        public RegistrationDirectoryForm()
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
            e.Graphics.DrawString("____________________________________________________________________________________________________", myfont, myBrush, 0, 0);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
