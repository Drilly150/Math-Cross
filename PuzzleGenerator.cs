using System;
using System.Collections.Generic;
using MathCross;

namespace MathCross
{
    public class PuzzleGenerator
    {
        private Random rand = new Random();

        public string[,] GenerarPuzzle(int size, int dificultad)
        {
            string[,] grid = new string[size, size];
            List<EcuacionLayout> ecuaciones = new();

            for (int r = 0; r < size; r += 2)
                if (r + 4 < size) ecuaciones.Add(CrearEcuacionLayout(r, 0, true));

            for (int c = 0; c < size; c += 2)
                if (c + 4 < size) ecuaciones.Add(CrearEcuacionLayout(0, c, false));

            foreach (var eq in ecuaciones)
            {
                foreach (var (row, col, val) in eq.GetTokens())
                {
                    if (string.IsNullOrEmpty(grid[row, col]))
                        grid[row, col] = val;
                    else if (grid[row, col] != val)
                        return GenerarPuzzle(size, dificultad); // regenerar si hay conflicto
                }
            }

            // 👇 Ocultar números según dificultad
            OcultarCeldas(grid, dificultad);

            return grid;
        }

        public PuzzleDificultad ObtenerDificultadPorNivel(string nivelId)
        {
            int num = int.Parse(nivelId.Substring(1));

            int size = 5 + 2 * (num - 1); // P1 = 5, P2 = 7, etc.
            int ocultar = Math.Min(20 + (num * 8), 65); // hasta 65%
            int pistas = Math.Max(3 - (num / 2), 0); // 3, 2, 1, 0...

            List<string> ops = new() { "+", "-" };
            if (num >= 2) ops.Add("*");
            if (num >= 3) ops.Add("/");

            return new PuzzleDificultad
            {
                Size = size,
                PorcentajeCeldasOcultas = ocultar,
                OperadoresPermitidos = ops,
                MaxPistas = pistas
            };
        }

        private void OcultarCeldas(string[,] grid, int dificultad)
        {
            Random rand = new Random();
            int rows = grid.GetLength(0);
            int cols = grid.GetLength(1);

            int ocultarPorcentaje = dificultad * 15; // entre 15% y 75%
            int total = rows * cols;
            int aOcultar = (total * ocultarPorcentaje) / 100;

            while (aOcultar > 0)
            {
                int r = rand.Next(rows);
                int c = rand.Next(cols);

                if (grid[r, c] != null && int.TryParse(grid[r, c], out _)) // solo números
                {
                    grid[r, c] = ""; // dejarlo vacío
                    aOcultar--;
                }
            }
        }

        private List<string> operadoresPermitidos = new List<string>() { "+", "-" };

        private EcuacionLayout CrearEcuacionLayout(int row, int col, bool horizontal);
        {
            int a = rand.Next(1, 10);
            int b = rand.Next(1, 10);
            string[] ops = { "+", "-", "*", "/" };
            string op = ops[rand.Next(ops.Length)];


            int result = Calcular(a, b, op);
            if (result == int.MinValue) return null;

            return new EcuacionLayout
            {
                StartRow = row,
                StartCol = col,
                Horizontal = horizontal,
                A = a.ToString(),
                B = b.ToString(),
                Op = op,
                Resultado = result.ToString()
            };
        }

        private int Calcular(int a, int b, string op)
        {
            return op switch
            {
                "+" => a + b,
                "-" => a - b,
                "*" => a * b,
                "/" => (b != 0 && a % b == 0) ? a / b : int.MinValue,
                _ => int.MinValue
            };
        }
    }
}
