using System;
using System.Drawing;
using System.Windows.Forms;

namespace MathCross
{
    public class DifficultySelectionMenu : UserControl
    {
        public event Action<string> OnDifficultySelected;

        private Button easyButton, normalButton, hardButton;
        private string selectedDifficulty = null;

        public DifficultySelectionMenu()
        {
            this.Size = new Size(500, 300);
            this.BackColor = Color.White;

            Label title = new Label()
            {
                Text = "Selecciona la dificultad",
                Font = new Font("Arial", 18, FontStyle.Bold),
                Size = new Size(500, 40),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(0, 30)
            };
            this.Controls.Add(title);

            easyButton = CreateDifficultyButton("Fácil", Color.SkyBlue, 60);
            normalButton = CreateDifficultyButton("Normal", Color.Gold, 120);
            hardButton = CreateDifficultyButton("Difícil", Color.IndianRed, 180);

            this.Controls.Add(easyButton);
            this.Controls.Add(normalButton);
            this.Controls.Add(hardButton);
        }

        private Button CreateDifficultyButton(string text, Color hoverColor, int top)
        {
            Button btn = new Button();
            btn.Text = text;
            btn.Font = new Font("Arial", 14, FontStyle.Bold);
            btn.Size = new Size(200, 40);
            btn.Location = new Point((this.Width - btn.Width) / 2, top);
            btn.FlatStyle = FlatStyle.Flat;
            btn.BackColor = Color.White;
            btn.ForeColor = Color.Black;

            btn.MouseEnter += (s, e) =>
            {
                if (selectedDifficulty == null)
                {
                    btn.BackColor = hoverColor;
                }
            };

            btn.MouseLeave += (s, e) =>
            {
                if (selectedDifficulty == null || btn.Text != selectedDifficulty)
                {
                    btn.BackColor = Color.White;
                }
            };

            btn.Click += (s, e) =>
            {
                if (selectedDifficulty == null)
                {
                    selectedDifficulty = text;
                    btn.BackColor = hoverColor;
                    DisableOtherButtons(btn);
                    OnDifficultySelected?.Invoke(text);
                }
            };

            return btn;
        }

        private void DisableOtherButtons(Button selected)
        {
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is Button btn && btn != selected)
                {
                    btn.Enabled = false;
                    btn.BackColor = Color.LightGray;
                }
            }
        }
    }
}
