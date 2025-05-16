using System;
using System.Drawing;
using System.Windows.Forms;

namespace MathCross
{
    public class MainMenu : Form
    {
        private Label titleLabel;
        private Button playButton;
        private Button settingsButton;
        private Button infoButton;
        private Button exitButton;

        public MainMenu()
        {
            // Propiedades generales de la ventana
            this.Text = "Math Cross - Menú Principal";
            this.Size = new Size(600, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.White;

            // Título
            titleLabel = new Label();
            titleLabel.Text = "Math Cross";
            titleLabel.Font = new Font("Arial", 28, FontStyle.Bold);
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            titleLabel.Dock = DockStyle.Top;
            titleLabel.Height = 100;
            this.Controls.Add(titleLabel);

            // Botones del menú
            playButton = CreateMenuButton("Jugar", 150);
            settingsButton = CreateMenuButton("Configurar", 210);
            infoButton = CreateMenuButton("Información", 270);
            exitButton = CreateMenuButton("Salir", 330);

            // Redirección al seleccionar "Jugar"
              playButton.Click += (s, e) =>
            {
                GameSlotMenu gameSlotMenu = new GameSlotMenu();
                gameSlotMenu.ShowDialog(); // Muestra como ventana modal
            };

            // Eventos
            playButton.Click += (s, e) => MessageBox.Show("Jugar - funcionalidad aún no implementada.");
            settingsButton.Click += (s, e) => MessageBox.Show("Configurar - funcionalidad aún no implementada.");
            infoButton.Click += (s, e) => MessageBox.Show("Información - funcionalidad aún no implementada.");
            exitButton.Click += (s, e) => ShowExitConfirmation();

            // Agregar botones al formulario
            this.Controls.Add(playButton);
            this.Controls.Add(settingsButton);
            this.Controls.Add(infoButton);
            this.Controls.Add(exitButton);
        }

        private Button CreateMenuButton(string text, int top)
        {
            private void ShowExitConfirmation()
        {
            Panel blurPanel = new Panel()
            {
                Size = this.ClientSize,
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(100, 0, 0, 0),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            this.Controls.Add(blurPanel);
            blurPanel.BringToFront();
        
            Form confirmDialog = new Form()
            {
                Size = new Size(350, 180),
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.CenterParent,
                BackColor = Color.White,
                ShowInTaskbar = false,
                TopLevel = true,
                Opacity = 0
            };
        
            confirmDialog.StartPosition = FormStartPosition.Manual;
            confirmDialog.Location = new Point(
                this.Location.X + (this.Width - confirmDialog.Width) / 2,
                this.Location.Y + (this.Height - confirmDialog.Height) / 2
            );
        
            Label message = new Label()
            {
                Text = "¿Estás seguro de que deseas salir del juego?",
                Font = new Font("Arial", 10, FontStyle.Regular),
                AutoSize = false,
                Size = new Size(300, 50),
                Location = new Point(25, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };
            confirmDialog.Controls.Add(message);
        
            Button cancelButton = new Button()
            {
                Text = "Cancelar",
                Size = new Size(100, 35),
                Location = new Point(50, 90),
                BackColor = Color.LightGray,
                Font = new Font("Arial", 10)
            };
            cancelButton.Click += (s, e) =>
            {
                confirmDialog.Close();
                blurPanel.Dispose();
            };
            confirmDialog.Controls.Add(cancelButton);
        
            Button exitButton = new Button()
            {
                Text = "Salir",
                Size = new Size(100, 35),
                Location = new Point(180, 90),
                BackColor = Color.IndianRed,
                Font = new Font("Arial", 10),
                ForeColor = Color.White
            };
            exitButton.Click += (s, e) =>
            {
                confirmDialog.Close();
                this.Close();
            };
            confirmDialog.Controls.Add(exitButton);
        
            Timer fadeIn = new Timer();
            fadeIn.Interval = 20;
            fadeIn.Tick += (s, e) =>
            {
                if (confirmDialog.Opacity < 1)
                {
                    confirmDialog.Opacity += 0.05;
                }
                else
                {
                    fadeIn.Stop();
                }
            };
            fadeIn.Start();
        
            confirmDialog.ShowDialog(this);
        }
            Button button = new Button();
            button.Text = text;
            button.Font = new Font("Arial", 14, FontStyle.Regular);
            button.Size = new Size(200, 40);
            button.Location = new Point((this.ClientSize.Width - button.Width) / 2, top);
            button.Anchor = AnchorStyles.Top;
            button.FlatStyle = FlatStyle.Flat;
            button.BackColor = Color.LightGray;

            // Evento para resaltar al pasar el mouse (animación pendiente)
            button.MouseEnter += (s, e) =>
            {
                button.Font = new Font("Arial", 16, FontStyle.Bold);
                button.BackColor = Color.DarkGray;
                // TODO: Agregar animación de fondo
            };
            button.MouseLeave += (s, e) =>
            {
                button.Font = new Font("Arial", 14, FontStyle.Regular);
                button.BackColor = Color.LightGray;
            };

            return button;
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.Run(new MainMenu());
        }
    }
}
