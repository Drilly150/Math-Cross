using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MathCross
{
    public class PuzzleGamePanel : UserControl
    {
        private const int gridSize = 5;
        private Button[,] cellButtons = new Button[gridSize, gridSize];
        private Panel sidebar;

        private int cellSize = 60;
        private int margin = 10;

        private string[,] exampleGrid = new string[5, 5]
        {
            { "3", "+", "4", "=", "7" },
            { "", "", "", "", ""     },
            { "9", "-", "2", "=", "7" },
            { "", "", "", "", ""     },
            { "6", "/", "2", "=", "3" }
        };

        private List<int> availableNumbers = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        private Label lblTiempo, lblPuntos, lblErrores;
        private Timer gameTimer;
        private int segundosTranscurridos = 0;
        private int puntos = 0;
        private int errores = 0;

        public PuzzleGamePanel()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;

            CreateGrid();
            CreateSidebar();
        }

        public void AgregarPuntos(int cantidad)
        {
            puntos += cantidad;
            lblPuntos.Text = $"Puntos: {puntos}";
        }

        public void RegistrarError()
        {
            errores++;
            lblErrores.Text = $"Errores: {errores}";
        }

        public class PuzzleCell
        {
            public enum CellType { Empty, Number, Operator, Equals }
            public CellType Type { get; set; }
            public string Value { get; set; }
            public bool Editable => Type == CellType.Empty;
        }

        private void CreateGrid()
        {
            int offsetX = 40;
            int offsetY = 40;

            for (int row = 0; row < gridSize; row++)
            {
                for (int col = 0; col < gridSize; col++)
                {
                    Button btn = new Button();
                    btn.Size = new Size(cellSize, cellSize);
                    btn.Location = new Point(offsetX + col * (cellSize + margin), offsetY + row * (cellSize + margin));
                    btn.Font = new Font("Arial", 14, FontStyle.Bold);
                    btn.Tag = new Point(row, col);

                    string content = exampleGrid[row, col];
                    if (!string.IsNullOrEmpty(content))
                    {
                        btn.Text = content;
                        btn.Enabled = false;
                        btn.BackColor = Color.LightGray;
                    }
                    else
                    {
                        btn.BackColor = Color.White;
                        btn.Click += OnEditableCellClick;
                    }

                    this.Controls.Add(btn);
                    cellButtons[row, col] = btn;
                }
            }
        }

        private void OnEditableCellClick(object sender, EventArgs e)
        {
            Button clicked = sender as Button;

            using (NumberSelector selector = new NumberSelector(availableNumbers))
            {
                if (selector.ShowDialog() == DialogResult.OK)
                {
                    clicked.Text = selector.SelectedValue.ToString();
                    AnimateCell(clicked);

                    if (TodasLasCeldasLlenas())
                        VerificarTablero();
                }
            }
        }

        private void AnimateCell(Button btn)
        {
            var originalSize = btn.Size;
            var timer = new Timer();
            int frame = 0;
            timer.Interval = 15;
            timer.Tick += (s, e) =>
            {
                frame++;
                if (frame <= 2)
                    btn.Size = new Size(originalSize.Width + 4, originalSize.Height + 4);
                else if (frame <= 4)
                    btn.Size = new Size(originalSize.Width - 2, originalSize.Height - 2);
                else
                {
                    btn.Size = originalSize;
                    timer.Stop();
                }
            };
            timer.Start();
        }

        private void CreateSidebar()
        {
            sidebar = new Panel()
            {
                Width = 220,
                Dock = DockStyle.Right,
                BackColor = Color.FromArgb(240, 240, 240)
            };

            Label title = new Label()
            {
                Text = "Progreso",
                Font = new Font("Arial", 16, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };
            sidebar.Controls.Add(title);

            lblTiempo = new Label()
            {
                Text = "Tiempo: 0:00",
                Font = new Font("Arial", 12),
                Location = new Point(20, 60),
                AutoSize = true
            };
            sidebar.Controls.Add(lblTiempo);

            lblPuntos = new Label()
            {
                Text = "Puntos: 0",
                Font = new Font("Arial", 12),
                Location = new Point(20, 100),
                AutoSize = true
            };
            sidebar.Controls.Add(lblPuntos);

            lblErrores = new Label()
            {
                Text = "Errores: 0",
                Font = new Font("Arial", 12),
                Location = new Point(20, 140),
                AutoSize = true
            };
            sidebar.Controls.Add(lblErrores);

            this.Controls.Add(sidebar);

            // Iniciar temporizador
            gameTimer = new Timer();
            gameTimer.Interval = 1000;
            gameTimer.Tick += (s, e) =>
            {
                segundosTranscurridos++;
                lblTiempo.Text = $"Tiempo: {segundosTranscurridos / 60}:{(segundosTranscurridos % 60).ToString("D2")}";
            };
            gameTimer.Start();
        }

        private bool VerificarTablero()
        {
            if (!TodasLasCeldasLlenas())
                return false;

            bool filasOK = VerificarFilas();
            bool columnasOK = VerificarColumnas();

            if (filasOK && columnasOK)
            {
                int estrellas = CalcularEstrellas();
                AgregarPuntos(100); // extra
                MostrarEstrellas(estrellas);

                // Simular guardar progreso (nivel actual fijo para prueba)
                LevelProgressManager.CompletarNivel("P1", estrellas, segundosTranscurridos);

                return true;
            }

            else
            {
                RegistrarError();
                MessageBox.Show("Hay errores en las operaciones. Revisa tus respuestas.");
                return false;
            }
        }

        private bool TodasLasCeldasLlenas()
        {
            foreach (var btn in cellButtons)
                if (btn.Enabled && string.IsNullOrEmpty(btn.Text))
                    return false;
            return true;
        }

        private bool VerificarFilas()
        {
            for (int row = 0; row < gridSize; row++)
            {
                try
                {
                    string[] expr = new string[5];
                    for (int col = 0; col < 5; col++)
                        expr[col] = cellButtons[row, col].Text;

                    if (!EvaluarOperacion(expr))
                        return false;
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }

        private bool VerificarColumnas()
        {
            for (int col = 0; col < gridSize; col++)
            {
                try
                {
                    string[] expr = new string[5];
                    for (int row = 0; row < 5; row++)
                        expr[row] = cellButtons[row, col].Text;

                    if (!EvaluarOperacion(expr))
                        return false;
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }

        private bool EvaluarOperacion(string[] tokens)
        {
            if (tokens.Length != 5) return false;

            int a = int.Parse(tokens[0]);
            string op = tokens[1];
            int b = int.Parse(tokens[2]);
            string igual = tokens[3];
            int resultado = int.Parse(tokens[4]);

            if (igual != "=") return false;

            return op switch
            {
                "+" => a + b == resultado,
                "-" => a - b == resultado,
                "×" or "*" => a * b == resultado,
                "÷" or "/" => b != 0 && a / b == resultado,
                _ => false
            };
        }

        private int CalcularEstrellas()
        {
            int estrellas = 1; // Por completarlo correctamente

            if (errores <= 3)
                estrellas++;

            if (segundosTranscurridos <= 90 || puntos >= 100)
                estrellas++;

            return estrellas;
        }
    }
}

//La base fundamental para la generación de niveles. 