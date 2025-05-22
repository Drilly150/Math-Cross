using System;
using System.Drawing;
using System.Windows.Forms;

namespace MathCross
{
    /// <summary>
    /// Diálogo de confirmación que solicita al usuario si desea reiniciar una partida.
    /// Emite un evento cuando el usuario confirma el reinicio.
    /// </summary>
    public class ConfirmResetDialog : Form
    {
        /// <summary>
        /// Evento que se dispara cuando el usuario confirma el reinicio de la partida.
        /// </summary>
        public event Action OnConfirmed;

        /// <summary>
        /// Inicializa una nueva instancia del diálogo de confirmación de reinicio.
        /// Configura la apariencia del diálogo y los botones de acción.
        /// </summary>
        public ConfirmResetDialog()
        {
            this.Size = new Size(320, 160);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.White;
            this.Text = "Confirmar reinicio";
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Etiqueta de mensaje para preguntar al usuario
            Label message = new Label()
            {
                Text = "¿Estás seguro de que deseas reiniciar esta partida?",
                Font = new Font("Arial", 10, FontStyle.Regular),
                AutoSize = false,
                Size = new Size(300, 50),
                Location = new Point(10, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(message);

            // Botón "Sí" (para confirmar el reinicio)
            Button yesBtn = new Button()
            {
                Text = "Sí",
                Size = new Size(100, 35),
                Location = new Point(40, 90),
                BackColor = Color.IndianRed,
                Font = new Font("Arial", 10),
                ForeColor = Color.White
            };
            yesBtn.Click += (s, e) =>
            {
                OnConfirmed?.Invoke(); // Dispara el evento de confirmación
                this.DialogResult = DialogResult.OK; // Establece el resultado del diálogo a OK
                this.Close(); // Cierra el diálogo
            };
            this.Controls.Add(yesBtn);

            // Botón "Cancelar"
            Button cancelBtn = new Button()
            {
                Text = "Cancelar",
                Size = new Size(100, 35),
                Location = new Point(170, 90),
                BackColor = Color.LightGray,
                Font = new Font("Arial", 10)
            };
            cancelBtn.Click += (s, e) =>
            {
                this.DialogResult = DialogResult.Cancel; // Establece el resultado del diálogo a Cancel
                this.Close(); // Cierra el diálogo
            };
            this.Controls.Add(cancelBtn);
        }
    }
}

//Este archivo es la continuacion del "SlotOptionsDialog", el cual se ejecuta a la hora de seleccionar el Reiniciar en vez de continuar en el slot donde se guarda la partida. En donde lanzara una cuadro que dira "¿Estas seguro?". Haciendo referencia a que si deseas eliminar/reiniciar partida.