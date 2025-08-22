using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace MathCross.Core
{
    /// <summary>
    /// Gestiona las transiciones entre escenas con un efecto de fundido (fade).
    /// </summary>
    public class SceneLoader
    {
        private enum FadeState { Idle, FadingOut, FadingIn }
        private FadeState _currentState = FadeState.Idle;

        private float _fadeAlpha = 0f; // 0.0 = transparente, 1.0 = negro opaco
        private const float FADE_SPEED = 2.0f;
        private Action _onFadeOutComplete;

        public bool IsTransitioning => _currentState != FadeState.Idle;

        /// <summary>
        /// Inicia la transición a una nueva escena.
        /// </summary>
        /// <param name="sceneSwitchAction">La acción que cambia la escena, se ejecuta en el punto más oscuro.</param>
        public void LoadScene(Action sceneSwitchAction)
        {
            if (!IsTransitioning)
            {
                _currentState = FadeState.FadingOut;
                _onFadeOutComplete = sceneSwitchAction;
            }
        }

        /// <summary>
        /// Actualiza el estado del fundido. Llamado en cada fotograma.
        /// </summary>
        public void Update(float deltaTime)
        {
            if (_currentState == FadeState.FadingOut)
            {
                _fadeAlpha += FADE_SPEED * deltaTime;
                if (_fadeAlpha >= 1.0f)
                {
                    _fadeAlpha = 1.0f;
                    _onFadeOutComplete?.Invoke(); // Cambia la escena
                    _currentState = FadeState.FadingIn;
                }
            }
            else if (_currentState == FadeState.FadingIn)
            {
                _fadeAlpha -= FADE_SPEED * deltaTime;
                if (_fadeAlpha <= 0f)
                {
                    _fadeAlpha = 0f;
                    _currentState = FadeState.Idle;
                }
            }
        }

        /// <summary>
        /// Dibuja el rectángulo de fundido sobre toda la pantalla.
        /// </summary>
        public void Draw(/* SpriteBatch spriteBatch */)
        {
            if (IsTransitioning)
            {
                // Dibujar un rectángulo negro que cubra toda la pantalla
                // con una transparencia igual a _fadeAlpha.
                // Color fadeColor = new Color(0, 0, 0, _fadeAlpha);
                // spriteBatch.Draw(pixelTexture, screenBounds, fadeColor);
            }
        }
    }
}