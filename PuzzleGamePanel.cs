using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

// Assuming these supporting classes are defined in separate files within the MathCross namespace:
// PuzzleGenerator, NumberSelector, GameStateManager, LevelProgressManager, MusicWidget
// And Properties.Resources has star_full and star_empty images.

namespace MathCross
{
    public class PuzzleGamePanel : UserControl
    {
        // Grid and puzzle data configuration
        private int gridSize = 5; // Default, will be updated by level difficulty
        private Button[,] cellButtons; // To be initialized after gridSize is set
        private Panel sidebar;
        private int cellSize = 60;
        private int margin = 10;
        private string[,] puzzleData; // Solution from PuzzleGenerator

        private List<int> availableNumbers = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        // UI elements for game stats
        private Label lblTiempo, lblPuntos, lblErrores;
        private System.Windows.Forms.Timer gameTimer; // Specify System.Windows.Forms.Timer
        private int segundosTranscurridos = 0;
        private int puntos = 0;
        private int errores = 0;

        // Hints system
        private int pistasDisponibles = 3; // Default, updated by level
        private Button btnPista;
        private Label lblPistas;
        private bool pistaEstrategicaActiva = false; // Field to determine hint type

        // UI for level completion
        private Panel estrellasPanel;
        private Button btnVolverMenu;
        private Button btnContinuarNivel;
        private Label felicidadesLabel;

        private Button btnAtras;

        // Game state
        private string nivelActual;
        private bool modoPractica = false; // Determines if progress is saved

        public event Action OnCloseRequested; // Event for navigation

        /// <summary>
        /// Main constructor for the game panel.
        /// </summary>
        /// <param name="nivelId">The ID of the level to load.</param>
        /// <param name="enModoPractica">Whether the game is in practice mode.</param>
        public PuzzleGamePanel(string nivelId, bool enModoPractica = false)
        {
            this.nivelActual = nivelId;
            this.modoPractica = enModoPractica;

            // Initialize UserControl properties
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;

            // Load puzzle data and difficulty settings
            PuzzleGenerator gen = new PuzzleGenerator();
            var dificultad = gen.ObtenerDificultadPorNivel(nivelId); // Assumes this method exists in PuzzleGenerator

            this.gridSize = dificultad.Size; // Update gridSize from difficulty
            this.cellButtons = new Button[this.gridSize, this.gridSize]; // Initialize cellButtons with correct size

            gen.SetOperadores(dificultad.OperadoresPermitidos); // Assumes this method exists
            this.puzzleData = gen.GenerarPuzzle(this.gridSize, dificultad.PorcentajeCeldasOcultas); // Assumes this method exists

            this.pistasDisponibles = dificultad.MaxPistas;

            // Create UI elements
            CreateBackButton();
            CreateGrid();    // Depends on gridSize and puzzleData
            CreateSidebar(); // Depends on pistasDisponibles

            // Start game timer
            StartGameTimer();
        }

        /// <summary>
        /// Creates a new puzzle. Call this if you need to regenerate the puzzle with new parameters
        /// after the panel is already constructed. Be mindful of UI state.
        /// </summary>
        public string[,] GenerarPuzzle(int size, int porcentajeOcultas)
        {
            this.gridSize = size; // Update internal grid size
            this.cellButtons = new Button[this.gridSize, this.gridSize]; // Re-initialize for new size

            PuzzleGenerator gen = new PuzzleGenerator();
            // gen.SetOperadores(...); // Potentially set operators if they change
            this.puzzleData = gen.GenerarPuzzle(this.gridSize, porcentajeOcultas);

            // Clear existing grid controls before recreating
            ClearGridControls();
            CreateGrid(); // Rebuild the grid UI

            // Sidebar might need an update too if its content depends on puzzle specifics
            // For simplicity, not recreating sidebar here to avoid duplicate controls without proper handling.
            // An UpdateSidebar() method would be better if dynamic changes are needed.

            return this.puzzleData;
        }
        
        private void ClearGridControls()
        {
            if (cellButtons == null) return;
            for (int row = 0; row < cellButtons.GetLength(0); row++)
            {
                for (int col = 0; col < cellButtons.GetLength(1); col++)
                {
                    if (cellButtons[row, col] != null)
                    {
                        this.Controls.Remove(cellButtons[row, col]);
                        cellButtons[row, col].Dispose();
                        cellButtons[row, col] = null;
                    }
                }
            }
        }


