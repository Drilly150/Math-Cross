using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace MathCross.UI
{
    /// <summary>
    /// Una clase de utilidad para manejar la lógica de un temporizador simple.
    /// </summary>
    public class Timer
    {
        private float _elapsedTime;
        private bool _isRunning;

        /// <summary>
        /// El tiempo total transcurrido en segundos.
        /// </summary>

        public float ElapsedTime => _elapsedTime;

        /// <summary>
        /// Proporciona el tiempo transcurrido en un formato de TimeSpan (ej: 00:05:30).
        /// </summary>
        public TimeSpan ElapsedTimeSpan => TimeSpan.FromSeconds(_elapsedTime);

        /// <summary>
        /// Actualiza el temporizador. Debe ser llamado en cada fotograma.
        /// </summary>
        /// <param name="deltaTime">El tiempo transcurrido desde el último fotograma.</param>
        public void Update(float deltaTime)
        {
            if (_isRunning)
            {
                _elapsedTime += deltaTime;
            }
        }

        /// <summary>
        /// Inicia o reanuda el temporizador.
        /// </summary>
        public void Start()
        {
            _isRunning = true;
        }

        /// <summary>
        /// Pausa el temporizador.
        /// </summary>
        public void Stop()
        {
            _isRunning = false;
        }

        /// <summary>
        /// Detiene y reinicia el temporizador a cero.
        /// </summary>
        public void Reset()
        {
            _isRunning = false;
            _elapsedTime = 0f;
        }
    }
}