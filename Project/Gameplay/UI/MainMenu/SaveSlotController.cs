using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
// Se necesitarían using para la librería gráfica (ej: Microsoft.Xna.Framework)

namespace MathCross
{
    /// <summary>
    /// Gestiona la pantalla de selección de ranura de partida.
    /// </summary>
    public class SaveSlotSelectController
    {
        private readonly GameManager _gameManager;
        private readonly SaveManager _saveManager;

        // Podríamos usar una lista de 'Button' o una clase más específica si fuera necesario.
        private readonly List<Button> _slotButtons;
        private readonly Button _backButton;

        public SaveSlotSelectController(GameManager gameManager, SaveManager saveManager)
        {
            _gameManager = gameManager;
            _saveManager = saveManager;
            
            _slotButtons = new List<Button>();

            // Crear los tres botones para las ranuras de guardado. 
            for (int i = 0; i < 3; i++)
            {
                // La acción del botón captura el índice 'i' para saber qué ranura se seleccionó.
                int slotIndex = i;
                _slotButtons.Add(new Button(GetSlotText(i), /* new Rectangle(x, y, w, h), */ () => {
                    _gameManager.SelectSaveSlot(slotIndex);
                }));
            }

            // Crear el botón para volver al menú principal. 
            _backButton = new Button("X", /* new Rectangle(x, y, w, h), */ () => {
                _gameManager.GoToMainMenu();
            });
        }
        
        /// <summary>
        /// Genera el texto que se mostrará en una ranura de guardado.
        /// </summary>
        /// <param name="slotIndex">El índice de la ranura (0, 1, o 2).</param>
        /// <returns>El texto formateado para la ranura.</returns>
        private string GetSlotText(int slotIndex)
        {
            GameSlot slotData = _saveManager.Data.GameSlots[slotIndex];

            if (slotData.IsEmpty)
            {
                return "Ranura Vacía"; // 
            }

            // Calcula el total de estrellas obtenidas.
            int totalStars = 0;
            foreach (var level in slotData.Levels)
            {
                if (level.StarFromTime) totalStars++;
                if (level.StarFromErrors) totalStars++;
                if (level.StarFromScore) totalStars++;
            }
            
            // Construye el string con toda la información de la partida. 
            return $"{slotData.SlotName}\n\n" +
                    $"Estrellas: ({totalStars:00}/30)\n" +
                    $"Dificultad: {slotData.Difficulty}\n" +
                    $"Fecha: {slotData.CreationDate:dd/MM/yy}";
        }

        /// <summary>
        /// Actualiza la lógica de la pantalla, como la interacción con los botones.
        /// </summary>
        public void Update()
        {
            // La lógica para comprobar el hover y el clic del ratón iría aquí,
            // similar a MainMenuController.
            
            _backButton.CheckHover(/* mousePosition */);
            // if (_backButton.IsHovered && mouseClicked) _backButton.OnClick();

            foreach (var button in _slotButtons)
            {
                button.CheckHover(/* mousePosition */);
                // if (button.IsHovered && mouseClicked) button.OnClick();
            }
        }

        /// <summary>
        /// Dibuja los elementos de la pantalla de selección de ranura.
        /// </summary>
        public void Draw()
        {
            // 1. Dibujar cada una de las tres ranuras de partida.
            foreach (var button in _slotButtons)
            {
                // Dibujar el recuadro para la ranura.
                // DrawRectangle(button.Bounds, backgroundColor);
                // Dibujar el texto multilínea dentro del recuadro.
                // DrawText(button.Text, textPosition, font);
            }

            // 2. Dibujar el botón de volver en la esquina superior izquierda.
            // Sería un cuadrado rojo con una 'X' en el centro. 
            // DrawRectangle(_backButton.Bounds, Color.Red);
            // DrawText(_backButton.Text, backButtonTextPosition, font, Color.White);
        }
    }
}