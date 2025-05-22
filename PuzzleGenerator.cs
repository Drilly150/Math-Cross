using System;
using System.Collections.Generic;
using System.Linq; //Agregada para Enumerable.Range y OrderBy

// Suponiendo que PuzzleDificultad y EcuacionLayout están dentro del mismo namespace
// o el namespace MathCross se referencia correctamente si están definidos en otro lugar.
// Para mayor claridad, los definiré dentro de este archivo si son específicos de la lógica del generador.
//o asumir que son accesibles si se definen en otro archivo dentro del namespace MathCross.

namespace MathCross
{
    /// <summary>
    /// Define los niveles de dificultad de un rompecabezas.
    /// Esta clase también estuvo presente en el análisis de PuzzleGamePanel; garantizar la consistencia.
    /// </summary>
    public class PuzzleDificultad
    {
        public int Size { get; set; } = 5;
        public int PorcentajeCeldasOcultas { get; set; } = 50; // Porcentaje de celdas numéricas a ocultar
        public List<string> OperadoresPermitidos { get; set; } = new List<string> { "+", "-" };
        public int MaxPistas { get; set; } = 3;
    }

    /// <summary>
    /// Representa una sola ecuación (por ejemplo, A + B = C) y su posición en la cuadrícula.
    /// </summary>
    public class EcuacionLayout
    {
        public int StartRow { get; set; }
        public int StartCol { get; set; }
        public bool Horizontal { get; set; } // Verdadero si es horizontal, falso si es vertical
        public string A { get; set; }
        public string B { get; set; }
        public string Op { get; set; }
        public string Resultado { get; set; }

        // Define la longitud estándar de una ecuación (por ejemplo, N1 op N2 = RES -> 5 celdas)
        public const int EquationLength = 5;

        /// <summary>
        /// Obtiene los tokens individuales (número, operador, igual, resultado) de la ecuación
        /// con sus posiciones de cuadrícula correspondientes.
        /// </summary>
        /// <returns>Una lista de tuplas, donde cada tupla es (fila, columna, valor).</returns>
        public List<(int row, int col, string value)> GetTokens()
        {
            var tokens = new List<(int, int, string)>();
            if (string.IsNullOrEmpty(A) || string.IsNullOrEmpty(Op) || string.IsNullOrEmpty(B) || string.IsNullOrEmpty(Resultado))
            {
                // Componentes de ecuación no válidos
                return tokens;
            }

            if (Horizontal)
            {
                tokens.Add((StartRow, StartCol, A));
                tokens.Add((StartRow, StartCol + 1, Op));
                tokens.Add((StartRow, StartCol + 2, B));
                tokens.Add((StartRow, StartCol + 3, "="));
                tokens.Add((StartRow, StartCol + 4, Resultado));
            }
            else // Vertical
            {
                tokens.Add((StartRow, StartCol, A));
                tokens.Add((StartRow + 1, StartCol, Op));
                tokens.Add((StartRow + 2, StartCol, B));
                tokens.Add((StartRow + 3, StartCol, "="));
                tokens.Add((StartRow + 4, StartCol, Resultado));
            }
            return tokens;
        }
    }

    public class PuzzleGenerator
    {
        private Random rand = new Random();
        private const int MAX_GENERATION_ATTEMPTS = 100; // Max intenta generar un rompecabezas válido para evitar bucles infinitos.

