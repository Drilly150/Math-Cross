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

//Tanto el archivo "LevelProgress" como su hermana "LevelProgressManager" estan diseñadas principalmente para guardar la partida de alguno de los tres slots y que cuando el usuario se salga del juego. Estas aun se mantengan vigentes, sin tener que reiniciar todo desde cero.