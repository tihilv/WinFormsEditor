using System;
using System.Windows.Forms;

namespace ClientSide
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

            string fileToOpen = null;
            if (args.Length == 1)
                fileToOpen = args[0];

            Application.Run(new MainForm(fileToOpen));
        }
    }
}
