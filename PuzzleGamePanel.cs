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

        private string[,] puzzleData;

        private List<int> availableNumbers = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        private Label lblTiempo, lblPuntos, lblErrores;
        private Timer gameTimer;
        private int segundosTranscurridos = 0;
        private int puntos = 0;
        private int errores = 0;

        private int pistasDisponibles = 3;
        private Button btnPista;
        private Label lblPistas;
        

        private Panel estrellasPanel;
        private Button btnVolverMenu;
        private Button btnContinuarNivel;
        private Label felicidadesLabel;

        public PuzzleGamePanel();
        public string[,] GenerarPuzzle(int size, int porcentajeOcultas)
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;

            PuzzleGenerator gen = new PuzzleGenerator();
            puzzleData = gen.GenerarPuzzle(gridSize);  // usa gridSize actual

            CreateGrid();
            CreateSidebar();

            OcultarCeldas(grid, porcentajeOcultas);
            return grid;
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

                    string content = puzzleData[row, col];
                    if (!string.IsNullOrEmpty(content))
                    {
                        if (EsNumero(content))
                        {
                            btn.Text = ""; // celda vacía para jugador
                            btn.BackColor = Color.White;
                            btn.Click += OnEditableCellClick;
                        }
                        else
                        {
                            btn.Text = content; // operador o "="
                            btn.Enabled = false;
                            btn.BackColor = Color.LightGray;
                        }
                    }
                    else
                    {
                        btn.Enabled = false;
                        btn.BackColor = Color.DarkGray;
                    }

                    this.Controls.Add(btn);
                    cellButtons[row, col] = btn;
                }
            }
        }

        private bool EsNumero(string val)
        {
            return int.TryParse(val, out _);
        }

        private bool EsNumero(string val)
        {
            return int.TryParse(val, out _);
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

            lblPistas = new Label()
            {
                Text = $"Pistas: {pistasDisponibles}",
                Font = new Font("Arial", 12),
                Location = new Point(20, 180),
                AutoSize = true
            };
            sidebar.Controls.Add(lblPistas);

            btnPista = new Button()
            {
                Text = "Usar pista",
                Size = new Size(160, 35),
                Location = new Point(20, 210),
                BackColor = Color.LightBlue,
                Font = new Font("Arial", 10)
            };
            btnPista.Click += (s, e) => 
                if (pistaEstrategicaActiva)
                    AplicarPistaEstrategica();
                else
                    UsarPista();

            sidebar.Controls.Add(btnPista);

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

        private void MostrarEstrellasEnSidebar(int cantidad)
        {
            // Evitar duplicados
            sidebar.Controls.Clear();

            // Título
            felicidadesLabel = new Label()
            {
                Text = "¡Felicidades!",
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = Color.Green,
                Location = new Point(20, 20),
                AutoSize = true
            };
            sidebar.Controls.Add(felicidadesLabel);

            // Estrellas
            estrellasPanel = new Panel()
            {
                Size = new Size(180, 50),
                Location = new Point(20, 60),
                BackColor = Color.Transparent
            };

            for (int i = 0; i < 3; i++)
            {
                PictureBox star = new PictureBox()
                {
                    Size = new Size(40, 40),
                    Location = new Point(i * 50, 5),
                    Image = i < cantidad ? Properties.Resources.star_full : Properties.Resources.star_empty,
                    SizeMode = PictureBoxSizeMode.Zoom
                };
                estrellasPanel.Controls.Add(star);
            }

            sidebar.Controls.Add(estrellasPanel);

            // Botón: Volver al menú
         string nivelActual = "P1"; // ← Luego esto debería venir como parámetro

            btnVolverMenu = new Button()
            {
                Text = "Volver al menú",
                Size = new Size(160, 35),
                Location = new Point(20, 130),
                BackColor = Color.LightGray,
                Font = new Font("Arial", 10)
            };
            btnVolverMenu.Click += (s, e) =>
            {
                if (Parent != null)
                {
                    this.Controls.Clear();
                    LevelSelectMenu menu = new LevelSelectMenu(nivelActual); // Aquí se pasa el nivel completado
                    menu.OnCloseRequested += OnCloseRequested;
                    this.Parent.Controls.Add(menu);
                    this.Dispose();
                }
            };

            sidebar.Controls.Add(btnVolverMenu);

            // Botón: Siguiente nivel
            btnContinuarNivel = new Button()
            {
                Text = "Siguiente nivel",
                Size = new Size(160, 35),
                Location = new Point(20, 180),
                BackColor = Color.LightGreen,
                Font = new Font("Arial", 10)
            };
            btnContinuarNivel.Click += (s, e) =>
            {
                // Simulación de siguiente nivel (P2, P3...)
                string nivelActual = "P1"; // Esto debe recibirse por parámetro más adelante
                int numero = int.Parse(nivelActual.Substring(1)) + 1;
                string siguiente = $"P{numero}";

                LevelProgressManager.CompletarNivel(nivelActual, cantidad, segundosTranscurridos);

                this.Controls.Clear();
                PuzzleGamePanel siguienteNivel = new PuzzleGamePanel(); // más adelante se pasa el ID real
                siguienteNivel.OnCloseRequested += OnCloseRequested;
                this.Parent.Controls.Add(siguienteNivel);
                this.Dispose();
            };
            sidebar.Controls.Add(btnContinuarNivel);

            // Animación simple: fade-in
            Timer fade = new Timer { Interval = 20 };
            int alpha = 0;
            fade.Tick += (s, e) =>
            {
                alpha += 15;
                if (alpha >= 255)
                {
                    fade.Stop();
                    alpha = 255;
                }
                estrellasPanel.ForeColor = Color.FromArgb(alpha, Color.Black);
                felicidadesLabel.ForeColor = Color.FromArgb(alpha, Color.Green);
            };
            fade.Start();
        }

        private string nivelActual;

        public PuzzleGamePanel(string nivelId)
        {
            this.nivelActual = nivelId;

            PuzzleGenerator gen = new PuzzleGenerator();
            var dificultad = gen.ObtenerDificultadPorNivel(nivelId);

            gridSize = dificultad.Size;
            gen.SetOperadores(dificultad.OperadoresPermitidos);
            puzzleData = gen.GenerarPuzzle(gridSize, dificultad.PorcentajeCeldasOcultas);

            pistasDisponibles = dificultad.MaxPistas;

            this.BackColor = Color.White;
            this.Dock = DockStyle.Fill;

            CreateGrid();
            CreateSidebar();
        }

        private void UsarPista()
        {
            if (pistasDisponibles <= 0)
            {
                MessageBox.Show("Ya no tienes pistas disponibles.");
                return;
            }

            for (int r = 0; r < gridSize; r++)
            {
                for (int c = 0; c < gridSize; c++)
                {
                    if (cellButtons[r, c].Enabled && string.IsNullOrEmpty(cellButtons[r, c].Text))
                    {
                        string valorCorrecto = puzzleData[r, c];
                        if (!string.IsNullOrEmpty(valorCorrecto))
                        {
                            cellButtons[r, c].Text = valorCorrecto;
                            cellButtons[r, c].BackColor = Color.LightGreen;
                            cellButtons[r, c].Enabled = false;

                            pistasDisponibles--;
                            lblPistas.Text = $"Pistas: {pistasDisponibles}";
                            return;
                        }
                    }
                }
            }

            MessageBox.Show("No quedan celdas vacías con valor conocido.");
        }

        private void AplicarPistaEstrategica()
        {
            if (pistasDisponibles <= 0)
            {
                MessageBox.Show("Ya no tienes pistas disponibles.");
                return;
            }

            // Buscar una ecuación horizontal incorrecta
            for (int row = 0; row < gridSize; row += 2)
            {
                string[] tokens = new string[5];
                bool completa = true;

                for (int col = 0; col < 5; col++)
                {
                    string txt = cellButtons[row, col].Text;
                    tokens[col] = txt;
                    if (string.IsNullOrEmpty(txt) && cellButtons[row, col].Enabled)
                        completa = false;
                }

                if (completa && !EvaluarOperacion(tokens))
                {
                    ResaltarEcuacion(row, horizontal: true);
                    pistasDisponibles--;
                    lblPistas.Text = $"Pistas: {pistasDisponibles}";
                    return;
                }
            }

            // Buscar vertical
            for (int col = 0; col < gridSize; col += 2)
            {
                string[] tokens = new string[5];
                bool completa = true;

                for (int row = 0; row < 5; row++)
                {
                    string txt = cellButtons[row, col].Text;
                    tokens[row] = txt;
                    if (string.IsNullOrEmpty(txt) && cellButtons[row, col].Enabled)
                        completa = false;
                }

                if (completa && !EvaluarOperacion(tokens))
                {
                    ResaltarEcuacion(col, horizontal: false);
                    pistasDisponibles--;
                    lblPistas.Text = $"Pistas: {pistasDisponibles}";
                    return;
                }
            }

            MessageBox.Show("No se encontraron ecuaciones incorrectas completas para señalar.");
        }

        private void ResaltarEcuacion(int index, bool horizontal)
        {
            for (int i = 0; i < 5; i++)
            {
                int r = horizontal ? index : i;
                int c = horizontal ? i : index;

                cellButtons[r, c].BackColor = Color.LightCoral;
            }

            // Animación temporal de retorno al color normal
            Timer t = new Timer { Interval = 1000 };
            t.Tick += (s, e) =>
            {
                for (int i = 0; i < 5; i++)
                {
                    int r = horizontal ? index : i;
                    int c = horizontal ? i : index;

                    if (cellButtons[r, c].Enabled)
                        cellButtons[r, c].BackColor = Color.White;
                    else
                        cellButtons[r, c].BackColor = Color.LightGray;
                }
                t.Stop();
            };
            t.Start();
        }



    }
}

//La base fundamental para la generación de niveles. 