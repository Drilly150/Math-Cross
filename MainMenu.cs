using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
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
            // Ventana principal
            this.Text = "Math Cross - Menú Principal";
            this.Size = new Size(600, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.White;

            GameStateManager.Inicializar(this, this);
            MusicManager.Inicializar();

            // Título
            titleLabel = new Label()
            {
                Text = "Math Cross",
                Font = new Font("Arial", 28, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 100
            };
            this.Controls.Add(titleLabel);

            // Botones
            playButton = CreateMenuButton("Jugar", 150);
            settingsButton = CreateMenuButton("Configurar", 210);
            infoButton = CreateMenuButton("Información", 270);
            exitButton = CreateMenuButton("Salir", 330);

            playButton.Click += (s, e) => ShowGameSlotMenu();
            settingsButton.Click += (s, e) => MessageBox.Show("Configurar - funcionalidad aún no implementada.");
            infoButton.Click += (s, e) => ShowInfoPopup();
            exitButton.Click += (s, e) => ShowExitConfirmation();

            this.Controls.Add(playButton);
            this.Controls.Add(settingsButton);
            this.Controls.Add(infoButton);
            this.Controls.Add(exitButton);
        }

        private Button CreateMenuButton(string text, int top)
        {
            Button button = new Button();
            button.Text = text;
            button.Font = new Font("Arial", 14, FontStyle.Regular);
            button.Size = new Size(200, 40);
            button.Location = new Point((this.ClientSize.Width - button.Width) / 2, top);
            button.Anchor = AnchorStyles.Top;
            button.FlatStyle = FlatStyle.Flat;
            button.BackColor = Color.LightGray;

            button.MouseEnter += (s, e) =>
            {
                button.Font = new Font("Arial", 16, FontStyle.Bold);
                button.BackColor = Color.DarkGray;
            };
            button.MouseLeave += (s, e) =>
            {
                button.Font = new Font("Arial", 14, FontStyle.Regular);
                button.BackColor = Color.LightGray;
            };

            return button;
        }

        private void ShowGameSlotMenu()
        {
            playButton.Visible = false;
            settingsButton.Visible = false;
            infoButton.Visible = false;
            exitButton.Visible = false;
            titleLabel.Visible = false;

            GameSlotMenu slotMenu = new GameSlotMenu();
            slotMenu.Location = new Point((this.ClientSize.Width - slotMenu.Width) / 2, (this.ClientSize.Height - slotMenu.Height) / 2);
            slotMenu.Anchor = AnchorStyles.None;
            this.Controls.Add(slotMenu);
            slotMenu.BringToFront();

            slotMenu.OnCloseRequested += () =>
            {
                this.Controls.Remove(slotMenu);
                playButton.Visible = true;
                settingsButton.Visible = true;
                infoButton.Visible = true;
                exitButton.Visible = true;
                titleLabel.Visible = true;
            };
        }

        private void ShowExitConfirmation()
        {
            Panel blurPanel = new Panel()
            {
                Size = this.ClientSize,
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(100, 0, 0, 0)
            };
            this.Controls.Add(blurPanel);
            blurPanel.BringToFront();

            Form confirmDialog = new Form()
            {
                Size = new Size(350, 180),
                FormBorderStyle = FormBorderStyle.None,
                BackColor = Color.White,
                ShowInTaskbar = false,
                StartPosition = FormStartPosition.Manual,
                TopLevel = true,
                Opacity = 0
            };

            confirmDialog.Location = new Point(
                this.Location.X + (this.Width - confirmDialog.Width) / 2,
                this.Location.Y + (this.Height - confirmDialog.Height) / 2
            );

            Label message = new Label()
            {
                Text = "¿Estás seguro de que deseas salir del juego?",
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
                BackColor = Color.LightGray
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
                    confirmDialog.Opacity += 0.05;
                else
                    fadeIn.Stop();
            };
            fadeIn.Start();

            confirmDialog.ShowDialog(this);
        }

        private void ShowInfoPopup()
        {
            Panel blurPanel = new Panel()
            {
                Size = this.ClientSize,
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(120, 0, 0, 0)
            };
            this.Controls.Add(blurPanel);
            blurPanel.BringToFront();

            Form infoForm = new Form()
            {
                Size = new Size(480, 400),
                FormBorderStyle = FormBorderStyle.None,
                BackColor = Color.White,
                ShowInTaskbar = false,
                TopLevel = true,
                Opacity = 0
            };

            infoForm.Location = new Point(
                this.Location.X + (this.Width - infoForm.Width) / 2,
                this.Location.Y + (this.Height - infoForm.Height) / 2
            );

            Button closeBtn = new Button()
            {
                Text = "❌",
                Size = new Size(30, 30),
                Location = new Point(infoForm.Width - 40, 10),
                BackColor = Color.LightGray
            };
            closeBtn.Click += (s, e) =>
            {
                infoForm.Close();
                blurPanel.Dispose();
            };
            infoForm.Controls.Add(closeBtn);

            Panel scrollablePanel = new Panel()
            {
                Location = new Point(30, 50),
                Size = new Size(420, 300),
                AutoScroll = true,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.WhiteSmoke
            };

            Label content = new Label()
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 10),
                MaximumSize = new Size(400, 0),
                Text =
                    "Math Cross es un juego de acertijos matemáticos tipo crucigrama.\n\n" +
                    "📌 Objetivo:\n" +
                    "- Coloca los números disponibles en las celdas vacías.\n" +
                    "- Asegúrate que todas las ecuaciones horizontales y verticales sean válidas.\n\n" +
                    "📌 Reglas:\n" +
                    "- Usa todos los números una sola vez.\n" +
                    "- Ganas estrellas por rapidez, precisión y lógica.\n\n" +
                    "---------------------------------------------\n" +
                    "      INTEGRANTES DEL EQUIPO\n" +
                    "---------------------------------------------\n\n"
            };
            scrollablePanel.Controls.Add(content);
            infoForm.Controls.Add(scrollablePanel);

            int baseY = content.Bottom + 20;
            string[] nombres = { "Christopher Medina", "Sebastian Salazar", "Jesus Marquez" };
            string[] rutas = { "integrante1.jpg", "integrante2.jpg", "integrante3.jpg" };

            for (int i = 0; i < 3; i++)
            {
                PictureBox foto = new PictureBox()
                {
                    Size = new Size(90, 90),
                    Location = new Point(30 + i * 130, baseY),
                    SizeMode = PictureBoxSizeMode.Zoom
                };

                try
                {
                    Image original = Image.FromFile(rutas[i]);
                    Bitmap circular = new Bitmap(foto.Width, foto.Height);
                    using (Graphics g = Graphics.FromImage(circular))
                    {
                        using (Brush br = new TextureBrush(original))
                        {
                            GraphicsPath path = new GraphicsPath();
                            path.AddEllipse(0, 0, foto.Width, foto.Height);
                            g.SetClip(path);
                            g.FillRectangle(br, 0, 0, foto.Width, foto.Height);
                        }
                    }
                    foto.Image = circular;
                }
                catch
                {
                    foto.BackColor = Color.LightGray;
                }

                Label nombre = new Label()
                {
                    Text = nombres[i],
                    Location = new Point(foto.Left - 10, foto.Bottom + 5),
                    Size = new Size(110, 20),
                    TextAlign = ContentAlignment.MiddleCenter
                };

                scrollablePanel.Controls.Add(foto);
                scrollablePanel.Controls.Add(nombre);
            }

            Timer fadeIn = new Timer() { Interval = 20 };
            fadeIn.Tick += (s, e) =>
            {
                if (infoForm.Opacity < 1)
                    infoForm.Opacity += 0.05;
                else
                    fadeIn.Stop();
            };
            fadeIn.Start();

            infoForm.ShowDialog(this);
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.Run(new MainMenu());
        }
    }
}

//Este es el archivo raíz de todo el codigo. Aqui se encuentra el menú, la opción de Informacion, y de salir. Antes solia llamarse "Math-Cross". Aunque prefiero llamarlo "MainMenu", ya que se sienta más (Profesional).