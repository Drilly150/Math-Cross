using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

// Se necesitarían using para la librería gráfica (ej: Microsoft.Xna.Framework)
// para tipos como Rectangle, Vector2, Color, etc.

namespace MathCross
{
    /// <summary>
    /// Clase auxiliar que representa un botón simple en la interfaz de usuario.
    /// </summary>
    public class Button
    {
        public string Text { get; }
        // public Rectangle Bounds { get; } // Representa la posición y tamaño del botón
        public Action OnClick { get; } // La función que se ejecutará al hacer clic

        public bool IsHovered { get; set; } // Para saber si el mouse está encima

        public Button(string text, /* Rectangle bounds, */ Action onClick)
        {
            Text = text;
            // Bounds = bounds;
            OnClick = onClick;
        }

        // En un motor gráfico, se usarían los Bounds para detectar la colisión con el mouse.
        public void CheckHover(/* Vector2 mousePosition */)
        {
            // IsHovered = Bounds.Contains(mousePosition);
        }
    }

    /// <summary>
    /// Gestiona la lógica y la presentación del menú principal del juego.
    /// </summary>
    public class MainMenuController
    {
        private readonly GameManager _gameManager;
        private readonly List<Button> _buttons;

        public MainMenuController(GameManager gameManager)
        {
            _gameManager = gameManager;
            _buttons = new List<Button>();

            // --- Creación de los botones del menú principal ---
            // Las posiciones (Rectangle) serían calculadas para centrarlos en pantalla.

            // Botón Jugar/Continuar partida [cite: 6]
            _buttons.Add(new Button("Jugar", /* new Rectangle(x, y, w, h), */ () => {
                _gameManager.GoToSaveSlotSelect();
            }));

            // Botón Configuración [cite: 6]
            _buttons.Add(new Button("Configuración", /* new Rectangle(x, y, w, h), */ () => {
                _gameManager.GoToSettings();
            }));

            // Botón Información [cite: 6]
            _buttons.Add(new Button("Información", /* new Rectangle(x, y, w, h), */ () => {
                _gameManager.GoToInfo();
            }));

            // Botón Salir [cite: 6]
            _buttons.Add(new Button("Salir", /* new Rectangle(x, y, w, h), */ () => {
                _gameManager.QuitGame();
            }));
        }

        /// <summary>
        /// Actualiza la lógica del menú, principalmente para detectar la interacción del usuario.
        /// Llamado por GameManager.
        /// </summary>
        public void Update()
        {
            // Obtener la posición actual del mouse desde el motor de juego
            // var mousePosition = Mouse.GetState().Position;
            
            foreach (var button in _buttons)
            {
                // button.CheckHover(mousePosition); // Actualiza el estado de hover

                // Si el botón está siendo 'hovered' y se hace clic izquierdo...
                // if (button.IsHovered && Mouse.GetState().LeftButton == ButtonState.Pressed)
                // {
                //     button.OnClick(); // Ejecuta la acción asociada al botón
                // }
            }
        }

        /// <summary>
        /// Dibuja todos los elementos del menú principal en la pantalla.
        /// Llamado por GameManager.
        /// </summary>
        public void Draw()
        {
            // Aquí iría el código de dibujado usando una librería gráfica.

            // 1. Dibujar el título del juego 
            // DrawText("Math-Cross", titlePosition, titleFont, titleColor);

            // 2. Dibujar cada botón
            foreach (var button in _buttons)
            {
                // El color del rectángulo cambia si el mouse está encima.
                // var aColor = button.IsHovered ? hoveredColor : defaultColor;
                
                // Dibuja el rectángulo del botón (gris claro por defecto) 
                // DrawRectangle(button.Bounds, aColor);

                // Dibuja el texto del botón
                // DrawText(button.Text, textPosition, buttonFont, textColor);
            }
        }
    }
}