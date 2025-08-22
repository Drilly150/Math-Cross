using System;
using System.Collections.Generic;

namespace MathCross.Core
{
    /// <summary>
    /// Almacena las configuraciones del juego que el usuario puede modificar. 
    /// </summary>
    public class GameSettings
    {
        public string Resolution { get; set; } = "1920x1080";
        public float Brightness { get; set; } = 1.0f;
        public float SwirlSpeed { get; set; } = 1.0f;
        public bool IsBackgroundStatic { get; set; } = false;
        public float MasterVolume { get; set; } = 1.0f;
        public float MusicVolume { get; set; } = 1.0f;
        public float SfxVolume { get; set; } = 1.0f;
        public List<string> FavoriteSongs { get; set; } = new List<string>();
    }

    /// <summary>
    /// Representa el progreso en un único nivel, incluyendo las tres estrellas. 
    /// </summary>
    public class LevelProgress
    {
        public bool IsUnlocked { get; set; } = false;
        public bool StarFromTime { get; set; } = false;
        public bool StarFromErrors { get; set; } = false;
        public bool StarFromScore { get; set; } = false;
    }

    /// <summary>
    /// Almacena todos los datos de una de las tres ranuras de partida. 
    /// </summary>
    public class GameSlot
    {
        public bool IsEmpty { get; set; } = true;
        public string SlotName { get; set; } = "Ranura Vacía"; [cite: 12]
        public Difficulty Difficulty { get; set; }
        public DateTime CreationDate { get; set; }
        public LevelProgress[] Levels { get; set; } = new LevelProgress[10];

        public GameSlot()
        {
            for (int i = 0; i < 10; i++)
            {
                Levels[i] = new LevelProgress();
            }
            // El primer nivel siempre está desbloqueado al crear una partida.
            Levels[0].IsUnlocked = true;
        }
    }

    /// <summary>
    /// La clase raíz que contiene TODOS los datos que se guardarán en el archivo JSON.
    /// Esto incluye la configuración del juego y las tres ranuras de guardado. [cite: 85]
    /// </summary>
    public class SaveData
    {
        public GameSettings Settings { get; set; } = new GameSettings();
        public List<GameSlot> GameSlots { get; set; } = new List<GameSlot> { new GameSlot(), new GameSlot(), new GameSlot() };
    }
}