using System.IO;
using System.Text.Json;
using System.Windows.Forms; // Necesario para Application.StartupPath

namespace MathCross
{
    public class GameSettings
    {
        public string Resolucion { get; set; } = "1280x720";
        public bool PantallaCompleta { get; set; } = false;
        public int VolumenMusica { get; set; } = 70;
        public int VolumenEfectos { get; set; } = 70;
        public bool TemaOscuro { get; set; } = false;

        // La ruta del archivo de configuración ahora usa Application.StartupPath,
        // que requiere la referencia a System.Windows.Forms
        private static string ruta => Path.Combine(Application.StartupPath, "config.json");

        /// <summary>
        /// Carga la configuración del juego desde el archivo config.json.
        /// Si el archivo no existe, devuelve una nueva instancia de GameSettings con valores predeterminados.
        /// </summary>
        /// <returns>Una instancia de GameSettings con la configuración cargada o predeterminada.</returns>
        public static GameSettings Cargar()
        {
            if (!File.Exists(ruta)) // Verifica si el archivo de configuración existe
                return new GameSettings(); // Si no existe, devuelve una nueva configuración con valores predeterminados

            string json = File.ReadAllText(ruta); // Lee todo el texto del archivo
            return JsonSerializer.Deserialize<GameSettings>(json); // Deserializa el JSON a un objeto GameSettings
        }

        /// <summary>
        /// Guarda la configuración actual del juego en el archivo config.json.
        /// El JSON se guarda con formato para una mejor legibilidad.
        /// </summary>
        public void Guardar()
        {
            // Serializa el objeto GameSettings a una cadena JSON con indentación para que sea legible
            string json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(ruta, json); // Escribe la cadena JSON en el archivo
        }
    }
}