using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks; // Añadido para métodos asíncronos opcionales

namespace MathCross
{
    /// <summary>
    /// Gestiona la carga, guardado y eliminación de las partidas guardadas del juego.
    /// Las partidas se almacenan de forma persistente en archivos JSON.
    /// </summary>
    public static class SaveManager
    {
        // Ruta del directorio donde se guardarán los archivos de partida.
        // Utiliza AppDomain.CurrentDomain.BaseDirectory para un guardado persistente
        // en el directorio de ejecución de la aplicación.
        private static string SaveDirectory => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "saves");

        /// <summary>
        /// Carga los datos de una partida guardada desde un slot específico.
        /// </summary>
        /// <param name="slot">El número de slot de la partida (ej. 1, 2, 3).</param>
        /// <returns>
        /// Un objeto SaveData con los datos de la partida si se cargó correctamente;
        /// de lo contrario, null si el archivo no existe o hay un error durante la carga.
        /// </returns>
        public static SaveData LoadSlot(int slot)
        {
            string path = Path.Combine(SaveDirectory, $"slot{slot}.json");

            // Verifica si el archivo de guardado existe para el slot dado.
            if (!File.Exists(path))
            {
                Console.WriteLine($"No se encontró el archivo de guardado para el slot {slot}.");
                return null;
            }

            try
            {
                string json = File.ReadAllText(path); // Lee el contenido del archivo JSON.
                // Deserializa el JSON a un objeto SaveData.
                return JsonSerializer.Deserialize<SaveData>(json);
            }
            catch (JsonException ex) // Captura excepciones específicas de JSON.
            {
                Console.Error.WriteLine($"Error al deserializar el archivo de guardado del slot {slot}: {ex.Message}");
                return null;
            }
            catch (Exception ex) // Captura cualquier otra excepción.
            {
                Console.Error.WriteLine($"Ocurrió un error inesperado al cargar el slot {slot}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Guarda los datos de una partida en un slot específico.
        /// El directorio de guardado se crea si no existe.
        /// </summary>
        /// <param name="slot">El número de slot donde se guardará la partida.</param>
        /// <param name="data">El objeto SaveData que contiene los datos a guardar.</param>
        public static void SaveSlot(int slot, SaveData data)
        {
            // Crea el directorio de guardado si no existe.
            if (!Directory.Exists(SaveDirectory))
            {
                Directory.CreateDirectory(SaveDirectory);
            }

            string path = Path.Combine(SaveDirectory, $"slot{slot}.json");
            // Serializa el objeto SaveData a una cadena JSON con formato legible.
            string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });

            try
            {
                File.WriteAllText(path, json); // Escribe la cadena JSON en el archivo.
                Console.WriteLine($"Partida guardada exitosamente en el slot {slot}.");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error al guardar la partida en el slot {slot}: {ex.Message}");
            }
        }

        /// <summary>
        /// Elimina los datos de una partida guardada de un slot específico.
        /// </summary>
        /// <param name="slot">El número de slot de la partida a eliminar.</param>
        public static void DeleteSlot(int slot)
        {
            string path = Path.Combine(SaveDirectory, $"slot{slot}.json");

            // Verifica si el archivo de guardado existe antes de intentar eliminarlo.
            if (File.Exists(path))
            {
                try
                {
                    File.Delete(path); // Elimina el archivo.
                    Console.WriteLine($"Partida del slot {slot} eliminada exitosamente.");
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Error al eliminar la partida del slot {slot}: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine($"No se encontró el archivo de guardado para eliminar en el slot {slot}.");
            }
        }

        // --- Opcional: Métodos asíncronos para evitar bloquear la UI en operaciones grandes ---

        /// <summary>
        /// Carga los datos de una partida guardada desde un slot específico de forma asíncrona.
        /// </summary>
        /// <param name="slot">El número de slot de la partida (ej. 1, 2, 3).</param>
        /// <returns>
        /// Una tarea que devuelve un objeto SaveData si se cargó correctamente;
        /// de lo contrario, null si el archivo no existe o hay un error durante la carga.
        /// </returns>
        public static async Task<SaveData> LoadSlotAsync(int slot)
        {
            string path = Path.Combine(SaveDirectory, $"slot{slot}.json");
            if (!File.Exists(path))
                return null;

            try
            {
                string json = await File.ReadAllTextAsync(path);
                return JsonSerializer.Deserialize<SaveData>(json);
            }
            catch (JsonException ex)
            {
                Console.Error.WriteLine($"Error asíncrono al deserializar el archivo de guardado del slot {slot}: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Ocurrió un error inesperado asíncrono al cargar el slot {slot}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Guarda los datos de una partida en un slot específico de forma asíncrona.
        /// El directorio de guardado se crea si no existe.
        /// </summary>
        /// <param name="slot">El número de slot donde se guardará la partida.</param>
        /// <param name="data">El objeto SaveData que contiene los datos a guardar.</param>
        /// <returns>Una tarea que representa la operación de guardado asíncrona.</returns>
        public static async Task SaveSlotAsync(int slot, SaveData data)
        {
            if (!Directory.Exists(SaveDirectory))
            {
                Directory.CreateDirectory(SaveDirectory);
            }

            string path = Path.Combine(SaveDirectory, $"slot{slot}.json");
            string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });

            try
            {
                await File.WriteAllTextAsync(path, json);
                Console.WriteLine($"Partida guardada asíncronamente exitosamente en el slot {slot}.");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error asíncrono al guardar la partida en el slot {slot}: {ex.Message}");
            }
        }

        /// <summary>
        /// Elimina los datos de una partida guardada de un slot específico de forma asíncrona.
        /// </summary>
        /// <param name="slot">El número de slot de la partida a eliminar.</param>
        /// <returns>Una tarea que representa la operación de eliminación asíncrona.</returns>
        public static async Task DeleteSlotAsync(int slot)
        {
            string path = Path.Combine(SaveDirectory, $"slot{slot}.json");
            if (File.Exists(path))
            {
                try
                {
                    // No hay una versión asíncrona directa de File.Delete,
                    // se puede envolver en Task.Run si se necesita mover a un hilo de fondo.
                    // Para archivos pequeños, la versión síncrona está bien.
                    await Task.Run(() => File.Delete(path));
                    Console.WriteLine($"Partida del slot {slot} eliminada asíncronamente exitosamente.");
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Error asíncrono al eliminar la partida del slot {slot}: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine($"No se encontró el archivo de guardado para eliminar asíncronamente en el slot {slot}.");
            }
        }
    }
}