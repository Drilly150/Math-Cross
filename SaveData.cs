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