using System;

namespace MathCross
{
    /// <summary>
    /// Representa los datos de una partida guardada, incluyendo dificultad, fecha y estrellas.
    /// Esta clase es serializable para permitir su almacenamiento en archivos.
    /// </summary>
    [Serializable] // Se mantiene para compatibilidad o futuros usos con otros serializadores.
    public class SaveData
    {
        public string Dificultad { get; set; } = "---";
        public string Fecha { get; set; } = "00/00/00"; // Se recomienda usar DateTime para fechas
        public int Estrellas { get; set; } = 0;
    }
}

//Como sucede en "LevelProgress" y "LevelProgressManager". "SaveData" y "SaveManager" funcionan similar, pero esto es un guardado temporal por seccion. ¿A que me refiero? Que define el como guardara las cosas en cada uno de los Slots existentes. Y "SaveManager" se encargara de crear un archivo que lo guarde, aun cuando el juego se cierra.