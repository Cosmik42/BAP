using System;
using System.Windows.Forms;

namespace LegoTrainProject
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            // Start the program
            var program = new Program();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainBoard());
        }

        public Program()
        {
            //
        }
    }
}
