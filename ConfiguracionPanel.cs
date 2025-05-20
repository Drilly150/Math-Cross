public class ConfiguracionPanel : UserControl
{
    private ComboBox comboResolucion;
    private CheckBox checkPantallaCompleta;
    private TrackBar volumenMusica;
    private TrackBar volumenEfectos;
    private Label labelFrase;
    private CheckBox checkTemaOscuro;

    public ConfiguracionPanel()
    {
        this.Dock = DockStyle.Fill;
        this.BackColor = Color.White;
        InicializarComponentes();
    }

    private void InicializarComponentes()
    {
        Label titulo = new Label()
        {
            Text = "Configuración",
            Font = new Font("Arial", 18, FontStyle.Bold),
            Location = new Point(20, 20),
            AutoSize = true
        };
        this.Controls.Add(titulo);

        // Resolución
        comboResolucion = new ComboBox()
        {
            Location = new Point(20, 70),
            Width = 200
        };
        comboResolucion.Items.AddRange(new string[] {
            "800x600", "1024x768", "1280x720", "1920x1080"
        });
        comboResolucion.SelectedIndex = 2; // por defecto
        this.Controls.Add(comboResolucion);

        checkPantallaCompleta = new CheckBox()
        {
            Text = "Pantalla completa",
            Location = new Point(20, 110),
            AutoSize = true
        };
        this.Controls.Add(checkPantallaCompleta);

        // Volumen
        Label lblMusica = new Label()
        {
            Text = "Volumen de música",
            Location = new Point(20, 160)
        };
        this.Controls.Add(lblMusica);

        volumenMusica = new TrackBar()
        {
            Location = new Point(20, 180),
            Width = 200,
            Minimum = 0,
            Maximum = 100,
            Value = 70
        };
        this.Controls.Add(volumenMusica);

        Label lblEfectos = new Label()
        {
            Text = "Volumen de efectos",
            Location = new Point(20, 220)
        };
        this.Controls.Add(lblEfectos);

        volumenEfectos = new TrackBar()
        {
            Location = new Point(20, 240),
            Width = 200,
            Minimum = 0,
            Maximum = 100,
            Value = 70
        };
        this.Controls.Add(volumenEfectos);

        // Tema oscuro
        checkTemaOscuro = new CheckBox()
        {
            Text = "Tema oscuro",
            Location = new Point(20, 290),
            AutoSize = true
        };
        checkTemaOscuro.CheckedChanged += (s, e) => ActualizarFrase();
        this.Controls.Add(checkTemaOscuro);

        // Frase
        labelFrase = new Label()
        {
            Text = "",
            Location = new Point(20, 320),
            Font = new Font("Arial", 10, FontStyle.Italic),
            AutoSize = true
        };
        this.Controls.Add(labelFrase);

        // Crear carpeta de álbumes
        CrearCarpetaMusical();
    }

    private void ActualizarFrase()
    {
        if (checkTemaOscuro.Checked)
        {
            var frasesOscuras = new[]
            {
                "Que viva el porno furry",
                "Lo malo de una tetas, es que están pegados a una mentirosa"
            };
            labelFrase.Text = frasesOscuras[new Random().Next(frasesOscuras.Length)];
            this.BackColor = Color.FromArgb(30, 30, 30);
            labelFrase.ForeColor = Color.White;
        }
        else
        {
            labelFrase.Text = "Se compilo porque Dios quiso";
            this.BackColor = Color.White;
            labelFrase.ForeColor = Color.Black;
        }
    }

    private void CrearCarpetaMusical()
    {
        string path = Path.Combine(Application.StartupPath, "MusicaCustom");
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
    }
}
