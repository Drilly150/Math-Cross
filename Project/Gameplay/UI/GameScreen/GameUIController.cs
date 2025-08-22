using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace MathCross.UI
{
    /// <summary>
    /// Gestiona y dibuja los elementos de la UI para la pantalla de juego principal.
    /// </summary>
    public class GameUIController
    {
        // private readonly GameScreenController _gameScreen; // Referencia al controlador principal
        // private Button _nextLevelButton;
        // private Button _backButton;

        public GameUIController(/*GameScreenController gameScreenController*/)
        {
            // _gameScreen = gameScreenController;
            // Inicializar botones y otros elementos de la UI
        }

        /// <summary>
        /// Actualiza la lógica de los botones de la UI (Siguiente Nivel, Volver).
        /// </summary>
        public void Update()
        {
            // Lógica de interacción con los botones
        }

        /// <summary>
        /// Dibuja el panel lateral y toda la información del juego.
        /// </summary>
        public void Draw()
        {
            // 1. Dibujar el panel lateral derecho.

            // 2. Obtener los datos actuales desde el GameScreenController
            // var currentTime = _gameScreen.GetCurrentTime();
            // var currentErrors = _gameScreen.GetErrorCount();
            // var currentScore = _gameScreen.GetScore();

            // 3. Dibujar la información en el panel
            // - Dibujar el temporizador en la estrella del tiempo límite.
            // - Dibujar el contador de errores.
            // - Dibujar el puntaje.

            // 4. Si el nivel está completado, mostrar el botón "Siguiente nivel".
            // if (_gameScreen.IsLevelComplete)
            // {
            //     _nextLevelButton.Draw();
            // }
        }
    }
}