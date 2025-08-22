using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
// Se necesitarían using para la librería gráfica (ej: Microsoft.Xna.Framework)

namespace MathCross
{
    /// <summary>
    /// Clase auxiliar que representa un único nivel en la pantalla de selección.
    /// </summary>
    public class LevelNode
    {
        public int LevelIndex { get; }
        // public Vector2 Position { get; set; } // Posición en la pantalla
        public bool IsUnlocked { get; set; }
        
        public LevelNode(int index)
        {
            LevelIndex = index;
        }

        public void UpdateAnimation()
        {
            // Lógica para el "pequeño movimiento que cada una tiene de manera individual" 
        }
    }
    
    /// <summary>
    /// Gestiona la lógica y la presentación de la pantalla de selección de niveles.
    /// </summary>
    public class LevelSelectController
    {
        private readonly GameManager _gameManager;
        private readonly SaveManager _saveManager;

        private readonly List<LevelNode> _levelNodes = new List<LevelNode>();
        private readonly Button _backButton;
        
        private int _selectedLevelIndex = 0;
        private bool _isPanelOpen = false;

        public LevelSelectController(GameManager gameManager, SaveManager saveManager)
        {
            _gameManager = gameManager;
            _saveManager = saveManager;
            
            // Crear los 10 nodos de nivel
            for (int i = 0; i < 10; i++)
            {
                _levelNodes.Add(new LevelNode(i));
            }
            
            _backButton = new Button("Volver", /* new Rectangle(x,y,w,h) */ () => {
                // El botón en la esquina superior izquierda te vuelve al menú [cite: 45] (o al menú principal)
                _gameManager.GoToMainMenu(); 
            });
        }
        
        /// <summary>
        /// Se llama cuando se entra a esta pantalla para configurar los niveles según el progreso guardado.
        /// </summary>
        public void LoadLevelData()
        {
            var progress = _saveManager.Data.GameSlots[_gameManager.ActiveSaveSlotIndex];
            for (int i = 0; i < 10; i++)
            {
                _levelNodes[i].IsUnlocked = progress.Levels[i].IsUnlocked;
            }
        }

        /// <summary>
        /// Actualiza la lógica de la pantalla.
        /// </summary>
        public void Update()
        {
            // Lógica para navegar entre niveles con las flechas del teclado 
            // Lógica para seleccionar un nivel con el ratón, lo que abre/cierra el panel 
            
            // Si el jugador pulsa "Jugar" en el panel lateral, inicia el nivel
            // if (playButtonClicked) _gameManager.StartLevel(_selectedLevelIndex);

            // Actualizar la animación de cada nodo
            foreach(var node in _levelNodes)
            {
                node.UpdateAnimation();
            }
        }

        /// <summary>
        /// Dibuja todos los elementos de la pantalla.
        /// </summary>
        public void Draw()
        {
            // 1. Dibujar el fondo con el color de la dificultad seleccionada [cite: 57]
            
            // 2. Dibujar las líneas que interconectan los niveles como un grafo [cite: 19]
            
            // 3. Dibujar cada nodo del nivel
            foreach (var node in _levelNodes)
            {
                // Dibujar la esfera del nivel, oscura si está bloqueada [cite: 21]
            }

            // 4. Dibujar la UI (paneles, contadores)
            if (_isPanelOpen)
            {
                DrawSidePanel();
            }
            else
            {
                // Dibujar el pequeño reproductor de música en una esquina [cite: 22]
                // Dibujar el contador de estrellas en la esquina inferior derecha [cite: 48]
            }
            
            // 5. Dibujar el botón para volver al menú
            // _backButton.Draw();
        }

        /// <summary>
        /// Dibuja el panel lateral con la información del nivel seleccionado.
        /// </summary>
        private void DrawSidePanel()
        {
            // Dibujar el rectángulo translúcido del panel [cite: 23]
            var selectedNode = _levelNodes[_selectedLevelIndex];

            if (selectedNode.IsUnlocked)
            {
                // Panel para nivel DESBLOQUEADO
                // El reproductor de música se posiciona en la parte superior 
                // Dibujar el reproductor con sus botones (anterior, pausa, siguiente) [cite: 27]

                // Mostrar información: tiempo límite, errores, puntaje [cite: 24]
                // Dibujar las 3 estrellas en forma de triángulo [cite: 26]
                // Dibujar el botón de "Jugar" en la parte inferior [cite: 26]
            }
            else
            {
                // Panel para nivel BLOQUEADO
                // El reproductor de música se posiciona centrado 
                // Mostrar el texto "Nivel bloqueado" [cite: 29]
            }
        }
    }
}