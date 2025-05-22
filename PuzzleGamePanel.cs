using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
// Asumo que estas clases existen en tu proyecto y namespace.
// using MathCross.Helpers; // Para PuzzleGenerator, LevelProgressManager, etc. si están en otro namespace.

namespace MathCross
{
    public class PuzzleGamePanel : UserControl
    {
        // Constantes y configuración de la cuadrícula
        private int gridSize = 5; // Valor por defecto, se puede sobrescribir por nivel
        private Button[,] cellButtons;
        private Panel sidebar;
        private int cellSize = 60;
        private int margin = 10;

        // Datos del puzzle y números disponibles para el selector
        private string[,] puzzleData; // Contendrá la solución del puzzle
        private List<int> availableNumbers = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        // Controles de UI para estadísticas del juego
        private Label lblTiempo, lblPuntos, lblErrores;
        private Timer gameTimer;
        private int segundosTranscurridos = 0;
        private int puntos = 0;
        private int errores = 0;

        // Sistema de pistas
        private int pistasDisponibles = 3; // Valor por defecto, se puede sobrescribir por nivel
        private Button btnPista;
        private Label lblPistas;
        private bool pistaEstrategicaActiva = false; // Asumo que esto es un campo, determina el tipo de pista

        // UI para finalización de nivel
        private Panel estrellasPanel;
        private Button btnVolverMenu;
        private Button btnContinuarNivel;
        private Label felicidadesLabel;

        // Botón para volver atrás (por ejemplo, al menú de selección de nivel)
        private Button btnAtras;

        // Identificador del nivel actual y modo de práctica
        private string nivelActual;
        private bool modoPractica = false; // Determina si se guarda el progreso

        // Evento para solicitar cierre (útil para que el contenedor principal maneje la navegación)
        public event Action OnCloseRequested;

        /// <summary>
        /// Constructor principal para el panel del juego.
        /// </summary>
        /// <param name="nivelId">El identificador único del nivel a cargar.</param>
        /// <param name="enModoPractica">Indica si el juego está en modo práctica (no guarda progreso).</param>
        public PuzzleGamePanel(string nivelId, bool enModoPractica = false)
        {
            this.nivelActual = nivelId;
            this.modoPractica = enModoPractica;
            this.cellButtons = new Button[gridSize, gridSize]; // Inicializar aquí por si gridSize cambia

            // Configuración inicial del UserControl
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;

            // Cargar datos y configuración del puzzle según el nivelId
            PuzzleGenerator gen = new PuzzleGenerator(); // Asumo que PuzzleGenerator existe
            var dificultad = gen.ObtenerDificultadPorNivel(nivelId); // Asumo este método existe

            this.gridSize = dificultad.Size; // Actualiza gridSize según la dificultad
            this.cellButtons = new Button[gridSize, gridSize]; // Re-inicializar con el tamaño correcto
            gen.SetOperadores(dificultad.OperadoresPermitidos); // Asumo este método existe
            this.puzzleData = gen.GenerarPuzzle(gridSize, dificultad.PorcentajeCeldasOcultas); // Asumo este método existe

            this.pistasDisponibles = dificultad.MaxPistas; // Cargar pistas según dificultad

            // Crear los elementos de la UI
            CreateBackButton();
            CreateGrid();
            CreateSidebar();

            // Iniciar temporizador del juego
            StartGameTimer();
        }

        /// <summary>
        /// Crea el botón para volver a la pantalla anterior.
        /// </summary>
        private void CreateBackButton()
        {
            btnAtras = new Button()
            {
                Text = "← Atrás",
                Size = new Size(80, 30),
                Location = new Point(10, 10), // Ajustar posición según sea necesario
                BackColor = Color.LightGray,
                Font = new Font("Arial", 9)
            };

            // Asumo que GameStateManager.VolverAtras() maneja la lógica de navegación
            btnAtras.Click += (s, e) => GameStateManager.VolverAtras();
            this.Controls.Add(btnAtras);
        }

