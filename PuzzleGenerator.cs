using System;
using System.Collections.Generic;
using System.Linq; // For Enumerable.Range and OrderBy

namespace MathCross
{
    // IMPORTANT:
    // The classes 'PuzzleDificultad' and 'EcuacionLayout' have been removed from this file.
    // You should place their definitions (which were originally in your PuzzleGenerator.cs)
    // into their own separate files:
    //  - PuzzleDificultad.cs
    //  - EcuacionLayout.cs
    // Ensure they are also within the 'MathCross' namespace.

    // For example, PuzzleDificultad.cs would contain:
    /*
    namespace MathCross
    {
        public class PuzzleDificultad
        {
            public int Size { get; set; } = 5;
            public int PorcentajeCeldasOcultas { get; set; } = 50;
            public List<string> OperadoresPermitidos { get; set; } = new List<string> { "+", "-" };
            public int MaxPistas { get; set; } = 3;
        }
    }
    */

    // And EcuacionLayout.cs would contain:
    /*
    using System.Collections.Generic; // Add this if not already present at the top of EcuacionLayout.cs
    namespace MathCross
    {
        public class EcuacionLayout
        {
            public int StartRow { get; set; }
            public int StartCol { get; set; }
            public bool Horizontal { get; set; }
            public string A { get; set; }
            public string B { get; set; }
            public string Op { get; set; }
            public string Resultado { get; set; }
            public const int EquationLength = 5;

            public List<(int row, int col, string value)> GetTokens()
            {
                var tokens = new List<(int, int, string)>();
                if (string.IsNullOrEmpty(A) || string.IsNullOrEmpty(Op) || string.IsNullOrEmpty(B) || string.IsNullOrEmpty(Resultado))
                {
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
    }
    */

    public class PuzzleGenerator
    {
        private Random rand = new Random();
        private const int MAX_GENERATION_ATTEMPTS = 100; // Max attempts to generate a valid puzzle

