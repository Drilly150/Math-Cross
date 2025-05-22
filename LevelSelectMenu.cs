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
            public int TiempoPromedio;
            public int TiempoRecord;
            public bool AnimarCompletado = false;
        }

        private List<LevelNode> levels = new List<LevelNode>();
        private Panel infoPanel;
        private System.Windows.Forms.Timer animationTimer;
        private System.Windows.Forms.Timer slideInTimer;
        private System.Windows.Forms.Timer slideOutTimer;

        private int panelTargetX;
        private LevelNode currentLevelShown;
        private const int PanelWidth = 250; // Renamed for clarity PanelWidth instead of panelWidth
        private bool panelVisible = false; // Kept for potential use, though state can be inferred

        private float backgroundOffset = 0;
        private Random rand = new Random();
        private LevelNode selectedNode = null; // Renamed from 'selected' for clarity

        private Button closeButton;
        private Button btnModoPractica;
        private MusicWidget musicWidget; // Field for MusicWidget

        // Labels for infoPanel
        private Label lblLevelName, lblLevelStars, lblLevelTime;
        private Button btnPlayLevel;

        private string nivelAResaltar;
        private bool modoPractica = false;

        public event Action OnCloseRequested;
        public event Action<string> RequestPlayLevel; // Event to signal playing a level

        // Parameterless constructor calls the parameterized one with default values
        public LevelSelectMenu() : this(null, false)
        {
        }

        public LevelSelectMenu(string nivelCompletado = null, bool modoPracticaJuego = false)
        {
            this.DoubleBuffered = true;
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.Black;

            // Initialize Timers
            animationTimer = new System.Windows.Forms.Timer();
            slideInTimer = new System.Windows.Forms.Timer();
            slideOutTimer = new System.Windows.Forms.Timer();

            // Setup specific state from parameters
            this.nivelAResaltar = nivelCompletado;
            this.modoPractica = modoPracticaJuego;

            InitializeControls(); // Initialize all visual components and event handlers

            GenerateLevels(); // Generate levels after state is set

            animationTimer.Interval = 30;
            animationTimer.Tick += AnimationTimer_Tick;
            animationTimer.Start();

            // Update btnModoPractica based on initial modoPractica state
            btnModoPractica.BackColor = this.modoPractica ? Color.DeepSkyBlue : Color.LightBlue;
        }

        private void InitializeControls()
        {
            // Close Button
            closeButton = new Button()
            {
                Text = "❌",
                Font = new Font("Arial", 10, FontStyle.Bold),
                Size = new Size(35, 35),
                Location = new Point(10, 10),
                BackColor = Color.Gray,
                FlatStyle = FlatStyle.Flat,
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };
            closeButton.Click += (s, e) => OnCloseRequested?.Invoke();
            this.Controls.Add(closeButton);

            // Info Panel
            infoPanel = new Panel()
            {
                Size = new Size(PanelWidth, this.Height), // Height will be adjusted
                Location = new Point(this.Width, 0), // Starts hidden off-screen
                BackColor = Color.FromArgb(30, 30, 30),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right
            };

            lblLevelName = new Label { Location = new Point(10, 20), ForeColor = Color.White, Font = new Font("Arial", 12, FontStyle.Bold), AutoSize = false, Size = new Size(PanelWidth - 20, 20), TextAlign = ContentAlignment.MiddleLeft };
            lblLevelStars = new Label { Location = new Point(10, 50), ForeColor = Color.White, AutoSize = false, Size = new Size(PanelWidth - 20, 20), TextAlign = ContentAlignment.MiddleLeft };
            lblLevelTime = new Label { Location = new Point(10, 80), ForeColor = Color.White, AutoSize = false, Size = new Size(PanelWidth - 20, 20), TextAlign = ContentAlignment.MiddleLeft };
            btnPlayLevel = new Button { Text = "Jugar", Location = new Point(10, 120), Size = new Size(PanelWidth - 20, 30), BackColor = Color.ForestGreen, ForeColor = Color.White, Font = new Font("Arial", 10, FontStyle.Bold) };
            btnPlayLevel.Click += BtnPlayLevel_Click;

            infoPanel.Controls.Add(lblLevelName);
            infoPanel.Controls.Add(lblLevelStars);
            infoPanel.Controls.Add(lblLevelTime);
            infoPanel.Controls.Add(btnPlayLevel);
            this.Controls.Add(infoPanel);

            // Slide Timers Setup
            slideInTimer.Interval = 10;
            slideInTimer.Tick += SlideInTimer_Tick;

            slideOutTimer.Interval = 10;
            slideOutTimer.Tick += SlideOutTimer_Tick;

            // Modo Práctica Button
            btnModoPractica = new Button()
            {
                Text = "Modo práctica libre",
                Size = new Size(180, 35),
                // Location will be set in OnLoad or rely on Anchor
                Font = new Font("Arial", 10),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnModoPractica.Click += BtnModoPractica_Click;
            this.Controls.Add(btnModoPractica);

            // Music Widget
            musicWidget = new MusicWidget();
            musicWidget.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.Controls.Add(musicWidget);

            this.MouseClick += OnMouseClick;

            this.Load += (s, e) =>
            {
                // Set locations that depend on final Width/Height
                btnModoPractica.Location = new Point(this.Width - btnModoPractica.Width - 20, 20);
                musicWidget.Location = new Point(this.Width - musicWidget.Width - 20, this.Height - musicWidget.Height - 20);
                infoPanel.Height = this.Height; // Ensure panel height matches menu height
            };
              this.SizeChanged += (s, e) => {
                if(infoPanel != null) infoPanel.Height = this.Height; // Keep panel height updated
            };
        }


        private void BtnModoPractica_Click(object sender, EventArgs e)
        {
            modoPractica = !modoPractica;
            btnModoPractica.BackColor = modoPractica ? Color.DeepSkyBlue : Color.LightBlue;
            RecalcularDesbloqueoNiveles();
            // Invalidate(); // RecalcularDesbloqueoNiveles already calls Invalidate
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            Animate();
        }

        private void SlideInTimer_Tick(object sender, EventArgs e)
        {
            if (infoPanel.Left > panelTargetX)
            {
                infoPanel.Left = Math.Max(panelTargetX, infoPanel.Left - 25); // Slightly faster animation
            }
            else
            {
                infoPanel.Left = panelTargetX;
                slideInTimer.Stop();
                panelVisible = true;
            }
        }

        private void SlideOutTimer_Tick(object sender, EventArgs e)
        {
            if (infoPanel.Left < this.Width)
            {
                infoPanel.Left = Math.Min(this.Width, infoPanel.Left + 25); // Slightly faster animation
            }
            else
            {
                infoPanel.Left = this.Width;
                slideOutTimer.Stop();
                panelVisible = false;
                currentLevelShown = null; // Clear selection once panel is hidden
            }
        }

        private void BtnPlayLevel_Click(object sender, EventArgs e)
        {
            if (currentLevelShown != null && currentLevelShown.Unlocked)
            {
                RequestPlayLevel?.Invoke(currentLevelShown.Name);
                HideInfoPanel();
            }
        }


        private void GenerateLevels()
        {
            levels.Clear(); // Clear existing levels before generating new ones
            var progreso = LevelProgressManager.Load();
            var NivelDataCollection = progreso?.Niveles;

            int count = 5; // Number of levels
            int spacing = 120; // Vertical spacing between levels
            int centerX = this.Width / 3; // Position levels more centrally, avoiding infoPanel initial area

            for (int i = 0; i < count; i++)
            {
                string id = $"P{i + 1}";
                NivelDataCollection?.TryGetValue(id, out var data);

                LevelNode newNode = new LevelNode
                {
                    Name = id,
                    Position = new PointF(centerX, 100 + i * spacing),
                    Offset = (float)(rand.NextDouble() * Math.PI * 2), // Full circle for diverse start
                    Unlocked = this.modoPractica || (data?.Desbloqueado ?? (i == 0 && !this.modoPractica)), // First level unlocked if no data & not practice
                    Estrellas = data?.Estrellas ?? 0,
                    TiempoRecord = data?.TiempoRecord ?? 0,
                    TiempoPromedio = data?.TiempoPromedio ?? 0,
                    AnimarCompletado = false
                };

                if (id == nivelAResaltar)
                {
                    newNode.AnimarCompletado = true;
                }
                levels.Add(newNode);
            }
        }

        private void RecalcularDesbloqueoNiveles()
        {
            var progreso = LevelProgressManager.Load();
            var NivelDataCollection = progreso?.Niveles;

            for (int i = 0; i < levels.Count; i++)
            {
                LevelNode currentLevelNode = levels[i];
                NivelDataCollection?.TryGetValue(currentLevelNode.Name, out var data);
                currentLevelNode.Unlocked = this.modoPractica || (data?.Desbloqueado ?? (i == 0 && !this.modoPractica));
            }
            Invalidate();
        }

        private void Animate()
        {
            backgroundOffset += 0.5f;
            if (backgroundOffset > float.MaxValue - 100) backgroundOffset = 0; // Prevent overflow

            for (int i = 0; i < levels.Count; i++)
            {
                levels[i].Offset += 0.05f;
                if (levels[i].Offset > float.MaxValue - 100) levels[i].Offset = 0; // Prevent overflow

                levels[i].Position = new PointF(
                    levels[i].Position.X, // X position remains constant unless map scrolls
                    100 + i * 120 + (float)Math.Sin(levels[i].Offset) * 8 // Slightly larger sway
                );
            }
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e); // Call base method
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;


            // Background 
            for (int i = 0; i < this.Height; i += 40)
            {
                int colorComponentOffset = (int)((i + backgroundOffset) % 80);
                // Ensure color components are within valid range [0-255]
                int red = Math.Min(255, Math.Max(0, 20 + colorComponentOffset));
                int blue = Math.Min(255, Math.Max(0, 40 + colorComponentOffset));
                using (Brush b = new SolidBrush(Color.FromArgb(red, 20, blue))) // Green component is fixed
                {
                    g.FillRectangle(b, 0, i, this.Width, 40);
                }
            }

            // Lines between nodes
            if (levels.Count > 1)
            {
                using (Pen linePen = new Pen(Color.FromArgb(150, Color.White), 2)) // Slightly transparent lines
                {
                    for (int i = 0; i < levels.Count - 1; i++)
                    {
                        if (levels[i].Unlocked && levels[i+1].Unlocked) // Only draw lines between unlocked connected levels for clarity
                            g.DrawLine(linePen, levels[i].Position, levels[i+1].Position);
                        else if (levels[i].Unlocked && !levels[i+1].Unlocked && !modoPractica) // Optional: line to first locked level
                            g.DrawLine(Pens.DarkSlateGray, levels[i].Position, levels[i+1].Position);


                    }
                }
            }

            // Nodes
            foreach (var levelNode in levels) // Changed variable name from level to levelNode
            {
                Color fillColor;
                if (levelNode.AnimarCompletado)
                    fillColor = Color.Gold;
                else if (levelNode.Unlocked)
                    fillColor = Color.LimeGreen;
                else
                    fillColor = Color.DarkGray;

                using (Brush nodeBrush = new SolidBrush(fillColor))
                using (Pen borderPen = new Pen(Color.White, selectedNode == levelNode ? 3 : 2)) // Use selectedNode
                {
                    g.FillEllipse(nodeBrush, levelNode.Bounds);
                    g.DrawEllipse(borderPen, levelNode.Bounds);
                }

                using (Font font = new Font("Arial", 10, FontStyle.Bold))
                using (Brush textBrush = new SolidBrush(Color.Black))
                {
                    SizeF textSize = g.MeasureString(levelNode.Name, font);
                    g.DrawString(levelNode.Name, font, textBrush,
                        levelNode.Position.X - textSize.Width / 2,
                        levelNode.Position.Y - textSize.Height / 2);
                }
            }
        }

        private void ShowInfoPanel(LevelNode node)
        {
            if (slideOutTimer.Enabled) slideOutTimer.Stop();

            currentLevelShown = node;
            lblLevelName.Text = "Nivel: " + node.Name;
            lblLevelStars.Text = "Estrellas: " + node.Estrellas + "/3"; // Assuming 3 stars max
            lblLevelTime.Text = "Récord: " + (node.TiempoRecord > 0 ? node.TiempoRecord + "s" : "N/A");
            
            infoPanel.Height = this.Height; // Ensure panel height is correct before sliding
            panelTargetX = this.Width - PanelWidth;
            infoPanel.Visible = true; // Make sure panel is visible before starting animation
            slideInTimer.Start();
        }

        private void HideInfoPanel()
        {
            if (slideInTimer.Enabled) slideInTimer.Stop();
            
            // currentLevelShown will be set to null when slideOut completes.
            slideOutTimer.Start();
            // selectedNode = null; // Deselect the node is handled in OnMouseClick or when panel fully hides
            // Invalidate(); // Redraw to show node deselected - will happen via OnMouseClick or animation
        }

        private void OnMouseClick(object sender, MouseEventArgs e)
        {
            // Check if click is within the infoPanel if it's visible
            if (panelVisible && infoPanel.Bounds.Contains(e.Location))
            {
                // Click was inside the panel, let panel controls handle it (e.g., play button)
                return;
            }

            bool clickedOnNode = false;
            foreach (var levelNode in levels) // Changed variable name
            {
                if (levelNode.Bounds.Contains(e.Location))
                {
                    clickedOnNode = true;
                    if (levelNode.Unlocked)
                    {
                        if (selectedNode == levelNode) // Clicked on already selected node
                        {
                            // Option 1: Hide panel (if game allows deselecting this way)
                            // HideInfoPanel();
                            // selectedNode = null;

                            // Option 2: Or, if a level is selected, clicking it again could mean "Play"
                            // For now, assume it might be an accidental double click, do nothing or play.
                            // If play is desired here: BtnPlayLevel_Click(null, EventArgs.Empty);
                        }
                        else // Clicked on a new, unlocked node
                        {
                            selectedNode = levelNode;
                            ShowInfoPanel(levelNode);
                        }
                    }
                    else // Clicked on a locked node
                    {
                        // Optionally, provide feedback like a "locked" sound or small animation
                        if (panelVisible) HideInfoPanel(); // Hide panel if it was showing another level
                        selectedNode = null; // Deselect any previously selected node
                    }
                    Invalidate(); // Redraw to show selection changes
                    break; 
                }
            }

            if (!clickedOnNode && panelVisible) // Clicked outside any node while panel is visible
            {
                HideInfoPanel();
                selectedNode = null;
                Invalidate();
            }
        }
    }
}