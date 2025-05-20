using System.IO;
using System.Text.Json;

namespace MathCross
{
    public class GameSettings
    {
        public string Resolucion { get; set; } = "1280x720";
        public bool PantallaCompleta { get; set; } = false;
        public int VolumenMusica { get; set; } = 70;
        public int VolumenEfectos { get; set; } = 70;
        public bool TemaOscuro { get; set; } = false;

        private static string ruta => Path.Combine(Application.StartupPath, "config.json");

        public static GameSettings Cargar()
        {
            if (!File.Exists(ruta))
                return new GameSettings();

            string json = File.ReadAllText(ruta);
            return JsonSerializer.Deserialize<GameSettings>(json);
        }

        public void Guardar()
        {
            string json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(ruta, json);
        }
    }
}
