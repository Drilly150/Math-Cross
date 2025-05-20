using System.Collections.Generic;
using System.Windows.Forms;

namespace MathCross
{
    public static class GameStateManager
    {
        private static Form mainForm;
        private static Control contenedor;

        public static string NivelActual { get; private set; }
        public static bool ModoPractica { get; private set; }

        private static Stack<Control> historial = new Stack<Control>();

        public static void Inicializar(Form formPrincipal, Control areaPrincipal)
        {
            mainForm = formPrincipal;
            contenedor = areaPrincipal;

            Configuracion = GameSettings.Cargar();
            AplicarConfiguracionVisual();
        }

        public static void AplicarConfiguracionVisual()
        {
            string[] resolucion = Configuracion.Resolucion.Split('x');
            if (resolucion.Length == 2 &&
                int.TryParse(resolucion[0], out int ancho) &&
                int.TryParse(resolucion[1], out int alto))
            {
                mainForm.Size = new Size(ancho, alto);
            }

            mainForm.FormBorderStyle = Configuracion.PantallaCompleta ? FormBorderStyle.None : FormBorderStyle.Sizable;
            mainForm.WindowState = Configuracion.PantallaCompleta ? FormWindowState.Maximized : FormWindowState.Normal;

            mainForm.BackColor = Configuracion.TemaOscuro ? Color.FromArgb(30, 30, 30) : Color.White;
        }

        public static GameSettings Configuracion { get; private set; }

        public static void IrAlMenuPrincipal()
        {
            NivelActual = null;
            ModoPractica = false;
            MostrarControl(new MainMenu());
        }

        public static void IrAlMenuNiveles(bool practica = false)
        {
            NivelActual = null;
            ModoPractica = practica;
            MostrarControl(new LevelSelectMenu());
        }

        public static void JugarNivel(string nivelId)
        {
            NivelActual = nivelId;
            MostrarControl(new PuzzleGamePanel(nivelId, ModoPractica));
        }

        public static void MostrarControl(Control nuevo)
        {
            if (contenedor.Controls.Count > 0)
                historial.Push(contenedor.Controls[0]); // guardar actual

            contenedor.Controls.Clear();
            contenedor.Controls.Add(nuevo);
            nuevo.Dock = DockStyle.Fill;
        }

        public static void VolverAtras()
        {
            if (historial.Count == 0)
            {
                IrAlMenuPrincipal();
                return;
            }

            var anterior = historial.Pop();
            contenedor.Controls.Clear();
            contenedor.Controls.Add(anterior);
            anterior.Dock = DockStyle.Fill;
        }

        public static void LimpiarHistorial()
        {
            historial.Clear();
        }
    }
}

//Este archivo unifica todo los menus que abarca el juego en cuestión.