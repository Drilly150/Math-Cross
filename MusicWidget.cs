using System;
using System.Drawing;
using System.Windows.Forms;
using MathCross; // Para acceder a MusicManager

namespace MathCross
{
    public class MusicWidget : Panel
    {
        private Label lblPista;
        private Button btnAnterior, btnPausa, btnSiguiente;
        private PictureBox disco;
        // CAMBIO: Especificar el tipo de Timer para resolver la ambigüedad
        private System.Windows.Forms.Timer animacion; 

        private float angulo = 0;

        /// <summary>
        /// Constructor del MusicWidget. Inicialice los controles visuales
        /// para mostrar la pista actual y permitir el control de la reproducción.
        /// </summary>
        public MusicWidget()
        {
            this.Size = new Size(300, 100);
            this.BackColor = Color.FromArgb(240, 240, 240);
            this.BorderStyle = BorderStyle.FixedSingle;

            // Etiqueta para mostrar el nombre de la pista actual
            lblPista = new Label()
            {
                Text = MusicManager.PistaActual, // Obtiene el nombre de la pista del MusicManager
                Font = new Font("Arial", 10, FontStyle.Bold),
                Location = new Point(100, 10),
                AutoSize = false,
                Size = new Size(190, 40),
                TextAlign = ContentAlignment.MiddleLeft
            };
            this.Controls.Add(lblPista);

            // PictureBox para el icono del disco, con animación de rotación
            disco = new PictureBox()
            {
                Image = Properties.Resources.disc_icon, // 🟢 Usa tu ícono circular
                Size = new Size(64, 64),
                Location = new Point(10, 18),
                SizeMode = PictureBoxSizeMode.Zoom
            };
            this.Controls.Add(disco);

            // Botón para retroceder a la pista anterior
            btnAnterior = new Button()
            {
                Text = "⏮",
                Location = new Point(90, 60),
                Size = new Size(40, 30)
            };
            btnAnterior.Click += (s, e) => { MusicManager.Anterior(); Refrescar(); };
            this.Controls.Add(btnAnterior);

            // Botón para pausar o reanudar la reproducción
            btnPausa = new Button()
            {
                Text = "⏯",
                Location = new Point(140, 60),
                Size = new Size(40, 30)
            };
            btnPausa.Click += (s, e) => MusicManager.PausarOContinuar();
            this.Controls.Add(btnPausa);

            // Botón para avanzar a la siguiente pista
            btnSiguiente = new Button()
            {
                Text = "⏭",
                Location = new Point(190, 60),
                Size = new Size(40, 30)
            };
            btnSiguiente.Click += (s, e) => { MusicManager.Siguiente(); Refrescar(); };
            this.Controls.Add(btnSiguiente);

            // Animación de rotación del disco
            // CAMBIO: Usar System.Windows.Forms.Timer para la animación de la UI
            animacion = new System.Windows.Forms.Timer() { Interval = 50 };
            animacion.Tick += (s, e) =>
            {
                angulo += 5;
                disco.Image = RotarImagen(Properties.Resources.disc_icon, angulo);
            };
            animacion.Start();
        }

        /// <summary>
        /// Actualiza la etiqueta de la pista actual.
        /// </summary>
        public void Refrescar()
        {
            lblPista.Text = MusicManager.PistaActual;
        }

        /// <summary>
        /// Rota una imagen dada por un ángulo específico.
        /// </summary>
        /// <param name="img">La imagen original.</param>
        /// <param name="angle">El ángulo de rotación en grados.</param>
        /// <returns>Una nueva imagen rotada.</returns>
        private Image RotarImagen(Image img, float angle)
        {
            Bitmap bmp = new Bitmap(img.Width, img.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.TranslateTransform(img.Width / 2, img.Height / 2); // Traslada al centro de la imagen
                g.RotateTransform(angle); // Rota la imagen
                g.TranslateTransform(-img.Width / 2, -img.Height / 2); // Traslada de vuelta
                g.DrawImage(img, new Point(0, 0)); // Dibuja la imagen original rotada en el nuevo bitmap
            }
            return bmp;
        }

        /// <summary>
        /// Se asegura de que la animación se detenga y los recursos se liberen
        /// cuando el widget es desechado.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (animacion != null)
                {
                    animacion.Stop();
                    animacion.Dispose();
                }
                // Liberar otros recursos si es necesario
            }
            base.Dispose(disposing);
        }
    }
}

//Módelo visual que muestra la pista actual y permite controlarla (play/pausa, anterior, siguiente). Interactua con "MusicManager", el cual controla. Insertado en "LevelSelectMenu" y "PuzzleGamePanel". 