        /// <summary>
        /// Generates a puzzle grid based on the specified size and difficulty settings.
        /// This version expects PuzzleDificultad to be defined elsewhere.
        /// </summary>
        /// <param name="dificultad">The PuzzleDificultad object containing all difficulty parameters.</param>
        /// <returns>A 2D string array representing the puzzle grid, or null if generation failed.</returns>
        public string[,] GenerarPuzzle(PuzzleDificultad dificultad)
        {
            for (int attempt = 0; attempt < MAX_GENERATION_ATTEMPTS; attempt++)
            {
                string[,] grid = new string[dificultad.Size, dificultad.Size];
                List<EcuacionLayout> ecuacionesGeneradas = new List<EcuacionLayout>();
                bool conflictoGeneral = false;

                List<(int r, int c, bool isHorizontal)> potentialStarts = new List<(int, int, bool)>();
                for (int r = 0; r < dificultad.Size; r += 2) 
                {
                    if (r < dificultad.Size) 
                    {
                        for (int c = 0; c <= dificultad.Size - EcuacionLayout.EquationLength; c += 2) 
                        {
                            potentialStarts.Add((r, c, true));
                        }
                    }
                }
                for (int c = 0; c < dificultad.Size; c += 2) 
                {
                    if (c < dificultad.Size) 
                    {
                        for (int r = 0; r <= dificultad.Size - EcuacionLayout.EquationLength; r += 2) 
                        {
                            potentialStarts.Add((r, c, false));
                        }
                    }
                }

                potentialStarts = potentialStarts.OrderBy(x => rand.Next()).ToList();

                foreach (var start in potentialStarts)
                {
                    int opRow = start.isHorizontal ? start.r : start.r + 1;
                    int opCol = start.isHorizontal ? start.c + 1 : start.c;

                    if (opRow < dificultad.Size && opCol < dificultad.Size && string.IsNullOrEmpty(grid[opRow, opCol]))
                    {
                        EcuacionLayout nuevaEcuacion = CrearEcuacionLayout(start.r, start.c, start.isHorizontal, dificultad.OperadoresPermitidos);
                        if (nuevaEcuacion != null)
                        {
                            bool currentEqConflict = false;
                            foreach (var (row, col, val) in nuevaEcuacion.GetTokens())
                            {
                                if (row >= dificultad.Size || col >= dificultad.Size) 
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
                        }
                    }
                }

                int filledCells = 0;
                for (int r = 0; r < dificultad.Size; r++) 
                    for (int c = 0; c < dificultad.Size; c++) 
                        if (!string.IsNullOrEmpty(grid[r, c])) 
                            filledCells++;

                if (filledCells < (dificultad.Size * dificultad.Size) / 3) 
                {
                    conflictoGeneral = true; 
                }

                if (!conflictoGeneral)
                {
                    OcultarCeldas(grid, dificultad.PorcentajeCeldasOcultas);
                    if (EsPuzzleValido(grid))
                    {
                        return grid;
                    }
                }
            }
            return null; // Failed to generate a valid puzzle
        }

        /// <summary>
        /// Gets difficulty settings based on a level identifier (e.g., "P1", "P2").
        /// This method should be defined only ONCE within the PuzzleGenerator class.
        /// </summary>
        public PuzzleDificultad ObtenerDificultadPorNivel(string nivelId)
        {
            int num = 1; 
            if (nivelId != null && nivelId.Length > 1 && nivelId.StartsWith("P"))
            {
                int.TryParse(nivelId.Substring(1), out num);
                if (num == 0) num = 1; 
            }

            int size = 5;
            if (num >= 3 && num <= 4) size = 5; 
            else if (num >= 5) size = 7; 

            int porcentajeOcultas = Math.Min(30 + (num * 5), 75); 
            int maxPistas = Math.Max(3 - (num / 2), 1); 

            List<string> ops = new List<string> { "+", "-" };
            if (num >= 2) ops.Add("*"); 
            if (num >= 4) ops.Add("/"); 

            return new PuzzleDificultad
            {
                Size = size,
                PorcentajeCeldasOcultas = porcentajeOcultas,
                OperadoresPermitidos = ops,
                MaxPistas = maxPistas
            };
        }

        private void OcultarCeldas(string[,] grid, int porcentajeAOcultar)
        {
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
            if (totalNumericCells == 0) return; // No numeric cells to hide

            int numCellsToHide = (totalNumericCells * porcentajeAOcultar) / 100;

            numericCells = numericCells.OrderBy(x => rand.Next()).ToList();

            for (int i = 0; i < numCellsToHide && i < numericCells.Count; i++)
            {
                grid[numericCells[i].r, numericCells[i].c] = ""; 
            }
        }

        private EcuacionLayout CrearEcuacionLayout(int row, int col, bool horizontal, List<string> operadoresPermitidos)
        {
            if (operadoresPermitidos == null || operadoresPermitidos.Count == 0)
            {
                return null; 
            }

            for (int i = 0; i < 10; i++) 
            {
                int a = rand.Next(1, 10); 
                int b = rand.Next(1, 10);
                string op = operadoresPermitidos[rand.Next(operadoresPermitidos.Count)];

                int? result = Calcular(a, b, op);

                if (result.HasValue && result.Value >= 0 && result.Value <= 99) 
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
            return null; 
        }

        private int? Calcular(int a, int b, string op)
        {
            switch (op)
            {
                case "+": return a + b;
                case "-": return a - b;
                case "*": return a * b;
                case "/":
                    if (b != 0 && a % b == 0) 
                    {
                        return a / b;
                    }
                    return null; 
                default:
                    return null; 
            }
        }
        
        private bool EsPuzzleValido(string[,] grid)
        {
            if (grid == null) return false;
            int rowCount = grid.GetLength(0);
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
                        emptyEditableCells++;
                    }
                }
            }
            return filledCells > 0 && emptyEditableCells > 0;
        }
    }
}


//Tecnicamente este archivo vendria siendo parte del "PuzzleGamePanel", este, en especial, es el generador procedural de los puzzle que el juego ofrece. Ajusta tamaño, dificultad y visibilidad según el nivel seleccionado.