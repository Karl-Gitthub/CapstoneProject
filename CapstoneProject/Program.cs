using System;
using System.Windows.Forms;

namespace CapstoneProject
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            AppDomain.CurrentDomain.SetData("DataDirectory", Infrastructure.Database.DirectoryPath);
            if (Array.IndexOf(args,"--preview-students")>=0) Application.Run(new RegistrarDirectoryForm(true));
            else Application.Run(new LoginForm());
        }
    }
}
