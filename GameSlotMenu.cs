using System;
using System.Drawing;
using System.Windows.Forms;

namespace MathCross
{
    public class GameSlotMenu : UserControl
    {
        private Panel[] slotPanels = new Panel[3];
        private Label[] slotTitles = new Label[3];
        private Label[] slotDates = new Label[3];
        private Label[] slotInfo = new Label[3];
        private Button closeButton;

        private SaveData[] saveStates = new SaveData[3];

        public event Action OnCloseRequested; // Evento para volver al menú

        public GameSlotMenu()
        {
            LoadSaveStates();
            InitializeComponent();
        }

        private void LoadSaveStates()
        {
            // Carga los estados de guardado
            for (int i = 0; i < 3; i++)
            {
                saveStates[i] = SaveManager.LoadSlot(i);
            }
        }

        private void InitializeComponent()
        {
            this.Controls.Clear(); // Limpia controles previos (útil si se llama para refrescar toda la UI)
            this.Size = new Size(700, 600);
            this.BackColor = Color.White;

            // Botón de Cerrar
            closeButton = new Button();
            closeButton.Text = "❌";
            closeButton.Font = new Font("Arial", 12, FontStyle.Bold);
            closeButton.Size = new Size(40, 40);
            closeButton.Location = new Point(this.Width - 50, 10);
            closeButton.FlatStyle = FlatStyle.Flat;
            closeButton.BackColor = Color.LightGray;
            closeButton.Anchor = AnchorStyles.Top | AnchorStyles.Right; // Anclaje añadido
            closeButton.Click += (s, e) => OnCloseRequested?.Invoke();
            this.Controls.Add(closeButton);

            // Paneles de Slots
            int initialY = 70; // Posición Y inicial para el primer panel
            int panelSpacing = 20; // Espacio entre paneles

            for (int i = 0; i < 3; i++)
            {
                // slotPanels[i], slotTitles[i], slotDates[i], slotInfo[i] son inicializados
                // y asignados dentro de CreateSlotPanel y por la asignación de retorno.
                Panel panel = CreateSlotPanel(i);
                slotPanels[i] = panel; // Almacena la referencia al panel creado

                panel.Location = new Point((this.Width - panel.Width) / 2, initialY + i * (panel.Height + panelSpacing));
                this.Controls.Add(panel);
            }
        }

        private void RefreshSlotDisplay(int index)
        {
            // Actualiza el contenido visual de las etiquetas de un slot existente
            if (slotTitles[index] == null || slotDates[index] == null || slotInfo[index] == null)
            {
                // Si las etiquetas no existen (estado inesperado), reinicializa todo por seguridad
                // Esto no debería ocurrir en el flujo normal si CreateSlotPanel se ejecutó correctamente.
                InitializeComponent();
                return;
            }

            if (saveStates[index] == null)
            {
                slotTitles[index].Text = "Nueva partida";
                slotDates[index].Text = "00/00/00";
                slotInfo[index].Text = "Dificultad: --- | 0/30 estrellas";
            }
            else
            {
                slotTitles[index].Text = $"Partida número {index + 1}";
                slotDates[index].Text = saveStates[index].Fecha;
                slotInfo[index].Text = $"Dificultad: {saveStates[index].Dificultad} | {saveStates[index].Estrellas}/30 estrellas";
            }
        }

        private Panel CreateSlotPanel(int index)
        {
            Panel panel = new Panel();
            panel.Size = new Size(550, 120);
            panel.BackColor = Color.Gainsboro;
            panel.BorderStyle = BorderStyle.FixedSingle;
            panel.Cursor = Cursors.Hand;

            // Fecha de creación o guardado
            slotDates[index] = new Label();
            slotDates[index].Font = new Font("Arial", 9, FontStyle.Italic);
            slotDates[index].Location = new Point(10, 5);
            slotDates[index].Size = new Size(200, 15);
            panel.Controls.Add(slotDates[index]);

            // Título principal
            slotTitles[index] = new Label();
            slotTitles[index].Font = new Font("Arial", 16, FontStyle.Bold);
            slotTitles[index].Location = new Point(10, 30);
            slotTitles[index].Size = new Size(400, 30);
            panel.Controls.Add(slotTitles[index]);

            // Información de dificultad y estrellas
            slotInfo[index] = new Label();
            slotInfo[index].Font = new Font("Arial", 10, FontStyle.Regular);
            slotInfo[index].Location = new Point(300, 80); // Ajustado para que no se solape con título largo
            slotInfo[index].Size = new Size(230, 20); // Tamaño consistente
            slotInfo[index].TextAlign = ContentAlignment.BottomRight;
            panel.Controls.Add(slotInfo[index]);

            // Rellenar los valores usando el método de refresco para consistencia inicial
            RefreshSlotDisplay(index);

            // Animación al pasar el mouse
            panel.MouseEnter += (s, e) =>
            {
                panel.BackColor = Color.LightSkyBlue;
                panel.Size = new Size(570, 130); // La lógica de cambio de tamaño se mantiene
                panel.Invalidate();
            };

            panel.MouseLeave += (s, e) =>
            {
                panel.BackColor = Color.Gainsboro;
                panel.Size = new Size(550, 120); // Restaura tamaño original
                panel.Invalidate();
            };

            // Evento click
            panel.Click += (s, e) =>
            {
                if (saveStates[index] != null) // Slot con partida guardada
                {
                    SlotOptionsDialog optionsDialog = new SlotOptionsDialog(index);

                    optionsDialog.OnContinueSelected += () =>
                    {
                        // Ir al menú de niveles
                        this.Controls.Clear();
                        LevelSelectMenu levelMenu = new LevelSelectMenu();
                        levelMenu.OnCloseRequested += () => this.OnCloseRequested?.Invoke();
                        this.Controls.Add(levelMenu);
                    };

                    optionsDialog.OnResetSelected += () =>
                    {
                        SaveManager.DeleteSlot(index);
                        saveStates[index] = null;    // Actualiza el modelo de datos local
                        RefreshSlotDisplay(index); // Actualiza solo la UI de este slot
                        // optionsDialog se cerrará solo si es un DialogResult, o llamar optionsDialog.Close();
                    };

                    optionsDialog.ShowDialog(this); // Mostrar como diálogo modal
                }
                else // Slot vacío → escoger dificultad
                {
                    this.Controls.Clear(); // Limpia el GameSlotMenu para mostrar DifficultySelectionMenu

                    DifficultySelectionMenu diffMenu = new DifficultySelectionMenu();
                    diffMenu.Location = new Point((this.Width - diffMenu.Width) / 2, (this.Height - diffMenu.Height) / 2);
                    
                    diffMenu.OnDifficultySelected += (difficulty) =>
                    {
                        SaveData newData = new SaveData
                        {
                            Dificultad = difficulty,
                            Fecha = DateTime.Now.ToString("dd/MM/yy"),
                            Estrellas = 0
                        };

                        SaveManager.SaveSlot(index, newData);
                        saveStates[index] = newData; // Actualiza el modelo de datos local ANTES de refrescar UI
                        MessageBox.Show($"Dificultad '{difficulty}' guardada en slot {index + 1}.");

                        // Vuelve a inicializar la UI del GameSlotMenu.
                        // Esto limpiará diffMenu y reconstruirá los slots con el estado actualizado.
                        InitializeComponent();
                    };
                    this.Controls.Add(diffMenu);
                }
            };
            return panel;
        }
    }
}