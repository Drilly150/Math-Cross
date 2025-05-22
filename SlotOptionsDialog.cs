using System;
using System.Drawing;
using System.Windows.Forms;

namespace MathCross
{
    /// <summary>
    /// Diálogo de opciones que se muestra al seleccionar un slot de partida guardada.
    /// Permite al usuario elegir entre "Continuar" la partida o "Reiniciar" el slot.
    /// </summary>
    public class SlotOptionsDialog : Form
    {
        /// <summary>
        /// Evento que se dispara cuando el usuario selecciona "Continuar" la partida.
        /// </summary>
        public event Action OnContinueSelected;

        /// <summary>
        /// Evento que se dispara cuando el usuario confirma el "Reinicio" de la partida.
        /// </summary>
        public event Action OnResetSelected;

        private int _slot; // Almacenar el slot para posible uso interno si fuera necesario

        /// <summary>
        /// Inicializa una nueva instancia del diálogo de opciones de slot.
        /// </summary>
        /// <param name="slot">El número de slot de la partida seleccionada (basado en 0 para indexación).</param>
        public SlotOptionsDialog(int slot)
        {
            _slot = slot; // Guarda el número de slot
            this.Size = new Size(300, 180);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.White;
            this.Text = $"Slot {slot + 1}"; // Muestra el número de slot para el usuario (slot + 1)
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Etiqueta de mensaje principal
            Label message = new Label()
            {
                Text = "¿Qué deseas hacer con esta partida?",
                Font = new Font("Arial", 10, FontStyle.Regular),
                AutoSize = false,
                Size = new Size(280, 50),
                Location = new Point(10, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(message);

            // Botón "Continuar"
            Button continueBtn = new Button()
            {
                Text = "Continuar",
                Size = new Size(100, 35),
                Location = new Point(30, 90),
                BackColor = Color.LightGreen,
                Font = new Font("Arial", 10)
            };
            continueBtn.Click += (s, e) =>
            {
                OnContinueSelected?.Invoke(); // Dispara el evento de continuar
                this.Close(); // Cierra el diálogo
            };
            this.Controls.Add(continueBtn);

            // Botón "Reiniciar"
            Button resetBtn = new Button()
            {
                Text = "Reiniciar",
                Size = new Size(100, 35),
                Location = new Point(160, 90),
                BackColor = Color.IndianRed,
                Font = new Font("Arial", 10),
                ForeColor = Color.White
            };
            resetBtn.Click += (s, e) =>
            {
                ConfirmResetDialog confirm = new ConfirmResetDialog();
                // Suscribirse al evento OnConfirmed del diálogo de confirmación
                confirm.OnConfirmed += () =>
                {
                    OnResetSelected?.Invoke(); // Solo si el usuario confirma el reinicio
                    // Opcional: Podrías querer cerrar SlotOptionsDialog aquí
                    // this.Close();
                };
                // Mostrar el diálogo de confirmación de forma modal
                // Usar ShowDialog para que el código espere a que ConfirmResetDialog se cierre
                DialogResult result = confirm.ShowDialog(this);

                // Si el usuario confirmó el reinicio, cerrar SlotOptionsDialog
                // Esto es una mejora para que el SlotOptionsDialog también se cierre después de la confirmación
                if (result == DialogResult.OK)
                {
                    this.Close();
                }
            };
            this.Controls.Add(resetBtn);
        }
    }
}

