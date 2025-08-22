using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
// Se necesitarían using para la librería gráfica (ej: Microsoft.Xna.Framework)

namespace MathCross.UI
{
    /// <summary>
    /// Representa un único nivel (esfera circular) en la pantalla de selección de niveles. 
    /// </summary>
    public class LevelNode
    {
        /// <summary>
        /// El índice del nivel (0 a 9).
        /// </summary>
        public int LevelIndex { get; }

        /// <summary>
        /// Determina si el nivel ha sido desbloqueado y es jugable. [cite: 21]
        /// </summary>
        public bool IsUnlocked { get; set; }

        // --- Propiedades para Animación ---
        // public Vector2 BasePosition { get; set; } // Posición central del nodo
        // private Vector2 _animationOffset; // Desplazamiento para la animación de movimiento

        public LevelNode(int index /*, Vector2 basePosition */)
        {
            LevelIndex = index;
            // BasePosition = basePosition;
        }

        /// <summary>
        /// Actualiza la animación de movimiento individual del nodo.
        /// </summary>
        public void UpdateAnimation(/* GameTime gameTime */)
        {
            // Este método implementa el "pequeño movimiento que cada una tiene de manera individual". 
            // Se podría usar una función seno o coseno con el tiempo de juego para crear
            // un efecto suave de flotación o balanceo.
            // float time = (float)gameTime.TotalGameTime.TotalSeconds;
            // _animationOffset.Y = (float)Math.Sin(time * 2f + LevelIndex) * 3f; // El "+ LevelIndex" desfasa las animaciones
        }

        /// <summary>
        /// Inicia la animación de "salto" al desbloquear o completar un nivel. 
        /// </summary>
        public void TriggerUnlockAnimation()
        {
            // Lógica para una animación de salto corta y rápida.
            Console.WriteLine($"Iniciando animación de desbloqueo para el nivel {LevelIndex + 1}.");
        }

        /// <summary>
        /// Dibuja el nodo del nivel en la pantalla.
        /// </summary>
        public void Draw(/* SpriteBatch spriteBatch */)
        {
            // 1. Calcular la posición final de dibujado
            // var drawPosition = BasePosition + _animationOffset;

            // 2. Determinar el color basado en si está desbloqueado y la dificultad del juego. 
            // Color nodeColor;
            // if (IsUnlocked)
            // {
            //     // Asignar color basado en la dificultad (azul, amarillo, rojo). [cite: 56]
            // }
            // else
            // {
            //     // El nivel estará oscuro si no está desbloqueado. [cite: 21]
            //     nodeColor = Color.DarkGray;
            // }

            // 3. Dibujar la esfera circular. 
            // spriteBatch.Draw(circleTexture, drawPosition, nodeColor);
        }
    }
}