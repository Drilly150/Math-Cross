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
                var freshProgress = new LevelProgress();
                freshProgress.Levels.TryAdd("P1", new LevelData { IsUnlocked = true });
                return freshProgress;
            }

            try
            {
                string json = File.ReadAllText(FilePath);
                return JsonSerializer.Deserialize<LevelProgress>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading progress: {ex.Message}");
                return new LevelProgress();
            }
        }

        public static void Save(LevelProgress progress)
        {
            try
            {
                string json = JsonSerializer.Serialize(progress, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(FilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving progress: {ex.Message}");
                throw; // Opcional: Relanzar para manejo externo
            }
        }

        public static void CompleteLevel(string levelId, int stars, int timeSeconds)
        {
            if (string.IsNullOrEmpty(levelId))
                throw new ArgumentException("Level ID cannot be null or empty.");

            if (stars < 0 || timeSeconds < 0)
                throw new ArgumentException("Stars and time cannot be negative.");

            var progress = Load();
            if (!progress.Levels.TryGetValue(levelId, out var data))
            {
                data = new LevelData();
                progress.Levels[levelId] = data;
            }

            data.IsUnlocked = true;
            data.Stars = Math.Max(stars, data.Stars);
            data.RecordTime = (data.RecordTime == 0 || timeSeconds < data.RecordTime) ? timeSeconds : data.RecordTime;

            // Actualizar métricas
            data.TotalAttempts++;
            data.CumulativeTime += timeSeconds;

            Save(progress);
        }
    }
}