        /// <summary>
        /// Genera una cuadrícula de rompecabezas según el tamaño y la configuración de dificultad especificados.
        /// </summary>
        /// <param name="dificultad">El objeto PuzzleDificultad que contiene todos los parámetros de dificultad.</param>
        /// <returns>Una matriz de cadenas 2D que representa la cuadrícula del rompecabezas o nula si falló la generación.</returns>
        public string[,] GenerarPuzzle(PuzzleDificultad dificultad)
        {
            for (int attempt = 0; attempt < MAX_GENERATION_ATTEMPTS; attempt++)
            {
                string[,] grid = new string[dificultad.Size, dificultad.Size];
                List<EcuacionLayout> ecuacionesGeneradas = new List<EcuacionLayout>();
                bool conflicto = false;

                // Determinar los posibles puntos de inicio de la ecuación
                List<(int r, int c, bool isHorizontal)> potentialStarts = new List<(int, int, bool)>();
                for (int r = 0; r < dificultad.Size; r += 2) // Ecuaciones horizontales
                {
                    // Asegúrese de que la ecuación se ajuste: r es el inicio, necesita espacio para 1 celda de alto y 5 celdas de ancho
                    if (r < dificultad.Size) // La fila debe ser válida
                    {
                        for (int c = 0; c <= dificultad.Size - EcuacionLayout.EquationLength; c+=2) // La columna de inicio debe permitir 5 celdas
                        {
                            potentialStarts.Add((r, c, true));
                        }
                    }
                }
                for (int c = 0; c < dificultad.Size; c += 2) // Ecuaciones verticales
                {
                     // Asegúrese de que la ecuación se ajuste: c es el inicio, necesita espacio para 1 celda de ancho y 5 celdas de alto
                    if (c < dificultad.Size) // Col debe ser válido
                    {
                        for (int r = 0; r <= dificultad.Size - EcuacionLayout.EquationLength; r+=2) // La fila de inicio debe permitir 5 celdas
                        {
                            potentialStarts.Add((r, c, false));
                        }
                    }
                }

                // El potencial de mezcla comienza a variar el diseño del rompecabezas
                potentialStarts = potentialStarts.OrderBy(x => rand.Next()).ToList();

                foreach (var start in potentialStarts)
                {
                    // Intente crear y colocar una ecuación solo si la celda del operador central está vacía
                    // Esta es una forma sencilla de evitar demasiadas superposiciones inicialmente.
                    // Para horizontal: grid[start.r, start.c + 1] (operador)
                    // Para vertical: grid[start.r + 1, start.c] (operador)
                    int opRow = start.isHorizontal ? start.r : start.r + 1;
                    int opCol = start.isHorizontal ? start.c + 1 : start.c;

                    if (opRow < dificultad.Size && opCol < dificultad.Size && string.IsNullOrEmpty(grid[opRow, opCol]))
                    {
                        EcuacionLayout nuevaEcuacion = CrearEcuacionLayout(start.r, start.c, start.isHorizontal, dificultad.OperadoresPermitidos);
                        if (nuevaEcuacion != null)
                        {
                            // Compruebe si hay conflictos antes de colocar
                            bool currentEqConflict = false;
                            foreach (var (row, col, val) in nuevaEcuacion.GetTokens())
                            {
                                if (row >= dificultad.Size || col >= dificultad.Size) // Fuera de límites
                                {
                                    currentEqConflict = true; break;
                                }
                                if (!string.IsNullOrEmpty(grid[row, col]) && grid[row, col] != val)
                                {
                                    currentEqConflict = true; break;
                                }
                            }

                            if (!currentEqConflict)
                            {
                                foreach (var (row, col, val) in nuevaEcuacion.GetTokens())
                                {
                                    grid[row, col] = val;
                                }
                                ecuacionesGeneradas.Add(nuevaEcuacion);
                            }
                            // Si hay un conflicto con una ecuación específica, simplemente la omitimos en este modelo simplificado
                            // Un generador más complejo podría intentar regenerar la ecuación conflictiva o retroceder.
                        }
                    }
                }

                // Validación básica: asegúrese de que se llene una cantidad razonable de celdas antes de ocultar
                int filledCells = 0;
                for(int r=0; r<dificultad.Size; r++) for(int c=0; c<dificultad.Size; c++) if(!string.IsNullOrEmpty(grid[r,c])) filledCells++;

                if (filledCells < (dificultad.Size * dificultad.Size) / 3) // Heurística: al menos 1/3 lleno
                {
                    conflicto = true; // No hay suficiente contenido, inténtalo de nuevo.
                }


                if (!conflicto)
                {
                    OcultarCeldas(grid, dificultad.PorcentajeCeldasOcultas);
                    // Comprobación final: asegúrese de que el rompecabezas no esté completamente vacío después de esconderse y que tenga algunas celdas editables.
                    if (EsPuzzleValido(grid))
                    {
                        return grid;
                    }
                }
            }
            return null; // No se pudo generar un rompecabezas válido después de MAX_ATTEMPTS
        }

        /// <summary>
        /// Obtiene configuraciones de dificultad según un identificador de nivel (por ejemplo, "P1", "P2").
        /// </summary>
        public PuzzleDificultad ObtenerDificultadPorNivel(string nivelId)
        {
            int num = 1; // Predeterminado al nivel 1
            if (nivelId.Length > 1 && nivelId.StartsWith("P"))
            {
                int.TryParse(nivelId.Substring(1), out num);
                if (num == 0) num = 1; // Asegúrese de que el número sea al menos 1
            }

            // Ajustar tamaño: P1=5x5, P2=5x5, P3=7x7, P4=7x7 etc.
            // La longitud de la ecuación es 5. Una cuadrícula de 5x5 puede contener una ecuación horizontal completa y una vertical completa.
            // Una cuadrícula de 7x7 puede contener dos horizontales completos y dos verticales completos si se intercalan.
            // Para simplificar, mantendremos 5x5 durante algunos niveles y luego aumentaremos.
            int size = 5;
            if (num >= 3 && num <= 4) size = 5; // Ejemplo: los niveles 3-4 son 5x5 pero más difíciles
            else if (num >= 5) size = 7; //Ejemplo: los niveles 5+ son 7x7

            // Porcentaje de celdas numéricas que se ocultan. Aumenta con el nivel.
            int porcentajeOcultas = Math.Min(30 + (num * 5), 75); // Comienza en 35%, máximo 75%

            // Número de pistas disponibles. Disminuye con el nivel.
            int maxPistas = Math.Max(3 - (num / 2), 1); //Mínimo 1 pista

            List<string> ops = new List<string> { "+", "-" };
            if (num >= 2) ops.Add("*"); // Multiplicación del nivel 2
            if (num >= 4) ops.Add("/"); // División de nivel 4 (asegúrese de que los resultados sean números enteros)

            return new PuzzleDificultad
            {
                Size = size,
                PorcentajeCeldasOcultas = porcentajeOcultas,
                OperadoresPermitidos = ops,
                MaxPistas = maxPistas
            };
        }

