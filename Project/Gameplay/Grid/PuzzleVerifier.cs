using System;
using System.Collections.Generic;
using System.Data; // Usado para evaluar expresiones matemáticas de forma segura

namespace MathCross.Gameplay
{
    /// <summary>
    /// Contiene los resultados después de verificar un puzle.
    /// </summary>
    public class VerificationResult
    {
        public int Errors { get; set; }
        public int Score { get; set; }
    }

    /// <summary>
    /// Analiza un tablero completado por el jugador para verificar errores y calcular el puntaje.
    /// </summary>
    public class PuzzleVerifier
    {
        /// <summary>
        /// Método principal que verifica un tablero de Math-Cross.
        /// </summary>
        /// <param name="grid">El tablero completado por el jugador.</param>
        /// <returns>Un objeto con el número de errores y el puntaje.</returns>
        public VerificationResult Verify(GameGrid grid)
        {
            var result = new VerificationResult();
            int correctEquations = 0;

            // Un verificador analizará todo y buscará errores en las sumas, restas, multiplicaciones y divisiones.
            
            // Lógica para iterar a través de cada fila y columna,
            // extraer las ecuaciones y evaluarlas.
            // ...

            // Si no hay errores, se otorga un puntaje extra por el combo.
            // ...en base a los errores que tengas, determinara tu puntaje como combos, de haber hecho todo bien, tendrás el puntaje que se requiere... 
            if (result.Errors == 0)
            {
                result.Score *= 2; // Ejemplo de un bono por combo
            }

            return result;
        }

        /// <summary>
        /// Evalúa una sola ecuación (una fila o columna) para ver si es correcta.
        /// </summary>
        /// <param name="equationString">La ecuación en formato de texto, ej: "2+3=5".</param>
        /// <returns>True si la ecuación es correcta, de lo contrario False.</returns>
        private bool IsEquationCorrect(string equationString)
        {
            try
            {
                // Divide la ecuación en la parte a calcular y el resultado esperado.
                string[] parts = equationString.Split('=');
                if (parts.Length != 2) return false; // Formato inválido

                string expression = parts[0];
                int expectedResult = int.Parse(parts[1]);

                // Usamos la clase DataTable para evaluar de forma segura la expresión matemática.
                var dt = new DataTable();
                var actualResult = Convert.ToInt32(dt.Compute(expression, ""));
                
                return actualResult == expectedResult;
            }
            catch
            {
                // Si ocurre cualquier error durante el parseo o cálculo, la ecuación es incorrecta.
                return false;
            }
        }
    }
}