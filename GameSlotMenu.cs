using System;
using System.Drawing;
using System.Windows.Forms;

namespace MathCross
{
    public class GameSlotMenu : Form
    {
        private Panel[] slotPanels = new Panel[3];
        private Label[] slotTitles = new Label[3];
        private Label[] slotDates = new Label[3];
        private Label[] slotInfo = new Label[3];
        private Button closeButton;

        private string[] saveStates = { null, null, null }; // Simula el estado de las partidas

        public GameSlotMenu()
        {
            this.Text = "Seleccionar Partida";
            this.Size = new Size(700, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.None;

            // Botón de cerrar (esquina superior derecha)
            closeButton = new Button();
            closeButton.Text = "❌";
            closeButton.Font = new Font("Arial", 12, FontStyle.Bold);
            closeButton.Size = new Size(40, 40);
            closeButton.Location = new Point(this.ClientSize.Width - 50, 10);
            closeButton.FlatStyle = FlatStyle.Flat;
            closeButton.FlatAppearance.BorderSize = 1;
            closeButton.BackColor = Color.LightGray;
            closeButton.Click += (s, e) => this.Close();
            this.Controls.Add(closeButton);

            // Crear los 3 slots
            for (int i = 0; i < 3; i++)
            {
                slotPanels[i] = CreateSlotPanel(i);
                slotPanels[i].Location = new Point(75, 80 + i * 150);
                this.Controls.Add(slotPanels[i]);
            }
        }

        private Panel CreateSlotPanel(int index)
        {
            Panel panel = new Panel();
            panel.Size = new Size(550, 120);
            panel.BackColor = Color.Gainsboro;
            panel.BorderStyle = BorderStyle.FixedSingle;
            panel.Cursor = Cursors.Hand;

            // Fecha
            slotDates[index] = new Label();
            slotDates[index].Text = "00/00/00";
            slotDates[index].Font = new Font("Arial", 9, FontStyle.Italic);
            slotDates[index].Location = new Point(10, 5);
            slotDates[index].Size = new Size(200, 15);
            panel.Controls.Add(slotDates[index]);

            // Título principal del slot
            slotTitles[index] = new Label();
            slotTitles[index].Text = saveStates[index] == null ? "Nueva partida" : $"Partida número {index + 1}";
            slotTitles[index].Font = new Font("Arial", 16, FontStyle.Bold);
            slotTitles[index].Location = new Point(10, 30);
            slotTitles[index].Size = new Size(400, 30);
            panel.Controls.Add(slotTitles[index]);

            // Información de dificultad y estrellas
            slotInfo[index] = new Label();
            slotInfo[index].Text = "Dificultad: --- | 0/30 estrellas";
            slotInfo[index].Font = new Font("Arial", 10, FontStyle.Regular);
            slotInfo[index].Location = new Point(300, 80);
            slotInfo[index].Size = new Size(230, 20);
            slotInfo[index].TextAlign = ContentAlignment.BottomRight;
            panel.Controls.Add(slotInfo[index]);

            // Animación al pasar el mouse
            panel.MouseEnter += (s, e) =>
            {
                panel.BackColor = Color.LightSkyBlue;
                panel.Size = new Size(570, 130);
                panel.Invalidate();
            };

            panel.MouseLeave += (s, e) =>
            {
                panel.BackColor = Color.Gainsboro;
                panel.Size = new Size(550, 120);
                panel.Invalidate();
            };

            panel.Click += (s, e) =>
            {
                MessageBox.Show($"Seleccionaste el slot #{index + 1}");
                // Aquí luego se puede cargar o crear una partida
            };

            return panel;
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.Run(new GameSlotMenu());
        }
    }
}