        /// <summary>
        /// Oculta un cierto porcentaje de celdas numéricas en la cuadrícula.
        /// </summary>
        private void OcultarCeldas(string[,] grid, int porcentajeAOcultar)
        {
            // Random rand = new Random(); // Utilice la instancia 'rand' a nivel de clase para una mejor aleatoriedad a lo largo del tiempo
            int rows = grid.GetLength(0);
            int cols = grid.GetLength(1);

            List<(int r, int c)> numericCells = new List<(int r, int c)>();
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    if (!string.IsNullOrEmpty(grid[r, c]) && int.TryParse(grid[r, c], out _))
                    {
                        numericCells.Add((r, c));
                    }
                }
            }

            int totalNumericCells = numericCells.Count;
            int numCellsToHide = (totalNumericCells * porcentajeAOcultar) / 100;

            // Baraja la lista de celdas numéricas y toma la primera 'numCellsToHide' para ocultarla
            numericCells = numericCells.OrderBy(x => rand.Next()).ToList();

            for (int i = 0; i < numCellsToHide && i < numericCells.Count; i++)
            {
                grid[numericCells[i].r, numericCells[i].c] = ""; // Marcar como vacío para que el jugador lo complete
            }
        }

        /// <summary>
        /// Crea un diseño de ecuación única (A op B = Resultado).
        /// </summary>
        private EcuacionLayout CrearEcuacionLayout(int row, int col, bool horizontal, List<string> operadoresPermitidos)
        {
            if (operadoresPermitidos == null || operadoresPermitidos.Count == 0)
            {
                return null; // No se permiten operadores
            }

            for (int i = 0; i < 10; i++) // Intente varias veces generar una ecuación válida
            {
                int a = rand.Next(1, 10); // Números del 1 al 9
                int b = rand.Next(1, 10);
                string op = operadoresPermitidos[rand.Next(operadoresPermitidos.Count)];

                int? result = Calcular(a, b, op);

                // Asegúrese de que el resultado esté dentro de un rango razonable (por ejemplo, 1-99 para la visualización) y sea válido.
                if (result.HasValue && result.Value >= 0 && result.Value <= 99) // Permitir 0 como resultado
                {
                    return new EcuacionLayout
                    {
                        StartRow = row,
                        StartCol = col,
                        Horizontal = horizontal,
                        A = a.ToString(),
                        B = b.ToString(),
                        Op = op,
                        Resultado = result.Value.ToString()
                    };
                }
            }
            return null; // No se pudo crear una ecuación adecuada
        }

        /// <summary>
        /// Calcula el resultado de una operación binaria.
        /// Devuelve nulo si la operación no es válida (por ejemplo, división por cero, resultado no entero de la división).
        /// </summary>
        private int? Calcular(int a, int b, string op)
        {
            switch (op)
            {
                case "+": return a + b;
                case "-": return a - b;
                case "*": return a * b;
                case "/":
                    if (b != 0 && a % b == 0) // Asegúrese de que se evite la división por cero y que el resultado sea un número entero
                    {
                        return a / b;
                    }
                    return null; // División no válida
                default:
                    return null; // Operador desconocido
            }
        }

        /// <summary>
        /// Comprueba si la cuadrícula generada es válida (por ejemplo, no está vacía y tiene partes solucionables).
        /// Esta es una comprobación básica; podría ser necesaria una validación más sofisticada.
        /// </summary>
        private bool EsPuzzleValido(string[,] grid)
        {
            introwCount = grid.GetLength(0);
            int colCount = grid.GetLength(1);
            int filledCells = 0;
            int emptyEditableCells = 0;

            for (int r = 0; r < rowCount; r++)
            {
                for (int c = 0; c < colCount; c++)
                {
                    if (!string.IsNullOrEmpty(grid[r, c]))
                    {
                        filledCells++;
                    }
                    else
                    {
                        // Esta es una comprobación simplificada. Una celda verdaderamente editable depende de las reglas del juego.
                        // (por ejemplo, si es parte de una ecuación y está destinado a un número).
                        // Por ahora, cualquier celda vacía se considera potencialmente editable.
                        emptyEditableCells++;
                    }
                }
            }
            //Un rompecabezas es válido si tiene algún contenido y algunas celdas vacías para que el jugador las llene.
            return filledCells > 0 && emptyEditableCells > 0;
        }
    }
}

//Tecnicamente este archivo vendria siendo parte del "PuzzleGamePanel", este, en especial, es el generador procedural de los puzzle que el juego ofrece. Ajusta tamaño, dificultad y visibilidad según el nivel seleccionado.