using System.Collections.Generic;

namespace MathCross
{
    public class EcuacionLayout
    {
        public int StartRow;
        public int StartCol;
        public bool Horizontal;

        public string A;
        public string Op;
        public string B;
        public readonly string Igual = "="; // Made readonly
        public string Resultado;

        public List<(int row, int col, string valor)> GetTokens()
        {
            List<(int, int, string)> tokens = new();
            for (int i = 0; i < 5; i++)
            {
                int r = Horizontal ? StartRow : StartRow + i;
                int c = Horizontal ? StartCol + i : StartCol;
                string val = i switch
                {
                    0 => A,
                    1 => Op,
                    2 => B,
                    3 => Igual,
                    4 => Resultado,
                    _ => "" // Should not be reached with the current loop
                };
                tokens.Add((r, c, val));
            }
            return tokens;
        }
    }
}

