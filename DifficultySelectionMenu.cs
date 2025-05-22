using System;
using System.Drawing;
using System.Windows.Forms;

namespace MathCross
{
    public class DifficultySelectionMenu : UserControl
    {
        // Evento que se activa cuando se selecciona una dificultad.
        public event Action<string> OnDifficultySelected;

        // Botones para cada nivel de dificultad.
        private Button easyButton, normalButton, hardButton;
        // Almacena la dificultad seleccionada. Es nula hasta que se selecciona una opción.
        private string selectedDifficulty = null;

        public DifficultySelectionMenu()
        {
            // Configuración básica para el UserControl.
            this.Size = new Size(500, 300); // Establece el tamaño del menú.
            this.BackColor = Color.White;   // Establece el color de fondo.
            
            // Etiqueta de título para el menú.
            Label title = new Label()
            {
                Text = "Selecciona la dificultad", // Texto que se muestra en la etiqueta.
                Font = new Font("Arial", 18, FontStyle.Bold), // Fuente para el título.
                Size = new Size(500, 40), // Tamaño de la etiqueta.
                TextAlign = ContentAlignment.MiddleCenter, // Centra el texto dentro de la etiqueta.
                Location = new Point(0, 30) // Posición de la etiqueta.
            };
            this.Controls.Add(title); // Agrega la etiqueta al control.

            // Crea y añade botones de dificultad.
            // Cada botón se crea con su texto, color al pasar el mouse y posición vertical.
            easyButton = CreateDifficultyButton("Fácil", Color.SkyBlue, 60);
            normalButton = CreateDifficultyButton("Normal", Color.Gold, 120);
            hardButton = CreateDifficultyButton("Difícil", Color.IndianRed, 180);

            this.Controls.Add(easyButton);
            this.Controls.Add(normalButton);
            this.Controls.Add(hardButton);
        }

        /// <summary>
        ///Crea un botón con estilo para seleccionar la dificultad.
        /// </summary>
        /// <param name="text">El texto que se mostrará en el botón (por ejemplo, "Fácil").</param>
        /// <param name="hoverColor">El color al que cambia el botón al pasar el mouse sobre él y cuando se selecciona.</param>
        /// <param name="top">La coordenada Y para la posición del botón.</param>
        /// <returns>Un control de botón configurado.</returns>
        private Button CreateDifficultyButton(string text, Color hoverColor, int top)
        {
            Button btn = new Button();
            btn.Text = text; // Establece el texto del botón.
            btn.Font = new Font("Arial", 14, FontStyle.Bold); // Establece la fuente del botón.
            btn.Size = new Size(200, 40); // Establece el tamaño del botón.
            // Centra el botón horizontalmente dentro del control.
            btn.Location = new Point((this.Width - btn.Width) / 2, top);
            btn.FlatStyle = FlatStyle.Flat; // Da al botón una apariencia plana.
            btn.BackColor = Color.White;    // Color de fondo predeterminado.
            btn.ForeColor = Color.Black;    // Color de texto predeterminado.

            // Controlador de eventos para cuando el mouse ingresa al área del botón.
            btn.MouseEnter += (s, e) =>
            {
                // Cambia de color al pasar el cursor solo si aún no se ha seleccionado ninguna dificultad.
                if (selectedDifficulty == null)
                {
                    btn.BackColor = hoverColor;
                }
            };

            // Controlador de eventos para cuando el mouse abandona el área del botón.
            btn.MouseLeave += (s, e) =>
            {
                // Restablecer color si el botón está habilitado Y
                // (no se ha seleccionado ninguna dificultad O este botón no es el seleccionado).
                // Esto evita cambiar el color de los botones deshabilitados o del botón seleccionado.
                if (btn.Enabled && (selectedDifficulty == null || btn.Text != selectedDifficulty))
                {
                    btn.BackColor = Color.White;
                }
            };

            // Controlador de eventos para cuando se hace clic en el botón.
            btn.Click += (s, e) =>
            {
                // Proceda sólo si aún no ha seleccionado ninguna dificultad (selección única).
                if (selectedDifficulty == null)
                {
                    selectedDifficulty = text; // Establezca la dificultad seleccionada.
                    btn.BackColor = hoverColor; // Establezca explícitamente el color del botón seleccionado.
                    DisableOtherButtons(btn);   // Deshabilitar otros botones.
                    OnDifficultySelected?.Invoke(text); // Desencadenar el evento.
                }
            };

            return btn;
        }

        /// <summary>
        /// Desactiva todos los botones del control excepto el que fue seleccionado.
        /// También cambia el color de fondo de los botones deshabilitados para indicar que están inactivos.
        /// </summary>
        /// <param name="selected">El botón en el que se hizo clic y se seleccionó.</param>
        private void DisableOtherButtons(Button selected)
        {
            // Iterar a través de todos los controles del formulario.
            foreach (Control ctrl in this.Controls)
            {
                // Comprueba si el control es un Botón y no es el botón seleccionado.
                if (ctrl is Button btn && btn != selected)
                {
                    btn.Enabled = false; // Desactiva el botón.
                    btn.BackColor = Color.LightGray; // Cambiar su color de fondo.
                }
            }
        }
    }
}

// Este archivo solo se ejecuta cuando vas a crear una nueva partida. En él aparecerán los tres tipos de dificultad que tendrá el juego: Fácil, Normal, Difícil. Luego, al seleccionar la dificultad deseada, se ejecutará el archivo "LevelSelectMenu".