        /// <summary>
        /// Crea la cuadrícula de botones para el puzzle.
        /// </summary>
        private void CreateGrid()
        {
            int offsetX = btnAtras != null ? btnAtras.Right + 20 : 40; // Posicionar la cuadrícula después del botón "Atrás"
            int offsetY = btnAtras != null ? btnAtras.Top : 40;

            for (int row = 0; row < gridSize; row++)
            {
                for (int col = 0; col < gridSize; col++)
                {
                    Button btn = new Button();
                    btn.Size = new Size(cellSize, cellSize);
                    btn.Location = new Point(offsetX + col * (cellSize + margin), offsetY + row * (cellSize + margin));
                    btn.Font = new Font("Arial", 14, FontStyle.Bold);
                    btn.Tag = new Point(row, col); // Guardar la posición para referencia

                    string content = puzzleData[row, col]; // puzzleData contiene la SOLUCIÓN

                    // Si la celda en puzzleData está vacía o es un número,
                    // significa que es una celda que el jugador debe llenar.
                    // El PuzzleGenerator debe haber dejado algunas celdas vacías (o con el número a adivinar)
                    // y otras con operadores/igual.
                    if (string.IsNullOrEmpty(content) || EsNumero(content)) // Celda para que el jugador ingrese un número
                    {
                        // Si puzzleData tiene un número aquí, es una celda pre-llenada (parte de la pista inicial)
                        // Si está vacía en puzzleData, es una celda que el jugador DEBE llenar.
                        // Para el propósito de la UI, si es un número en puzzleData, lo mostramos como pista.
                        // Si es un string vacío en puzzleData, el jugador la llena.
                        // La lógica de "PorcentajeCeldasOcultas" en PuzzleGenerator debería manejar esto.

                        // Si el PuzzleGenerator marca celdas a ocultar con un placeholder (ej. "?") o si simplemente
                        // queremos que todas las celdas numéricas (según puzzleData) sean rellenables por el jugador,
                        // la siguiente lógica se aplica:
                        if (string.IsNullOrEmpty(content)) // Celda que el jugador debe rellenar
                        {
                            btn.Text = ""; // Vacía para el jugador
                            btn.BackColor = Color.White;
                            btn.Click += OnEditableCellClick;
                        }
                        else // Celda con un número de la solución (pre-llenada como pista)
                        {
                            btn.Text = content;
                            btn.BackColor = Color.LightCyan; // Un color diferente para pistas iniciales
                            btn.Enabled = false; // No editable si es parte de la pista inicial
                        }
                    }
                    else // Celda con operador o "=" (no editable)
                    {
                        btn.Text = content;
                        btn.Enabled = false;
                        btn.BackColor = Color.LightGray;
                    }
                    this.Controls.Add(btn);
                    cellButtons[row, col] = btn;
                }
            }
        }

        /// <summary>
        /// Verifica si un string es un número.
        /// </summary>
        private bool EsNumero(string val)
        {
            return int.TryParse(val, out _);
        }

        /// <summary>
        /// Manejador de clic para celdas editables.
        /// </summary>
        private void OnEditableCellClick(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            if (clickedButton == null) return;

            // Asumo que NumberSelector es un Form o UserControl que devuelve un DialogResult
            using (NumberSelector selector = new NumberSelector(availableNumbers))
            {
                if (selector.ShowDialog() == DialogResult.OK && selector.SelectedValue.HasValue)
                {
                    clickedButton.Text = selector.SelectedValue.ToString();
                    AnimateCell(clickedButton); // Pequeña animación al colocar número

                    // Verificar si el tablero está completo después de cada entrada
                    if (TodasLasCeldasLlenas())
                    {
                        VerificarTableroCompleto();
                    }
                }
            }
        }

