using MathCross.Core; // Necesario para usar GameSlot y LevelProgress

namespace MathCross.Gameplay
{
    /// <summary>
    /// Mantiene el estado del progreso del jugador para la sesión de juego actual.
    /// </summary>
    public class PlayerProgress
    {
        /// <summary>
        /// La dificultad de la partida actual.
        /// </summary>
        public Difficulty CurrentDifficulty { get; private set; }

        /// <summary>
        /// El estado de cada uno de los 10 niveles.
        /// </summary>
        public LevelProgress[] Levels { get; private set; }

        /// <summary>
        /// Carga los datos de progreso desde una ranura de guardado.
        /// </summary>
        public void LoadFromSlot(GameSlot slot)
        {
            CurrentDifficulty = slot.Difficulty;
            Levels = slot.Levels;
        }

        /// <summary>
        /// Calcula el número total de estrellas que el jugador ha conseguido.
        /// </summary>
        public int GetTotalStars()
        {
            int totalStars = 0;
            if (Levels == null) return 0;

            foreach (var level in Levels)
            {
                if (level.StarFromTime) totalStars++;
                if (level.StarFromErrors) totalStars++;
                if (level.StarFromScore) totalStars++;
            }
            return totalStars;
        }
    }
}