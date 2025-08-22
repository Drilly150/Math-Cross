using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

// Se necesitarían using para la librería gráfica (ej: Microsoft.Xna.Framework)

namespace MathCross
{
    /// <summary>
    /// Gestiona la pantalla de selección de dificultad.
    /// </summary>
    public class DifficultySelectController
    {
        private readonly GameManager _gameManager;
        private readonly SaveManager _saveManager;
        private readonly List<Button> _difficultyButtons;

        public DifficultySelectController(GameManager gameManager, SaveManager saveManager)
        {
            _gameManager = gameManager;
            _saveManager = saveManager;
            _difficultyButtons = new List<Button>();

            // --- Creación de los botones de dificultad ---
            // La acción de cada botón llama a un método para confirmar la selección.

            // Botón Fácil
            _difficultyButtons.Add(new Button("Easy", /* new Rectangle(x, y, w, h), */ () => {
                ConfirmSelection(Difficulty.Easy);
            }));

            // Botón Normal
            _difficultyButtons.Add(new Button("Normal", /* new Rectangle(x, y, w, h), */ () => {
                ConfirmSelection(Difficulty.Normal);
            }));

            // Botón Difícil
            _difficultyButtons.Add(new Button("Dificult", /* new Rectangle(x, y, w, h), */ () => {
                ConfirmSelection(Difficulty.Difficult);
            }));
        }

        /// <summary>
        /// Se ejecuta cuando el jugador hace clic en una dificultad.
        /// </summary>
        /// <param name="selectedDifficulty">La dificultad elegida por el jugador.</param>
        private void ConfirmSelection(Difficulty selectedDifficulty)
        {
            // Obtenemos la ranura de guardado activa del GameManager.
            GameSlot activeSlot = _saveManager.Data.GameSlots[_gameManager.ActiveSaveSlotIndex];

            // Si es una partida nueva, la configuramos.
            if (activeSlot.IsEmpty)
            {
                activeSlot.IsEmpty = false;
                activeSlot.Difficulty = selectedDifficulty;
                activeSlot.SlotName = $"Partida {selectedDifficulty}"; // Nombre por defecto
                activeSlot.CreationDate = DateTime.Now;

                // Guardamos los cambios para que la nueva partida quede registrada.
                _saveManager.Save();
            }

            // Le decimos al GameManager que avance a la pantalla de niveles con la dificultad seleccionada.
            _gameManager.GoToLevelSelect(selectedDifficulty);
        }

        /// <summary>
        /// Actualiza la lógica de la pantalla, como la interacción del usuario.
        /// </summary>
        public void Update()
        {
            // Lógica para comprobar el hover y el clic del ratón, similar a los otros controladores.
            foreach (var button in _difficultyButtons)
            {
                button.CheckHover(/* mousePosition */);
                // if (button.IsHovered && mouseClicked) button.OnClick();
            }
        }

        /// <summary>
        /// Dibuja los elementos de la pantalla de selección de dificultad.
        /// </summary>
        public void Draw()
        {
            // Dibuja los tres rectángulos centrados para las dificultades[cite: 15].
            foreach (var button in _difficultyButtons)
            {
                // var buttonColor = GetDifficultyColor(button.Text, button.IsHovered);

                // Dibuja el rectángulo del botón.
                // DrawRectangle(button.Bounds, buttonColor);

                // Dibuja el texto centrado en el rectángulo.
                // DrawText(button.Text, textPosition, font);
            }
        }
        
        /// <summary>
        /// Devuelve el color apropiado para un botón de dificultad, cambiando si el mouse está encima.
        /// </summary>
        /// <returns>El color para el botón.</returns>
        private object GetDifficultyColor(string difficultyName, bool isHovered)
        {
            if (isHovered)
            {
                switch (difficultyName)
                {
                    case "Easy": return "LightBlue"; // Easy se iluminará de un azul claro 
                    case "Normal": return "Yellow";  // Normal de un amarillo 
                    case "Dificult": return "Red";     // Dificult de un rojo 
                }
            }
            
            // Color por defecto si no está 'hovered'
            return "LightGray"; // Las opciones seleccionables, estarán dentro de un rectángulo de color gris claro 
        }
    }
}