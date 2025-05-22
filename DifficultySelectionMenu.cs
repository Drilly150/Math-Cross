using System;
using System.Drawing;
using System.Windows.Forms;

namespace MathCross
{
    public class DifficultySelectionMenu : UserControl
    {
        // Event that is triggered when a difficulty is selected.
        public event Action<string> OnDifficultySelected;

        // Buttons for each difficulty level.
        private Button easyButton, normalButton, hardButton;
        // Stores the currently selected difficulty. It's null until a selection is made.
        private string selectedDifficulty = null;

        public DifficultySelectionMenu()
        {
            // Basic setup for the UserControl.
            this.Size = new Size(500, 300); // Sets the size of the menu.
            this.BackColor = Color.White;   // Sets the background color.

            // Title label for the menu.
            Label title = new Label()
            {
                Text = "Selecciona la dificultad", // Text displayed on the label.
                Font = new Font("Arial", 18, FontStyle.Bold), // Font for the title.
                Size = new Size(500, 40), // Size of the label.
                TextAlign = ContentAlignment.MiddleCenter, // Centers the text within the label.
                Location = new Point(0, 30) // Position of the label.
            };
            this.Controls.Add(title); // Adds the label to the control.

            // Create and add difficulty buttons.
            // Each button is created with its text, hover color, and vertical position.
            easyButton = CreateDifficultyButton("Fácil", Color.SkyBlue, 60);
            normalButton = CreateDifficultyButton("Normal", Color.Gold, 120);
            hardButton = CreateDifficultyButton("Difícil", Color.IndianRed, 180);

            this.Controls.Add(easyButton);
            this.Controls.Add(normalButton);
            this.Controls.Add(hardButton);
        }

        /// <summary>
        /// Creates a styled button for difficulty selection.
        /// </summary>
        /// <param name="text">The text to display on the button (e.g., "Fácil").</param>
        /// <param name="hoverColor">The color the button changes to on hover and when selected.</param>
        /// <param name="top">The Y-coordinate for the button's position.</param>
        /// <returns>A configured Button control.</returns>
        private Button CreateDifficultyButton(string text, Color hoverColor, int top)
        {
            Button btn = new Button();
            btn.Text = text; // Sets the button text.
            btn.Font = new Font("Arial", 14, FontStyle.Bold); // Sets the button font.
            btn.Size = new Size(200, 40); // Sets the button size.
            // Centers the button horizontally within the control.
            btn.Location = new Point((this.Width - btn.Width) / 2, top);
            btn.FlatStyle = FlatStyle.Flat; // Gives the button a flat appearance.
            btn.BackColor = Color.White;    // Default background color.
            btn.ForeColor = Color.Black;    // Default text color.

            // Event handler for when the mouse enters the button area.
            btn.MouseEnter += (s, e) =>
            {
                // Change color on hover only if no difficulty has been selected yet.
                if (selectedDifficulty == null)
                {
                    btn.BackColor = hoverColor;
                }
            };

            // Event handler for when the mouse leaves the button area.
            btn.MouseLeave += (s, e) =>
            {
                // Reset color if the button is enabled AND
                // (no difficulty is selected OR this button is not the selected one).
                // This prevents changing the color of disabled buttons or the selected button.
                if (btn.Enabled && (selectedDifficulty == null || btn.Text != selectedDifficulty))
                {
                    btn.BackColor = Color.White;
                }
            };

            // Event handler for when the button is clicked.
            btn.Click += (s, e) =>
            {
                // Only proceed if no difficulty has been selected yet (one-time selection).
                if (selectedDifficulty == null)
                {
                    selectedDifficulty = text; // Set the selected difficulty.
                    btn.BackColor = hoverColor; // Explicitly set the selected button's color.
                    DisableOtherButtons(btn);   // Disable other buttons.
                    OnDifficultySelected?.Invoke(text); // Trigger the event.
                }
            };

            return btn;
        }

        /// <summary>
        /// Disables all buttons in the control except for the one that was selected.
        /// Also changes the background color of disabled buttons to indicate they are inactive.
        /// </summary>
        /// <param name="selected">The button that was clicked and selected.</param>
        private void DisableOtherButtons(Button selected)
        {
            // Iterate through all controls on the form.
            foreach (Control ctrl in this.Controls)
            {
                // Check if the control is a Button and is not the selected button.
                if (ctrl is Button btn && btn != selected)
                {
                    btn.Enabled = false; // Disable the button.
                    btn.BackColor = Color.LightGray; // Change its background color.
                }
            }
        }
    }
}

// Este archivo solo se ejecuta cuando vas a crear una nueva partida. En él aparecerán los tres tipos de dificultad que tendrá el juego: Fácil, Normal, Difícil. Luego, al seleccionar la dificultad deseada, se ejecutará el archivo "LevelSelectMenu".