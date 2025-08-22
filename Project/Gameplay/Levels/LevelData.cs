namespace MathCross.Gameplay
{
    /// <summary>
    /// Define los parámetros y objetivos para un único nivel.
    /// Funciona como una estructura de datos inmutable.
    /// </summary>
    public class LevelData
    {
        /// <summary>
        /// El tiempo máximo en segundos para obtener la estrella de tiempo.
        /// </summary>
        public float TimeLimitInSeconds { get; }

        /// <summary>
        /// El número máximo de errores permitidos para obtener la estrella de errores.
        /// </summary>
        public int MaxErrorsForStar { get; }

        /// <summary>
        /// El puntaje mínimo requerido para obtener la estrella de puntuación.
        /// </summary>
        public int ScoreRequirement { get; }

        public LevelData(float timeLimit, int errorLimit, int scoreRequirement)
        {
            TimeLimitInSeconds = timeLimit;
            MaxErrorsForStar = errorLimit;
            ScoreRequirement = scoreRequirement;
        }
    }
}