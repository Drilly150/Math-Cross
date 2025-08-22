using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics; 
// Necesario para abrir la carpeta de música

namespace MathCross
{
    #region Estructuras de Datos de Audio

    /// <summary>
    /// Representa una única pista de música encontrada en la biblioteca del usuario.
    /// </summary>
    public class MusicTrack
    {
        public string Title { get; set; }
        public string Album { get; set; } // Será "Pistas independientes" si no está en una carpeta
        public string FilePath { get; set; }
    }

    #endregion

    /// <summary>
    /// Gestiona la carga y reproducción de música y efectos de sonido.
    /// </summary>
    public class AudioManager
    {
        private readonly SaveManager _saveManager;
        private readonly string _musicLibraryPath;

        public List<MusicTrack> MusicLibrary { get; private set; } = new List<MusicTrack>();
        public MusicTrack CurrentTrack { get; private set; }

        // Aquí irían las variables para manejar la reproducción de audio real.
        // Ejemplo: private SoundPlayer _sfxPlayer;
        // Ejemplo: private MediaPlayer _musicPlayer;

        /// <summary>
        /// Constructor del gestor de audio.
        /// </summary>
        /// <param name="saveManager">Referencia al gestor de datos para acceder a la configuración.</param>
        public AudioManager(SaveManager saveManager)
        {
            _saveManager = saveManager;

            // Define la ruta a la biblioteca de música en Documentos\Math-Cross\Music
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            _musicLibraryPath = Path.Combine(documentsPath, "Math-Cross", "Music");

            // Carga la biblioteca de música al iniciar
            ScanMusicLibrary();
        }

        /// <summary>
        /// Escanea la carpeta de música del usuario para encontrar y organizar las pistas.
        /// </summary>
        public void ScanMusicLibrary()
        {
            MusicLibrary.Clear();
            Directory.CreateDirectory(_musicLibraryPath); // Crea la carpeta si no existe 

            // Procesa las pistas independientes en la raíz de la carpeta de música
            string[] rootFiles = Directory.GetFiles(_musicLibraryPath, "*.mp3"); // Asumimos MP3 por simplicidad
            foreach (var file in rootFiles)
            {
                MusicLibrary.Add(new MusicTrack
                {
                    Title = Path.GetFileNameWithoutExtension(file),
                    Album = "Pistas independientes",
                    FilePath = file
                });
            }

            // Procesa las carpetas como álbumes 
            string[] albumDirectories = Directory.GetDirectories(_musicLibraryPath);
            foreach (var dir in albumDirectories)
            {
                string[] albumFiles = Directory.GetFiles(dir, "*.mp3");
                foreach (var file in albumFiles)
                {
                    MusicLibrary.Add(new MusicTrack
                    {
                        Title = Path.GetFileNameWithoutExtension(file),
                        Album = Path.GetFileName(dir), // El nombre de la carpeta es el álbum
                        FilePath = file
                    });
                }
            }
            Console.WriteLine($"Biblioteca de música actualizada. {MusicLibrary.Count} pistas encontradas.");
        }

        /// <summary>
        /// Abre la carpeta de la biblioteca de música en el explorador de archivos del sistema. 
        /// </summary>
        public void OpenMusicFolder()
        {
            Process.Start("explorer.exe", _musicLibraryPath);
        }

        #region Controles de Reproducción

        public void PlayTrack(MusicTrack track)
        {
            if (track == null || !File.Exists(track.FilePath)) return;
            
            CurrentTrack = track;
            // Lógica de la librería de audio para reproducir el archivo desde track.FilePath
            // _musicPlayer.Open(new Uri(track.FilePath));
            // _musicPlayer.Play();
            Console.WriteLine($"Reproduciendo: {CurrentTrack.Title} - {CurrentTrack.Album}");
            ApplyVolumeSettings();
        }
        
        public void TogglePause() // [cite: 27]
        {
            // Lógica para pausar o reanudar
            Console.WriteLine("Música pausada/reanudada.");
        }

        public void PlayNextTrack() // [cite: 27]
        {
            // Lógica para encontrar y reproducir la siguiente canción de la lista
            Console.WriteLine("Reproduciendo siguiente pista.");
        }

        public void PlayPreviousTrack() // [cite: 27]
        {
            // Lógica para encontrar y reproducir la canción anterior
            Console.WriteLine("Reproduciendo pista anterior.");
        }
        
        public void PlaySfx(string sfxName)
        {
            // Lógica para reproducir un efecto de sonido (ej. "button_click.wav")
            // _sfxPlayer.SoundLocation = $"Assets/SFX/{sfxName}";
            // _sfxPlayer.Play();
            Console.WriteLine($"Reproduciendo SFX: {sfxName}");
        }

        #endregion

        /// <summary>
        /// Aplica los niveles de volumen guardados en la configuración.
        /// </summary>
        public void ApplyVolumeSettings()
        {
            var settings = _saveManager.Data.Settings;
            // _musicPlayer.Volume = settings.MasterVolume * settings.MusicVolume; 
            // _sfxPlayer.Volume = settings.MasterVolume * settings.SfxVolume; 
             Console.WriteLine($"Volumen aplicado: Música a {settings.MasterVolume * settings.MusicVolume}, SFX a {settings.MasterVolume * settings.SfxVolume}");
        }
        
        /// <summary>
        /// Marca o desmarca una canción como favorita y guarda el cambio. 
        /// </summary>
        public void ToggleFavorite(MusicTrack track)
        {
            var favoriteList = _saveManager.Data.Settings.FavoriteSongs;
            if (favoriteList.Contains(track.FilePath))
            {
                favoriteList.Remove(track.FilePath);
                Console.WriteLine($"'{track.Title}' eliminado de favoritos.");
            }
            else
            {
                favoriteList.Add(track.FilePath);
                Console.WriteLine($"'{track.Title}' agregado a favoritos.");
            }
            _saveManager.Save(); // Guarda los cambios en el archivo de configuración
        }
    }
}