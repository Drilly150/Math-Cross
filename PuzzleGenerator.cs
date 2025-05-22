using System;
using System.Collections.Generic;
using System.Linq; // Added for Enumerable.Range and OrderBy

// Assuming PuzzleDificultad and EcuacionLayout are within the same namespace
// or MathCross namespace is correctly referenced if they are defined elsewhere.
// For clarity, I'll define them within this file if they are specific to the generator's logic
// or assume they are accessible if defined in another file within the MathCross namespace.

namespace MathCross
{
    /// <summary>
    /// Defines the difficulty settings for a puzzle.
    /// This class was also present in the PuzzleGamePanel analysis; ensure consistency.
    /// </summary>
    public class PuzzleDificultad
    {
        public int Size { get; set; } = 5;
        public int PorcentajeCeldasOcultas { get; set; } = 50; // Percentage of numeric cells to hide
        public List<string> OperadoresPermitidos { get; set; } = new List<string> { "+", "-" };
        public int MaxPistas { get; set; } = 3;
    }

    /// <summary>
    /// Represents a single equation (e.g., A + B = C) and its position on the grid.
    /// </summary>
    public class EcuacionLayout
    {
        public int StartRow { get; set; }
        public int StartCol { get; set; }
        public bool Horizontal { get; set; } // True if horizontal, false if vertical
        public string A { get; set; }
        public string B { get; set; }
        public string Op { get; set; }
        public string Resultado { get; set; }

        // Defines the standard length of an equation (e.g., N1 op N2 = RES -> 5 cells)
        public const int EquationLength = 5;

