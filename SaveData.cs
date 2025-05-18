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

//Como sucede en "LevelProgress" y "LevelProgressManager". "SaveData" y "SaveManager" funcionan similar, pero esto es un guardado temporal por seccion. ¿A que me refiero? Que solo guarda la partida mientras tenga el juego abierto, una vez cerrado. El progreso se borra. Para demostraciones, esta configuracion sirve. Sin embargo, si queres jugar varias veces. Esta opción deja de ser viable.