using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

// El namespace (espacio de nombres) principal para todo el proyecto.
namespace MathCross
{
    /// <summary>
    /// La clase principal que contiene el punto de entrada de la aplicación.
    /// </summary>
    public static class Game
    {
        /// <summary>
        /// El punto de entrada principal para la aplicación Math-Cross.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // El bloque 'using' asegura que los recursos del juego se liberen correctamente al cerrar la aplicación.
            // Aquí se crea una instancia de la clase principal del juego y se inicia.
            using (var game = new MathCrossGame())
            {
                game.Run();
            }
        }
    }

    /// <summary>
    /// Esta sería la clase principal del juego.
    /// En un proyecto real, estaría en su propio archivo (ej: MathCrossGame.cs).
    /// Heredaría de una clase base de un framework como MonoGame o FNA.
    /// </summary>
    public class MathCrossGame // : Microsoft.Xna.Framework.Game (Ejemplo de herencia)
    {
        // Administrador de gráficos (un ejemplo, provisto por el framework)
        // private GraphicsDeviceManager _graphics;

        // Gestores principales del juego que diseñamos en la estructura.
        private GameManager _gameManager;
        private AudioManager _audioManager;
        private SaveManager _saveManager;

        public MathCrossGame()
        {
            // Aquí se inicializaría la ventana del juego, por ejemplo, su tamaño inicial.
            // _graphics = new GraphicsDeviceManager(this);
            // Window.Title = "Math-Cross";

            // Se crean las instancias de los gestores principales.
            _saveManager = new SaveManager();
            _audioManager = new AudioManager();
            
            // El GameManager podría necesitar referencias a otros gestores.
            _gameManager = new GameManager(_saveManager, _audioManager);
        }

        /// <summary>
        /// El método para iniciar el bucle principal del juego.
        /// </summary>
        public void Run()
        {
            // En un framework real, este método inicia el bucle que llama a
            // los métodos Initialize(), LoadContent(), Update(), y Draw() repetidamente.
            
            // Para este ejemplo, simulamos el inicio.
            Console.WriteLine("Iniciando el juego Math-Cross...");
            Console.WriteLine("Cargando menú principal...");
            // Aquí comenzaría la lógica para mostrar el menú principal[cite: 6].
            // Por ejemplo: _gameManager.GoToMainMenu();
            Console.WriteLine("Juego en ejecución. Cierre la ventana para salir.");
            
            // El bucle real del juego estaría aquí.
            while (true)
            {
                // 1. Procesar entrada (teclado, mouse).
                // 2. Actualizar estado del juego (lógica, animaciones). _gameManager.Update();
                // 3. Dibujar el frame actual en pantalla. _gameManager.Draw();
            }
        }
    }
}