using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MathCross
{
    public class NumberSelector : Form
    {
        public int SelectedValue { get; private set; }

        public NumberSelector(List<int> numbers)
        {
            if (numbers == null || numbers.Count == 0)
                throw new ArgumentException("Number list cannot be null or empty.");

            this.Text = "Select a number";
            this.Size = new Size(350, 200);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;

            FlowLayoutPanel panel = new FlowLayoutPanel()
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(15),
                AutoScroll = true,
                FlowDirection = FlowDirection.LeftToRight
            };

            foreach (int num in numbers)
            {
                int currentNumber = num; // Variable local para closure
                Button btn = new Button()
                {
                    Text = currentNumber.ToString(),
                    Width = TextRenderer.MeasureText(currentNumber.ToString(), new Font("Arial", 12)).Width + 20,
                    Height = 40,
                    Font = new Font("Arial", 12, FontStyle.Bold),
                    Margin = new Padding(5),
                    TextAlign = ContentAlignment.MiddleCenter
                };
                btn.Click += (s, e) =>
                {
                    SelectedValue = currentNumber;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                };
                panel.Controls.Add(btn);
            }

            this.Controls.Add(panel);
        }
    }
}

//Es una clase auxiliar de "PuzzleGamePanel". Realmente no se que hace. Me da paja explicar. Pero es parte de la generación de los niveles.