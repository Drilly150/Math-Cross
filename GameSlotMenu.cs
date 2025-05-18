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
        // Simula el estado de las partidas

        public event Action OnCloseRequested; // Evento para volver al menú

        public GameSlotMenu()
        {
            this.Size = new Size(700, 600);
            this.BackColor = Color.White;

            closeButton = new Button();
            closeButton.Text = "❌";
            closeButton.Font = new Font("Arial", 12, FontStyle.Bold);
            closeButton.Size = new Size(40, 40);
            closeButton.Location = new Point(this.Width - 50, 10);
            closeButton.FlatStyle = FlatStyle.Flat;
            closeButton.BackColor = Color.LightGray;
            closeButton.Click += (s, e) => OnCloseRequested?.Invoke();
            this.Controls.Add(closeButton);

            for (int i = 0; i < 3; i++)
            {
                saveStates[i] = SaveManager.LoadSlot(i);
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
            slotInfo[index].Location = new Point(300, 80);
            slotInfo[index].Size = new Size(230, 20);
            slotInfo[index].TextAlign = ContentAlignment.BottomRight;
            panel.Controls.Add(slotInfo[index]);

            // ✅ Rellenar los valores según el estado del slot
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

            // Animación al pasar el mouse
            panel.MouseEnter += (s, e) =>
            {
                panel.BackColor = Color.LightSkyBlue;
                panel.Size = new Size(570, 130);
                panel.Invalidate();
            };

            panel.MouseLeave += (s, e) =>
            {
                panel.BackColor = Color.Gainsboro;
                panel.Size = new Size(550, 120);
                panel.Invalidate();
            };

            // Evento click (se implementa en otro paso)
                        panel.Click += (s, e) =>
            {
                if (saveStates[index] != null)
                {
                    // Mostrar opciones: Continuar o Reiniciar
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
                        // Eliminar archivo del slot y refrescar
                        SaveManager.DeleteSlot(index);
                        this.Controls.Clear();
                        GameSlotMenu refreshed = new GameSlotMenu();
                        refreshed.OnCloseRequested += () => this.OnCloseRequested?.Invoke();
                        this.Controls.Add(refreshed);
                    };

                    optionsDialog.ShowDialog(this);
                    return;
                }

                // Slot vacío → escoger dificultad
                DifficultySelectionMenu diffMenu = new DifficultySelectionMenu();
                diffMenu.Location = new Point((this.Width - diffMenu.Width) / 2, (this.Height - diffMenu.Height) / 2);
                this.Controls.Clear();
                this.Controls.Add(diffMenu);

                diffMenu.OnDifficultySelected += (difficulty) =>
                {
                    SaveData newData = new SaveData
                    {
                        Dificultad = difficulty,
                        Fecha = DateTime.Now.ToString("dd/MM/yy"),
                        Estrellas = 0
                    };

                    SaveManager.SaveSlot(index, newData);
                    MessageBox.Show($"Dificultad '{difficulty}' guardada en slot {index + 1}.");

                    this.Controls.Clear();
                    GameSlotMenu refreshed = new GameSlotMenu();
                    refreshed.OnCloseRequested += () => this.OnCloseRequested?.Invoke();
                    this.Controls.Add(refreshed);
                };
            };
            return panel;
        }

    }
}

//Como dice su nombre, en este archivo es donde se ejecuta el menu en donde aparecera los "slots". Que son tres apartados para guardar partidas. El cual al seleccionar alguno de los tres slots, se ejecutara el archivo "DifficultySelectionMenu".