        private void CreateBackButton()
        {
            btnAtras = new Button()
            {
                Text = "← Atrás",
                Size = new Size(80, 30),
                Location = new Point(10, 10),
                BackColor = Color.LightGray,
                Font = new Font("Arial", 9)
            };
            btnAtras.Click += (s, e) => GameStateManager.VolverAtras(); // Assumes GameStateManager.VolverAtras() exists
            this.Controls.Add(btnAtras);
        }

        public void AgregarPuntos(int cantidad)
        {
            puntos += cantidad;
            if (lblPuntos != null) lblPuntos.Text = $"Puntos: {puntos}";
        }

        public void RegistrarError()
        {
            errores++;
            if (lblErrores != null) lblErrores.Text = $"Errores: {errores}";
        }

        // This class seems to be a conceptual model, not directly driving the Button logic.
        // Buttons get their state directly from puzzleData in CreateGrid.
        public class PuzzleCell
        {
            public enum CellType { Empty, Number, Operator, Equals }
            public CellType Type { get; set; }
            public string Value { get; set; }
            public bool Editable => Type == CellType.Empty; // Original logic, might need adjustment
        }

        private void CreateGrid()
        {
            // Ensure previous grid buttons are cleared if this method can be called multiple times
            // ClearGridControls(); // Call this if CreateGrid can be re-invoked on an existing panel

            int offsetX = (btnAtras != null) ? btnAtras.Right + 15 : 20; // Position grid relative to back button
            int offsetY = (btnAtras != null) ? btnAtras.Top : 20;

            for (int row = 0; row < gridSize; row++)
            {
                for (int col = 0; col < gridSize; col++)
                {
                    Button btn = new Button();
                    btn.Size = new Size(cellSize, cellSize);
                    btn.Location = new Point(offsetX + col * (cellSize + margin), offsetY + row * (cellSize + margin));
                    btn.Font = new Font("Arial", 14, FontStyle.Bold);
                    btn.Tag = new Point(row, col); // Store cell coordinates

                    string content = puzzleData[row, col];

                    if (!string.IsNullOrEmpty(content))
                    {
                        if (EsNumero(content)) // If the solution cell is a number
                        {
                            // This is a cell the player might interact with.
                            // The PuzzleGenerator might leave some of these cells empty in puzzleData (player fills),
                            // or pre-fill some as initial hints.
                            // The original V2 code from user had: if(EsNumero(content)) { btn.Text = ""; /* editable */}
                            // This implies puzzleData contains the solution, and numeric cells are made blank for the player.
                            btn.Text = ""; // Player needs to fill this
                            btn.BackColor = Color.White;
                            btn.Click += OnEditableCellClick;
                            btn.Enabled = true;
                        }
                        else // Operator or "="
                        {
                            btn.Text = content;
                            btn.Enabled = false;
                            btn.BackColor = Color.LightGray;
                        }
                    }
                    else
                    {
                        // If puzzleData has an empty string for a cell that should be numeric.
                        // This means the player should fill it.
                        btn.Text = "";
                        btn.BackColor = Color.White;
                        btn.Click += OnEditableCellClick;
                        btn.Enabled = true;
                        // The original V2 code's else block for string.IsNullOrEmpty(content) was:
                        // btn.Enabled = false; btn.BackColor = Color.DarkGray;
                        // This implied empty content in puzzleData means a non-interactive cell.
                        // The current logic assumes empty content for numeric cells means player-editable.
                        // This should be consistent with how PuzzleGenerator prepares puzzleData.
                        // If PuzzleGenerator uses string.Empty for player-fillable number cells, current logic is fine.
                    }

                    this.Controls.Add(btn);
                    cellButtons[row, col] = btn;
                }
            }
        }

        // Keep only one definition of EsNumero
        private bool EsNumero(string val)
        {
            return int.TryParse(val, out _);
        }

        private void OnEditableCellClick(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            if (clickedButton == null) return;

            using (NumberSelector selector = new NumberSelector(availableNumbers)) // Assumes NumberSelector is a Form
            {
                // ShowDialog should ideally take an owner form
                if (selector.ShowDialog(this.FindForm()) == DialogResult.OK && selector.SelectedValue.HasValue)
                {
                    clickedButton.Text = selector.SelectedValue.Value.ToString();
                    AnimateCell(clickedButton);

                    if (TodasLasCeldasLlenas())
                    {
                        VerificarTableroCompleto(); // Renamed from VerificarTablero for clarity
                    }
                }
            }
        }

