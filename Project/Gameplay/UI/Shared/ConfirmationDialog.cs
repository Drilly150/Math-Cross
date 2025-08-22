using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace MathCross.UI
{
    /// <summary>
    /// Gestiona un cuadro de diálogo de confirmación emergente (Sí/No).
    /// </summary>
    public class ConfirmationDialog
    {
        public bool IsActive { get; private set; }

        private string _message;
        private Action _onYesAction;
        private Action _onNoAction;

        // private Button _yesButton;
        // private Button _noButton;

        public ConfirmationDialog()
        {
            IsActive = false;
        }

        /// <summary>
        /// Muestra el diálogo con un mensaje y acciones específicas.
        /// </summary>
        /// <param name="message">El texto a mostrar en el diálogo.</param>
        /// <param name="onYes">La acción a ejecutar si el usuario presiona "Sí".</param>
        /// <param name="onNo">La acción opcional si el usuario presiona "No". Por defecto, solo cierra el diálogo.</param>
        public void Show(string message, Action onYes, Action onNo = null)
        {
            _message = message;
            _onYesAction = onYes;

            // Si no se proporciona una acción para "No", la acción por defecto será simplemente cerrar el diálogo.
            _onNoAction = onNo ?? Hide;

            // Aquí se crearían los botones "Si" y "No" con sus respectivas acciones.
            // _yesButton = new Button("Si", ..., () => { _onYesAction?.Invoke(); Hide(); });
            // _noButton = new Button("No", ..., () => { _onNoAction?.Invoke(); });
            
            IsActive = true;
            Console.WriteLine($"Mostrando Diálogo: '{_message}'");
        }

        /// <summary>
        /// Oculta el cuadro de diálogo.
        /// </summary>
        public void Hide()
        {
            IsActive = false;
        }

        /// <summary>
        /// Actualiza la lógica del diálogo (solo si está activo).
        /// </summary>
        public void Update()
        {
            if (!IsActive) return;

            // Este método capturaría toda la entrada del mouse.
            // Si se hace clic en el botón "Sí", se invoca _onYesAction.
            // Si se hace clic en el botón "No", se invoca _onNoAction.
        }

        /// <summary>
        /// Dibuja el diálogo en pantalla (solo si está activo).
        /// </summary>
        public void Draw()
        {
            if (!IsActive) return;

            // 1. Dibujar un rectángulo semi-transparente que oscurezca toda la pantalla de fondo.

            // 2. Dibujar el panel del cuadro de diálogo encima.

            // 3. Dibujar el texto del mensaje en el centro del panel.
            //    Ej: "¿Deseas salir?"

            // 4. Dibujar los botones "Si" y "No".
        }
    }
}