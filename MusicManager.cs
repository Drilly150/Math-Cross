using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using WMPLib; // Windows Media Player COM reference

namespace MathCross
{
    public static class MusicManager
    {
        private static List<string> pistas = new();
        private static WindowsMediaPlayer reproductor = new();
        private static Random rand = new();

        public static void Inicializar()
        {
            string carpeta = Path.Combine(Application.StartupPath, "MusicaCustom");

            if (!Directory.Exists(carpeta))
                Directory.CreateDirectory(carpeta);

            pistas = Directory.GetFiles(carpeta, "*.mp3", SearchOption.AllDirectories).ToList();

            if (pistas.Count > 0)
            {
                reproductor.settings.volume = GameStateManager.Configuracion.VolumenMusica;
                reproductor.PlayStateChange += Reproductor_PlayStateChange;
                ReproducirAleatoria();
            }
        }

        private static void Reproductor_PlayStateChange(int estado)
        {
            // Cuando termine, reproducir otra
            if ((WMPPlayState)estado == WMPPlayState.wmppsMediaEnded)
                ReproducirAleatoria();
        }

        private static int indiceActual = 0;
        public static string PistaActual => Path.GetFileNameWithoutExtension(pistas.ElementAtOrDefault(indiceActual) ?? "Sin música");

        public static void Siguiente()
        {
            if (pistas.Count == 0) return;
            indiceActual = (indiceActual + 1) % pistas.Count;
            ReproducirActual();
        }

        public static void Anterior()
        {
            if (pistas.Count == 0) return;
            indiceActual = (indiceActual - 1 + pistas.Count) % pistas.Count;
            ReproducirActual();
        }

        public static void PausarOContinuar()
        {
            if (reproductor.playState == WMPPlayState.wmppsPlaying)
                reproductor.controls.pause();
            else
                reproductor.controls.play();
        }

        private static void ReproducirActual()
        {
            reproductor.URL = pistas[indiceActual];
            reproductor.controls.play();
        }

        public static void ReproducirAleatoria()
        {
            if (pistas.Count == 0) return;

            string pista = pistas[rand.Next(pistas.Count)];
            reproductor.URL = pista;
            reproductor.controls.play();
        }

        public static void ActualizarVolumen(int nuevoVolumen)
        {
            reproductor.settings.volume = nuevoVolumen;
        }

        public static void Detener()
        {
            reproductor.controls.stop();
        }
    }
}
