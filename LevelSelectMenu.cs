using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MathCross; // Necesario para acceder a GameStateManager, PuzzleGenerator, etc.

namespace MathCross
{
    public class LevelSelectMenu : UserControl
    {
        // Clase interna para representar un nivel en el mapa
        private class LevelNode
        {
            public string Name;
            public PointF Position;
            public float Offset; // Para animación del fondo
            public bool Unlocked;
            public RectangleF Bounds => new RectangleF(Position.X - 20, Position.Y - 20, 40, 40); // Área de clic
            public int Estrellas;
            public int TiempoPromedio; // Puedes usar un TimeSpan aquí si necesitas más precisión
            public int TiempoRecord;   // Puedes usar un TimeSpan aquí si necesitas más precisión
            public bool AnimarCompletado = false;
        }

        // Declaración de los Timers con el namespace completo para evitar ambigüedad (CS0104)
        private System.Windows.Forms.Timer slideIn;
        private System.Windows.Forms.Timer slideOut;
        private System.Windows.Forms.Timer animationTimer; // Este ya estaba bien declarado

        private List<LevelNode> levels = new List<LevelNode>();
        private Panel infoPanel;
        // private Timer slideIn, slideOut; // ELIMINAR ESTA LÍNEA DUPLICADA Y AMBIGUA
        private int panelTargetX;
        private LevelNode? currentLevelShown; // Hacerlo anulable para aceptar null
        private const int panelWidth = 250;
        private bool panelVisible = false;
        // private System.Windows.Forms.Timer animationTimer; // Esta línea estaba duplicada
        private float backgroundOffset = 0;
        private Random rand = new Random();
        private LevelNode? selected = null; // CAMBIO: Hacerlo anulable (CS8625)

        private Button closeButton;
        private Button btnModoPractica; // Solo una declaración
        private Button btnJugarNivel; // Asumo que este botón existe o se agregará

        private string? nivelAResaltar; // CAMBIO: Declarar como anulable (CS8625)

