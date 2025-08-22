using System.Collections.Generic;

namespace MathCross.Gameplay
{
    /// <summary>
    /// Gestiona la carga de datos de los niveles y proporciona los objetivos del nivel actual.
    /// </summary>
    public class LevelManager
    {
        private readonly Dictionary<int, LevelData> _levelDatabase = new Dictionary<int, LevelData>();
        
        public LevelData CurrentLevelData { get; private set; }

        public LevelManager()
        {
            LoadAllLevelData();
        }

        /// <summary>
        /// Carga todos los datos de los niveles en memoria.
        /// En un juego real, esto podría cargarse desde un archivo de configuración (JSON, XML).
        /// </summary>
        private void LoadAllLevelData()
        {
            // Aquí se definirían los objetivos para cada uno de los 10 niveles.
            // Los valores aumentan en dificultad.
            _levelDatabase.Add(0, new LevelData(timeLimit: 60f, errorLimit: 3, scoreRequirement: 1000));  // Nivel 1
            _levelDatabase.Add(1, new LevelData(timeLimit: 55f, errorLimit: 3, scoreRequirement: 1200));  // Nivel 2
            _levelDatabase.Add(2, new LevelData(timeLimit: 50f, errorLimit: 2, scoreRequirement: 1500));  // Nivel 3
            // ... y así sucesivamente hasta el nivel 9 (el décimo nivel).
            _levelDatabase.Add(9, new LevelData(timeLimit: 30f, errorLimit: 0, scoreRequirement: 5000));  // Nivel 10
        }

        /// <summary>
        /// Establece el nivel actual basado en el índice proporcionado.
        /// </summary>
        /// <param name="levelIndex">El índice del nivel a cargar (0-9).</param>
        public void SetCurrentLevel(int levelIndex)
        {
            if (_levelDatabase.ContainsKey(levelIndex))
            {
                CurrentLevelData = _levelDatabase[levelIndex];
            }
            else
            {
                // Manejar el caso de un índice de nivel inválido.
                CurrentLevelData = null;
            }
        }
    }
}