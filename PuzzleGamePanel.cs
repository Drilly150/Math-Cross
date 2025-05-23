using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MathCross
{
    public class PuzzleGamePanel : UserControl
    {
        private const int DefaultGridSize = 5; // Renamed gridSize to DefaultGridSize for clarity if needed, actual gridSize is now a field
        private Button[,] cellButtons; // Initialized in constructor based on gridSize
        private Panel sidebar;

        private int cellSize = 60;
        private int margin = 10;

        private string[,] puzzleData;
        private int gridSize; // Made gridSize a field to be set by level difficulty

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

        private Button btnAtras;

        // Field for strategic hint state - assumed default
        private bool pistaEstrategicaActiva = false;
        // Field for practice mode state
        private bool modoPractica = false;
        private string nivelActual; // Field to store the current level ID

        // Event for closing requests, to be handled by the parent form/manager
        public event EventHandler OnCloseRequested;


        // Constructor: Main way to create and initialize the puzzle panel for a specific level
        public PuzzleGamePanel(string nivelId)
        {
            this.nivelActual = nivelId;
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;

            PuzzleGenerator gen = new PuzzleGenerator();
            var dificultad = gen.ObtenerDificultadPorNivel(nivelId);

            this.gridSize = dificultad.Size; // Set grid size from difficulty
            this.cellButtons = new Button[this.gridSize, this.gridSize]; // Initialize cellButtons array with correct size
            gen.SetOperadores(dificultad.OperadoresPermitidos);
            puzzleData = gen.GenerarPuzzle(this.gridSize, dificultad.PorcentajeCeldasOcultas);

            pistasDisponibles = dificultad.MaxPistas;
            // modoPractica could be set based on nivelId or another parameter if needed
            // e.g., if (nivelId.StartsWith("Practice_")) { this.modoPractica = true; }


            CreateBackButton(); // Create back button first so it's potentially behind other elements if overlap
            CreateGrid();
            CreateSidebar(); // Sidebar should be created once
            
            // Ensure game timer starts after everything is set up
            InitializeGameTimer();
        }

        // Public method to potentially regenerate a puzzle.
        // Consider implications if called on an already initialized panel (clearing old controls, etc.)
        // For now, assumes it's for a new setup or a controlled reset.
        public string[,] GenerarPuzzle(int size, int porcentajeOcultas)
        {
            this.Dock = DockStyle.Fill; // Should ideally be set once
            this.BackColor = Color.White; // Should ideally be set once

            this.gridSize = size; // Update the class field for grid size
            this.cellButtons = new Button[this.gridSize, this.gridSize]; // Re-initialize for new size

            PuzzleGenerator gen = new PuzzleGenerator();
            // Assuming PuzzleGenerator can be configured or has appropriate methods
            // gen.SetOperadores(...); // Might be needed if operators can change per generation
            puzzleData = gen.GenerarPuzzle(this.gridSize, porcentajeOcultas);

            // Important: If this method is called on an existing panel,
            // existing grid controls need to be cleared and disposed of properly before recreating.
            // For simplicity, this example assumes CreateGrid handles it or this is for a fresh setup.
            // A more robust solution would be to have a dedicated panel for grid buttons and clear that panel.
            
            // Clear existing grid buttons if any
            ClearGridControls();

            CreateGrid(); // Re-creates buttons based on new puzzleData and gridSize
            
            // Sidebar should ideally not be recreated if it exists.
            // If it needs updates, specific update methods are better.
            // If CreateSidebar is called again, it might add duplicate controls.
            // For now, commenting out to prevent duplication if GenerarPuzzle is called mid-game.
            // If a full UI reset is intended, CreateSidebar might need to be called,
            // but it should also clear its previous controls.
            // CreateSidebar(); 

            return puzzleData;
        }
        
        private void ClearGridControls()
        {
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
                Location = new Point(10, 10), // Ensure this doesn't overlap with grid
                BackColor = Color.LightGray,
                Font = new Font("Arial", 9)
            };

            // Assuming GameStateManager is a static class or you have an instance
            btnAtras.Click += (s, e) => {
                gameTimer?.Stop(); // Stop timer before going back
                // GameStateManager.VolverAtras(); // If GameStateManager handles view changes
                OnCloseRequested?.Invoke(this, EventArgs.Empty); // More generic way to request close
            };
            this.Controls.Add(btnAtras);
            btnAtras.BringToFront(); // Ensure back button is visible
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

        // PuzzleCell class seems to be a conceptual representation, not directly used for Button logic in this snippet.
        // If it were to be used, cellButtons[,] would be of type PuzzleCell or Buttons would store PuzzleCell in Tag.
        public class PuzzleCell
        {
            public enum CellType { Empty, Number, Operator, Equals }
            public CellType Type { get; set; }
            public string Value { get; set; }
            public bool Editable => Type == CellType.Empty; // Original logic; typically numbers are editable, not empty cells
        }

        private void CreateGrid()
        {
            int offsetX = btnAtras != null ? btnAtras.Right + 20 : 40; // Adjust offset if back button exists
            int offsetY = btnAtras != null ? btnAtras.Top : 40; // Align top or provide margin

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

                    // Cells are determined by puzzleData.
                    // Numbers are initially hidden (empty text) and editable.
                    // Operators and '=' are shown and disabled.
                    // Null or empty strings in puzzleData for a cell means it's part of the structure but not interactive initially (e.g. truly empty, or for aesthetics)
                    // For this game, it seems all cells should have some content from puzzleData (number, operator, or '=')
                    // If puzzleData[row,col] itself is empty/null, it means it's a cell to be filled by the player or it's an invalid puzzle.
                    // Let's assume puzzleData always provides a hint: a number (to be guessed) or an operator/equals.

                    if (!string.IsNullOrEmpty(content))
                    {
                        if (EsNumero(content)) // If the true value is a number, it's a guessable cell
                        {
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
                        // This case implies the puzzle definition from PuzzleGenerator
                        // has an empty cell that is not a number placeholder.
                        // For MathCross, cells are usually numbers or fixed operators/equals.
                        // If it's an empty cell that should be filled by the player, EsNumero(puzzleData[row,col]) would handle it if puzzleData held the answer.
                        // If puzzleData has "" for a cell the player should fill, then this logic needs adjustment.
                        // For now, assume empty content from puzzleData means it's a non-interactive, dark cell.
                        // This might need to align with how PuzzleGenerator defines "occultas" (hidden cells for player input)
                        btn.Text = ""; // Should be empty for player input
                        btn.BackColor = Color.White; // Editable cells are white
                        btn.Click += OnEditableCellClick;
                        btn.Enabled = true;
                        // However, the original logic:
                        // if (!string.IsNullOrEmpty(content)) { if (EsNumero(content)) { btn.Text = ""; /* editable */ } else { /* fixed op */ } }
                        // else { btn.Enabled = false; btn.BackColor = Color.DarkGray; /* non-interactive */ }
                        // This implies if puzzleData has "" for a cell, it's disabled and dark. This is unusual for a player-fillable cell.
                        // Let's stick to the original interpretation: if puzzleData[row,col] is a number, it's fillable.
                        // The else case (IsNullOrEmpty) should ideally not happen if puzzleData correctly represents all cells.
                        // For safety, let's assume if content is null/empty from puzzleData, it's an error or non-interactive.
                        // Reverting to a safer interpretation of original logic for cells not holding numbers:
                        // If puzzleData[r,c] is a number, it's a player cell (initially blank text).
                        // If puzzleData[r,c] is an operator/equals, it's fixed.
                        // If puzzleData[r,c] is string.Empty, it implies a cell not meant for numbers (e.g. structural, or an issue in puzzleData)
                        // Given the original:
                        // if (!string.IsNullOrEmpty(content)) { if (EsNumero(content)) { btn.Text = ""; } else { btn.Text = content; btn.Enabled=false; }}
                        // else { btn.Enabled = false; btn.BackColor = DarkGray }
                        // This means only cells from puzzleData that are numbers are playable.
                        // If a cell in puzzleData is empty, it's disabled. This seems correct if the puzzle generator defines some cells as "truly blank" / non-interactive.

                        // Corrected logic based on re-evaluation:
                        // A cell is either:
                        // 1. A number placeholder (puzzleData[r,c] is the actual number, btn.Text is initially empty) -> Editable
                        // 2. An operator/equals sign (puzzleData[r,c] is the operator, btn.Text is the operator) -> Disabled
                        // 3. If puzzleData[r,c] is empty or null from generator, it implies a structural non-interactive cell.
                        // The provided code from user had:
                        // if (!string.IsNullOrEmpty(content)) { if (EsNumero(content)) { btn.Text = ""; btn.BackColor = Color.White; btn.Click += OnEditableCellClick; } ...}
                        // else { btn.Enabled = false; btn.BackColor = Color.DarkGray; }
                        // This means if `puzzleData[row,col]` is empty, it becomes a disabled dark cell. This is what I'll stick to.
                        btn.Enabled = false;
                        btn.BackColor = Color.DarkGray;
                    }

                    this.Controls.Add(btn);
                    cellButtons[row, col] = btn;
                }
            }
        }

        // Only one definition of EsNumero needed
        private bool EsNumero(string val)
        {
            return int.TryParse(val, out _);
        }

        private void OnEditableCellClick(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            if (clickedButton == null) return;

            using (NumberSelector selector = new NumberSelector(availableNumbers))
            {
                if (selector.ShowDialog(this.FindForm()) == DialogResult.OK) // Pass owner form for dialog
                {
                    clickedButton.Text = selector.SelectedValue.ToString();
                    AnimateCell(clickedButton);

                    if (TodasLasCeldasLlenas())
                    {
                        VerificarTablero();
                    }
                    // The LevelProgressManager.CompletarNivel call was removed from here
                    // as VerificarTablero now handles it correctly with this.nivelActual.
                }
            }
        }

        private void AnimateCell(Button btn)
        {
            var originalSize = btn.Size;
            var animationTimer = new System.Windows.Forms.Timer(); // Be specific if System.Timers.Timer is also used
            int frame = 0;
            animationTimer.Interval = 15;
            animationTimer.Tick += (s, e) =>
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
                    animationTimer.Dispose(); // Dispose timer when done
                }
            };
            animationTimer.Start();
        }

        private void CreateSidebar()
        {
            if (sidebar == null) // Create sidebar only once
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
                sidebar.Controls.Clear(); // Clear existing controls if refreshing
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
                Text = $"Puntos: {puntos}", // Initial points
                Font = new Font("Arial", 12),
                Location = new Point(20, 100),
                AutoSize = true
            };
            sidebar.Controls.Add(lblPuntos);

            lblErrores = new Label()
            {
                Text = $"Errores: {errores}", // Initial errores
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
                Size = new Size(160, 35),
                Location = new Point(20, 210),
                BackColor = Color.LightBlue,
                Font = new Font("Arial", 10)
            };
            btnPista.Click += (s, e) =>
            {
                if (pistaEstrategicaActiva) // pistaEstrategicaActiva is now a field
                    AplicarPistaEstrategica();
                else
                    UsarPista();
            };
            sidebar.Controls.Add(btnPista);

            // MusicWidget - Assuming MusicWidget is a UserControl or similar
            // Ensure MusicWidget class is correctly defined in its own file or accessible.
            MusicWidget widget = new MusicWidget();
            widget.Location = new Point(10, sidebar.Height - widget.Height - 10); // Example positioning at bottom
            widget.Anchor = AnchorStyles.Bottom | AnchorStyles.Left; // Anchor if sidebar resizes
            sidebar.Controls.Add(widget);
        }
        
        private void InitializeGameTimer()
        {
            if (gameTimer == null)
            {
                gameTimer = new Timer();
                gameTimer.Interval = 1000;
                gameTimer.Tick += (s, e) =>
                {
                    segundosTranscurridos++;
                    if (lblTiempo != null) 
                        lblTiempo.Text = $"Tiempo: {segundosTranscurridos / 60}:{(segundosTranscurridos % 60).ToString("D2")}";
                };
            }
            segundosTranscurridos = 0; // Reset timer
            if (lblTiempo != null) lblTiempo.Text = "Tiempo: 0:00";
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
                gameTimer.Stop(); // Stop timer on successful completion
                int estrellasCalculadas = CalcularEstrellas();
                AgregarPuntos(100); // Extra points for completing

                // Use this.nivelActual instead of hardcoded "P1"
                LevelProgressManager.CompletarNivel(this.nivelActual, estrellasCalculadas, segundosTranscurridos);
                MostrarEstrellasEnSidebar(estrellasCalculadas); // Changed method name for clarity

                return true;
            }
            else
            {
                RegistrarError();
                MessageBox.Show("Hay errores en las operaciones. Revisa tus respuestas.", "Verificación Fallida", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private bool TodasLasCeldasLlenas()
        {
            foreach (Button btn in cellButtons)
            {
                // An enabled button that is empty means not all cells are filled.
                // If btn is null (e.g. gridSize changed and array not fully populated yet), skip.
                if (btn != null && btn.Enabled && string.IsNullOrEmpty(btn.Text))
                    return false;
            }
            return true;
        }

        private bool VerificarFilas()
        {
            for (int row = 0; row < gridSize; row++)
            {
                // Equations are typically every other row if grid includes results within the same structure.
                // Assuming standard MathCross where each number cell participates.
                // For a 5x5 grid, this implies expressions like N Op N = N.
                // Need to ensure we only try to evaluate valid equation structures.
                // If gridSize implies equations are not always 5 tokens long, this needs adjustment.
                // Current EvaluarOperacion expects 5 tokens.
                if (gridSize != 5 && (row % 2 != 0)) continue; // Skip if not an equation row for non-5x5, or adjust logic
                if (gridSize != 5) { /* More complex logic needed for general gridSize */ }


                try
                {
                    string[] expr = new string[gridSize]; // Assuming expression length matches gridSize
                    for (int col = 0; col < gridSize; col++)
                    {
                        if (cellButtons[row, col] == null) return false; // Grid not fully initialized
                        expr[col] = cellButtons[row, col].Text;
                        if (string.IsNullOrEmpty(expr[col]) && cellButtons[row,col].Enabled) return false; // Empty editable cell
                    }
                    
                    // If EvaluarOperacion strictly needs 5 tokens, adjust this part or EvaluarOperacion
                    if (gridSize == 5 && !EvaluarOperacion(expr)) // Only evaluate if gridSize is 5
                        return false;
                    else if (gridSize != 5)
                    {
                        // Implement or call a more generic evaluation method for other grid sizes
                        // For now, assume gridSize 5 for this evaluation part, or return false.
                        // return false; // Or handle other grid sizes
                    }
                }
                catch (FormatException) // Catch if int.Parse fails for an incomplete/invalid number
                {
                    return false; // Invalid format in a cell considered an error
                }
                catch (Exception) // General catch, consider logging
                {
                    return false; // Any other error during evaluation
                }
            }
            return true;
        }

        private bool VerificarColumnas()
        {
            for (int col = 0; col < gridSize; col++)
            {
                 if (gridSize != 5 && (col % 2 != 0)) continue; // Skip if not an equation column
                 if (gridSize != 5) { /* More complex logic needed */ }

                try
                {
                    string[] expr = new string[gridSize];
                    for (int row = 0; row < gridSize; row++)
                    {
                        if (cellButtons[row, col] == null) return false;
                        expr[row] = cellButtons[row, col].Text;
                        if (string.IsNullOrEmpty(expr[row]) && cellButtons[row,col].Enabled) return false;
                    }

                    if (gridSize == 5 && !EvaluarOperacion(expr))
                        return false;
                    else if (gridSize != 5)
                    {
                        // return false; // Or handle other grid sizes
                    }
                }
                catch (FormatException)
                {
                    return false;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return true;
        }

        private bool EvaluarOperacion(string[] tokens)
        {
            // This evaluation strictly expects a 5-token expression: num op num = num
            if (tokens.Length != 5) return false; // Or throw new ArgumentException("Expression must have 5 tokens.");

            if (string.IsNullOrEmpty(tokens[0]) || string.IsNullOrEmpty(tokens[1]) ||
                string.IsNullOrEmpty(tokens[2]) || string.IsNullOrEmpty(tokens[3]) || string.IsNullOrEmpty(tokens[4]))
            {
                return false; // Incomplete expression
            }

            int a, b, resultado;
            if (!int.TryParse(tokens[0], out a)) return false;
            string op = tokens[1];
            if (!int.TryParse(tokens[2], out b)) return false;
            string igual = tokens[3];
            if (!int.TryParse(tokens[4], out resultado)) return false;


            if (igual != "=") return false;

            switch (op)
            {
                case "+": return a + b == resultado;
                case "-": return a - b == resultado;
                case "×": case "*": return a * b == resultado;
                case "÷": case "/": return b != 0 && (double)a / b == resultado; // Use floating point for division check if result can be non-integer. Or ensure integer division.
                                                                         // If results must be integers, then a % b == 0 && a / b == resultado
                default: return false;
            }
        }

        private int CalcularEstrellas()
        {
            int estrellas = 1; // Base star for completion

            if (errores == 0) // Stricter: 0 errors for a star
                estrellas++;
            else if (errores <= 2) // Looser: 1-2 errors still gets a star (adjust as needed)
                estrellas++;


            // Time-based star: adjust threshold as needed
            if (segundosTranscurridos <= 60) // e.g., 1 minute for a 5x5
                estrellas++;
            else if (segundosTranscurridos <= 120 && estrellas < 3) // Tiered time
                estrellas++;


            // Ensure max 3 stars
            return Math.Min(estrellas, 3);
        }

        private void MostrarEstrellasEnSidebar(int cantidadEstrellas)
        {
            if (sidebar == null) return; // Sidebar not created
            sidebar.Controls.Clear(); // Clear previous sidebar content (timer, points, etc.)

            felicidadesLabel = new Label()
            {
                Text = "¡Felicidades!",
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = Color.Green,
                Location = new Point(20, 20),
                AutoSize = true
            };
            sidebar.Controls.Add(felicidadesLabel);

            estrellasPanel = new Panel()
            {
                Size = new Size(180, 50), // Width for 3 stars (40*3 + 10*2 margins approx)
                Location = new Point(20, 60),
                BackColor = Color.Transparent // Or sidebar.BackColor
            };

            for (int i = 0; i < 3; i++) // Always show 3 star placeholders
            {
                PictureBox starPic = new PictureBox() // Renamed from star to avoid conflict
                {
                    Size = new Size(40, 40),
                    Location = new Point(i * (40 + 10), 5), // Star size + margin
                    // Ensure Properties.Resources.star_full and star_empty exist
                    Image = i < cantidadEstrellas ? Properties.Resources.star_full : Properties.Resources.star_empty,
                    SizeMode = PictureBoxSizeMode.Zoom
                };
                estrellasPanel.Controls.Add(starPic);
            }
            sidebar.Controls.Add(estrellasPanel);

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
                // No need to call LevelProgressManager.CompletarNivel again here.
                OnCloseRequested?.Invoke(this, EventArgs.Empty); // Request parent to handle view change
                // Example of how parent might handle:
                // if (this.ParentForm is MainForm main) { main.ShowLevelSelect(this.nivelActual); }
            };
            sidebar.Controls.Add(btnVolverMenu);

            // Logic for "Siguiente nivel" button
            // Determine if there IS a next level before showing the button
            string proximoNivelId = LevelProgressManager.ObtenerSiguienteNivel(this.nivelActual); // Assumed method

            if (!string.IsNullOrEmpty(proximoNivelId))
            {
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
                    // No need to call LevelProgressManager.CompletarNivel again.
                    // Request parent to load the next level
                    if (this.Parent is Panel parentPanel) // Or whatever container you use
                    {
                        parentPanel.Controls.Remove(this);
                        PuzzleGamePanel siguienteNivelPanel = new PuzzleGamePanel(proximoNivelId);
                        // siguienteNivelPanel.OnCloseRequested += ... (re-hook event if needed)
                        parentPanel.Controls.Add(siguienteNivelPanel);
                        this.Dispose();
                    }
                    else
                    {
                         OnCloseRequested?.Invoke(this, new NextLevelEventArgs(proximoNivelId)); // Alternative: event to request next level
                    }
                };
                sidebar.Controls.Add(btnContinuarNivel);
            }


            // Simple fade-in animation for the stars and label
            Timer fadeTimer = new Timer { Interval = 20 };
            int alpha = 0;
            felicidadesLabel.ForeColor = Color.FromArgb(alpha, Color.Green); // Initial transparent
            // estrellasPanel does not have ForeColor. Animate child PictureBoxes if needed, or Panel's content.
            // For simplicity, animating only felicidadesLabel's fade-in.
            fadeTimer.Tick += (s, e) =>
            {
                alpha += 15;
                if (alpha >= 255)
                {
                    alpha = 255;
                    fadeTimer.Stop();
                    fadeTimer.Dispose();
                }
                felicidadesLabel.ForeColor = Color.FromArgb(alpha, Color.Green);
            };
            fadeTimer.Start();
        }


        private void UsarPista()
        {
            if (pistasDisponibles <= 0)
            {
                MessageBox.Show("Ya no tienes pistas disponibles.", "Sin Pistas", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Find the first empty, enabled cell and reveal its correct value
            for (int r = 0; r < gridSize; r++)
            {
                for (int c = 0; c < gridSize; c++)
                {
                    if (cellButtons[r, c] != null && cellButtons[r, c].Enabled && string.IsNullOrEmpty(cellButtons[r, c].Text))
                    {
                        string valorCorrecto = puzzleData[r, c]; // Get the answer from the original puzzle data
                        if (!string.IsNullOrEmpty(valorCorrecto) && EsNumero(valorCorrecto)) // Ensure it's a number cell
                        {
                            cellButtons[r, c].Text = valorCorrecto;
                            cellButtons[r, c].BackColor = Color.LightYellow; // Highlight revealed cell
                            cellButtons[r, c].Enabled = false; // Disable cell after revealing

                            pistasDisponibles--;
                            if(lblPistas != null) lblPistas.Text = $"Pistas: {pistasDisponibles}";
                            
                            // Check if revealing this cell completes the puzzle
                            if (TodasLasCeldasLlenas())
                            {
                                VerificarTablero();
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

            // Try to find a completed but incorrect row
            for (int row = 0; row < gridSize; row++)
            {
                // Assuming equations are always 5 elements for strategic hints
                if (gridSize != 5) continue; // Or adapt for other sizes

                string[] tokens = new string[5];
                bool filaCompleta = true;
                for (int col = 0; col < 5; col++)
                {
                    if (cellButtons[row, col] == null) { filaCompleta = false; break; }
                    tokens[col] = cellButtons[row, col].Text;
                    if (string.IsNullOrEmpty(tokens[col]) && cellButtons[row,col].Enabled) // Check if an editable cell is empty
                    {
                        filaCompleta = false;
                        break;
                    }
                }

                if (filaCompleta)
                {
                    try {
                        if (!EvaluarOperacion(tokens))
                        {
                            ResaltarEcuacion(row, true); // true for horizontal
                            pistasDisponibles--;
                                if(lblPistas != null) lblPistas.Text = $"Pistas: {pistasDisponibles}";
                            return;
                        }
                    } catch (FormatException) { /* Skip if malformed, not truly complete */ }
                }
            }

            // Try to find a completed but incorrect column
            for (int col = 0; col < gridSize; col++)
            {
                if (gridSize != 5) continue;

                string[] tokens = new string[5];
                bool columnaCompleta = true;
                for (int row = 0; row < 5; row++)
                {
                    if (cellButtons[row, col] == null) { columnaCompleta = false; break; }
                    tokens[row] = cellButtons[row, col].Text;
                        if (string.IsNullOrEmpty(tokens[row]) && cellButtons[row,col].Enabled)
                    {
                        columnaCompleta = false;
                        break;
                    }
                }

                if (columnaCompleta)
                {
                    try {
                        if (!EvaluarOperacion(tokens))
                        {
                            ResaltarEcuacion(col, false); // false for vertical
                            pistasDisponibles--;
                            if(lblPistas != null) lblPistas.Text = $"Pistas: {pistasDisponibles}";
                            return;
                        }
                    } catch (FormatException) { /* Skip */ }
                }
            }

            MessageBox.Show("No se encontraron ecuaciones completas incorrectas para señalar.", "Pista Estratégica", MessageBoxButtons.OK, MessageBoxIcon.Information);
            // Optionally, fallback to regular hint if strategic one not found and pistaEstrategicaActiva was true
            // if (pistaEstrategicaActiva) UsarPista();
        }

        private void ResaltarEcuacion(int index, bool esHorizontal)
        {
            Color originalBackColor;
            List<Tuple<Button, Color>> originalColors = new List<Tuple<Button, Color>>();

            for (int i = 0; i < gridSize; i++) // Assuming equations span the full grid size
            {
                int r = esHorizontal ? index : i;
                int c = esHorizontal ? i : index;

                if (r < gridSize && c < gridSize && cellButtons[r, c] != null)
                {
                    originalColors.Add(Tuple.Create(cellButtons[r,c], cellButtons[r,c].BackColor));
                    cellButtons[r, c].BackColor = Color.LightCoral;
                }
            }

            Timer t = new Timer { Interval = 2000 }; // Increased highlight duration
            t.Tick += (s, e) =>
            {
                foreach(var item in originalColors)
                {
                    item.Item1.BackColor = item.Item2; // Restore original color
                }
                t.Stop();
                t.Dispose();
            };
            t.Start();
        }
    }

    // Example EventArgs for requesting next level, if using an event-based approach
    public class NextLevelEventArgs : EventArgs
    {
        public string NextLevelId { get; }
        public NextLevelEventArgs(string nextLevelId) { NextLevelId = nextLevelId; }
    }

    // NOTE TO USER:
    // The following classes (GameStateManager, LevelProgressManager, MusicWidget, NumberSelector, PuzzleGenerator, EcuacionLayout)
    // should be defined in their OWN SEPARATE .cs FILES.
    // If you have their full definitions pasted at the END of YOUR PuzzleGamePanel.cs file (e.g., around lines 786, 809, etc.,
    // as indicated by your original compiler errors), YOU MUST DELETE THOSE DUPLICATE DEFINITIONS from PuzzleGamePanel.cs.
    // This file should only contain the PuzzleGamePanel class and related helper classes if they are very small and specific to it.

    /*
    Example: GameStateManager.cs would look something like:
    namespace MathCross
    {
        public static class GameStateManager
        {
            public static void VolverAtras()
            {
                // Logic to go back, e.g., show main menu
                // This might involve interacting with the main form or a navigation service
            }
            // ... other game state methods
        }
    }
    */
}

//La pantalla principal del juego donde se juega el Puzzle. Carga un puzzle desde "PuzzleGenerator". Usa "MusicWidget" en el panel lateral. Usa "GameStateManager" para guardar estrellas y volver al menú y por ultimo, utiliza "LevelProgressManager" para guardar tiempo, estrellas y promedio del nivel. 