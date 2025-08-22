namespace MathCross.Core
{
    /// <summary>
    /// Representa una única pista de música encontrada en la biblioteca del usuario.
    /// Funciona como una estructura de datos simple.
    /// </summary>
    public class MusicTrack
    {
        /// <summary>
        /// El título de la canción, generalmente extraído del nombre del archivo.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// El nombre del álbum. Si la canción está en una subcarpeta, será el nombre de la carpeta;
        /// de lo contrario, será "Pistas independientes".
        /// </summary>
        public string Album { get; set; }

        /// <summary>
        /// La ruta completa al archivo de música en el disco.
        /// </summary>
        public string FilePath { get; set; }
    }
}