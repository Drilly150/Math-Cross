using System;
using System.IO;
using System.Text.Json;

namespace MathCross
{
    public static class SaveManager
    {
        private static string SaveDirectory => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "saves");

        public static SaveData LoadSlot(int slot)
        {
            string path = Path.Combine(SaveDirectory, $"slot{slot}.json");
            if (!File.Exists(path))
                return null;

            try
            {
                string json = File.ReadAllText(path);
                return JsonSerializer.Deserialize<SaveData>(json);
            }
            catch
            {
                return null;
            }
        }

        public static void SaveSlot(int slot, SaveData data)
        {
            if (!Directory.Exists(SaveDirectory))
                Directory.CreateDirectory(SaveDirectory);

            string path = Path.Combine(SaveDirectory, $"slot{slot}.json");
            string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, json);
        }

        public static void DeleteSlot(int slot)
        {
            string path = Path.Combine(SaveDirectory, $"slot{slot}.json");
            if (File.Exists(path))
                File.Delete(path);
        }
    }
}
