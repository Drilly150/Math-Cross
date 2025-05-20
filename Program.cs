using System;
using System.Windows.Forms;

namespace MathCross
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Aquí debes tener tu formulario principal
            MainMenu main = new MainMenu();
            Application.Run(main);
        }
    }
}
