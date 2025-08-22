namespace MathCross.Gameplay
{
    /// <summary>
    /// Contiene los resultados después de que el PuzzleVerifier analiza un tablero.
    /// Funciona como un objeto de transferencia de datos (DTO).
    /// </summary>
    public class VerificationResult
    {
        /// <summary>
        /// El número total de ecuaciones incorrectas encontradas en el tablero.
        /// </summary>
        public int Errors { get; set; }

        /// <summary>
        /// La puntuación final obtenida por el jugador en el nivel.
        /// </summary>
        public int Score { get; set; }
    }
}