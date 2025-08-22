using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using MathCross.Gameplay; // Necesario para LevelNode

namespace MathCross.UI
{
    /// <summary>
    /// Gestiona el panel de información que aparece al seleccionar un nivel.
    /// </summary>
    public class LevelInfoPanel
    {
        public bool IsVisible { get; private set; }
        private bool _isShowingLockedLevel;

        // private Button _playButton;
        // private Button _musicPrevButton;
        // ... otros botones de UI

        /// <summary>
        /// Muestra el panel con la información del nodo seleccionado.
        /// </summary>
        public void Show(LevelNode selectedNode)
        {
            _isShowingLockedLevel = !selectedNode.IsUnlocked;
            IsVisible = true;
        }

        /// <summary>
        /// Oculta el panel.
        /// </summary>
        public void Hide()
        {
            IsVisible = false;
        }

        /// <summary>
        /// Actualiza la lógica del panel (ej. clics en sus botones).
        /// </summary>
        public void Update()
        {
            if (!IsVisible) return;

            // Lógica para los botones del reproductor de música y el botón de Jugar.
        }

        /// <summary>
        /// Dibuja el panel y su contenido.
        /// </summary>
        public void Draw()
        {
            if (!IsVisible) return;

            // 1. Dibujar el fondo del panel (una pequeña barra translúcida del lateral derecho) [cite: 23]

            if (_isShowingLockedLevel)
            {
                // 2a. Dibujar el panel en modo "bloqueado".
                // El reproductor se posiciona centrado y aparece el texto "Nivel bloqueado". 
            }
            else
            {
                // 2b. Dibujar el panel en modo "desbloqueado".
                // El reproductor se posiciona en la parte superior. [cite: 26, 31]
                // Dibujar la información de los objetivos (tiempo, errores, puntaje). [cite: 24]
                // Dibujar las 3 estrellas en forma de triángulo. [cite: 26]
                // Dibujar el botón de "Jugar" en la parte inferior. [cite: 26]
            }
        }
    }
}