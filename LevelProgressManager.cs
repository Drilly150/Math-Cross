using System;
using System.IO;
using System.Text.Json;

namespace MathCross
{
    public static class LevelProgressManager
    {
        private static string FilePath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "progress.json");

        public static LevelProgress Load()
        {
            if (!File.Exists(FilePath))
            {
                var fresh = new LevelProgress();
                fresh.Niveles["P1"] = new LevelData { Desbloqueado = true };
                return fresh;
            }

            try
            {
                string json = File.ReadAllText(FilePath);
                return JsonSerializer.Deserialize<LevelProgress>(json);
            }
            catch
            {
                return new LevelProgress();
            }
        }

        public static void Save(LevelProgress progress)
        {
            string json = JsonSerializer.Serialize(progress, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(FilePath, json);
        }

        public static void CompletarNivel(string nivelId, int estrellas, int tiempoSegundos)
        {
            var progreso = Load();
            if (!progreso.Niveles.TryGetValue(nivelId, out var data))
            {
                data = new LevelData();
                progreso.Niveles[nivelId] = data;
            }

            data.Desbloqueado = true;

            if (estrellas > data.Estrellas)
                data.Estrellas = estrellas;

            if (data.TiempoRecord == 0 || tiempoSegundos < data.TiempoRecord)
                data.TiempoRecord = tiempoSegundos;

            // 🔹 Actualizar promedio
            data.TotalIntentos++;
            data.TiempoAcumulado += tiempoSegundos;

            Save(progreso);
        }
    }
}