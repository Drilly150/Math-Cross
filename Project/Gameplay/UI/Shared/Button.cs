using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
// Se necesitarían using para la librería gráfica (ej: Microsoft.Xna.Framework)
// para tipos como Rectangle, Vector2, Color, etc.

namespace MathCross.UI
{
    /// <summary>
    /// Representa un botón genérico y reutilizable para la interfaz de usuario.
    /// </summary>
    public class Button
    {
        /// <summary>
        /// El texto que se muestra en el botón.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// El área rectangular que ocupa el botón en la pantalla.
        /// </summary>
        // public Rectangle Bounds { get; }

        /// <summary>
        /// La acción que se ejecuta cuando se hace clic en el botón.
        /// </summary>
        private readonly Action _onClick;

        /// <summary>
        /// Indica si el cursor del ratón está actualmente sobre el botón.
        /// </summary>
        public bool IsHovered { get; private set; }

        /// <summary>
        /// Crea una nueva instancia de un botón.
        /// </summary>
        /// <param name="bounds">La posición y tamaño del botón.</param>
        /// <param name="text">El texto a mostrar.</param>
        /// <param name="onClick">La función a ejecutar al hacer clic.</param>
        public Button(/*Rectangle bounds,*/ string text, Action onClick)
        {
            // Bounds = bounds;
            Text = text;
            _onClick = onClick;
        }

        /// <summary>
        /// Actualiza el estado del botón, comprobando la interacción del ratón.
        /// </summary>
        /// <param name="mousePosition">La posición actual del cursor del ratón.</param>
        /// <param name="isLeftMouseButtonClicked">True si el botón izquierdo del ratón fue presionado en este fotograma.</param>
        public void Update(/*Vector2 mousePosition, bool isLeftMouseButtonClicked*/)
        {
            // Comprueba si el ratón está sobre el botón.
            // IsHovered = Bounds.Contains(mousePosition);

            // Si el ratón está encima y se hace clic, ejecuta la acción.
            // if (IsHovered && isLeftMouseButtonClicked)
            // {
            //     _onClick?.Invoke();
            // }
        }

        /// <summary>
        /// Dibuja el botón en la pantalla.
        /// </summary>
        public void Draw(/*SpriteBatch spriteBatch, SpriteFont font*/)
        {
            // Determina el color basado en si el ratón está encima o no.
            // var color = IsHovered ? HoverColor : DefaultColor;

            // Dibuja el rectángulo de fondo del botón.
            // spriteBatch.Draw(pixelTexture, Bounds, color);

            // Dibuja el texto centrado dentro del botón.
            // var textSize = font.MeasureString(Text);
            // var textPosition = new Vector2(Bounds.Center.X - textSize.X / 2, Bounds.Center.Y - textSize.Y / 2);
            // spriteBatch.DrawString(font, Text, textPosition, TextColor);
        }
    }
}