        /// <summary>
        /// Anima una celda brevemente después de que se ingresa un número.
        /// </summary>
        private void AnimateCell(Button btn)
        {
            var originalSize = btn.Size;
            var animationTimer = new System.Windows.Forms.Timer(); // Especificar System.Windows.Forms.Timer
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
                    animationTimer.Dispose(); // Liberar recursos del timer
                }
            };
            animationTimer.Start();
        }

        /// <summary>
        /// Crea el panel lateral (sidebar) con información y controles.
        /// </summary>
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
                Size = new Size(180, 35), // Ancho ajustado
                Location = new Point(20, 210),
                BackColor = Color.LightBlue,
                Font = new Font("Arial", 10)
            };
            btnPista.Click += (s, ev) =>
            {
                if (pistaEstrategicaActiva) // Asumo que 'pistaEstrategicaActiva' es un campo de clase booleano
                    AplicarPistaEstrategica();
                else
                    UsarPistaSimple();
            };
            sidebar.Controls.Add(btnPista);

            // Asumo que MusicWidget es un UserControl existente
            MusicWidget widget = new MusicWidget();
            widget.Location = new Point(20, 260); // Ajustar posición
            sidebar.Controls.Add(widget);

            this.Controls.Add(sidebar);
        }

        /// <summary>
        /// Inicia el temporizador del juego.
        /// </summary>
        private void StartGameTimer()
        {
            gameTimer = new System.Windows.Forms.Timer(); // Especificar System.Windows.Forms.Timer
            gameTimer.Interval = 1000; // 1 segundo
            gameTimer.Tick += (s, e) =>
            {
                segundosTranscurridos++;
                lblTiempo.Text = $"Tiempo: {segundosTranscurridos / 60}:{(segundosTranscurridos % 60).ToString("D2")}";
            };
            gameTimer.Start();
        }

        /// <summary>
        /// Verifica si todas las celdas editables han sido llenadas por el jugador.
        /// </summary>
        private bool TodasLasCeldasLlenas()
        {
            for (int row = 0; row < gridSize; row++)
            {
                for (int col = 0; col < gridSize; col++)
                {
                    // Una celda está "vacía para el jugador" si es editable Y no tiene texto.
                    if (cellButtons[row, col].Enabled && string.IsNullOrEmpty(cellButtons[row, col].Text))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        
        /// <summary>
        /// Se llama cuando todas las celdas están llenas para verificar la solución.
        /// </summary>
        private void VerificarTableroCompleto()
        {
            gameTimer.Stop(); // Detener el tiempo al verificar

            bool filasOK = VerificarFilas();
            bool columnasOK = VerificarColumnas();

            if (filasOK && columnasOK)
            {
                int estrellasObtenidas = CalcularEstrellas();
                AgregarPuntos(100); // Puntos extra por completar correctamente

                if (!modoPractica)
                {
                    // Asumo que LevelProgressManager existe y maneja el guardado.
                    LevelProgressManager.CompletarNivel(nivelActual, estrellasObtenidas, segundosTranscurridos, puntos, errores);
                }
                MostrarPantallaDeVictoria(estrellasObtenidas);
            }
            else
            {
                RegistrarError();
                MessageBox.Show("Hay errores en las operaciones. Revisa tus respuestas.", "Verificación Fallida", MessageBoxButtons.OK, MessageBoxIcon.Error);
                gameTimer.Start(); // Reanudar el tiempo si hay errores
            }
        }


        private bool VerificarFilas()
        {
            for (int row = 0; row < gridSize; row++)
            {
                // Solo verificar filas que contienen una operación completa (ej. N OP N = N)
                // Esto asume que las filas impares (0, 2, 4) son números/operadores y las pares (1, 3) son operadores/números
                // Para un MathCross típico, las operaciones están en cada fila y columna.
                // La estructura es N OP N = R
                if (gridSize == 5) // Asumiendo formato N1 OP N2 = R
                {
                    string[] expr = new string[5];
                    for (int col = 0; col < 5; col++)
                        expr[col] = cellButtons[row, col].Text;

                    if (!EvaluarOperacion(expr))
                        return false;
                }
            }
            return true;
        }

        private bool VerificarColumnas()
        {
            for (int col = 0; col < gridSize; col++)
            {
                 // Asumiendo formato N1 OP N2 = R para columnas también
                if (gridSize == 5)
                {
                    string[] expr = new string[5];
                    for (int row = 0; row < 5; row++)
                        expr[row] = cellButtons[row, col].Text;
                    
                    if (!EvaluarOperacion(expr))
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Evalúa una expresión matemática simple dada como un array de strings.
        /// Formato esperado: [num1, operador, num2, igual, resultado]
        /// </summary>
        private bool EvaluarOperacion(string[] tokens)
        {
            if (tokens.Length != 5) return false; // Formato incorrecto

            // Validar que los tokens necesarios sean parseables y correctos
            if (!int.TryParse(tokens[0], out int num1) ||
                !int.TryParse(tokens[2], out int num2) ||
                tokens[3] != "=" ||
                !int.TryParse(tokens[4], out int resultadoEsperado))
            {
                return false; // Datos inválidos en la expresión
            }

            string op = tokens[1];

            switch (op)
            {
                case "+": return num1 + num2 == resultadoEsperado;
                case "-": return num1 - num2 == resultadoEsperado;
                case "×": // Usar × para visualización, * para posible entrada
                case "*": return num1 * num2 == resultadoEsperado;
                case "÷": // Usar ÷ para visualización, / para posible entrada
                case "/": return num2 != 0 && num1 / (double)num2 == resultadoEsperado; // Usar double para división decimal si es necesario, o asegurar que sea entera
                default: return false; // Operador no reconocido
            }
        }

        /// <summary>
        /// Calcula el número de estrellas ganadas.
        /// </summary>
        private int CalcularEstrellas()
        {
            int numEstrellas = 1; // Mínimo una estrella por completar

            if (errores == 0) // Sin errores
                numEstrellas++;
            else if (errores <= 3) // Pocos errores
                numEstrellas = Math.Max(numEstrellas, 1); // Asegurar al menos 1 si hubo errores

            // Ejemplo de criterio de tiempo (ajustar según dificultad)
            // Estos umbrales deberían venir de la configuración de 'dificultad'
            if (segundosTranscurridos <= 60 && errores == 0) // Muy rápido y perfecto
                numEstrellas = 3;
            else if (segundosTranscurridos <= 120) // Tiempo razonable
                numEstrellas = Math.Max(numEstrellas, 2);
            
            return Math.Min(numEstrellas, 3); // Máximo 3 estrellas
        }

        /// <summary>
        /// Muestra la pantalla de victoria/finalización del nivel en el sidebar.
        /// </summary>
        private void MostrarPantallaDeVictoria(int estrellasGanadas)
        {
            // Limpiar controles existentes del sidebar relacionados con el juego activo
            sidebar.Controls.Remove(lblTiempo);
            sidebar.Controls.Remove(lblPuntos);
            sidebar.Controls.Remove(lblErrores);
            sidebar.Controls.Remove(lblPistas);
            sidebar.Controls.Remove(btnPista);
            // Considerar si MusicWidget también debe removerse o si es persistente

            felicidadesLabel = new Label()
            {
                Text = "¡Nivel Completado!",
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = Color.Green,
                Location = new Point(20, (sidebar.Controls.Count > 0 ? sidebar.Controls[sidebar.Controls.Count -1].Bottom : 0) + 20 ), // Ajustar posición
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter,
                Width = sidebar.Width - 40
            };
            sidebar.Controls.Add(felicidadesLabel);

            estrellasPanel = new Panel()
            {
                Size = new Size(180, 50),
                Location = new Point((sidebar.Width - 180) / 2, felicidadesLabel.Bottom + 15),
                BackColor = Color.Transparent
            };

            for (int i = 0; i < 3; i++)
            {
                PictureBox star = new PictureBox()
                {
                    Size = new Size(40, 40),
                    Location = new Point(i * (40 + 10), 5), // 40 de estrella + 10 de espacio
                    // Asumo que tienes recursos 'star_full' y 'star_empty'
                    Image = i < estrellasGanadas ? Properties.Resources.star_full : Properties.Resources.star_empty,
                    SizeMode = PictureBoxSizeMode.Zoom
                };
                estrellasPanel.Controls.Add(star);
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
            btnVolverMenu.Click += (s, e) => OnCloseRequested?.Invoke(); // Notificar al contenedor para que cierre este panel
            sidebar.Controls.Add(btnVolverMenu);

            // Lógica para el botón "Siguiente Nivel"
            // Necesitarías una forma de saber cuál es el siguiente nivel.
            string siguienteNivelId = LevelProgressManager.ObtenerSiguienteNivel(nivelActual); // Asumo este método
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
                    // Limpiar este panel y cargar el siguiente
                    this.Parent.Controls.Remove(this); // Quitar el panel actual
                    PuzzleGamePanel proximoNivel = new PuzzleGamePanel(siguienteNivelId, modoPractica);
                    proximoNivel.OnCloseRequested += () => OnCloseRequested?.Invoke(); // Propagar el evento
                    this.Parent.Controls.Add(proximoNivel); // Añadir el nuevo panel
                    this.Dispose();
                };
                sidebar.Controls.Add(btnContinuarNivel);
            }
            // Animación simple de aparición para los nuevos elementos
            // (Omitida por brevedad, similar a AnimateCell si se desea)
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
            // Podrías añadir una penalización de puntos aquí si quieres
            // puntos = Math.Max(0, puntos - 10);
            // lblPuntos.Text = $"Puntos: {puntos}";
        }
        
        /// <summary>
        /// Usa una pista simple: revela el contenido de la primera celda vacía editable.
        /// </summary>
        private void UsarPistaSimple()
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
                    // Celda editable y vacía
                    if (cellButtons[r, c].Enabled && string.IsNullOrEmpty(cellButtons[r, c].Text))
                    {
                        string valorCorrecto = puzzleData[r, c]; // Obtener de la solución
                        if (!string.IsNullOrEmpty(valorCorrecto) && EsNumero(valorCorrecto))
                        {
                            cellButtons[r, c].Text = valorCorrecto;
                            cellButtons[r, c].BackColor = Color.LightGreen; // Indicar que fue una pista
                            cellButtons[r, c].Enabled = false; // Ya no es editable

                            pistasDisponibles--;
                            lblPistas.Text = $"Pistas: {pistasDisponibles}";
                            AnimateCell(cellButtons[r,c]);

                            if (TodasLasCeldasLlenas())
                            {
                                VerificarTableroCompleto();
                            }
                            return; // Usar solo una pista a la vez
                        }
                    }
                }
            }
            MessageBox.Show("No hay celdas vacías donde aplicar una pista simple o el puzzle ya está resuelto.", "Pista no aplicable", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Aplica una pista estratégica: resalta una ecuación incorrecta.
        /// </summary>
        private void AplicarPistaEstrategica()
        {
            if (pistasDisponibles <= 0)
            {
                MessageBox.Show("Ya no tienes pistas disponibles.", "Sin Pistas", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Buscar una ecuación horizontal incorrecta que esté completamente llena
            for (int row = 0; row < gridSize; row++)
            {
                // Asumiendo que las filas con operaciones son las que tienen un operador en la segunda celda, etc.
                // Esta lógica depende mucho de la estructura de tu puzzleData.
                // Aquí asumimos una estructura simple N OP N = R
                if (gridSize == 5 && !EsNumero(cellButtons[row,1].Text)) // Si la segunda celda es un operador
                {
                    string[] tokens = new string[5];
                    bool filaCompleta = true;
                    for (int col = 0; col < 5; col++)
                    {
                        tokens[col] = cellButtons[row, col].Text;
                        if (string.IsNullOrEmpty(tokens[col])) filaCompleta = false;
                    }

                    if (filaCompleta && !EvaluarOperacion(tokens))
                    {
                        ResaltarEcuacion(row, true); // true para horizontal
                        pistasDisponibles--;
                        lblPistas.Text = $"Pistas: {pistasDisponibles}";
                        return;
                    }
                }
            }

            // Buscar una ecuación vertical incorrecta que esté completamente llena
            for (int col = 0; col < gridSize; col++)
            {
                 if (gridSize == 5 && !EsNumero(cellButtons[1,col].Text)) // Si la segunda celda (en la columna) es un operador
                {
                    string[] tokens = new string[5];
                    bool colCompleta = true;
                    for (int row = 0; row < 5; row++)
                    {
                        tokens[row] = cellButtons[row, col].Text;
                        if (string.IsNullOrEmpty(tokens[row])) colCompleta = false;
                    }
                    if (colCompleta && !EvaluarOperacion(tokens))
                    {
                        ResaltarEcuacion(col, false); // false para vertical
                        pistasDisponibles--;
                        lblPistas.Text = $"Pistas: {pistasDisponibles}";
                        return;
                    }
                }
            }
            MessageBox.Show("No se encontraron ecuaciones incorrectas completas para señalar, o todas son correctas.", "Pista Estratégica", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Resalta visualmente una fila o columna que contiene un error.
        /// </summary>
        private void ResaltarEcuacion(int index, bool esHorizontal)
        {
            Color colorOriginalCeldaEditable = Color.White;
            Color colorOriginalCeldaFija = Color.LightGray;
            Color colorResaltado = Color.LightCoral;

            for (int i = 0; i < gridSize; i++)
            {
                int r = esHorizontal ? index : i;
                int c = esHorizontal ? i : index;
                if (r < gridSize && c < gridSize) // Comprobación de límites
                {
                    cellButtons[r, c].BackColor = colorResaltado;
                }
            }

            // Timer para revertir el resaltado
            System.Windows.Forms.Timer revertTimer = new System.Windows.Forms.Timer { Interval = 1500 }; // 1.5 segundos
            revertTimer.Tick += (s, e) =>
            {
                for (int i = 0; i < gridSize; i++)
                {
                    int r = esHorizontal ? index : i;
                    int c = esHorizontal ? i : index;
                     if (r < gridSize && c < gridSize) // Comprobación de límites
                    {
                        // Revertir al color original basado en si es editable o no
                        if (cellButtons[r,c].Enabled) // Editable (o fue revelada por pista simple)
                        {
                            // Si fue pista simple, podría tener LightGreen, mantenerlo.
                            // Si no, es White.
                            cellButtons[r,c].BackColor = cellButtons[r,c].BackColor == Color.LightGreen ? Color.LightGreen : colorOriginalCeldaEditable;
                        }
                        else // Celda fija (operador, =, o pista inicial)
                        {
                            cellButtons[r,c].BackColor = cellButtons[r,c].BackColor == Color.LightCyan ? Color.LightCyan : colorOriginalCeldaFija;
                        }
                    }
                }
                revertTimer.Stop();
                revertTimer.Dispose();
            };
            revertTimer.Start();
        }
        
        // Limpiar recursos, especialmente timers, al destruir el control.
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
                // Cualquier otro timer o recurso desechable debería limpiarse aquí.
                // Por ejemplo, los timers de animación si no se auto-disponen.
            }
            base.Dispose(disposing);
        }
    }

    // Clases stub/ejemplo que necesitarías definir en otra parte de tu proyecto:
    // (Estas son solo para que el código anterior compile y para ilustrar dependencias)

    public class PuzzleGenerator
    {
        public class DificultadNivel
        {
            public int Size { get; set; } = 5;
            public List<string> OperadoresPermitidos { get; set; } = new List<string> { "+", "-" };
            public int PorcentajeCeldasOcultas { get; set; } = 50; // % de celdas numéricas a ocultar
            public int MaxPistas { get; set; } = 3;
        }

        public DificultadNivel ObtenerDificultadPorNivel(string nivelId)
        {
            // Lógica para cargar la dificultad basada en nivelId (ej. desde un archivo, DB, o hardcodeado)
            DificultadNivel dif = new DificultadNivel();
            if (nivelId == "P2") { dif.OperadoresPermitidos = new List<string> { "+", "-", "*" }; dif.PorcentajeCeldasOcultas = 60; }
            return dif;
        }

        public void SetOperadores(List<string> operadores) { /* Lógica para configurar operadores */ }

        public string[,] GenerarPuzzle(int size, int porcentajeOcultas)
        {
            // Lógica real para generar el puzzle.
            // Devuelve una cuadrícula string[size, size] con números, operadores, "="
            // y algunas celdas numéricas como string.Empty o un placeholder para que el jugador las llene.
            string[,] mockPuzzle = new string[size, size];
            // Ejemplo simple para un puzzle 5x5:
            //  1 + 2 = 3
            //  +   -   +
            //  4 * 1 = 4
            //  =   =   =
            //  5 - 1 = 4 
            // Esto es solo un ejemplo, la generación real es compleja.
            if (size == 5)
            {
                mockPuzzle = new string[,] {
                    {"1", "+", "2", "=", "3"},
                    {"+", "", "-", "", "+"}, // Las "" podrían ser operadores o números dependiendo de la estructura
                    {"4", "*", "1", "=", "4"},
                    {"=", "", "=", "", "="},
                    {"5", "-", "1", "=", "4"}
                };
                 // Aplicar porcentajeOcultas: algunas celdas numéricas de la solución se dejan vacías (string.Empty)
                 // para que el jugador las llene. Otras se muestran como parte del puzzle inicial.
                 // La lógica de CreateGrid diferencia entre string.Empty (jugador llena) y un número (pista inicial).
            }
            return mockPuzzle;
        }
    }

    public class NumberSelector : Form // O UserControl, dependiendo de tu implementación
    {
        public int? SelectedValue { get; private set; }
        public NumberSelector(List<int> availableNumbers)
        {
            // Lógica para mostrar botones/opciones para cada número en availableNumbers
            // Al seleccionar, se establece SelectedValue y se cierra con DialogResult.OK
            // Ejemplo:
            int yPos = 10;
            foreach (int num in availableNumbers)
            {
                Button btnNum = new Button { Text = num.ToString(), Location = new Point(10, yPos), Size = new Size(50,30) };
                btnNum.Click += (s, e) => { SelectedValue = num; this.DialogResult = DialogResult.OK; this.Close(); };
                this.Controls.Add(btnNum);
                yPos += 35;
            }
            this.ClientSize = new Size(70, yPos);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Seleccionar";
        }
    }

    public static class GameStateManager
    {
        public static void VolverAtras() { /* Lógica para navegar a la pantalla anterior */ }
    }

    public static class LevelProgressManager
    {
        public static void CompletarNivel(string nivelId, int estrellas, int tiempo, int puntos, int errores)
        { /* Lógica para guardar el progreso del nivel */ }

        public static string ObtenerSiguienteNivel(string nivelActualId)
        {
            // Lógica para determinar el ID del siguiente nivel.
            // Ejemplo: si nivelActualId es "P1", devuelve "P2". Si es el último, devuelve null o string.Empty.
            if (nivelActualId == "P1") return "P2";
            return null;
        }
    }

    public class MusicWidget : UserControl
    {
        public MusicWidget()
        {
            this.Size = new Size(180, 50);
            this.BackColor = Color.AliceBlue;
            Label lblMusic = new Label { Text = "Music Widget Placeholder", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter };
            this.Controls.Add(lblMusic);
        }
    }
}

//La pantalla principal del juego donde se juega el Puzzle. Carga un puzzle desde "PuzzleGenerator". Usa "MusicWidget" en el panel lateral. Usa "GameStateManager" para guardar estrellas y volver al menú y por ultimo, utiliza "LevelProgressManager" para guardar tiempo, estrellas y promedio del nivel. 