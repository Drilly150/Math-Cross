using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics; // Necesario para abrir el enlace del navegador
// Se necesitarían using para la librería gráfica y otras clases del proyecto.

namespace MathCross
{
    /// <summary>
    /// Gestiona la lógica y presentación de la pantalla de Información y Créditos.
    /// </summary>
    public class InfoScreenController
    {
        private readonly GameManager _gameManager;
        private readonly string _githubUrl = "https://github.com/Drilly150/Math-Cross.git"; // URL del proyecto

        // Botones y elementos de la UI
        private readonly Button _backButton;
        private readonly Button _githubButton;
        // private ScrollableTextBlock _guideText;
        // private List<ContributorProfile> _profiles;

        public InfoScreenController(GameManager gameManager)
        {
            _gameManager = gameManager;

            // Aquí se cargaría el texto de la guía completa del juego [cite: 73]
            // _guideText = new ScrollableTextBlock("Todo lo que debería de saber el jugador sobre el juego se encuentra en ese apartado...");

            _backButton = new Button("Volver", /* new Rectangle(x,y,w,h) */ () => {
                _gameManager.GoToMainMenu();
            });

            _githubButton = new Button("Visitar en GitHub", /* new Rectangle(x,y,w,h) */ () => {
                // Al seleccionar el link, se te redirecciona al navegador [cite: 75]
                // pero antes saldrá un pequeño aviso[cite: 76].
                ShowGitHubConfirmation();
            });
        }

        private void ShowGitHubConfirmation()
        {
            // La lógica para mostrar un diálogo de confirmación estaría en el GameManager
            // para poder reutilizarlo (ej. en el botón de salir).
            // _gameManager.ShowConfirmationDialog(
            //      "Serás llevado afuera del juego. ¿Estás seguro de querer salir?",
            //      () => { OpenBrowser(); } // Acción si se presiona "Si"
            // );
        }

        private void OpenBrowser()
        {
            // Este código abre el navegador por defecto con la URL especificada. [cite: 77]
            try
            {
                Process.Start(new ProcessStartInfo(_githubUrl) { UseShellExecute = true });
            }
            catch (Exception e)
            {
                Console.WriteLine("No se pudo abrir el enlace: " + e.Message);
            }
        }
        
        /// <summary>
        /// Actualiza la lógica de la pantalla.
        /// </summary>
        public void Update()
        {
            // Lógica para el scroll del texto de la guía.
            // Lógica para la interacción con los botones.
        }

        /// <summary>
        /// Dibuja todos los elementos de la pantalla de información.
        /// </summary>
        public void Draw()
        {
            // 1. Dibujar el texto de la guía del juego. [cite: 71, 72]
            
            // 2. Dibujar la sección de "Integrantes". [cite: 74]
            // - Dibujar el título centrado. [cite: 74]
            // - Dibujar cuatro círculos con las fotos. [cite: 74]
            // - Debajo de cada foto, su nombre y una frase. [cite: 74]

            // 3. Dibujar el botón/link de GitHub. [cite: 75]
            
            // 4. Dibujar el botón para volver al menú.
        }
    }
}