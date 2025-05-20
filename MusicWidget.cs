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
      private Timer animacion;

      private float angulo = 0;

      public MusicWidget()
      {
          this.Size = new Size(300, 100);
          this.BackColor = Color.FromArgb(240, 240, 240);
          this.BorderStyle = BorderStyle.FixedSingle;

          lblPista = new Label()
          {
              Text = MusicManager.PistaActual,
              Font = new Font("Arial", 10, FontStyle.Bold),
              Location = new Point(100, 10),
              AutoSize = false,
              Size = new Size(190, 40),
              TextAlign = ContentAlignment.MiddleLeft
          };
          this.Controls.Add(lblPista);

          disco = new PictureBox()
          {
              Image = Properties.Resources.disc_icon, // 🟢 Usa tu ícono circular
              Size = new Size(64, 64),
              Location = new Point(10, 18),
              SizeMode = PictureBoxSizeMode.Zoom
          };
          this.Controls.Add(disco);

          btnAnterior = new Button()
          {
              Text = "⏮",
              Location = new Point(100, 60),
              Size = new Size(40, 30)
          };
          btnAnterior.Click += (s, e) => { MusicManager.Anterior(); Refrescar(); };
          this.Controls.Add(btnAnterior);

          btnPausa = new Button()
          {
              Text = "⏯",
              Location = new Point(145, 60),
              Size = new Size(40, 30)
          };
          btnPausa.Click += (s, e) => MusicManager.PausarOContinuar();
          this.Controls.Add(btnPausa);

          btnSiguiente = new Button()
          {
              Text = "⏭",
              Location = new Point(190, 60),
              Size = new Size(40, 30)
          };
          btnSiguiente.Click += (s, e) => { MusicManager.Siguiente(); Refrescar(); };
          this.Controls.Add(btnSiguiente);

          // Animación de rotación del disco
          animacion = new Timer() { Interval = 50 };
          animacion.Tick += (s, e) =>
          {
              angulo += 5;
              disco.Image = RotarImagen(Properties.Resources.disc_icon, angulo);
          };
          animacion.Start();
      }

      public void Refrescar()
      {
          lblPista.Text = MusicManager.PistaActual;
      }

      private Image RotarImagen(Image img, float angle)
      {
          Bitmap bmp = new Bitmap(img.Width, img.Height);
          using (Graphics g = Graphics.FromImage(bmp))
          {
              g.TranslateTransform(img.Width / 2, img.Height / 2);
              g.RotateTransform(angle);
              g.TranslateTransform(-img.Width / 2, -img.Height / 2);
              g.DrawImage(img, new Point(0, 0));
          }
          return bmp;
      }
  }
}