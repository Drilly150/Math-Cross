using System;
using System.Collections.Generic;

namespace MathCross
{
    [Serializable]
    public class LevelProgress
    {
        public Dictionary<string, LevelData> Niveles { get; set; } = new Dictionary<string, LevelData>();
    }

    [Serializable]
    public class LevelData
    {
        public bool Desbloqueado { get; set; } = false;
        public int Estrellas { get; set; } = 0;
        public int TiempoRecord { get; set; } = 0; // en segundos
        public bool Completado => Estrellas > 0;
    }
}