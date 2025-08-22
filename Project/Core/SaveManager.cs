using System;
using System.IO;
using System.Collections.Generic;
// Se requiere una librería para manejar JSON. System.Text.Json es la estándar de .NET.
using System.Text.Json;

namespace MathCross
{
    #region Estructuras de Datos para Guardado
    // Estas clases definen qué datos vamos a guardar en el archivo JSON.

    /// <summary>
    /// Almacena las configuraciones del juego que el usuario puede modificar.
    /// </summary>
    public class GameSettings
    {
        public string Resolution { get; set; } = "1920x1080"; // [cite: 59]
        public float Brightness { get; set; } = 1.0f; // [cite: 60]
        public float SwirlSpeed { get; set; } = 1.0f; // [cite: 60]
        public bool IsBackgroundStatic { get; set; } = false; // [cite: 60]
        public float MasterVolume { get; set; } = 1.0f; // [cite: 61]
        public float MusicVolume { get; set; } = 1.0f; // [cite: 61]
        public float SfxVolume { get; set; } = 1.0f; // [cite: 61]
        public List<string> FavoriteSongs { get; set; } = new List<string>(); // [cite: 67]
    }

    /// <summary>
    /// Representa el progreso en un único nivel.
    /// </summary>
    public class LevelProgress
    {
        public bool IsUnlocked { get; set; } = false;
        public bool StarFromTime { get; set; } = false;   // [cite: 25]
        public bool StarFromErrors { get; set; } = false; // [cite: 25]
        public bool StarFromScore { get; set; } = false;  // [cite: 25]
    }
    
    /// <summary>
    /// Almacena todos los datos de una de las tres ranuras de partida.
    /// </summary>
    public class GameSlot
    {
        public bool IsEmpty { get; set; } = true; // [cite: 12]
        public string SlotName { get; set; } = "Ranura Vacía"; // [cite: 12]
        public Difficulty Difficulty { get; set; } // [cite: 11]
        public DateTime CreationDate { get; set; } // [cite: 11]
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
    /// La clase principal que contiene TODOS los datos que se guardarán en el archivo.
    /// </summary>
    public class SaveData
    {
        public GameSettings Settings { get; set; } = new GameSettings();
        public List<GameSlot> GameSlots { get; set; } = new List<GameSlot> { new GameSlot(), new GameSlot(), new GameSlot() }; // [cite: 11]
    }

    #endregion

    /// <summary>
    /// Se encarga de leer y escribir el estado del juego (progreso y configuración) en un archivo.
    /// </summary>
    public class SaveManager
    {
        public SaveData Data { get; private set; }
        private readonly string _savePath;

        public SaveManager()
        {
            // Construye la ruta al archivo de guardado en AppData\Roaming\Math-Cross\savedata.json 
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string gameDirectory = Path.Combine(appDataPath, "Math-Cross");
            
            // Crea la carpeta si no existe.
            Directory.CreateDirectory(gameDirectory);

            _savePath = Path.Combine(gameDirectory, "savedata.json");
            Load();
        }

        /// <summary>
        /// Carga los datos desde el archivo JSON. Si el archivo no existe, crea datos por defecto.
        /// </summary>
        public void Load()
        {
            if (File.Exists(_savePath))
            {
                string json = File.ReadAllText(_savePath);
                Data = JsonSerializer.Deserialize<SaveData>(json);
                Console.WriteLine($"Datos cargados desde: {_savePath}");
            }
            else
            {
                // Si no hay archivo de guardado, crea un nuevo objeto SaveData con valores iniciales.
                Data = new SaveData();
                Console.WriteLine("No se encontró archivo de guardado. Creando datos por defecto.");
            }
        }

        /// <summary>
        /// Escribe el estado actual del objeto 'Data' al archivo JSON en el disco.
        /// </summary>
        public void Save()
        {
            var options = new JsonSerializerOptions { WriteIndented = true }; // Hace el JSON más legible
            string json = JsonSerializer.Serialize(Data, options);
            File.WriteAllText(_savePath, json);
            Console.WriteLine($"Datos guardados en: {_savePath}");
        }
    }
}