using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

// El mismo namespace que usamos en Game.cs
namespace MathCross
{
    /// <summary>
    /// Define los posibles estados o pantallas en las que se puede encontrar el juego.
    /// Esto nos permite saber qué lógica y qué elementos visuales mostrar en cada momento.
    /// </summary>
    public enum GameState
    {
        MainMenu,
        SaveSlotSelect,
        DifficultySelect,
        LevelSelect,
        InGame,
        Settings,
        Info
    }

    /// <summary>
    /// Define las dificultades del juego, extraídas de la documentación. [cite: 11, 14]
    /// </summary>
    public enum Difficulty
    {
        Easy,
        Normal,
        Difficult
    }

    /// <summary>
    /// Gestiona el flujo general y el estado del juego. Actúa como el director de orquesta,
    /// diciendo a los otros componentes qué hacer y cuándo.
    /// </summary>
    public class GameManager
    {
        // Propiedad para acceder al estado actual del juego desde otras clases. Es de solo lectura pública.
        public GameState CurrentState { get; private set; }
        public Difficulty SelectedDifficulty { get; private set; }

        // Referencias a los otros gestores que necesita para funcionar.
        private readonly SaveManager _saveManager;
        private readonly AudioManager _audioManager;
        
        // --- CONTROLADORES DE ESCENA (se crearán en archivos futuros) ---
        // Cada controlador manejará la lógica de una pantalla específica.
        // private readonly MainMenuController _mainMenuController;
        // private readonly LevelSelectController _levelSelectController;
        // ... y así sucesivamente.

        /// <summary>
        /// El constructor del GameManager se llama una sola vez, al iniciar el juego.
        /// </summary>
        /// <param name="saveManager">Una instancia del gestor de guardado.</param>
        /// <param name="audioManager">Una instancia del gestor de audio.</param>
        public GameManager(SaveManager saveManager, AudioManager audioManager)
        {
            _saveManager = saveManager;
            _audioManager = audioManager;
            
            // Cuando el juego comienza, el primer estado es siempre el menú principal. [cite: 6]
            CurrentState = GameState.MainMenu;

            // Aquí inicializaríamos los controladores de cada pantalla.
            // _mainMenuController = new MainMenuController(this);
            // _levelSelectController = new LevelSelectController(this);
            // ...etc.
        }

        #region Métodos de Transición de Estado
        // Estos métodos son llamados por la UI para cambiar de una pantalla a otra.

        public void GoToSaveSlotSelect()
        {
            Console.WriteLine("Transición: Seleccionando ranura de partida...");
            CurrentState = GameState.SaveSlotSelect;
        }
        
        public void SelectSaveSlot(int slotIndex)
        {
            Console.WriteLine($"Ranura {slotIndex} seleccionada. Yendo a selección de dificultad...");
            // Aquí cargaríamos los datos de la ranura si existe. [cite: 12]
            // _saveManager.LoadGame(slotIndex);
            CurrentState = GameState.DifficultySelect;
        }

        public void SelectDifficultyAndStart(Difficulty difficulty)
        {
            Console.WriteLine($"Dificultad '{difficulty}' seleccionada. Yendo a la pantalla de niveles...");
            SelectedDifficulty = difficulty;
            CurrentState = GameState.LevelSelect;
        }

        public void GoToSettings()
        {
            Console.WriteLine("Transición: Abriendo configuración...");
            CurrentState = GameState.Settings;
        }

        public void GoToInfo()
        {
            Console.WriteLine("Transición: Abriendo información...");
            CurrentState = GameState.Info;
        }

        public void GoToMainMenu()
        {
            Console.WriteLine("Transición: Volviendo al menú principal...");
            CurrentState = GameState.MainMenu;
        }
        
        public void QuitGame()
        {
            // La lógica para mostrar el diálogo de confirmación estaría aquí. [cite: 80, 81]
            Console.WriteLine("Mostrando diálogo: '¿Deseas salir?'");
            // Si el usuario hace clic en "Si", entonces:
            // Environment.Exit(0);
        }

        #endregion

        /// <summary>
        /// Se llamará repetidamente desde el bucle principal del juego (en MathCrossGame.cs).
        /// Actualiza la lógica del estado actual.
        /// </summary>
        public void Update()
        {
            // Un switch nos permite ejecutar la lógica correspondiente al estado actual.
            switch (CurrentState)
            {
                case GameState.MainMenu:
                    // _mainMenuController.Update();
                    break;
                case GameState.LevelSelect:
                    // _levelSelectController.Update();
                    break;
                // ... y así para cada estado.
            }
        }

        /// <summary>
        /// Se llamará repetidamente desde el bucle principal del juego.
        /// Dibuja los elementos visuales del estado actual.
        /// </summary>
        public void Draw()
        {
             // De manera similar a Update, dibujamos la pantalla que corresponde.
            switch (CurrentState)
            {
                case GameState.MainMenu:
                    // _mainMenuController.Draw();
                    break;
                case GameState.LevelSelect:
                    // _levelSelectController.Draw();
                    break;
                // ... y así para cada estado.
            }
        }
    }
}