        public LevelSelectMenu()
        {
            this.Size = new Size(800, 600); // Tamaño por defecto para el control
            this.BackColor = Color.FromArgb(40, 40, 40); // Fondo oscuro
            this.DoubleBuffered = true; // Para evitar parpadeos en el dibujado

            // Inicializar infoPanel (simplificado, debes tener una implementación completa)
            infoPanel = new Panel()
            {
                Size = new Size(panelWidth, this.Height),
                BackColor = Color.FromArgb(60, 60, 60),
                Location = new Point(this.Width, 0), // Inicialmente fuera de pantalla
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(infoPanel);

            // Botón de cerrar para el infoPanel
            closeButton = new Button()
            {
                Text = "X",
                Font = new Font("Arial", 12, FontStyle.Bold),
                Size = new Size(30, 30),
                Location = new Point(panelWidth - 35, 5),
                BackColor = Color.DarkRed,
                ForeColor = Color.White
            };
            closeButton.Click += (s, e) => HideInfoPanel();
            infoPanel.Controls.Add(closeButton);

            // Botón de Modo Práctica
            btnModoPractica = new Button()
            {
                Text = "Modo Práctica",
                Font = new Font("Arial", 12, FontStyle.Bold),
                Size = new Size(200, 50),
                Location = new Point(25, infoPanel.Height - 120), // Posición relativa al panel
                BackColor = Color.FromArgb(80, 80, 80),
                ForeColor = Color.White
            };
            btnModoPractica.Click += (s, e) =>
            {
                // Lógica para iniciar modo práctica
                GameStateManager.NavegarA(new PuzzleGamePanel(0, true)); // Nivel 0 para práctica
            };
            infoPanel.Controls.Add(btnModoPractica);

            // Botón para jugar el nivel seleccionado (si hay uno)
            btnJugarNivel = new Button()
            {
                Text = "Jugar Nivel",
                Font = new Font("Arial", 12, FontStyle.Bold),
                Size = new Size(200, 50),
                Location = new Point(25, infoPanel.Height - 60), // Posición relativa al panel
                BackColor = Color.LightGreen,
                ForeColor = Color.Black,
                Visible = false // Inicialmente oculto
            };
            btnJugarNivel.Click += (s, e) =>
            {
                if (currentLevelShown != null)
                {
                    // Asumo que tu GameStateManager tiene un método para iniciar un nivel
                    GameStateManager.NavegarA(new PuzzleGamePanel(currentLevelShown.Name)); // Pasa el nombre del nivel
                }
            };
            infoPanel.Controls.Add(btnJugarNivel);


            // Configurar los niveles (ejemplo)
            ConfigurarNiveles();

            // Configuración del Timer de animación (ya estaba bien declarado)
            animationTimer = new System.Windows.Forms.Timer() { Interval = 50, Enabled = true };
            animationTimer.Tick += (s, e) =>
            {
                backgroundOffset += 0.5f; // Para simular movimiento de fondo
                Invalidate(); // Redibujar el control
            };

            // Suscribir eventos de ratón
            this.Paint += OnPaint;
            this.MouseClick += OnMouseClick;

            // Inicializar Timers de slide-in/out
            slideIn = new System.Windows.Forms.Timer() { Interval = 10 };
            slideIn.Tick += SlideInTick;

            slideOut = new System.Windows.Forms.Timer() { Interval = 10 };
            slideOut.Tick += SlideOutTick;

            // Integración del MusicWidget
            MusicWidget musicWidget = new MusicWidget()
            {
                Location = new Point(10, 10) // Ajusta la posición según tu diseño
            };
            this.Controls.Add(musicWidget);
            musicWidget.BringToFront(); // Asegurarse de que esté visible sobre otros elementos
        }

        private void ConfigurarNiveles()
        {
            // Ejemplo de configuración de niveles. Deberías cargar esto desde LevelProgressManager.
            // Para propósitos de este ejemplo, los creamos directamente.
            levels.Add(new LevelNode { Name = "Nivel 1", Position = new PointF(100, 100), Unlocked = true, Estrellas = 3, TiempoPromedio = 60, TiempoRecord = 45 });
            levels.Add(new LevelNode { Name = "Nivel 2", Position = new PointF(250, 150), Unlocked = true, Estrellas = 2, TiempoPromedio = 90, TiempoRecord = 70 });
            levels.Add(new LevelNode { Name = "Nivel 3", Position = new PointF(400, 200), Unlocked = false, Estrellas = 0, TiempoPromedio = 0, TiempoRecord = 0 });
            // ... más niveles

            // Simula cargar el progreso desde LevelProgressManager
            foreach (var level in levels)
            {
                var progress = LevelProgressManager.GetLevelProgress(level.Name);
                if (progress != null) // CAMBIO: El resultado de GetLevelProgress puede ser null
                {
                    level.Unlocked = progress.Unlocked;
                    level.Estrellas = progress.Estrellas;
                    level.TiempoPromedio = progress.TiempoPromedio;
                    level.TiempoRecord = progress.TiempoRecord;
                }
            }
        }

        private void ShowInfoPanel(LevelNode level)
        {
            // Muestra la información del nivel
            infoPanel.Controls.Clear(); // Limpiar contenido anterior
            infoPanel.Controls.Add(closeButton); // Añadir el botón de cerrar de nuevo
            infoPanel.Controls.Add(btnModoPractica); // Añadir el botón de modo práctica de nuevo

            // Mostrar el botón de jugar nivel si el nivel está desbloqueado
            btnJugarNivel.Visible = level.Unlocked;
            infoPanel.Controls.Add(btnJugarNivel);


            Label lblNombre = new Label()
            {
                Text = level.Name,
                Font = new Font("Arial", 16, FontStyle.Bold),
                Location = new Point(10, 40),
                AutoSize = true,
                ForeColor = Color.White
            };
            infoPanel.Controls.Add(lblNombre);

            Label lblEstado = new Label()
            {
                Text = level.Unlocked ? "Estado: Desbloqueado" : "Estado: Bloqueado",
                Font = new Font("Arial", 10),
                Location = new Point(10, 70),
                AutoSize = true,
                ForeColor = Color.White
            };
            infoPanel.Controls.Add(lblEstado);

            if (level.Unlocked)
            {
                Label lblEstrellas = new Label()
                {
                    Text = $"Estrellas: {level.Estrellas}",
                    Font = new Font("Arial", 10),
                    Location = new Point(10, 90),
                    AutoSize = true,
                    ForeColor = Color.White
                };
                infoPanel.Controls.Add(lblEstrellas);

                Label lblTiempoPromedio = new Label()
                {
                    Text = $"Tiempo Promedio: {level.TiempoPromedio}s",
                    Font = new Font("Arial", 10),
                    Location = new Point(10, 110),
                    AutoSize = true,
                    ForeColor = Color.White
                };
                infoPanel.Controls.Add(lblTiempoPromedio);

                Label lblTiempoRecord = new Label()
                {
                    Text = $"Tiempo Récord: {level.TiempoRecord}s",
                    Font = new Font("Arial", 10),
                    Location = new Point(10, 130),
                    AutoSize = true,
                    ForeColor = Color.White
                };
                infoPanel.Controls.Add(lblTiempoRecord);
            }

            panelTargetX = this.Width - panelWidth;
            panelVisible = true;
            slideIn.Start();
            currentLevelShown = level; // Guardar el nivel que se está mostrando
        }

        private void HideInfoPanel()
        {
            panelTargetX = this.Width;
            panelVisible = false;
            slideOut.Start();
            selected = null; // Deseleccionar cuando se oculta el panel
            currentLevelShown = null; // Limpiar el nivel mostrado
            Invalidate(); // Redibujar para que se desmarque el nivel si estaba resaltado
        }

        private void SlideInTick(object sender, EventArgs e)
        {
            if (infoPanel.Location.X > panelTargetX)
            {
                infoPanel.Location = new Point(Math.Max(panelTargetX, infoPanel.Location.X - 20), infoPanel.Location.Y);
            }
            else
            {
                slideIn.Stop();
                infoPanel.Location = new Point(panelTargetX, infoPanel.Location.Y);
            }
        }

        private void SlideOutTick(object sender, EventArgs e)
        {
            if (infoPanel.Location.X < panelTargetX)
            {
                infoPanel.Location = new Point(Math.Min(panelTargetX, infoPanel.Location.X + 20), infoPanel.Location.Y);
            }
            else
            {
                slideOut.Stop();
                infoPanel.Location = new Point(panelTargetX, infoPanel.Location.Y);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Dibujar fondo animado
            for (int i = 0; i < this.Width; i += 20)
            {
                for (int j = 0; j < this.Height; j += 20)
                {
                    float hue = (float)((i + j + backgroundOffset * 5) % 360);
                    Color color = ColorFromHSL(hue, 0.8f, 0.3f);
                    using (SolidBrush brush = new SolidBrush(color))
                    {
                        g.FillRectangle(brush, i, j, 20, 20);
                    }
                }
            }

            // Dibujar conexiones entre niveles (ejemplo muy básico)
            using (Pen linePen = new Pen(Color.Gray, 2))
            {
                // Conectar Nivel 1 con Nivel 2 (ejemplo)
                if (levels.Count >= 2)
                {
                    g.DrawLine(linePen, levels[0].Position, levels[1].Position);
                }
            }


            // Dibujar los niveles
            using (Font font = new Font("Arial", 10, FontStyle.Bold))
            using (SolidBrush textBrush = new SolidBrush(Color.White))
            using (SolidBrush lockedBrush = new SolidBrush(Color.FromArgb(100, 100, 100))) // Gris oscuro para bloqueado
            {
                foreach (var level in levels)
                {
                    SolidBrush levelBrush;
                    if (level == selected)
                    {
                        levelBrush = new SolidBrush(Color.Gold); // Resaltar nivel seleccionado
                    }
                    else if (level.Unlocked)
                    {
                        levelBrush = new SolidBrush(Color.LimeGreen); // Desbloqueado
                    }
                    else
                    {
                        levelBrush = lockedBrush; // Bloqueado
                    }

                    g.FillEllipse(levelBrush, level.Bounds);
                    g.DrawEllipse(Pens.Black, level.Bounds); // Borde

                    SizeF textSize = g.MeasureString(level.Name, font);
                    g.DrawString(level.Name, font, textBrush,
                        level.Position.X - textSize.Width / 2,
                        level.Position.Y - textSize.Height / 2);
                }
            }
        }

        // Método auxiliar para generar color desde HSL (asumo que existe o lo necesitas)
        private Color ColorFromHSL(float h, float s, float l)
        {
            float r, g, b;

            if (s == 0)
            {
                r = g = b = l; // achromatic
            }
            else
            {
                float Hue2RGB(float p, float q, float t)
                {
                    if (t < 0) t += 1;
                    if (t > 1) t -= 1;
                    if (t < 1f / 6f) return p + (q - p) * 6f * t;
                    if (t < 1f / 2f) return q;
                    if (t < 2f / 3f) return p + (q - p) * (2f / 3f - t) * 6f;
                    return p;
                }

                float q = l < 0.5 ? l * (1 + s) : l + s - l * s;
                float p = 2 * l - q;
                r = Hue2RGB(p, q, h + 1f / 3f);
                g = Hue2RGB(p, q, h);
                b = Hue2RGB(p, q, h - 1f / 3f);
            }

            return Color.FromArgb(255, (int)(r * 255), (int)(g * 255), (int)(b * 255));
        }


        private void OnMouseClick(object sender, MouseEventArgs e)
        {
            foreach (var level in levels)
            {
                if (level.Bounds.Contains(e.Location) && level.Unlocked)
                {
                    if (selected == level)
                    {
                        HideInfoPanel();
                        selected = null;
                    }
                    else
                    {
                        selected = level;
                        ShowInfoPanel(level);
                    }
                    Invalidate();
                    break;
                }
            }
        }
    }
}

//Aquí se representara el menu en donde se veran los niveles. Usa "LevelProgressManager" para saber qué niveles están desbloqueados. Usa "GameStateManager" para navegar hacia "PuzzleGamePanel". Contiene la lógica para el uso del modo práctica libre e integra el "MusicWidget" para mostrar pista actuales.

//El aspecto del menu de niveles. Según a los Promts que le he estado dando a ChatGPT. La forma de visualizarse el menu es con una distorsión de fondo, niveles conectados entre si como si fueran grafos y una pestaña que aparece desde la derecha al tocar un nivel y que menciona el estado actual del nivel.