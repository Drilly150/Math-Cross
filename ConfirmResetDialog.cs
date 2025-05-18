using System;
using System.Drawing;
using System.Windows.Forms;

namespace MathCross
{
    public class ConfirmResetDialog : Form
    {
        public event Action OnConfirmed;

        public ConfirmResetDialog()
        {
            this.Size = new Size(320, 160);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.White;
            this.Text = "Confirmar reinicio";
            this.MaximizeBox = false;
            this.MinimizeBox = false;

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

            // Botón "Sí"
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
                OnConfirmed?.Invoke();
                this.Close();
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
            cancelBtn.Click += (s, e) => this.Close();
            this.Controls.Add(cancelBtn);
        }
    }
}

//Este archivo es la continuacion del "SlotOptionsDialog", el cual se ejecuta a la hora de seleccionar el Reiniciar. En donde lanzara una cuadro que dira "¿Estas seguro?". Haciendo referencia a que si deseas eliminar/reiniciar partida.