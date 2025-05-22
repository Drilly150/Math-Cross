using System;
using System.Collections.Generic;

namespace MathCross
{
    [Serializable]
    public class LevelProgress
    {
        public Dictionary<string, LevelData> Levels { get; set; } = new Dictionary<string, LevelData>();
    }

    [Serializable]
    public class LevelData
    {
        public bool IsUnlocked { get; set; }
        public int Stars { get; set; }
        public int RecordTime { get; set; }
        
        private int _totalAttempts;
        public int TotalAttempts
        {
            get => _totalAttempts;
            set => _totalAttempts = value >= 0 ? value : 0;
        }

        public int CumulativeTime { get; set; } // en segundos

        public float AverageTime => TotalAttempts > 0 ? (float)CumulativeTime / TotalAttempts : 0f;
    }
}

//Tanto el archivo "LevelProgress" como su hermana "LevelProgressManager" estan diseñadas principalmente para guardar la partida de alguno de los tres slots y que cuando el usuario se salga del juego. Estas aun se mantengan vigentes, sin tener que reiniciar todo desde cero.