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

            try
            {
                // Inicializar los sistemas básicos del juego
                GameStateManager.Initialize();

                //Iniciar el menú principal
                Application.Run(new MainMenu());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fatal error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }
        }
    }
}

//Punto de entrada del juego. Aqui contiene el metodo main que utiliza el juego para ejecutarse. Llama tanto "GameStateManager.Inicializar()" para preparar el juego. He inicia "MainMenu", que es la base principal de todo. 