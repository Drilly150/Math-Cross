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
        private const int PanelWidth = 250; // Renombrado para mayor claridad PanelWidth en lugar de panelWidth
        private bool panelVisible = false; // Se conserva para un uso potencial, aunque se puede inferir su estado.

        private float backgroundOffset = 0;
        private Random rand = new Random();
        private LevelNode selectedNode = null; // Se cambió el nombre de "seleccionado" para mayor claridad.

        private Button closeButton;
        private Button btnModoPractica;
        private MusicWidget musicWidget; // Campo para MusicWidget

        // Etiquetas para el panel de InfoPanel
        private Label lblLevelName, lblLevelStars, lblLevelTime;
        private Button btnPlayLevel;

        private string nivelAResaltar;
        private bool modoPractica = false;

        public event Action OnCloseRequested;
        public event Action<string> RequestPlayLevel; // Event to signal playing a level

        // El constructor sin parámetros llama al constructor parametrizado con valores predeterminados
        public LevelSelectMenu() : this(null, false)
        {
        }

        public LevelSelectMenu(string nivelCompletado = null, bool modoPracticaJuego = false)
        {
            this.DoubleBuffered = true;
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.Black;

            // Inicializar temporizadores
            animationTimer = new System.Windows.Forms.Timer();
            slideInTimer = new System.Windows.Forms.Timer();
            slideOutTimer = new System.Windows.Forms.Timer();

            // Configurar un estado específico a partir de parámetros
            this.nivelAResaltar = nivelCompletado;
            this.modoPractica = modoPracticaJuego;

            InitializeControls(); // Inicializar todos los componentes visuales y controladores de eventos

            GenerateLevels(); // Generar niveles después de establecer el estado

            animationTimer.Interval = 30;
            animationTimer.Tick += AnimationTimer_Tick;
            animationTimer.Start();

            // Actualizar btnModoPractica según el estado inicial de modoPractica
            btnModoPractica.BackColor = this.modoPractica ? Color.DeepSkyBlue : Color.LightBlue;
        }

        private void InitializeControls()
        {
            // Botón de cerrar
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
                Size = new Size(PanelWidth, this.Height), // Se ajustará la altura
                Location = new Point(this.Width, 0), // Comienza oculto fuera de la pantalla.
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

            // Configuración de temporizadores de diapositivas
            slideInTimer.Interval = 10;
            slideInTimer.Tick += SlideInTimer_Tick;

            slideOutTimer.Interval = 10;
            slideOutTimer.Tick += SlideOutTimer_Tick;

            // Botón de Modo Práctica 
            btnModoPractica = new Button()
            {
                Text = "Modo práctica libre",
                Size = new Size(180, 35),
                // La ubicación se establecerá en OnLoad o dependerá de Anchor
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
                // Establecer ubicaciones que dependen del ancho/alto final
                btnModoPractica.Location = new Point(this.Width - btnModoPractica.Width - 20, 20);
                musicWidget.Location = new Point(this.Width - musicWidget.Width - 20, this.Height - musicWidget.Height - 20);
                infoPanel.Height = this.Height; // Asegúrese de que la altura del panel coincida con la altura del menú
            };
                this.SizeChanged += (s, e) => {
                if(infoPanel != null) infoPanel.Height = this.Height; // Mantenga la altura del panel actualizada
            };
        }


        private void BtnModoPractica_Click(object sender, EventArgs e)
        {
            modoPractica = !modoPractica;
            btnModoPractica.BackColor = modoPractica ? Color.DeepSkyBlue : Color.LightBlue;
            RecalcularDesbloqueoNiveles();
            // Invalidate(); // RecalcularDesbloqueoNiveles llama a Invalidar
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            Animate();
        }

        private void SlideInTimer_Tick(object sender, EventArgs e)
        {
            if (infoPanel.Left > panelTargetX)
            {
                infoPanel.Left = Math.Max(panelTargetX, infoPanel.Left - 25); // Animación un poco más rápida
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
                infoPanel.Left = Math.Min(this.Width, infoPanel.Left + 25); // Animación un poco más rápida
            }
            else
            {
                infoPanel.Left = this.Width;
                slideOutTimer.Stop();
                panelVisible = false;
                currentLevelShown = null; // Borrar la selección una vez que el panel esté oculto
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
            levels.Clear(); // Limpia los niveles existentes antes de generar nuevos
            var progreso = LevelProgressManager.Load();
            var NivelDataCollection = progreso?.Niveles;

            int count = 5; // Número de niveles
            int spacing = 120; // Espaciado vertical entre niveles
            int centerX = this.Width / 3; // Posicione los niveles más centralmente, evitando el área inicial del panel de información

            for (int i = 0; i < count; i++)
            {
                string id = $"P{i + 1}";
                NivelDataCollection?.TryGetValue(id, out var data);

                LevelNode newNode = new LevelNode
                {
                    Name = id,
                    Position = new PointF(centerX, 100 + i * spacing),
                    Offset = (float)(rand.NextDouble() * Math.PI * 2), // Círculo completo para un comienzo diverso
                    Unlocked = this.modoPractica || (data?.Desbloqueado ?? (i == 0 && !this.modoPractica)), // Primer nivel desbloqueado si no hay datos ni práctica
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
            if (backgroundOffset > float.MaxValue - 100) backgroundOffset = 0; // Prevenir desbordamientos

            for (int i = 0; i < levels.Count; i++)
            {
                levels[i].Offset += 0.05f;
                if (levels[i].Offset > float.MaxValue - 100) levels[i].Offset = 0; // Prevenir desbordamientos

                levels[i].Position = new PointF(
                    levels[i].Position.X, // La posición X permanece constante a menos que el mapa se desplace
                    100 + i * 120 + (float)Math.Sin(levels[i].Offset) * 8 // Balanceo ligeramente mayor
                );
            }
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e); // Llamar al método base
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;


            // Fondo
            for (int i = 0; i < this.Height; i += 40)
            {
                int colorComponentOffset = (int)((i + backgroundOffset) % 80);
                // Asegúrese de que los componentes de color estén dentro del rango válido [0-255]
                int red = Math.Min(255, Math.Max(0, 20 + colorComponentOffset));
                int blue = Math.Min(255, Math.Max(0, 40 + colorComponentOffset));
                using (Brush b = new SolidBrush(Color.FromArgb(red, 20, blue))) // El componente verde es fijo
                {
                    g.FillRectangle(b, 0, i, this.Width, 40);
                }
            }

            // Líneas entre nodos
            if (levels.Count > 1)
            {
                using (Pen linePen = new Pen(Color.FromArgb(150, Color.White), 2)) // Líneas ligeramente transparentes
                {
                    for (int i = 0; i < levels.Count - 1; i++)
                    {
                        if (levels[i].Unlocked && levels[i+1].Unlocked) // Solo dibuje líneas entre los niveles conectados desbloqueados para mayor claridad
                            g.DrawLine(linePen, levels[i].Position, levels[i+1].Position);
                        else if (levels[i].Unlocked && !levels[i+1].Unlocked && !modoPractica) // Opcional: línea al primer nivel bloqueado
                            g.DrawLine(Pens.DarkSlateGray, levels[i].Position, levels[i+1].Position);


                    }
                }
            }

            // Nodos
            foreach (var levelNode in levels) // Se cambió el nombre de la variable de nivel a LevelNode
            {
                Color fillColor;
                if (levelNode.AnimarCompletado)
                    fillColor = Color.Gold;
                else if (levelNode.Unlocked)
                    fillColor = Color.LimeGreen;
                else
                    fillColor = Color.DarkGray;

                using (Brush nodeBrush = new SolidBrush(fillColor))
                using (Pen borderPen = new Pen(Color.White, selectedNode == levelNode ? 3 : 2)) // Usa selectedNode
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
            lblLevelStars.Text = "Estrellas: " + node.Estrellas + "/3"; // Suponiendo 3 estrellas máximo
            lblLevelTime.Text = "Récord: " + (node.TiempoRecord > 0 ? node.TiempoRecord + "s" : "N/A");
            
            infoPanel.Height = this.Height; // Asegúrese de que la altura del panel sea correcta antes de deslizarlo
            panelTargetX = this.Width - PanelWidth;
            infoPanel.Visible = true; // Asegúrese de que el panel esté visible antes de comenzar la animación
            slideInTimer.Start();
        }

        private void HideInfoPanel()
        {
            if (slideInTimer.Enabled) slideInTimer.Stop();
            
            // currentLevelShown se establecerá en nulo cuando se complete el deslizamiento.
            slideOutTimer.Start();
            // selectedNode = null; // La deselección del nodo se realiza en OnMouseClick o cuando el panel se oculta por completo
            // Invalidar(); // Redibujar para mostrar el nodo deseleccionado - sucederá a través de OnMouseClick o animación
        }

        private void OnMouseClick(object sender, MouseEventArgs e)
        {
            //Comprueba si el clic está dentro del panel de información si está visible
            if (panelVisible && infoPanel.Bounds.Contains(e.Location))
            {
                // El clic estaba dentro del panel, deja que los controles del panel lo manejen (por ejemplo, el botón de reproducción)
                return;
            }

            bool clickedOnNode = false;
            foreach (var levelNode in levels) // Se cambió el nombre de la variable
            {
                if (levelNode.Bounds.Contains(e.Location))
                {
                    clickedOnNode = true;
                    if (levelNode.Unlocked)
                    {
                        if (selectedNode == levelNode) // Hizo clic en el nodo ya seleccionado
                        {
                            // Opción 1: Ocultar el panel (si el juego permite deseleccionarlo de esta manera)
                            // HideInfoPanel();
                            // selectedNode = null;

                            // Opción 2: O, si se selecciona un nivel, hacer clic en él nuevamente podría significar "Jugar"
                            // Por ahora, supongamos que puede ser un doble clic accidental, no haga nada o juegue".
                            //Si se desea reproducir aquí: BtnPlayLevel_Click(null, EventArgs.Empty);
                        }
                        else // Hice clic en un nodo nuevo y desbloqueado
                        {
                            selectedNode = levelNode;
                            ShowInfoPanel(levelNode);
                        }
                    }
                    else // Hizo clic en un nodo bloqueado
                    {
                        // Opcionalmente, proporcione comentarios como un sonido de "bloqueo" o una pequeña animación.
                        if (panelVisible) HideInfoPanel(); // Ocultar el panel si estaba mostrando otro nivel
                        selectedNode = null; // Deseleccionar cualquier nodo seleccionado previamente
                    }
                    Invalidate(); // Redibujar para mostrar los cambios de selección
                    break; 
                }
            }

            if (!clickedOnNode && panelVisible) // Se hizo clic fuera de cualquier nodo mientras el panel estaba visible
            {
                HideInfoPanel();
                selectedNode = null;
                Invalidate();
            }
        }
    }
}

//Aquí se representara el menu en donde se veran los niveles. Usa "LevelProgressManager" para saber qué niveles están desbloqueados. Usa "GameStateManager" para navegar hacia "PuzzleGamePanel". Contiene la lógica para el uso del modo práctica libre e integra el "MusicWidget" para mostrar pista actuales.

//El aspecto del menu de niveles. Según a los Promts que le he estado dando a ChatGPT. La forma de visualizarse el menu es con una distorsión de fondo, niveles conectados entre si como si fueran grafos y una pestaña que aparece desde la derecha al tocar un nivel y que menciona el estado actual del nivel.