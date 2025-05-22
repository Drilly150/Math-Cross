using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms; // Necesario para Application.StartupPath
using WMPLib; // Windows Media Player COM reference

namespace MathCross
{
    public static class MusicManager
    {
        private static List<string> pistas = new List<string>(); // Inicialización explícita para evitar posibles nulos
        private static WindowsMediaPlayer reproductor = new WindowsMediaPlayer(); // Inicialización explícita
        private static Random rand = new Random(); // Inicialización explícita

        /// <summary>
        /// Inicializa el gestor de música, creando la carpeta "MusicaCustom" si no existe,
        /// cargando las pistas de audio y configurando el reproductor.
        /// </summary>
        public static void Inicializar()
        {
            string carpeta = Path.Combine(Application.StartupPath, "MusicaCustom");

            if (!Directory.Exists(carpeta)) // Verifica si la carpeta de música existe
                Directory.CreateDirectory(carpeta); // Si no existe, la crea

            // Carga todas las pistas .mp3 de la carpeta "MusicaCustom" y sus subdirectorios
            pistas = Directory.GetFiles(carpeta, "*.mp3", SearchOption.AllDirectories).ToList();

            if (pistas.Count > 0) // Solo si hay pistas disponibles
            {
                // Asegúrate de que GameStateManager.Configuracion esté inicializado
                // Esto es una suposición, GameStateManager.Configuracion debe cargar los ajustes antes.
                reproductor.settings.volume = GameStateManager.Configuracion.VolumenMusica;
                reproductor.PlayStateChange += Reproductor_PlayStateChange;
                ReproducirAleatoria();
            }
        }

        /// <summary>
        /// Maneja los cambios de estado de reproducción del Windows Media Player.
        /// Si una pista termina, reproduce otra aleatoriamente.
        /// </summary>
        /// <param name="estado">El estado actual de reproducción.</param>
        private static void Reproductor_PlayStateChange(int estado)
        {
            // Cuando termine, reproducir otra
            if ((WMPPlayState)estado == WMPPlayState.wmppsMediaEnded)
                ReproducirAleatoria();
        }

        private static int indiceActual = 0;

        /// <summary>
        /// Obtiene el nombre de la pista actual sin la extensión del archivo.
        /// </summary>
        public static string PistaActual => Path.GetFileNameWithoutExtension(pistas.ElementAtOrDefault(indiceActual) ?? "Sin música");

        /// <summary>
        /// Avanza a la siguiente pista en la lista y la reproduce.
        /// </summary>
        public static void Siguiente()
        {
            if (pistas.Count == 0) return; // No hacer nada si no hay pistas
            indiceActual = (indiceActual + 1) % pistas.Count; // Calcula el índice de la siguiente pista
            ReproducirActual(); // Reproduce la pista actual
        }

        /// <summary>
        /// Retrocede a la pista anterior en la lista y la reproduce.
        /// </summary>
        public static void Anterior()
        {
            if (pistas.Count == 0) return; // No hacer nada si no hay pistas
            indiceActual = (indiceActual - 1 + pistas.Count) % pistas.Count; // Calcula el índice de la pista anterior
            ReproducirActual(); // Reproduce la pista actual
        }

        /// <summary>
        /// Pausa o reanuda la reproducción de la pista actual.
        /// </summary>
        public static void PausarOContinuar()
        {
            if (reproductor.playState == WMPPlayState.wmppsPlaying) // Si está reproduciendo
                reproductor.controls.pause(); // Pausa
            else // Si no está reproduciendo (pausado, detenido, etc.)
                reproductor.controls.play(); // Reproduce
        }

        /// <summary>
        /// Reproduce la pista actualmente seleccionada por 'indiceActual'.
        /// </summary>
        private static void ReproducirActual()
        {
            reproductor.URL = pistas[indiceActual]; // Establece la URL de la pista
            reproductor.controls.play(); // Inicia la reproducción
        }

        /// <summary>
        /// Selecciona una pista aleatoria de la lista y la reproduce.
        /// </summary>
        public static void ReproducirAleatoria()
        {
            if (pistas.Count == 0) return; // No hacer nada si no hay pistas

            string pista = pistas[rand.Next(pistas.Count)]; // Obtiene una pista aleatoria
            reproductor.URL = pista; // Establece la URL
            reproductor.controls.play(); // Inicia la reproducción
        }

        /// <summary>
        /// Actualiza el volumen del reproductor de música.
        /// </summary>
        /// <param name="nuevoVolumen">El nuevo nivel de volumen (0-100).</param>
        public static void ActualizarVolumen(int nuevoVolumen)
        {
            reproductor.settings.volume = nuevoVolumen; // Establece el nuevo volumen
        }

        /// <summary>
        /// Detiene la reproducción de la música.
        /// </summary>
        public static void Detener()
        {
            reproductor.controls.stop(); // Detiene la reproducción
        }
    }
}

//Maneja la reproducción de música desde la carpeta "MusicaCustom" la cual se crea automaticamente al inciar el juego por primera vez. Llamado por "MainMenu" o "GameStateManager" en el arranque. Controla visualmente desde "MusicWidget.cs" y actualiza el volumen desde "ConfiguracionPanel".

//Cabe mencionar que la palabra "<Summary>" sirve para separar los comentarios del codigo en si. No afecta directamente en el codigo debido a que es un comentario.  