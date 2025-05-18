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

        public static void CompletarNivel(string nivel, int estrellas, int tiempo)
        {
            var progress = Load();
            if (!progress.Niveles.ContainsKey(nivel))
                progress.Niveles[nivel] = new LevelData();

            var data = progress.Niveles[nivel];
            data.Estrellas = Math.Max(data.Estrellas, estrellas);
            data.TiempoRecord = (data.TiempoRecord == 0) ? tiempo : Math.Min(data.TiempoRecord, tiempo);

            int num = int.Parse(nivel.Substring(1));
            string siguiente = $"P{num + 1}";
            if (!progress.Niveles.ContainsKey(siguiente))
                progress.Niveles[siguiente] = new LevelData();
            progress.Niveles[siguiente].Desbloqueado = true;

            Save(progress);
        }
    }
}