        private void AnimateCell(Button btn)
        {
            var originalSize = btn.Size;
            var animationTimer = new System.Windows.Forms.Timer(); // Be specific
            int frame = 0;
            animationTimer.Interval = 15;
            animationTimer.Tick += (s, ev) =>
            {
                frame++;
                if (frame <= 2)
                    btn.Size = new Size(originalSize.Width + 4, originalSize.Height + 4);
                else if (frame <= 4)
                    btn.Size = new Size(originalSize.Width - 2, originalSize.Height - 2);
                else
                {
                    btn.Size = originalSize;
                    animationTimer.Stop();
                    animationTimer.Dispose(); // Dispose timer
                }
            };
            animationTimer.Start();
        }

        private void CreateSidebar()
        {
            if (sidebar == null) // Create sidebar only if it doesn't exist
            {
                sidebar = new Panel()
                {
                    Width = 220,
                    Dock = DockStyle.Right,
                    BackColor = Color.FromArgb(240, 240, 240)
                };
                this.Controls.Add(sidebar);
            }
            else
            {
                sidebar.Controls.Clear(); // Clear if refreshing
            }


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
                Text = $"Puntos: {puntos}",
                Font = new Font("Arial", 12),
                Location = new Point(20, 100),
                AutoSize = true
            };
            sidebar.Controls.Add(lblPuntos);

            lblErrores = new Label()
            {
                Text = $"Errores: {errores}",
                Font = new Font("Arial", 12),
                Location = new Point(20, 140),
                AutoSize = true
            };
            sidebar.Controls.Add(lblErrores);

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
                Size = new Size(180, 35),
                Location = new Point(20, 210),
                BackColor = Color.LightBlue,
                Font = new Font("Arial", 10)
            };
            btnPista.Click += (s, ev) => // Corrected lambda syntax
            {
                if (pistaEstrategicaActiva) // pistaEstrategicaActiva is now a field
                    AplicarPistaEstrategica();
                else
                    UsarPistaSimple(); // Renamed from UsarPista for clarity
            };
            sidebar.Controls.Add(btnPista);
            
