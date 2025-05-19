using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MathCross
{
    public class LevelSelectMenu : UserControl
    {
        private class LevelNode
        {
            public string Name;
            public PointF Position;
            public float Offset;
            public bool Unlocked;
            public RectangleF Bounds => new RectangleF(Position.X - 20, Position.Y - 20, 40, 40);
            public int Estrellas;
            public int TiempoRecord;
            public bool AnimarCompletado = false;

        }

        private List<LevelNode> levels = new List<LevelNode>();
        private Panel infoPanel;
        private Timer slideIn, slideOut;
        private int panelTargetX;
        private LevelNode currentLevelShown;
        private const int panelWidth = 250;
        private bool panelVisible = false;
        private Timer animationTimer;
        private float backgroundOffset = 0;
        private Random rand = new Random();
        private LevelNode selected = null;
        private Panel infoPanel;

        private Button closeButton;

        private string nivelAResaltar;

        public LevelSelectMenu(string nivelCompletado = null)
        {
            this.nivelAResaltar = nivelCompletado;
            InitializeComponent();
        }

        public event Action OnCloseRequested;

        public LevelSelectMenu()
        {
            this.DoubleBuffered = true;
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.Black;

            GenerateLevels();

            animationTimer = new Timer();
            animationTimer.Interval = 30;
            animationTimer.Tick += (s, e) => Animate();
            animationTimer.Start();

            // Botón para volver al menú principal
            closeButton = new Button()
            {
                Text = "❌",
                Font = new Font("Arial", 10, FontStyle.Bold),
                Size = new Size(35, 35),
                Location = new Point(10, 10),
                BackColor = Color.Gray,
                FlatStyle = FlatStyle.Flat
            };
            closeButton.Click += (s, e) => OnCloseRequested?.Invoke();
            this.Controls.Add(closeButton);

            this.MouseClick += OnMouseClick;

                        // Panel de información del nivel
            infoPanel = new Panel()
            {
                Size = new Size(panelWidth, this.Height),
                Location = new Point(this.Width, 0), // inicia oculto fuera de pantalla
                BackColor = Color.FromArgb(30, 30, 30),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right
            };
            this.Controls.Add(infoPanel);

            // Timers para animar deslizamiento
            slideIn = new Timer { Interval = 10 };
            slideOut = new Timer { Interval = 10 };

            slideIn.Tick += (s, e) =>
            {
                if (infoPanel.Left > panelTargetX)
                {
                    infoPanel.Left -= 20;
                }
                else
                {
                    infoPanel.Left = panelTargetX;
                    slideIn.Stop();
                    panelVisible = true;
                }
            };

            slideOut.Tick += (s, e) =>
            {
                if (infoPanel.Left < this.Width)
                {
                    infoPanel.Left += 20;
                }
                else
                {
                    infoPanel.Left = this.Width;
                    slideOut.Stop();
                    panelVisible = false;
                    currentLevelShown = null;
                }
            };
        }

        private void GenerateLevels()
        {
            var progreso = LevelProgressManager.Load();
            int count = 5;
            int spacing = 120;
            int centerX = 300;

            for (int i = 0; i < count; i++)
            {
                if (id == nivelAResaltar)
    nodo.AnimarCompletado = true;
                progreso.Niveles.TryGetValue(id, out var data);

                levels.Add(new LevelNode
                {
                    Name = id,
                    Position = new PointF(centerX, 100 + i * spacing),
                    Offset = (float)(rand.NextDouble() * 10),
                    Unlocked = data?.Desbloqueado ?? false,
                    Estrellas = data?.Estrellas ?? 0,
                    TiempoRecord = data?.TiempoRecord ?? 0
                });
            }
        }

        private void Animate()
        {
            backgroundOffset += 0.5f;
            for (int i = 0; i < levels.Count; i++)
            {
                levels[i].Offset += 0.05f;
                levels[i].Position = new PointF(
                    levels[i].Position.X,
                    100 + i * 120 + (float)Math.Sin(levels[i].Offset) * 5
                );
            }
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // Fondo distorsionado (simulado como franjas de colores)
            for (int i = 0; i < this.Height; i += 40)
            {
                int offset = (int)((i + backgroundOffset) % 80);
                using (Brush b = new SolidBrush(Color.FromArgb(20 + offset, 20, 40 + offset)))
                {
                    g.FillRectangle(b, 0, i, this.Width, 40);
                }
            }

            // Líneas entre nodos
            using (Pen linePen = new Pen(Color.White, 2))
            {
                for (int i = 0; i < levels.Count - 1; i++)
                {
                    PointF p1 = levels[i].Position;
                    PointF p2 = levels[i + 1].Position;
                    g.DrawLine(linePen, p1, p2);
                }
            }

            // Nodos
            foreach (var level in levels)
            {
                Color fill = level.Unlocked ? Color.LimeGreen : Color.DarkGray;
                if (level.AnimarCompletado)
                    fill = Color.Gold; //Aquí en donde dice "Color.Gold", puede modificarse por por cualquier otro color, dejando eso si el "Color.(Color que deseas colocar)".

                Brush nodeBrush = new SolidBrush(fill);
                Pen border = new Pen(Color.White, selected == level ? 3 : 2);

                g.FillEllipse(nodeBrush, level.Bounds);
                g.DrawEllipse(border, level.Bounds);

                using (Font font = new Font("Arial", 10, FontStyle.Bold))
                using (Brush textBrush = new SolidBrush(Color.Black))
                {
                    SizeF textSize = g.MeasureString(level.Name, font);
                    g.DrawString(level.Name, font, textBrush,
                        level.Position.X - textSize.Width / 2,
                        level.Position.Y - textSize.Height / 2);
                }
            }
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

//Aquí se representara el menu en donde se veran los niveles. Que segun puedo pensar, se debe visualizar un menu con una distorsión de fondo, niveles conectados como si fueran grafos y pestaña que aparece desde la derecha que menciona el estado actual del nivel.