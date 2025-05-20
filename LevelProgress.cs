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
        public bool Desbloqueado { get; set; }
        public int Estrellas { get; set; }
        public int TiempoRecord { get; set; }

        // Nuevo:
        public int TotalIntentos { get; set; }
        public int TiempoAcumulado { get; set; } // en segundos

        public int TiempoPromedio => TotalIntentos > 0 ? TiempoAcumulado / TotalIntentos : 0;
    }
}

//Tanto el archivo "LevelProgress" como su hermana "LevelProgressManager" estan diseñadas principalmente para guardar la partida de alguno de los tres slots y que cuando el usuario se salga del juego. Estas aun se mantengan vigentes, sin tener que reiniciar todo desde cero.