            MusicWidget musicWidget = new MusicWidget(); // Assumes MusicWidget exists
            musicWidget.Location = new Point(20, btnPista.Bottom + 15); // Position relative to hint button
            sidebar.Controls.Add(musicWidget);
        }

        private void StartGameTimer()
        {
            if (gameTimer == null)
            {
                gameTimer = new System.Windows.Forms.Timer();
                gameTimer.Interval = 1000;
                gameTimer.Tick += (s, e) =>
                {
                    segundosTranscurridos++;
                    if (lblTiempo != null)
                        lblTiempo.Text = $"Tiempo: {segundosTranscurridos / 60}:{(segundosTranscurridos % 60).ToString("D2")}";
                };
            }
            segundosTranscurridos = 0; // Reset
            if (lblTiempo != null) lblTiempo.Text = "Tiempo: 0:00";
            gameTimer.Start();
        }


        private void VerificarTableroCompleto() // Renamed from VerificarTablero
        {
            if (!TodasLasCeldasLlenas()) return; // Should not happen if called from OnEditableCellClick after this check

            gameTimer?.Stop(); // Stop timer

            bool filasOK = VerificarFilas();
            bool columnasOK = VerificarColumnas();

            if (filasOK && columnasOK)
            {
                int estrellasGanadas = CalcularEstrellas();
                AgregarPuntos(100); // Bonus for completion

                if (!modoPractica)
                {
                    // Assumes LevelProgressManager.CompletarNivel takes these parameters
                    LevelProgressManager.CompletarNivel(this.nivelActual, estrellasGanadas, segundosTranscurridos, puntos, errores);
                }
                MostrarPantallaDeVictoria(estrellasGanadas); // Renamed from MostrarEstrellasEnSidebar
            }
            else
            {
                RegistrarError();
                MessageBox.Show("Hay errores en las operaciones. Revisa tus respuestas.", "Verificación Fallida", MessageBoxButtons.OK, MessageBoxIcon.Error);
                gameTimer?.Start(); // Resume timer if verification failed
            }
        }

        private bool TodasLasCeldasLlenas()
        {
            if (cellButtons == null) return false;
            for (int r = 0; r < cellButtons.GetLength(0); r++)
            {
                for (int c = 0; c < cellButtons.GetLength(1); c++)
                {
                    if (cellButtons[r, c] != null && cellButtons[r, c].Enabled && string.IsNullOrEmpty(cellButtons[r, c].Text))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool VerificarFilas()
        {
            if (cellButtons == null) return false;
            for (int row = 0; row < this.gridSize; row++)
            {
                // Assuming equations are always 5 elements for verification for now
                if (this.gridSize != 5)
                {
                    // Add logic for other grid sizes or return true/false based on design
                    // For now, only validate if gridSize is 5 to match EvaluarOperacion
                    continue;
                }
                try
                {
                    string[] expr = new string[5];
                    for (int col = 0; col < 5; col++)
                    {
                        if (cellButtons[row,col] == null) return false; // Should not happen
                        expr[col] = cellButtons[row, col].Text;
                        if (string.IsNullOrEmpty(expr[col]) && cellButtons[row,col].Enabled) return false; // Incomplete
                    }

                    if (!EvaluarOperacion(expr))
                        return false;
                }
                catch (FormatException) { return false; } // Catch if TryParse fails inside EvaluarOperacion indirectly
                catch (Exception) { return false; } // General catch
            }
            return true;
        }

        private bool VerificarColumnas()
        {
            if (cellButtons == null) return false;
            for (int col = 0; col < this.gridSize; col++)
            {
                if (this.gridSize != 5)
                {
                    continue; // Only validate if gridSize is 5
                }
                try
                {
                    string[] expr = new string[5];
                    for (int row = 0; row < 5; row++)
                    {
                        if (cellButtons[row,col] == null) return false;
                        expr[row] = cellButtons[row, col].Text;
                        if (string.IsNullOrEmpty(expr[row]) && cellButtons[row,col].Enabled) return false; // Incomplete
                    }

                    if (!EvaluarOperacion(expr))
                        return false;
                }
                catch (FormatException) { return false; }
                catch (Exception) { return false; }
            }
            return true;
        }

        private bool EvaluarOperacion(string[] tokens)
        {
            if (tokens.Length != 5) return false;

            if (!int.TryParse(tokens[0], out int num1) ||
                !int.TryParse(tokens[2], out int num2) ||
                tokens[3] != "=" || // Equals sign must be present
                !int.TryParse(tokens[4], out int resultadoEsperado))
            {
                return false; // Invalid format or non-numeric values where expected
            }

            string op = tokens[1];
            switch (op)
            {
                case "+": return num1 + num2 == resultadoEsperado;
                case "-": return num1 - num2 == resultadoEsperado;
                case "×": // Fall-through for visual multiplication symbol
                case "*": return num1 * num2 == resultadoEsperado;
                case "÷": // Fall-through for visual division symbol
                case "/":
                    if (num2 == 0) return false; // Division by zero
                    // Assuming integer division for MathCross puzzles.
                    // If decimal results are allowed, this needs adjustment: (double)num1 / num2 == resultadoEsperado
                    return num1 % num2 == 0 && num1 / num2 == resultadoEsperado; // Exact integer division
                default: return false; // Unknown operator
            }
        }

        private int CalcularEstrellas()
        {
            // Simpler star calculation, can be expanded based on game's difficulty/design
            int estrellas = 1; // Base for completion
            if (errores == 0) estrellas++;
            if (segundosTranscurridos < (gridSize == 5 ? 90 : 180)) estrellas++; // Example time threshold

            return Math.Min(estrellas, 3); // Max 3 stars
        }

        private void MostrarPantallaDeVictoria(int cantidadEstrellas) // Renamed from MostrarEstrellasEnSidebar
        {
            if (sidebar == null) return;
            sidebar.Controls.Clear(); // Clear previous sidebar content for end-game screen

            felicidadesLabel = new Label()
            {
                Text = "¡Nivel Completado!",
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = Color.Green,
                Location = new Point(20, 20),
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter, // Center text if AutoSize is false and Width is set
                Width = sidebar.Width - 40 // Example width
            };
            sidebar.Controls.Add(felicidadesLabel);

            estrellasPanel = new Panel()
            {
                Size = new Size(180, 50), // Approx for 3 stars
                Location = new Point((sidebar.Width - 180) / 2, felicidadesLabel.Bottom + 20),
                BackColor = Color.Transparent
            };

            for (int i = 0; i < 3; i++)
            {
                PictureBox starPic = new PictureBox()
                {
                    Size = new Size(40, 40),
                    Location = new Point(i * (40 + 10), 5), // Star size + margin
                    Image = i < cantidadEstrellas ? Properties.Resources.star_full : Properties.Resources.star_empty, // Assumes these resources exist
                    SizeMode = PictureBoxSizeMode.Zoom
                };
                estrellasPanel.Controls.Add(starPic);
            }
            sidebar.Controls.Add(estrellasPanel);

            btnVolverMenu = new Button()
            {
                Text = "Volver al Menú",
                Size = new Size(160, 35),
                Location = new Point((sidebar.Width - 160) / 2, estrellasPanel.Bottom + 20),
                BackColor = Color.LightGray,
                Font = new Font("Arial", 10)
            };
            btnVolverMenu.Click += (s, e) =>
            {
                // GameStateManager.VolverAtras(); // Alternative if GameStateManager handles this
                OnCloseRequested?.Invoke(); // Notify parent to switch view
            };
            sidebar.Controls.Add(btnVolverMenu);

            string siguienteNivelId = LevelProgressManager.ObtenerSiguienteNivel(this.nivelActual); // Assumes this method exists
            if (!string.IsNullOrEmpty(siguienteNivelId))
            {
                btnContinuarNivel = new Button()
                {
                    Text = "Siguiente Nivel",
                    Size = new Size(160, 35),
                    Location = new Point((sidebar.Width - 160) / 2, btnVolverMenu.Bottom + 10),
                    BackColor = Color.LightGreen,
                    Font = new Font("Arial", 10)
                };
                btnContinuarNivel.Click += (s, e) =>
                {
                    if (this.Parent != null)
                    {
                        this.Parent.Controls.Remove(this);
                        PuzzleGamePanel proximoNivelPanel = new PuzzleGamePanel(siguienteNivelId, this.modoPractica);
                        // Propagate the close request handler if needed
                        // proximoNivelPanel.OnCloseRequested += ... 
                        this.Parent.Controls.Add(proximoNivelPanel);
                        this.Dispose(); // Dispose current panel
                    }
                };
                sidebar.Controls.Add(btnContinuarNivel);
            }
            
            // Fade-in animation for felicidadesLabel
            Timer fade = new Timer { Interval = 20 };
            int alpha = 0;
            felicidadesLabel.ForeColor = Color.FromArgb(alpha, Color.Green); // Initial transparent
            fade.Tick += (s, e) =>
            {
                alpha += 15;
                if (alpha >= 255)
                {
                    alpha = 255;
                    fade.Stop();
                    fade.Dispose(); // Dispose timer
                }
                felicidadesLabel.ForeColor = Color.FromArgb(alpha, Color.Green);
            };
            fade.Start();
        }


        private void UsarPistaSimple() // Renamed from UsarPista
        {
            if (pistasDisponibles <= 0)
            {
                MessageBox.Show("Ya no tienes pistas disponibles.", "Sin Pistas", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            for (int r = 0; r < gridSize; r++)
            {
                for (int c = 0; c < gridSize; c++)
                {
                    if (cellButtons[r, c] != null && cellButtons[r, c].Enabled && string.IsNullOrEmpty(cellButtons[r, c].Text))
                    {
                        string valorCorrecto = puzzleData[r, c]; // Get the answer from the original puzzle data
                        if (!string.IsNullOrEmpty(valorCorrecto) && EsNumero(valorCorrecto)) // Ensure it's a number cell to reveal
                        {
                            cellButtons[r, c].Text = valorCorrecto;
                            cellButtons[r, c].BackColor = Color.LightYellow; // Highlight revealed cell
                            cellButtons[r, c].Enabled = false; // Disable cell after revealing

                            pistasDisponibles--;
                            if (lblPistas != null) lblPistas.Text = $"Pistas: {pistasDisponibles}";
                            AnimateCell(cellButtons[r,c]); // Animate the revealed cell

                            if (TodasLasCeldasLlenas())
                            {
                                VerificarTableroCompleto();
                            }
                            return; // Use only one pista at a time
                        }
                    }
                }
            }
            MessageBox.Show("No quedan celdas vacías adecuadas para dar una pista.", "Pista no aplicable", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void AplicarPistaEstrategica()
        {
            if (pistasDisponibles <= 0)
            {
                MessageBox.Show("Ya no tienes pistas disponibles.", "Sin Pistas", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Simplified: Try to find a completed but incorrect row (assuming gridSize 5 for this logic)
            if (gridSize == 5) {
                for (int row = 0; row < gridSize; row++)
                {
                    string[] tokens = new string[5];
                    bool completa = true;
                    for (int col = 0; col < 5; col++)
                    {
                        if(cellButtons[row,col] == null) { completa = false; break; }
                        tokens[col] = cellButtons[row, col].Text;
                        if (string.IsNullOrEmpty(tokens[col]) && cellButtons[row,col].Enabled) // If an editable cell is empty
                            completa = false;
                    }

                    if (completa) {
                        try { // EvaluarOperacion might throw if tokens are not numbers where expected
                            if (!EvaluarOperacion(tokens))
                            {
                                ResaltarEcuacion(row, true); // true for horizontal
                                pistasDisponibles--;
                                if(lblPistas != null) lblPistas.Text = $"Pistas: {pistasDisponibles}";
                                return;
                            }
                        } catch (FormatException) { /* Malformed expression, skip */ }
                    }
                }

                // Try to find a completed but incorrect column
                for (int col = 0; col < gridSize; col++)
                {
                    string[] tokens = new string[5];
                    bool completa = true;
                    for (int row = 0; row < 5; row++)
                    {
                        if(cellButtons[row,col] == null) { completa = false; break; }
                        tokens[row] = cellButtons[row, col].Text;
                        if (string.IsNullOrEmpty(tokens[row]) && cellButtons[row,col].Enabled)
                            completa = false;
                    }

                    if (completa) {
                        try {
                            if (!EvaluarOperacion(tokens))
                            {
                                ResaltarEcuacion(col, false); // false for vertical
                                pistasDisponibles--;
                                if(lblPistas != null) lblPistas.Text = $"Pistas: {pistasDisponibles}";
                                return;
                            }
                        } catch (FormatException) { /* Malformed expression, skip */ }
                    }
                }
            }

            MessageBox.Show("No se encontraron ecuaciones incorrectas completas para señalar, o la función no soporta el tamaño actual de la cuadrícula.", "Pista Estratégica", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ResaltarEcuacion(int index, bool esHorizontal)
        {
            // Store original colors to revert
            List<Tuple<Button, Color>> originalColors = new List<Tuple<Button, Color>>();

            for (int i = 0; i < gridSize; i++) // Iterate through the line/column
            {
                int r = esHorizontal ? index : i;
                int c = esHorizontal ? i : index;

                if (r < gridSize && c < gridSize && cellButtons[r, c] != null)
                {
                    originalColors.Add(new Tuple<Button, Color>(cellButtons[r,c], cellButtons[r,c].BackColor));
                    cellButtons[r, c].BackColor = Color.LightCoral; // Highlight color
                }
            }

            System.Windows.Forms.Timer revertTimer = new System.Windows.Forms.Timer { Interval = 1500 };
            revertTimer.Tick += (s, e) =>
            {
                foreach(var item in originalColors)
                {
                    if(item.Item1 != null) item.Item1.BackColor = item.Item2; // Restore original color
                }
                revertTimer.Stop();
                revertTimer.Dispose(); // Dispose timer
            };
            revertTimer.Start();
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (gameTimer != null)
                {
                    gameTimer.Stop();
                    gameTimer.Dispose();
                    gameTimer = null;
                }
                // Dispose other timers if they are class members and not disposed in their own logic
            }
            base.Dispose(disposing);
        }
    }
}

//La pantalla principal del juego donde se juega el Puzzle. Carga un puzzle desde "PuzzleGenerator". Usa "MusicWidget" en el panel lateral. Usa "GameStateManager" para guardar estrellas y volver al menú y por ultimo, utiliza "LevelProgressManager" para guardar tiempo, estrellas y promedio del nivel. 