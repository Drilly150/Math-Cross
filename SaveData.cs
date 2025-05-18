using System;

namespace MathCross
{
    [Serializable]
    public class SaveData
    {
        public string Dificultad { get; set; } = "---";
        public string Fecha { get; set; } = "00/00/00";
        public int Estrellas { get; set; } = 0;
    }
}