        /// <summary>
        /// Gets the individual tokens (number, operator, equals, result) of the equation
        /// with their corresponding grid positions.
        /// </summary>
        /// <returns>A list of tuples, where each tuple is (row, column, value).</returns>
        public List<(int row, int col, string value)> GetTokens()
        {
            var tokens = new List<(int, int, string)>();
            if (string.IsNullOrEmpty(A) || string.IsNullOrEmpty(Op) || string.IsNullOrEmpty(B) || string.IsNullOrEmpty(Resultado))
            {
                // Invalid equation components
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
        private const int MAX_GENERATION_ATTEMPTS = 100; // Max attempts to generate a valid puzzle to avoid infinite loops

        /// <summary>
        /// Generates a puzzle grid based on the specified size and difficulty settings.
        /// </summary>
        /// <param name="dificultad">The PuzzleDificultad object containing all difficulty parameters.</param>
        /// <returns>A 2D string array representing the puzzle grid, or null if generation failed.</returns>
        public string[,] GenerarPuzzle(PuzzleDificultad dificultad)
        {
            for (int attempt = 0; attempt < MAX_GENERATION_ATTEMPTS; attempt++)
            {
                string[,] grid = new string[dificultad.Size, dificultad.Size];
                List<EcuacionLayout> ecuacionesGeneradas = new List<EcuacionLayout>();
                bool conflicto = false;

                // Determine potential equation starting points
                List<(int r, int c, bool isHorizontal)> potentialStarts = new List<(int, int, bool)>();
                for (int r = 0; r < dificultad.Size; r += 2) // Horizontal equations
                {
                    // Ensure equation fits: r is start, needs space for 1 cell high, 5 cells wide
                    if (r < dificultad.Size) // Row must be valid
                    {
                        for (int c = 0; c <= dificultad.Size - EcuacionLayout.EquationLength; c+=2) // Start col must allow 5 cells
                        {
                             potentialStarts.Add((r, c, true));
                        }
                    }
                }
                for (int c = 0; c < dificultad.Size; c += 2) // Vertical equations
                {
                     // Ensure equation fits: c is start, needs space for 1 cell wide, 5 cells high
                    if (c < dificultad.Size) // Col must be valid
                    {
                        for (int r = 0; r <= dificultad.Size - EcuacionLayout.EquationLength; r+=2) // Start row must allow 5 cells
                        {
                            potentialStarts.Add((r, c, false));
                        }
                    }
                }

                // Shuffle potential starts to vary puzzle layout
                potentialStarts = potentialStarts.OrderBy(x => rand.Next()).ToList();

                foreach (var start in potentialStarts)
                {
                    // Try to create and place an equation only if the central operator cell is empty
                    // This is a simple way to avoid too many overlaps initially.
                    // For horizontal: grid[start.r, start.c + 1] (operator)
                    // For vertical:   grid[start.r + 1, start.c] (operator)
                    int opRow = start.isHorizontal ? start.r : start.r + 1;
                    int opCol = start.isHorizontal ? start.c + 1 : start.c;

                    if (opRow < dificultad.Size && opCol < dificultad.Size && string.IsNullOrEmpty(grid[opRow, opCol]))
                    {
                        EcuacionLayout nuevaEcuacion = CrearEcuacionLayout(start.r, start.c, start.isHorizontal, dificultad.OperadoresPermitidos);
                        if (nuevaEcuacion != null)
                        {
                            // Check for conflicts before placing
                            bool currentEqConflict = false;
                            foreach (var (row, col, val) in nuevaEcuacion.GetTokens())
                            {
                                if (row >= dificultad.Size || col >= dificultad.Size) // Out of bounds
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
                            // If there's a conflict with a specific equation, we just skip it in this simplified model
                            // A more complex generator might try to regenerate the conflicting equation or backtrack.
                        }
                    }
                }

                // Basic validation: ensure a reasonable number of cells are filled before hiding
                int filledCells = 0;
                for(int r=0; r<dificultad.Size; r++) for(int c=0; c<dificultad.Size; c++) if(!string.IsNullOrEmpty(grid[r,c])) filledCells++;

                if (filledCells < (dificultad.Size * dificultad.Size) / 3) // Heuristic: at least 1/3 filled
                {
                    conflicto = true; // Not enough content, retry
                }


                if (!conflicto)
                {
                    OcultarCeldas(grid, dificultad.PorcentajeCeldasOcultas);
                    // Final check: ensure puzzle is not entirely empty after hiding, and has some editable cells
                    if (EsPuzzleValido(grid))
                    {
                        return grid;
                    }
                }
            }
            return null; // Failed to generate a valid puzzle after MAX_ATTEMPTS
        }

        /// <summary>
        /// Gets difficulty settings based on a level identifier (e.g., "P1", "P2").
        /// </summary>
        public PuzzleDificultad ObtenerDificultadPorNivel(string nivelId)
        {
            int num = 1; // Default to level 1
            if (nivelId.Length > 1 && nivelId.StartsWith("P"))
            {
                int.TryParse(nivelId.Substring(1), out num);
                if (num == 0) num = 1; // Ensure num is at least 1
            }

            // Adjust size: P1=5x5, P2=5x5, P3=7x7, P4=7x7 etc.
            // Equation length is 5. A 5x5 grid can hold one full horizontal and one full vertical equation.
            // A 7x7 grid can hold two full horizontal and two full vertical if they interleave.
            // For simplicity, let's keep 5x5 for a few levels, then increase.
            int size = 5;
            if (num >= 3 && num <= 4) size = 5; // Example: levels 3-4 are 5x5 but harder
            else if (num >= 5) size = 7; // Example: levels 5+ are 7x7

            // Percentage of numeric cells to hide. Increases with level.
            int porcentajeOcultas = Math.Min(30 + (num * 5), 75); // Starts at 35%, max 75%

            // Number of hints available. Decreases with level.
            int maxPistas = Math.Max(3 - (num / 2), 1); // Min 1 pista

            List<string> ops = new List<string> { "+", "-" };
            if (num >= 2) ops.Add("*"); // Multiplication from level 2
            if (num >= 4) ops.Add("/"); // Division from level 4 (ensure results are integers)

            return new PuzzleDificultad
            {
                Size = size,
                PorcentajeCeldasOcultas = porcentajeOcultas,
                OperadoresPermitidos = ops,
                MaxPistas = maxPistas
            };
        }

        /// <summary>
        /// Hides a certain percentage of numeric cells in the grid.
        /// </summary>
        private void OcultarCeldas(string[,] grid, int porcentajeAOcultar)
        {
            // Random rand = new Random(); // Use the class-level 'rand' instance for better randomness over time
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

            // Shuffle the list of numeric cells and take the first 'numCellsToHide' to hide
            numericCells = numericCells.OrderBy(x => rand.Next()).ToList();

            for (int i = 0; i < numCellsToHide && i < numericCells.Count; i++)
            {
                grid[numericCells[i].r, numericCells[i].c] = ""; // Mark as empty for player to fill
            }
        }

        /// <summary>
        /// Creates a single equation layout (A op B = Result).
        /// </summary>
        private EcuacionLayout CrearEcuacionLayout(int row, int col, bool horizontal, List<string> operadoresPermitidos)
        {
            if (operadoresPermitidos == null || operadoresPermitidos.Count == 0)
            {
                return null; // No operators allowed
            }

            for (int i = 0; i < 10; i++) // Try a few times to generate a valid equation
            {
                int a = rand.Next(1, 10); // Numbers from 1 to 9
                int b = rand.Next(1, 10);
                string op = operadoresPermitidos[rand.Next(operadoresPermitidos.Count)];

                int? result = Calcular(a, b, op);

                // Ensure result is within a reasonable range (e.g., 1-99 for display) and valid
                if (result.HasValue && result.Value >= 0 && result.Value <= 99) // Allow 0 as a result
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
            return null; // Failed to create a suitable equation
        }

        /// <summary>
        /// Calculates the result of a binary operation.
        /// Returns null if the operation is invalid (e.g., division by zero, non-integer result for division).
        /// </summary>
        private int? Calcular(int a, int b, string op)
        {
            switch (op)
            {
                case "+": return a + b;
                case "-": return a - b;
                case "*": return a * b;
                case "/":
                    if (b != 0 && a % b == 0) // Ensure division by zero is avoided and result is an integer
                    {
                        return a / b;
                    }
                    return null; // Invalid division
                default:
                    return null; // Unknown operator
            }
        }

        /// <summary>
        /// Checks if the generated grid is valid (e.g., not empty, has solvable parts).
        /// This is a basic check; more sophisticated validation might be needed.
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
                        // This is a simplified check. A truly editable cell depends on game rules
                        // (e.g., if it's part of an equation and meant for a number).
                        // For now, any empty cell is considered potentially editable.
                        emptyEditableCells++;
                    }
                }
            }
            // Puzzle is valid if it has some content and some empty cells for the player to fill.
            return filledCells > 0 && emptyEditableCells > 0;
        }
    }
}