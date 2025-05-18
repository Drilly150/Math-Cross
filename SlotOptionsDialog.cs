using System;
using System.Drawing;
using System.Windows.Forms;

namespace MathCross
{
    public class SlotOptionsDialog : Form
    {
        public event Action OnContinueSelected;
        public event Action OnResetSelected;

        public SlotOptionsDialog(int slot)
        {
            this.Size = new Size(300, 180);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.White;
            this.Text = $"Slot {slot + 1}";
            this.MaximizeBox = false;
            this.MinimizeBox = false;

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

            // Continuar
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
                OnContinueSelected?.Invoke();
                this.Close();
            };
            this.Controls.Add(continueBtn);

            // Reiniciar
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
              confirm.OnConfirmed += () =>
              {
                  OnResetSelected?.Invoke();
              };
              confirm.ShowDialog(this);
            };
            this.Controls.Add(resetBtn);
        }
    }
}

//Este archivo funciona principalmente para la hora de saber que hacer con la partida ya seleccionada, en caso de ya tener una partida guardada. Abriendo un Menú dentro del juego que dice "Reanudar" o "Reiniciar". En el caso de Reanudar, no pasa nada. Simplemente se ejecuta el archivo de "LevelSelectMenu". En caso de seleccionar Reiniciar, se ejecuta el archivo "ConfirmResertDialog". 