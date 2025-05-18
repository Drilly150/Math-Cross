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
            this.Text = "Selecciona un número";
            this.Size = new Size(300, 150);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;

            FlowLayoutPanel panel = new FlowLayoutPanel()
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                AutoScroll = true
            };

            foreach (int num in numbers)
            {
                Button btn = new Button()
                {
                    Text = num.ToString(),
                    Width = 40,
                    Height = 40,
                    Font = new Font("Arial", 12)
                };
                btn.Click += (s, e) =>
                {
                    SelectedValue = num;
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